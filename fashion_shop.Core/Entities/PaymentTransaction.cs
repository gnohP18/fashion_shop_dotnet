using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using fashion_shop.Core.Common;

namespace fashion_shop.Core.Entities;

public class PaymentTransaction : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = default!;

    public decimal TotalAmount { get; set; }

    public string Currency { get; set; } = "VND";

    public PaymentStatusEnum Status { get; set; }

    public PaymentMethodEnum Method { get; set; }

    public string? ProviderTransactionId { get; set; }

    public string? Signature { get; set; }

    public string? RawResponse { get; set; }

    public DateTime? PaymentTime { get; set; }

    public string? FailReason { get; set; }

    public bool IpnVerified { get; set; } = false;
}