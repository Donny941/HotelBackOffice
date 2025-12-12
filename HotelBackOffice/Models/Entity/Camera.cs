using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBackOffice.Models.Entity
{
    public class Camera
    {
        [Key]
        public int CameraId { get; set; }

        [Required]

        public string Numero { get; set; }

        [Required]

        public string Tipo { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Prezzo { get; set; }


        public virtual ICollection<Prenotazione> Prenotazioni { get; set; }
    }
}
