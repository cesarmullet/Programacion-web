using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration; // Necesario para leer la configuración

namespace UPIICafeWeb.Controllers
{
    public class AccesoController : Controller
    {
        private readonly string cadenaSQL;

        public AccesoController(IConfiguration config)
        {
            // Leemos la conexión desde appsettings.json
            cadenaSQL = config.GetConnectionString("CadenaSQL");
        }

        // ==========================================
        // 1. MOSTRAR LA PANTALLA DE LOGIN (GET)
        // ==========================================
        [HttpGet]
        public IActionResult Index()
        {
            // Esto busca el archivo en: Views/Acceso/Index.cshtml
            return View(); 
        }

        // ==========================================
        // 2. VALIDAR ALUMNO/PROFESOR (POST)
        // ==========================================
        [HttpPost]
        public IActionResult Validar(string boleta_rfc)
        {
            using (SqlConnection cn = new SqlConnection(cadenaSQL))
            {
                // Buscamos el ID del rol basado en la boleta O el RFC
                string query = "SELECT id_rol FROM Usuarios WHERE boleta_rfc = @dato";
                
                SqlCommand cmd = new SqlCommand(query, cn);
                cmd.Parameters.AddWithValue("@dato", boleta_rfc);

                cn.Open();
                object resultado = cmd.ExecuteScalar(); // Obtenemos el ID del rol

                if (resultado != null)
                {
                    int idRol = Convert.ToInt32(resultado);

                    // Guardamos el rol en la sesión para usarlo en el Menú
                    HttpContext.Session.SetInt32("RolUsuario", idRol);

                    // Redirigimos al Menú (Controlador Menu, Acción Index)
                    return RedirectToAction("Index", "Menu"); 
                }
                else
                {
                    // Si falla, recargamos el Login (Index)
                    // (Ojo: ya no usamos login.html porque lo borraste)
                    return RedirectToAction("Index");
                }
            }
        }

        // ==========================================
        // 3. ENTRAR COMO VISITANTE (GET)
        // ==========================================
        [HttpGet]
        public IActionResult EntrarComoVisitante()
        {
            // Asignamos manualmente el ID 4 (Visitante)
            HttpContext.Session.SetInt32("RolUsuario", 4);

            return RedirectToAction("Index", "Menu");
        }
    }
}