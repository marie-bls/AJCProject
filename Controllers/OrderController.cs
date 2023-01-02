using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System;
using System.Linq;
using AjcProject.Models;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using AjcProject.Areas.Identity.Data;

namespace AjcProject.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {


        private readonly AjcProjectDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public OrderController(AjcProjectDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        // GET: OrderController
        [Route("Index/{idPanier}")]
        public async Task<IActionResult> Index(string idPanier)
        {

            List<CartItem> CartItems = _context.CartItems.Include(p => p.Product).Where(
                c => c.CartId == idPanier).ToList();


            Order order = new Order();

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                order.Username = user.UserName;
                order.Email = user.Email;
                order.FirstName = user.FirstName;
                order.LastName = user.LastName;
            }

            //Calcul du total
            order.Total = Convert.ToDecimal(CartItems.Sum(item => item.PrixTotal));

            foreach (CartItem item in CartItems)
            {
                item.Product = _context.Products.Where(p => p.ProductId == item.ProductId).ToList()[0];

                OrderDetail detail = new OrderDetail()
                {
                    OrderId = order.OrderId,

                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = item.Product.UnitPrice,
                    Username = order.Username,
                    // Attirer l'attention du stagiaires ici sur le NotMapped
                    // car le nom du produit, on en a besoin uniquement
                    // pour l'afficher dans la liste des détails
                    // de commande
                    ProductName = item.Product.ProductName
                };
                order.OrderDetails.Add(detail);
            }


            _context.Add(order);
            _context.SaveChanges();

            ViewData["CartID"] = idPanier;

            return View(order);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(string idPanier, [Bind("OrderId,FirstName,LastName,Address,City,Country,PostalCode,Total,Username,Email")] Order order)
        {
            order.OrderDate = DateTime.Now;

            DeleteCart(idPanier);

            string Pid = System.Guid.NewGuid().ToString();
            order.PaymentTransactionId = Pid;

            _context.Orders.Update(order);
            _context.SaveChanges();

            return View(order);
        }

        private void DeleteCart(string id)
        {
            List<CartItem> CartItems = _context.CartItems.Where(
                c => c.CartId == id).ToList();

            foreach (CartItem item in CartItems)
            {
                _context.Remove(item);
            }
        }

    }
}
