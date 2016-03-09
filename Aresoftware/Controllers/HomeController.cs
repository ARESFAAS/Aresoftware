using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Aresoftware.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Products()
        {
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }

        public void SendMailBase(string body, string subject, string to)
        {
            var message = new MailMessage();
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                try
                {
                    smtp.Send(message);
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public ViewResult SendMail(string name, string email, string textMessage)
        {
            try
            {
                StringBuilder body = new StringBuilder();
                body.Append("<html>");
                body.Append("<body>");
                body.Append("<div>");
                body.Append("<br/>");
                body.Append("<h2>AreSoftware</h2>");
                body.Append("</div>");
                body.Append("<br>");
                body.Append("<div>");
                body.Append("<p>{0}</p>");
                body.Append("<p>{1}</p>");
                body.Append("<p>Cordialmente,</p>");
                body.Append("<p>AreSoftware</p>");
                body.Append("</div>");
                body.Append("<br/>");
                body.Append("</body>");
                body.Append("</html>");

                SendMailBase(string.Format(body.ToString(), email, textMessage), "AreSoftware - " + "Contacto de cliente: " + name, System.Configuration.ConfigurationManager.AppSettings["contactEmail"]);

                return View("Contact");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}