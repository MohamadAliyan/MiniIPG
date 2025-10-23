using Microsoft.Extensions.Options;
using Moq;
using PaymentService.Application.Common.Models;
using PaymentService.Application.Payments.Commands;
using PaymentService.Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;

namespace PaymetService.Test
{

    
        public class CreateTransactionTests
        {
            private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
            private readonly Mock<IOptions<AppSettings>> _mockAppMock;
            private readonly CreateTransactionHandler _handler;

            public CreateTransactionTests()
            {
                _transactionRepositoryMock = new Mock<ITransactionRepository>();
                _mockAppMock = new Mock<IOptions<AppSettings>>();

                //var mockOptions = new Mock<IOptions<AppSettings>>();
                var appSettings = new AppSettings
                {
                    GatewayUrl = "http://localhost:5002"
            
               
                };
                _mockAppMock.Setup(x => x.Value).Returns(appSettings);

            _handler = new CreateTransactionHandler(_transactionRepositoryMock.Object, _mockAppMock.Object);
            }

            [Fact]
            public async Task Handle_Should_Create_Transaction_And_Return_Success()
            {
                var command = new CreateTransactionCommand(

                    "12345",
                    50000,
                    "https://example.com/redirect",
                    "RES123",
                    "09123456789");

                _transactionRepositoryMock
                    .Setup(r => r.AddAsync(It.IsAny<Transaction>()));

                var result = await _handler.Handle(command, CancellationToken.None);

                result.Should().NotBeNull();
                result.IsSuccess.Should().BeTrue();
                result.Message.Should().Be("توکن با موفقیت ایجاد شد.");
                result.Data.Should().NotBeNull();
                result.Data!.Token.Should().NotBeEmpty();

                _transactionRepositoryMock.Verify(
                    r => r.AddAsync(It.IsAny<Transaction>()),
                    Times.Once);
            }

           
        }
}