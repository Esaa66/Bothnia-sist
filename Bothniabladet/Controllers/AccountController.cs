﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Bothniabladet.Models;
using Bothniabladet.Data;
using Microsoft.EntityFrameworkCore;


namespace Bothniabladet.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;
        private SignInManager<ApplicationUser> signInManager;
        private RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userMgr, SignInManager<ApplicationUser> signInMgr, RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMgr;
            signInManager = signInMgr;
            roleManager = roleMgr;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel login)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = await userManager.Users.SingleOrDefaultAsync(u => u.Email == login.Email);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    if ((await signInManager.PasswordSignInAsync(user, login.Password, false, false)).Succeeded)
                    {
                        return Redirect(login?.ReturnUrl ?? "/Images/Index");
                    }
                    else
                    {
                        ViewBag.Message = "Incorrect e-mail address or password";
                    }
                }
            }

            return View(login);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel newUser)
        {
            if (ModelState.IsValid)
            {

                var user = new ApplicationUser
                {
                    UserName = newUser.UserName,
                    Email = newUser.Email
                };
                var insertrec = await userManager.CreateAsync(user, newUser.Password);
                var roleExist = await roleManager.RoleExistsAsync("Customer");
                if (!roleExist)
                {
                    var role = new IdentityRole();
                    role.Name = "Customer";
                    await roleManager.CreateAsync(role);
                }
                await userManager.AddToRoleAsync(user, "Customer");
                if (insertrec.Succeeded)
                {
                    ModelState.Clear();
                    ViewBag.Message = "Användaren " + newUser.UserName + " is now registered";
                }
                else
                {
                    foreach (var error in insertrec.Errors)
                    {
                        ModelState.AddModelError("", error.Description);
                    }
                }
            }
            return View();
        }
        public async Task<IActionResult> signOut(string returnurl = "/")
        {
            await signInManager.SignOutAsync();
            return Redirect(returnurl);
        }
    }
}
