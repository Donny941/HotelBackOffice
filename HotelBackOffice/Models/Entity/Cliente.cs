using System.ComponentModel.DataAnnotations;

namespace HotelBackOffice.Models.Entity
{
    public class Cliente
    {
        [Key]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Nome")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [MaxLength(100)]
        [Display(Name = "Cognome")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Email non valida")]
        [MaxLength(256)]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Telefono non valido")]
        [MaxLength(20)]
        [Display(Name = "Telefono")]
        public string? Telefono { get; set; }


        public virtual ICollection<Prenotazione>? Prenotazioni { get; set; }
    }
}
