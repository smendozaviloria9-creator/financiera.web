using Financiera.Web.Services;
using Financiera.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Financiera.Web.Controllers
{
    public class MovementsController : Controller
    {
        private readonly BankingService _bankingService;

        public MovementsController(BankingService bankingService)
        {
            _bankingService = bankingService;
        }

        public IActionResult Deposit(string? document)
        {
            return View(new MovementFormViewModel { DocumentNumber = document ?? string.Empty });
        }

        [HttpPost]
        public IActionResult Deposit(MovementFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message) = _bankingService.RegisterDeposit(model.DocumentNumber, model.Amount);
            TempData[success ? "Success" : "Error"] = message;

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            return RedirectToAction("Details", "Associateds", new { id = model.DocumentNumber });
        }

        public IActionResult Withdraw(string? document)
        {
            return View(new MovementFormViewModel { DocumentNumber = document ?? string.Empty });
        }

        [HttpPost]
        public IActionResult Withdraw(MovementFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message) = _bankingService.RegisterWithdrawal(model.DocumentNumber, model.Amount);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            TempData["Success"] = message;
            return RedirectToAction("Details", "Associateds", new { id = model.DocumentNumber });
        }
    }
}