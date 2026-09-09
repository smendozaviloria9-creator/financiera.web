using System.Collections.Generic;
using System.Linq;
using Financiera.Web.Interfaces;
using Financiera.Web.Models;

namespace Financiera.Web.Repositories
{
    public class MovementRepository : IMovementRepository
    {
        public void Insert(Movement movement)
        {
            InMemoryDatabase.Movements.Add(movement);
        }

        public List<Movement> GetByDocument(string documentNumber)
        {
            return InMemoryDatabase.Movements
                .Where(m => m.DocumentNumber == documentNumber)
                .OrderByDescending(m => m.Date)
                .ToList();
        }

        public List<Movement> GetAll()
        {
            return InMemoryDatabase.Movements.ToList();
        }
    }
}