using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ren.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using System.Net.Mail;

namespace ren.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Message"] = "Home page.";
            return View();

        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult PageNotFound()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpGet]
        public IActionResult EmailSender()
        {
            return View();
        }

        [Authorize(Roles = "user")]
        [HttpPost]
        public async Task<IActionResult> EmailSender(int id = 1)
        {
            var adress = HttpContext.User.Identity.Name;
            Debug.WriteLine(adress.ToString());
            Debug.WriteLine(adress.ToString());
            Debug.WriteLine(adress.ToString());
            try
            {
                SmtpClient smtpClient = new SmtpClient("mail7.meuhost.net", 25);
                smtpClient.Credentials = new System.Net.NetworkCredential("info@goodstream.eu", "Info4!891");
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.EnableSsl = true;
                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("info@goodstream.eu");
                mail.To.Add(new MailAddress(adress.ToString()));
                mail.Subject = "Star Wars: Renegade - рассылка";
                mail.Body = "Спасибо что оформили подписку на имейл рассылку новостей по проекту Star Wars: Renegade от студии Fly-Banana";
                try
                {
                    await Task.Run(() =>
                    {
                        smtpClient.Send(mail);
                    });
                }
                catch (Exception ex)
                {
                    Debug.WriteLine("Exception caught in CreateMessage(): {0}",
                                ex.ToString());
                }
                string message = "Email sent to: " + adress;
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
      
        }
}
