using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelBackOffice.Models.Entity
{
    public class Prenotazione
    {
        [Key]
        public int PrenotazioneId { get; set; }

        [Required]
        public int ClienteId { get; set; }

        [Required]
        public int CameraId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataInizio { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DataFine { get; set; }

        [Required]
        [MaxLength(50)]
        public string Stato { get; set; }


        [ForeignKey("ClienteId")]
        public virtual Cliente Cliente { get; set; }

        [ForeignKey("CameraId")]
        public virtual Camera Camera { get; set; }
        [ForeignKey("CreataDaUserId")]
        public virtual AppUser? CreataDa { get; set; }
    }
}
