using System.ComponentModel.DataAnnotations;

namespace Financiera.Web.ViewModels
{
    public class MovementFormViewModel
    {
        [Required(ErrorMessage = "El número de documento es obligatorio.")]
        [Display(Name = "Número de documento del asociado")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Range(0.01, double.MaxValue, ErrorMessage = "El valor debe ser mayor a cero.")]
        [Display(Name = "Valor")]
        public decimal Amount { get; set; }
    }
}