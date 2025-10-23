
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GatewayService.Domain.Common;

namespace GatewayService.Domain.Entities;

public class PaymentLog: BaseAuditableEntity
{
    public decimal Amount { get; set; }
    public bool IsSuccess { get; set; }
    public DateTime ProcessedAt { get; set; } 
    public string Token { get; set; } 
    public string? RRN { get; set; }


}