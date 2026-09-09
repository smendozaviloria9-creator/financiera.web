namespace Financiera.Web.ViewModels
{
    public class BalanceViewModel
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal BalanceCop { get; set; }
        public decimal? BalanceUsd { get; set; }
        public decimal? TrmValue { get; set; }
        public string? TrmValidFrom { get; set; }
        public string? TrmValidUntil { get; set; }
        public string? ErrorMessage { get; set; }
    }
}