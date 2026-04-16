using System.ComponentModel.DataAnnotations;

namespace SmartCampus.API.Domain.Entities
{
    public class CategoriaDoc
    {
        [Key]
        public int IdCategoria { get; set; }
        public int? IdCategoriaPadre { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int Nivel { get; set; } = 0;

        public CategoriaDoc? CategoriaPadre { get; set; }
        public ICollection<CategoriaDoc> SubCategorias { get; set; }
            = new List<CategoriaDoc>();
        public ICollection<Documento> Documentos { get; set; }
            = new List<Documento>();
    }
}