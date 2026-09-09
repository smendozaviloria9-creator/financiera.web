using System;
using System.Collections.Generic;

namespace Financiera.Web.ViewModels
{
    public class TotalMoneyReportVM
    {
        public decimal TotalMoney { get; set; }
        public int TotalAssociateds { get; set; }
        public decimal AverageMoney { get; set; }
    }

    public class TopAssociatedRowVM
    {
        public int Rank { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }

    public class SleepingAssociatedRowVM
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class PeriodSummaryVM
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalDeposits { get; set; }
        public int CountDeposits { get; set; }
        public decimal TotalWithdrawals { get; set; }
        public int CountWithdrawals { get; set; }
        public decimal Difference { get; set; }
    }

    public class LargestMovementRowVM
    {
        public DateTime Date { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal TotalValue { get; set; }
        public string AssociatedName { get; set; } = string.Empty;
    }

    public class MovementActivityRowVM
    {
        public string FullName { get; set; } = string.Empty;
        public int Count { get; set; }
        public decimal TotalDeposited { get; set; }
        public decimal TotalWithdrawn { get; set; }
        public decimal CurrentBalance { get; set; }
    }

    public class ReportsIndexViewModel
    {
        public TotalMoneyReportVM Report1 { get; set; } = new();
        public List<TopAssociatedRowVM> Report2 { get; set; } = new();
        public List<SleepingAssociatedRowVM> Report3 { get; set; } = new();
        public PeriodSummaryVM Report4 { get; set; } = new();
        public List<LargestMovementRowVM> Report5 { get; set; } = new();
        public List<MovementActivityRowVM> Report6 { get; set; } = new();
    }
}