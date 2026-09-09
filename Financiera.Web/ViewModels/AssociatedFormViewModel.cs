using System.ComponentModel.DataAnnotations;

namespace Financiera.Web.ViewModels
{
    public class AssociatedFormViewModel
    {
        [Required(ErrorMessage = "El documento es obligatorio.")]
        [Display(Name = "Número de documento")]
        public string DocumentNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [Display(Name = "Nombre completo")]
        public string FullName { get; set; } = string.Empty;

        [RegularExpression(@"^\d+$", ErrorMessage = "El teléfono solo debe contener números.")]
        [Display(Name = "Teléfono")]
        public string Phone { get; set; } = string.Empty;

        [Display(Name = "Dirección")]
        public string Address { get; set; } = string.Empty;

        public bool IsEdit { get; set; } = false;
    }
}