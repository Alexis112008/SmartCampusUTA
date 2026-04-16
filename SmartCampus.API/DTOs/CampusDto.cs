namespace SmartCampus.API.DTOs
{
    public class RutaRequestDto
    {
        public int IdNodoOrigen { get; set; }
        public int IdNodoDestino { get; set; }
    }

    public class RutaResponseDto
    {
        public List<string> Pasos { get; set; } = new();
        public string Mensaje { get; set; } = string.Empty;
    }

    public class NodoResponseDto
    {
        public int IdNodo { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public double PosX { get; set; }
        public double PosY { get; set; }
    }
}