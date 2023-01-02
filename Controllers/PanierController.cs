using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AjcProject.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AjcProject.Controllers
{
    public class PanierController : Controller
    {
        private readonly AjcProjectDbContext _context;

        public PanierController(AjcProjectDbContext context)
        {
            _context = context;
        }

        // GET: CartItems
        public async Task<IActionResult> Index()
        {

            //Pour renvoyer l'id du panier
            var cartId = GetCartId();

            //Je vais chercher ma liste de cartItem avec une jointure sur le produit
            var ajcProjectDbContext = _context.CartItems.Include(c => c.Product).Where(c => c.CartId == cartId);
            var liste = await ajcProjectDbContext.ToListAsync();

            //Je calcule le montant total du panier
            ViewData["MontantTotal"] = liste.Sum(item => item.PrixTotal);

            //Je teste si la quantité des articles du panier est sup à zéro pour mettre le bouton valider en visible
            ViewData["btnVisible"] = (liste.Sum(item => item.Quantity) > 0);

            //Pour renvoyer l'id du panier
            ViewData["id"] = cartId;

            return View(liste);
        }

        // POST: CartItems
        [HttpPost]
        public async Task<IActionResult> Recalculer(List<CartItem> cartItemList)
        {
            //Supp des item du panier
            var ListToDelete = cartItemList.Where(item => item.ToDelete).ToList();
            _context.CartItems.RemoveRange(ListToDelete);

            //Mise à jour des qté item dans le panier
            var listToUpdate = cartItemList.Where(item => item.ToDelete == false).ToList();
            listToUpdate.ForEach(item => UpdateQteItem(item.ItemId, item.Quantity));

            //Verif si qté inf ou égal à 0
            for (int i = 0; i < listToUpdate.Count; i++)
            {
                if (listToUpdate[i].Quantity <= 0 )
                {
                    return RedirectToAction("Index");
                }
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private void UpdateQteItem(String itemId, int quantite)
        {
            var MyItem = _context.CartItems.FirstOrDefault(i => i.ItemId == itemId);
            if (MyItem != null && MyItem.Quantity != quantite)
            {
                MyItem.Quantity = quantite;
            }
        }

        private string GetCartId()
        {
            //Pas de temps pour générer un id panier pour chaque session donc juste un random
            return "6afd9c0b-4009-4723-a719-fcb9e200db35";
        }

        //methode add product
        public async Task<IActionResult> AddProduct(int id)
        {
            var product = _context.Products.Find(id);
            if (product == null)
            {
                // Return error message or redirect to appropriate page
                return RedirectToAction("Index");
            }

            var cartItem = _context.CartItems.FirstOrDefault(i => i.ProductId == id);
            if (cartItem != null)
            {
                cartItem.Quantity++;
            }
            else
            {
                cartItem = new CartItem
                {
                    ProductId = id,
                    Quantity = 1,
                    ItemId = System.Guid.NewGuid().ToString(),
                    DateCreated = DateTime.Now,
                    CartId = GetCartId()
                };
                _context.Add(cartItem);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
    }

}