using System.ComponentModel.DataAnnotations;

namespace OrderApi.Application.DTOs
{
    public record class AppUserDTO(
        int Id,
        [Required] string Name,
        [Required, EmailAddress] string Email,
        [Required] string Address,
        [Required] string PhoneNumber,
        [Required] string Password,
        [Required] string Role
        );
}

