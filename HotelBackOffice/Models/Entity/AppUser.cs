using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace HotelBackOffice.Models.Entity
{
    public class AppUser : IdentityUser
    {
        [Required]

        public string Nome { get; set; }

        [Required]

        public string Cognome { get; set; }


        public virtual ICollection<Prenotazione> PrenotazioniCreate { get; set; }

    }
}
