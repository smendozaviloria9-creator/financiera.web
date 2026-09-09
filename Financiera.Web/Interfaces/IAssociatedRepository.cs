using System.Collections.Generic;
using Financiera.Web.Models;

namespace Financiera.Web.Interfaces
{
    public interface IAssociatedRepository
    {
        void Insert(Associated associated);
        Associated? GetByDocument(string documentNumber);
        List<Associated> GetByName(string nameQuery);
        List<Associated> GetAll();
        void Update(Associated associated);
        void Delete(string documentNumber);
    }
}