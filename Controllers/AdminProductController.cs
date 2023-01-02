using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AjcProject.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using System.Net.Mime;
using System.Drawing;
using System.Net;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;

namespace AjcProject.Controllers
{
    [Authorize]
    public class AdminProductController : Controller
    {
        private readonly AjcProjectDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly long _fileSizeLimit;
        private readonly string[] _permittedExtensions = { "image/jpeg", "image/png", "image/gif", "image/svg+xml" };

        private readonly IDateTime _dateTime;
        private readonly IRandomCode _rand;

        public AdminProductController(AjcProjectDbContext context, IWebHostEnvironment hostEnvironment, IConfiguration config, IDateTime dateTime, IRandomCode rand)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            _dateTime = dateTime;
            _rand = rand;
        }

        // GET: AdminProduct
        public async Task<IActionResult> Index()
        {
            var ajcProjectDbContext = _context.Products.Include(p => p.Category);
            return View(await ajcProjectDbContext.ToListAsync());
        }

        // GET: AdminProduct/Details/5
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

        // GET: AdminProduct/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: AdminProduct/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,ProductName,Description,ImagePath,UnitPrice,CategoryId")] Product product)
        {

            if (ModelState.IsValid)
            {
                var prdReq = (from p in _context.Products
                              where p.ProductName.Contains(product.ProductName)
                              select p).Any();

                if (prdReq)
                {

                    ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                    ViewData["error"] += "Ce nom de service existe déjà !";
                    return View(product);
                }

                if (Request.Form.Files.Any())
                {
                    IFormFile file = Request.Form.Files[0];
                    string ctType = file.ContentType;
                    long filelength = file.Length;
                    string fileName = WebUtility.HtmlEncode(file.FileName);
                    string extension = Path.GetExtension(fileName).ToLowerInvariant();


                    if (string.IsNullOrEmpty(ctType) || !_permittedExtensions.Contains(ctType))
                    {
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                        ViewData["error"] += "Type de fichier non autorisé";
                        return View(product);
                    }


                    if (filelength > _fileSizeLimit)
                    {
                        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
                        ViewData["error"] += "Fichier trop volumineux !";
                        return View(product);
                    }

                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    fileName = _rand.RndCode + DateTime.Now.ToFileTime();
                    product.ImagePath = WebUtility.HtmlEncode(fileName + extension);
                    string path = Path.Combine(wwwRootPath, "Catalog", "images");
                    var downloadResult = Download(file, path, fileName + extension);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // GET: AdminProduct/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var product = await _context.Products.FindAsync(id);


            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }

        // POST: AdminProduct/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,Description,ImagePath,UnitPrice,CategoryId")] Product product)
        {

            if (id != product.ProductId)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);

            if (ModelState.IsValid)
            {
                var sameService = _context.Products.FirstOrDefault(prod => prod.ProductId != id
                && prod.ProductName == product.ProductName);

                if (sameService != null)
                {
                    ViewData["error"] += "Ce nom de service existe déjà !";
                    return View(product);
                }
            }
            else
            {
                return RedirectToAction(nameof(Index));
            }



            if (Request.Form.Files.Any())
            {
                IFormFile file = Request.Form.Files[0];
                string ctType = file.ContentType;
                long filelength = file.Length;
                string fileName = WebUtility.HtmlEncode(file.FileName);
                string extension = Path.GetExtension(fileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(ctType) || !_permittedExtensions.Contains(ctType))
                {
                    ViewData["error"] += "Type de fichier non autorisé";
                    return View(product);
                }
                if (filelength > _fileSizeLimit)
                {
                    ViewData["error"] += "Fichier trop volumineux !";
                    return View(product);
                }

                string wwwRootPath = _hostEnvironment.WebRootPath;
                fileName = _rand.RndCode + DateTime.Now.ToFileTime();
                product.ImagePath = fileName + extension;
                string path = Path.Combine(wwwRootPath, "Catalog", "images");

                var downloadResult = Download(file, path, fileName + extension);

            }

            try
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return NotFound();
            }
            return RedirectToAction(nameof(Index));
        }


        // GET: AdminProduct/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: AdminProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        public async Task<IActionResult> OrderByPriceAsc()
        {
            var list = _context.Products.OrderBy(produit => produit.UnitPrice).ToList();
            return View("Index", list);
        }
        public async Task<IActionResult> OrderByPricedesc()
        {
            var list = _context.Products.OrderByDescending(produit => produit.UnitPrice).ToList();
            return View("Index", list);
        }
        public async Task<IActionResult> OrderByNameAsc()
        {
            var list = _context.Products.OrderBy(produit => produit.ProductName).ToList();
            return View("Index", list);
        }
        public async Task<IActionResult> OrderByNamedesc()
        {
            var list = _context.Products.OrderByDescending(produit => produit.ProductName).ToList();
            return View("Index", list);
        }

        public bool Download(IFormFile file, string folder, string fileName)
        {
            try
            {
                if (Directory.Exists(folder) == false)
                {
                    Directory.CreateDirectory(folder);
                }
                if (file.Length > 0)
                {
                    string filePath = Path.Combine(folder, fileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        file.CopyTo(fileStream);
                    }



                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

    }
}
