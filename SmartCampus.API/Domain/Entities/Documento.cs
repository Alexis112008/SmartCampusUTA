using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class Documento
    {
        [Key]
        public int IdDocumento { get; set; }
        public int IdCategoria { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Ruta { get; set; } = string.Empty;
        public DateTime FechaSubida { get; set; } = DateTime.Now;
        public int IdUsuario { get; set; }

        public CategoriaDoc Categoria { get; set; } = null!;
        public Usuario Usuario { get; set; } = null!;
    }
}