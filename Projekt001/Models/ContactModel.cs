using System.ComponentModel.DataAnnotations;

namespace Projekt001.Models
{
    public class ContactModel
    {
        [Required(ErrorMessage = "Pole 'Imię i nazwisko' jest wymagane.")]
        [StringLength(100, ErrorMessage = "Imię i nazwisko nie może być dłuższe niż 100 znaków.")]
        [Display(Name = "Imię i nazwisko")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Pole 'Adres email' jest wymagane.")]
        [EmailAddress(ErrorMessage = "Wprowadź poprawny adres email.")]
        [StringLength(100, ErrorMessage = "Adres email nie może być dłuższy niż 100 znaków.")]
        [Display(Name = "Adres email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Pole 'Wiadomość' jest wymagane.")]
        [StringLength(2000, ErrorMessage = "Wiadomość nie może być dłuższa niż 2000 znaków.")]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Wiadomość")]
        public string? Message { get; set; }
    }
}