using System.ComponentModel.DataAnnotations;

namespace HotelBackOffice.Models.Dto
{
    public class LoginDto
    {
        [EmailAddress]
        public required string Email { get; set; }

        public required string Password { get; set; }
    }
}
