using System.Threading.Tasks;
using Financiera.Web.Services;
using Financiera.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Financiera.Web.Controllers
{
    public class AssociatedsController : Controller
    {
        private readonly BankingService _bankingService;

        public AssociatedsController(BankingService bankingService)
        {
            _bankingService = bankingService;
        }

        public IActionResult Index(string? search)
        {
            var list = string.IsNullOrWhiteSpace(search)
                ? _bankingService.ListAllAssociateds()
                : _bankingService.SearchByName(search);

            ViewBag.Search = search;
            return View(list);
        }

        public IActionResult Create()
        {
            return View(new AssociatedFormViewModel());
        }

        [HttpPost]
        public IActionResult Create(AssociatedFormViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var (success, message) = _bankingService.RegisterAssociated(
                model.DocumentNumber, model.FullName, model.Phone, model.Address);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Edit(string id)
        {
            var associated = _bankingService.SearchByDocument(id);
            if (associated == null) return NotFound();

            var model = new AssociatedFormViewModel
            {
                DocumentNumber = associated.DocumentNumber,
                FullName = associated.FullName,
                Phone = associated.Phone,
                Address = associated.Address,
                IsEdit = true
            };

            return View(model);
        }

        [HttpPost]
        public IActionResult Edit(string id, AssociatedFormViewModel model)
        {
            model.IsEdit = true;
            if (!ModelState.IsValid) return View(model);

            var (success, message) = _bankingService.UpdateAssociated(
                id, model.FullName, model.Phone, model.Address);

            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            TempData["Success"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public IActionResult Delete(string id)
        {
            var (success, message) = _bankingService.DeleteAssociated(id);
            TempData[success ? "Success" : "Error"] = message;
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(string id)
        {
            var associated = _bankingService.SearchByDocument(id);
            if (associated == null) return NotFound();

            ViewBag.Movements = _bankingService.GetMovements(id);
            ViewBag.Balance = await _bankingService.GetBalanceViewModelAsync(id);

            return View(associated);
        }
    }
}