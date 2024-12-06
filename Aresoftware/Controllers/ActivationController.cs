using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace Aresoftware.Controllers
{
    public class ActivationController : Controller
    {

        // Acción para mostrar el formulario de login
        [HttpGet]
        public ActionResult Login()
        {
            // Si ya está logueado, redirigimos al inicio
            if (Session["User"] != null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Acción para manejar el login
        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            // Aquí verificarías las credenciales de la base de datos o de algún otro medio
            if (model.Username == "xxxxxx" && model.Password == "xxxx") // Verifica con tus credenciales reales
            {
                // Guardamos al usuario en la sesión para mantenerlo logueado
                Session["User"] = model.Username;
                return RedirectToAction("GenerateActivationKey", "Activation"); // Redirigir a la página principal u otra página de inicio
            }
            else
            {
                // Si las credenciales son incorrectas, mostramos un mensaje de error
                ViewBag.ErrorMessage = "Credenciales incorrectas. Intenta nuevamente.";
                return View();
            }
        }

        // Acción para cerrar sesión
        public ActionResult Logout()
        {
            // Limpiamos la sesión
            Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AreSoftwareAuthorize]        
        public ActionResult GenerateActivationKey() 
        {
            return View();
        }

        // Método para generar un GUID y guardarlo en el archivo como "no usado"
        [HttpPost]
        [AreSoftwareAuthorize] // Aseguramos que la solicitud esté autenticada
        public JsonResult GetActivationKey()
        {
            string filePath = Server.MapPath("~/App_Data/guidfile.json"); 
            string newGuid = Guid.NewGuid().ToString();

            // Crear el objeto con el GUID y el estado "no usado" (false)
            var newGuidEntry = new GuidEntry { Guid = newGuid, Used = false };

            // Leer el archivo existente o crear uno nuevo
            List<GuidEntry> guidList = new List<GuidEntry>();
            if (System.IO.File.Exists(filePath))
            {
                string fileContent = System.IO.File.ReadAllText(filePath);
                guidList = JsonConvert.DeserializeObject<List<GuidEntry>>(fileContent);
            }

            if (guidList == null)
                guidList = new List<GuidEntry>();
            // Agregar la nueva entrada
            guidList.Add(newGuidEntry);

            // Guardar el archivo con la nueva lista de GUIDs
            string jsonContent = JsonConvert.SerializeObject(guidList, Formatting.Indented);
           
            System.IO.File.WriteAllText(filePath, jsonContent);

            // Retornar el GUID generado
            return Json(new { ActivationKey = newGuid, Success = true });
        }

        // Método para validar y activar un GUID
        [HttpPost]
        public ActionResult ValidateAndActivate(string guid)
        {
            string filePath = Server.MapPath("~/App_Data/guidfile.json");

            if (!System.IO.File.Exists(filePath))
            {
                return Json(new { Success = false, Message = "No se encontraron registros de GUIDs." });
            }

            string fileContent = System.IO.File.ReadAllText(filePath);
            List<GuidEntry> guidList = JsonConvert.DeserializeObject<List<GuidEntry>>(fileContent);

            var guidEntry = guidList.FirstOrDefault(g => g.Guid == guid);
            if (guidEntry == null)
            {
                return Json(new { Success = false, Message = "Este GUID no existe." });
            }

            if (guidEntry.Used)
            {
                return Json(new { Success = false, Message = "Este GUID ya ha sido usado." });
            }

            // Actualizar el estado del GUID a "usado"
            guidEntry.Used = true;

            // Asegurarse de que el objeto `guidEntry` esté actualizado en la lista
            var updatedGuidList = guidList.Select(g => g.Guid == guid ? guidEntry : g).ToList(); // Reemplaza la entrada si es necesario

            // Serializar la lista de GUIDs actualizada a JSON
            string jsonContent = JsonConvert.SerializeObject(updatedGuidList, Formatting.Indented);

            System.IO.File.WriteAllText(filePath, jsonContent);

            return Json(new { Success = true, Message = "El GUID ha sido activado correctamente." });
        }
        
    }

    public class GuidEntry
    {
        public string Guid { get; set; }
        public bool Used { get; set; }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class AreSoftwareAuthorizeAttribute : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Lógica personalizada, por ejemplo, comprobar una sesión o variable específica
            var userSession = httpContext.Session["User"];

            if (userSession != null)
            {
               return true; // Si son correctos, permite la ejecución del método               
            }
            return false; // Deniega el acceso si no hay sesión válida
        }
    }
}