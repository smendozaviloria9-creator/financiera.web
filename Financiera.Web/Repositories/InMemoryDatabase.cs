using System.Collections.Generic;
using Financiera.Web.Models;

namespace Financiera.Web.Repositories
{
    public static class InMemoryDatabase
    {
        public static List<Associated> Associateds { get; set; } = new List<Associated>();
        public static List<Movement> Movements { get; set; } = new List<Movement>();
    }
}