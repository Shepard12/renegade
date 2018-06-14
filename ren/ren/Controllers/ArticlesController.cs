using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ren.Models;
using ren.Models.ViewModels;

namespace ren.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly ApplicationContext _context;

        public ArticlesController(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Article.ToListAsync());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .SingleOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Description,Text")] ArticleModel editArticle)
        {
            Article article = new Article();
            article.Description = editArticle.Description;
            article.Title = editArticle.Title;
            article.Text = editArticle.Text;

            if (ModelState.IsValid)
            {
                List<string> recievers = new List<string>();
                int c = 0;

                _context.Add(article);
                await _context.SaveChangesAsync();

                //Send email to all subscribed users
                var users = await _context.Users
                   .Include(u => u.Role)
                   .Where(u => u.Subscribed == 1).ToListAsync();

                foreach(User u in users)
                {
                    recievers.Add(u.Email.ToString()); 
                }

                try
                {
                    SmtpClient smtpClient = new SmtpClient("mail7.meuhost.net", 25);
                    smtpClient.Credentials = new System.Net.NetworkCredential("info@goodstream.eu", "Info4!891");
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.EnableSsl = true;
                    MailMessage mail = new MailMessage();
                    mail.From = new MailAddress("info@goodstream.eu");
                    foreach(string s in recievers)
                    mail.To.Add(new MailAddress(s));
                    mail.Subject = "Star Wars: Renegade - новая новость";
                    mail.Body = article.Description + " Читайте по ссылке: http://flybanana.azurewebsites.net/Articles/Details/" + article.Id.ToString();
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
                    return RedirectToAction("Index");
                }

                catch (Exception ex)
                {
                    return RedirectToAction("Index");
                }

                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article.SingleOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }
            return View(article);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Text")] ArticleModel editArticle)
        {
            Article article = new Article();
            article.Id = editArticle.Id;
            article.Description = editArticle.Description;
            article.Title = editArticle.Title;
            article.Text = editArticle.Text;
            if (id != article.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(article);
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Article
                .SingleOrDefaultAsync(m => m.Id == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Article.SingleOrDefaultAsync(m => m.Id == id);
            _context.Article.Remove(article);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Article.Any(e => e.Id == id);
        }
    }
}
