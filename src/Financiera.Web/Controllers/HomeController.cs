using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Financiera.Web.Models;
using Financiera.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace Financiera.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly BankingService _bankingService;
        private readonly ExchangeRateService _exchangeRateService;

        public HomeController(BankingService bankingService, ExchangeRateService exchangeRateService)
        {
            _bankingService = bankingService;
            _exchangeRateService = exchangeRateService;
        }

        public async Task<IActionResult> Index()
        {
            var associateds = _bankingService.ListAllAssociateds();

            ViewBag.TotalAssociateds = associateds.Count;
            ViewBag.TotalMoney = associateds.Sum(a => _bankingService.CalculateBalance(a.DocumentNumber));
            ViewBag.RecentMovements = _bankingService.GetRecentMovements(5);

            var (trm, error) = await _exchangeRateService.FetchCurrentTrmAsync();
            ViewBag.TrmValue = trm?.Value;
            ViewBag.TrmError = error;

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}