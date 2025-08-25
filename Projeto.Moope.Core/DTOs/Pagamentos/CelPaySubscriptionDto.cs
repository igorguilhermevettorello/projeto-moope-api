namespace Projeto.Moope.Core.DTOs.Pagamentos
{
    /// <summary>
    /// DTO para criação de subscription com plano no CelPay
    /// </summary>
    public class CelPaySubscriptionRequestDto
    {
        public string ExternalId { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public CardInfo Card { get; set; } = new();
        public CustomerInfo Customer { get; set; } = new();
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public string? PromoCode { get; set; }
        public SubscriptionMetadata? Metadata { get; set; }
    }

    /// <summary>
    /// DTO para resposta de subscription do CelPay
    /// </summary>
    public class CelPaySubscriptionResponseDto
    {
        public string Id { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string ExternalId { get; set; } = string.Empty;
        public string PlanId { get; set; } = string.Empty;
        public CustomerResponse Customer { get; set; } = new();
        public CardResponse Card { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? NextChargeDate { get; set; }
        public DateTime? CancelledAt { get; set; }
        public string? Description { get; set; }
        public SubscriptionPlanInfo? Plan { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
    }

    /// <summary>
    /// Informações do plano na subscription
    /// </summary>
    public class SubscriptionPlanInfo
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "BRL";
        public string Interval { get; set; } = string.Empty; // monthly, yearly, etc.
        public int IntervalCount { get; set; }
        public int? TrialPeriodDays { get; set; }
    }

    /// <summary>
    /// Metadata adicional para subscription
    /// </summary>
    public class SubscriptionMetadata
    {
        public string? ClienteId { get; set; }
        public string? VendedorId { get; set; }
        public string? Observacoes { get; set; }
        public Dictionary<string, string>? CustomFields { get; set; }
    }

    /// <summary>
    /// DTO para cancelamento de subscription
    /// </summary>
    public class CelPayCancelSubscriptionDto
    {
        public string SubscriptionId { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public bool CancelAtPeriodEnd { get; set; } = false;
    }

    /// <summary>
    /// DTO para alteração de subscription
    /// </summary>
    public class CelPayUpdateSubscriptionDto
    {
        public string SubscriptionId { get; set; } = string.Empty;
        public string? NewPlanId { get; set; }
        public CardInfo? NewCard { get; set; }
        public SubscriptionMetadata? Metadata { get; set; }
    }
}
