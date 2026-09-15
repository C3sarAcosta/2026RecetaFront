using System.ComponentModel.DataAnnotations;

namespace _2026RecetaFront.DTOs
{
    public class CredencialesUsuario
    {
        //Autenticac fluent api
        [Required(ErrorMessage = "El email es requerido")]
        [EmailAddress(ErrorMessage = "El email es invalido")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "La contraseña es requerida")]
        [MinLength(8, ErrorMessage = "La contraseña debe tener al menos 8  caracteres")]
        public string Password { get; set; } = string.Empty;
    }

    public class RespuestaAutenticacion
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiracion { get; set; }
        public string UsuarioId { get; set; } = string.Empty;
    }
}