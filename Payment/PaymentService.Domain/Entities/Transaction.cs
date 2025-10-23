using PaymentService.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PaymentService.Domain.Common;
using Shared;


namespace PaymentService.Domain.Entities;

public class Transaction:BaseAuditableEntity
{
    public string TerminalNo { get; set; } 
    public decimal Amount { get; set; }
    public string RedirectUrl { get; set; } 
    public string ReservationNumber { get; set; } 
    public string PhoneNumber { get; set; } 
    public string Token { get; set; } 
    public string? RRN { get; set; }
    public PaymentStatus Status { get; set; } 
    public string? AppCode { get; set; }
}