using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Financiera.Web.Interfaces;
using Financiera.Web.Models;
using Financiera.Web.ViewModels;

namespace Financiera.Web.Services
{
    public class BankingService
    {
        private readonly IAssociatedRepository _associatedRepository;
        private readonly IMovementRepository _movementRepository;
        private readonly ExchangeRateService _exchangeRateService;

        public BankingService(
            IAssociatedRepository associatedRepository,
            IMovementRepository movementRepository,
            ExchangeRateService exchangeRateService)
        {
            _associatedRepository = associatedRepository;
            _movementRepository = movementRepository;
            _exchangeRateService = exchangeRateService;
        }

        public (bool success, string message) RegisterAssociated(string document, string name, string phone, string address)
        {
            if (string.IsNullOrWhiteSpace(document) || string.IsNullOrWhiteSpace(name))
                return (false, "El documento y el nombre son obligatorios.");

            if (_associatedRepository.GetByDocument(document) != null)
                return (false, "Ya existe un asociado registrado con ese número de documento.");

            _associatedRepository.Insert(new Associated
            {
                DocumentNumber = document,
                FullName = name,
                Phone = phone,
                Address = address
            });

            return (true, "Asociado registrado exitosamente con cuenta de ahorros en cero.");
        }

        public List<Associated> ListAllAssociateds() => _associatedRepository.GetAll();

        public Associated? SearchByDocument(string document) => _associatedRepository.GetByDocument(document);

        public List<Associated> SearchByName(string name) => _associatedRepository.GetByName(name);

        public (bool success, string message) UpdateAssociated(string document, string name, string phone, string address)
        {
            var associated = _associatedRepository.GetByDocument(document);
            if (associated == null) return (false, "Asociado no encontrado.");

            associated.FullName = name;
            associated.Phone = phone;
            associated.Address = address;
            _associatedRepository.Update(associated);
            return (true, "Datos actualizados correctamente.");
        }

        public (bool success, string message) DeleteAssociated(string document)
        {
            var associated = _associatedRepository.GetByDocument(document);
            if (associated == null) return (false, "Asociado no encontrado.");

            var movements = _movementRepository.GetByDocument(document);
            decimal balance = CalculateBalance(document);

            if (balance > 0 || movements.Count > 0)
                return (false, "No se puede eliminar un asociado que tenga saldo o movimientos registrados.");

            _associatedRepository.Delete(document);
            return (true, "Asociado eliminado lógicamente.");
        }

        public decimal CalculateBalance(string document)
        {
            var movements = _movementRepository.GetByDocument(document);
            decimal balance = 0;
            foreach (var m in movements)
            {
                if (m.Type == MovementType.Deposit) balance += m.Amount;
                else if (m.Type == MovementType.Withdrawal) balance -= (m.Amount + m.Commission);
            }
            return balance;
        }

        public (bool success, string message) RegisterDeposit(string document, decimal amount)
        {
            if (amount <= 0) return (false, "El valor del movimiento debe ser mayor a cero.");
            var associated = _associatedRepository.GetByDocument(document);
            if (associated == null) return (false, "Asociado no encontrado.");

            _movementRepository.Insert(new Movement
            {
                DocumentNumber = document,
                Type = MovementType.Deposit,
                Amount = amount,
                Commission = 0,
                Date = DateTime.Now
            });

            return (true, $"Consignación registrada con éxito. Nuevo saldo: {CalculateBalance(document):C}");
        }

        public (bool success, string message) RegisterWithdrawal(string document, decimal amount)
        {
            if (amount <= 0) return (false, "El valor del movimiento debe ser mayor a cero.");
            var associated = _associatedRepository.GetByDocument(document);
            if (associated == null) return (false, "Asociado no encontrado.");

            decimal commission = amount > 1_000_000 ? 8000m : 0m;
            decimal currentBalance = CalculateBalance(document);
            decimal totalDeduction = amount + commission;

            if (currentBalance < totalDeduction)
                return (false, $"Fondos insuficientes. El saldo actual ({currentBalance:C}) no cubre el retiro más la comisión de manejo ({commission:C}).");

            _movementRepository.Insert(new Movement
            {
                DocumentNumber = document,
                Type = MovementType.Withdrawal,
                Amount = amount,
                Commission = commission,
                Date = DateTime.Now
            });

            return (true, $"Retiro registrado con éxito. Comisión aplicada: {commission:C}. Nuevo saldo: {CalculateBalance(document):C}");
        }

        public List<Movement> GetMovements(string document) => _movementRepository.GetByDocument(document);

        public List<Movement> GetRecentMovements(int count) =>
            _movementRepository.GetAll().OrderByDescending(m => m.Date).Take(count).ToList();

        public async Task<BalanceViewModel> GetBalanceViewModelAsync(string document)
        {
            var associated = _associatedRepository.GetByDocument(document);
            var vm = new BalanceViewModel { DocumentNumber = document };

            if (associated == null)
            {
                vm.ErrorMessage = "Asociado no encontrado.";
                return vm;
            }

            vm.FullName = associated.FullName;
            vm.BalanceCop = CalculateBalance(document);

            var (trm, error) = await _exchangeRateService.FetchCurrentTrmAsync();
            if (trm == null || !decimal.TryParse(trm.Value.Replace(",", "."), out decimal trmValue) || trmValue <= 0)
            {
                vm.ErrorMessage = "No fue posible obtener la tasa oficial del sistema. Operando con normalidad, pero sin conversión disponible.";
                return vm;
            }

            vm.TrmValue = trmValue;
            vm.TrmValidFrom = trm.ValidFrom;
            vm.TrmValidUntil = trm.ValidUntil;
            vm.BalanceUsd = vm.BalanceCop / trmValue;
            return vm;
        }

        // ---- Informes de gerencia ----

        public TotalMoneyReportVM GetReport1TotalMoney()
        {
            var associateds = _associatedRepository.GetAll();
            int total = associateds.Count;
            decimal totalMoney = associateds.Sum(a => CalculateBalance(a.DocumentNumber));
            return new TotalMoneyReportVM
            {
                TotalMoney = totalMoney,
                TotalAssociateds = total,
                AverageMoney = total > 0 ? totalMoney / total : 0
            };
        }

        public List<TopAssociatedRowVM> GetReport2TopAssociateds()
        {
            return _associatedRepository.GetAll()
                .Select(a => new { Associated = a, Balance = CalculateBalance(a.DocumentNumber) })
                .OrderByDescending(x => x.Balance)
                .Take(5)
                .Select((x, i) => new TopAssociatedRowVM
                {
                    Rank = i + 1,
                    DocumentNumber = x.Associated.DocumentNumber,
                    FullName = x.Associated.FullName,
                    Balance = x.Balance
                }).ToList();
        }

        public List<SleepingAssociatedRowVM> GetReport3SleepingAssociateds()
        {
            var movements = _movementRepository.GetAll();
            return _associatedRepository.GetAll()
                .Where(a => !movements.Any(m => m.DocumentNumber == a.DocumentNumber))
                .Select(a => new SleepingAssociatedRowVM { DocumentNumber = a.DocumentNumber, FullName = a.FullName })
                .ToList();
        }

        public PeriodSummaryVM GetReport4PeriodSummary(DateTime startDate, DateTime endDate)
        {
            var movements = _movementRepository.GetAll()
                .Where(m => m.Date.Date >= startDate.Date && m.Date.Date <= endDate.Date)
                .ToList();

            decimal totalDeposits = movements.Where(m => m.Type == MovementType.Deposit).Sum(m => m.Amount);
            decimal totalWithdrawals = movements.Where(m => m.Type == MovementType.Withdrawal).Sum(m => m.Amount + m.Commission);

            return new PeriodSummaryVM
            {
                StartDate = startDate,
                EndDate = endDate,
                TotalDeposits = totalDeposits,
                CountDeposits = movements.Count(m => m.Type == MovementType.Deposit),
                TotalWithdrawals = totalWithdrawals,
                CountWithdrawals = movements.Count(m => m.Type == MovementType.Withdrawal),
                Difference = totalDeposits - totalWithdrawals
            };
        }

        public List<LargestMovementRowVM> GetReport5LargestMovements()
        {
            return _movementRepository.GetAll()
                .OrderByDescending(m => m.Amount + m.Commission)
                .Take(10)
                .Select(m => new LargestMovementRowVM
                {
                    Date = m.Date,
                    Type = m.Type == MovementType.Deposit ? "Consignación" : "Retiro",
                    TotalValue = m.Amount + m.Commission,
                    AssociatedName = _associatedRepository.GetByDocument(m.DocumentNumber)?.FullName ?? "Desconocido"
                }).ToList();
        }

        public List<MovementActivityRowVM> GetReport6MovementActivity()
        {
            return _associatedRepository.GetAll().Select(a =>
            {
                var movs = _movementRepository.GetByDocument(a.DocumentNumber);
                return new MovementActivityRowVM
                {
                    FullName = a.FullName,
                    Count = movs.Count,
                    TotalDeposited = movs.Where(m => m.Type == MovementType.Deposit).Sum(m => m.Amount),
                    TotalWithdrawn = movs.Where(m => m.Type == MovementType.Withdrawal).Sum(m => m.Amount + m.Commission),
                    CurrentBalance = CalculateBalance(a.DocumentNumber)
                };
            }).OrderByDescending(x => x.Count).ToList();
        }
    }
}