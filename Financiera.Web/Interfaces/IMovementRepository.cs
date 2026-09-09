using System.Collections.Generic;
using Financiera.Web.Models;

namespace Financiera.Web.Interfaces
{
    public interface IMovementRepository
    {
        void Insert(Movement movement);
        List<Movement> GetByDocument(string documentNumber);
        List<Movement> GetAll();
    }
}