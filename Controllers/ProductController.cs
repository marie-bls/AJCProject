using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AjcProject.Models;

namespace AjcProject.Controllers
{
    public class ProductController : Controller
    {
        private readonly AjcProjectDbContext _context;

        public ProductController(AjcProjectDbContext context)
        {
            _context = context;
        }

        // GET: Product/SearchString="Nom des produits", Searchcategory="Categories des produits", SearchNombreMax = "Prix max des produits"
        public async Task<IActionResult> Index(string SearchString, string Searchcategory,double SearchNombreMax)
        {
            ViewData["category"] = "Toutes les catégories";

            //Recuperation et injection de la liste des categories dans le viewdata "ListCategory"
            ViewData["ListCategory"] = _context.Categories.ToList().OrderBy(p => p.CategoryName);

            //Check si la list des produits n'est pas vide
            if (_context.Products == null)
            {
                return Problem("L'entite '_context.Categories' est null.");
            }
            //Recuperation de la list des produits
            var ajcProjectDbContext = _context.Products.Include(p => p.Category);

            //Recherche par Nom des produits
            if (!String.IsNullOrEmpty(SearchString))
            {
                ajcProjectDbContext = ajcProjectDbContext.Where(s => s.ProductName!.Contains(SearchString)).Include(p => p.Category);
            }

            //Recherche par Categories
            if (!String.IsNullOrEmpty(Searchcategory) && (Searchcategory != "Toutes les catégories"))
            {
                ViewData["category"] = Searchcategory;
                ajcProjectDbContext = ajcProjectDbContext.Where(s => s.Category.CategoryName!.Contains(Searchcategory)).Include(p => p.Category);
            }

            //Recherche par Prix max
            if (SearchNombreMax > 0)
            {
                ajcProjectDbContext = ajcProjectDbContext.Where(s => s.UnitPrice! <= SearchNombreMax).Include(p => p.Category);
            }
            return View(await ajcProjectDbContext.ToListAsync());
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }
    }
}
