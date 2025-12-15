using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Data;
using UPIICafeWeb.Models; // Importamos el modelo que acabamos de crear

namespace UPIICafeWeb.Controllers
{
    public class MenuController : Controller
    {
        private readonly string cadenaSQL;

        public MenuController(IConfiguration config)
        {
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        public IActionResult Index()
        {
            // 1. SEGURIDAD: Verificar si el usuario inició sesión
            // Si la sesión está vacía, lo regresamos al Login
            int? idRol = HttpContext.Session.GetInt32("RolUsuario");
            if (idRol == null) return RedirectToAction("Index", "Acceso"); // O redirigir a Login

            List<ProductoModel> lista = new List<ProductoModel>();

            using (SqlConnection cn = new SqlConnection(cadenaSQL))
            {
                // 2. CONSULTA INTELIGENTE
                // Traemos el producto y cruzamos (LEFT JOIN) con la tabla Descuento
                // para ver si ESTE rol tiene descuento en ESTE producto.
                string query = @"
                    SELECT 
                        p.id_prod, 
                        p.nom_prod, 
                        p.descrip, 
                        p.precio, 
                        p.img_url,
                        ISNULL(d.porc_desc, 0) AS descuento_aplicable
                    FROM Productos p
                    LEFT JOIN Descuento d ON p.id_prod = d.id_prod AND d.id_rol = @idRol";

                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@idRol", idRol);

                cn.Open();
                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    lista.Add(new ProductoModel
                    {
                        Id = Convert.ToInt32(reader["id_prod"]),
                        Nombre = reader["nom_prod"].ToString(),
                        Descripcion = reader["descrip"].ToString(),
                        PrecioOriginal = Convert.ToDecimal(reader["precio"]),
                        ImagenUrl = reader["img_url"].ToString(),
                        Descuento = Convert.ToDecimal(reader["descuento_aplicable"])
                    });
                }
            }

            // Enviamos la lista llena a la Vista
            return View(lista);
        }
    }
}