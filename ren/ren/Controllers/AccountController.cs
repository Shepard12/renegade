﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ren.Models;
using ren.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace RolesApp.Controllers
{
    public class AccountController : Controller
    {
        private ApplicationContext _context;
        public AccountController(ApplicationContext context)
        {
            _context = context;
            DatabaseInitialize(); 
        }

        private void DatabaseInitialize()
        {
            if (!_context.Roles.Any())
            {
                string adminRoleName = "admin";
                string userRoleName = "user";

                string adminEmail = "admin";
                string adminPassword = "password";
               
                Role adminRole = new Role { Name = adminRoleName };
                Role userRole = new Role { Name = userRoleName };

                _context.Roles.Add(userRole);
                _context.Roles.Add(adminRole);

                _context.Users.Add(new User { Email = adminEmail, Password = adminPassword, Role = adminRole });

                _context.SaveChanges();
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            if (User.IsInRole("admin") || User.IsInRole("user"))
                return RedirectToAction("Index", "Home");
            else
                return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var email = new EmailAddressAttribute();

            if(!email.IsValid(model.Email))
            {
                ModelState.AddModelError("Email", "Incorrect Email");
            }

            if (ModelState.IsValid)
            {
                User user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    user = new User { Email = model.Email, Password = model.Password, Subscribed = 0  };
                    Role userRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == "user");
                    if (userRole != null)
                        user.Role = userRole;

                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();

                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                else
                    ModelState.AddModelError("", "Incorrect login/password");
            }
            return View(model);
        }
        [HttpGet]
        public IActionResult Login()
        {
            if (User.IsInRole("admin") || User.IsInRole("user"))
                return RedirectToAction("Index", "Home");
            else
                return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await _context.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null)
                {
                    await Authenticate(user);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Incorrect login/password");
            }
            return View(model);
        }
        private async Task Authenticate(User user)
        {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType,
                ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("Cookies");
            return RedirectToAction("Login", "Account");
        }   
    }
}