using System;
using Financiera.Web.Services;
using Financiera.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Financiera.Web.Controllers
{
    public class ReportsController : Controller
    {
        private readonly BankingService _bankingService;

        public ReportsController(BankingService bankingService)
        {
            _bankingService = bankingService;
        }

        public IActionResult Index(DateTime? startDate, DateTime? endDate)
        {
            var start = startDate ?? DateTime.Now.AddDays(-30);
            var end = endDate ?? DateTime.Now;

            var model = new ReportsIndexViewModel
            {
                Report1 = _bankingService.GetReport1TotalMoney(),
                Report2 = _bankingService.GetReport2TopAssociateds(),
                Report3 = _bankingService.GetReport3SleepingAssociateds(),
                Report4 = _bankingService.GetReport4PeriodSummary(start, end),
                Report5 = _bankingService.GetReport5LargestMovements(),
                Report6 = _bankingService.GetReport6MovementActivity()
            };

            return View(model);
        }
    }
}