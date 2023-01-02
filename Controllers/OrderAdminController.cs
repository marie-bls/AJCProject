using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AjcProject.Models;
using Microsoft.AspNetCore.Authorization;
using System.Data;

namespace AjcProject.Controllers
{

    [Authorize]
    public class OrderAdminController : Controller
    {
        private readonly AjcProjectDbContext _context;

        public OrderAdminController(AjcProjectDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            return View(await _context.Orders.ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.Include(p => p.OrderDetails)
                .FirstOrDefaultAsync(m => m.OrderId == id);


            order.OrderDetails.ToList().ForEach(o => {
                o.ProductName = _context.Products.FirstOrDefault(p => p.ProductId == o.ProductId).ProductName;
                 });


            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
