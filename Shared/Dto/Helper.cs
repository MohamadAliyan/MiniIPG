using Microsoft.Extensions.DependencyInjection;
using Refit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Shared.Dto
{
    public static class Helper
    {
        public static long Generate12DigitNumber(Random random)
        {
            // محدوده اعداد 12 رقمی: 100,000,000,000 تا 999,999,999,999
            long min = 100_000_000_000L;
            long max = 999_999_999_999L;

            byte[] buf = new byte[8];
            random.NextBytes(buf);
            long longRand = BitConverter.ToInt64(buf, 0) & 0x7FFFFFFFFFFFFFFF;

            return (longRand % (max - min)) + min;
        }

        public static void ResolveAllTypes(IServiceCollection services,
            ServiceLifetime serviceLifetime, Type refType, string suffix)
        {
            Assembly assemblyCurrent = refType.GetTypeInfo().Assembly;
            IEnumerable<Type> allServices = assemblyCurrent.GetTypes().Where(t =>
                t.GetTypeInfo().IsClass && !t.GetTypeInfo().IsAbstract &&
                !t.GetType().IsInterface && t.Name.EndsWith(suffix));


            foreach (Type type in allServices)
            {
                Type[] allInterfaces = type.GetInterfaces();
                IEnumerable<Type> mainInterfaces =
                    allInterfaces.Except(allInterfaces.SelectMany(t => t.GetInterfaces()));
                foreach (Type itype in mainInterfaces)
                {
                    if (allServices.Any(x => !x.Equals(type) && itype.IsAssignableFrom(x)))
                    {
                        throw new Exception("The " + itype.Name +
                                            " type has more than one implementations, please change your filter");
                    }

                    services.Add(new ServiceDescriptor(itype, type, serviceLifetime));
                }
            }
        }


    }
}
