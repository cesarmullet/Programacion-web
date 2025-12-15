namespace UPIICafeWeb.Models
{
    public class ProductoModel
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal PrecioOriginal { get; set; }
        public decimal Descuento { get; set; } // Porcentaje (ej: 0.10)
        public string ImagenUrl { get; set; }

        // Calculamos el precio final automáticamente
        public decimal PrecioFinal => PrecioOriginal - (PrecioOriginal * Descuento);
    }
}