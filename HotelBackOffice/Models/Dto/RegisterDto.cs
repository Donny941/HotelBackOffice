using System.ComponentModel.DataAnnotations;

namespace HotelBackOffice.Models.Dto
{
    public class RegisterDto
    {
        [EmailAddress]
        public required string Email { get; set; }

        public required string Password { get; set; }

        [Compare(nameof(Password))]
        public required string ConfirmPassword { get; set; }

        public required string Name { get; set; }
        public required string Surname { get; set; }

    }
}
