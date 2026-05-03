using SmartCampus.API.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class TokenRecuperacion
{
    [Key]                           // ← esto es lo que faltaba
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int IdToken { get; set; }

    public int IdUsuario { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime FechaExpira { get; set; }
    public bool Usado { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.Now;

    // Navegación
    public Usuario Usuario { get; set; } = null!;
}