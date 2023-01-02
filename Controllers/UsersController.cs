using AjcProject.Areas.Identity.Data;
using AjcProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using AjcProject.Models;
using Microsoft.AspNetCore.Authorization;

namespace AjcProject.Controllers
{
    [Authorize]
    public class UsersController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AjcProjectDbContext _db;

        public UsersController(UserManager<ApplicationUser> userMrg, AjcProjectDbContext db)
        {
            _db = db;
            _userManager = userMrg;
        }

        [HttpGet]
        public IActionResult Index()
        {

            var users = _userManager.Users;
            
            return View(users);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User not found";
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("id,FirstName, LastName ,UserName, Email, LockoutEnabled, LockoutEnd")] ApplicationUser ajcUser)
        {
            var user = await _userManager.FindByIdAsync(id);
          
            if (user == null)
            {
                return NotFound();
            }

            user.FirstName= ajcUser.FirstName;
            user.LastName= ajcUser.LastName;
            user.UserName= ajcUser.UserName;
            user.Email= ajcUser.Email;
            user.LockoutEnabled= ajcUser.LockoutEnabled;
            user.LockoutEnd= ajcUser.LockoutEnd;
            
            await _userManager.UpdateAsync(user);

            return View(user);           
        }

        public async Task<IActionResult> Delete(string? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            
            await _userManager.DeleteAsync(user);
            
            return RedirectToAction(nameof(Index));
        }

    }
}
