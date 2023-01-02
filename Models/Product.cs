using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
#nullable disable

namespace AjcProject.Models
{
    public partial class Product
    {
        public Product()
        {
            CartItems = new HashSet<CartItem>();
        }

        public int ProductId { get; set; }
        [Required (ErrorMessage ="Veuillez saisir un nom de produit(service) (150 caractères max)")]
        [DisplayName("Nom du service (produit)")]
        [StringLength(150, ErrorMessage = "150 caractère max autorisés !")]
        [RegularExpression(@"^[-’'0-9a-zA-ZÀ-ÖØ-öø-ÿ@!*().:zéèà,~ ]{2,150}$", ErrorMessage = "Caractère non autorisés !")]
        public string ProductName { get; set; }
        [DisplayName("Description")]
        [StringLength(500, ErrorMessage = "500 caractères max autorisés !")]
        [RegularExpression(@"^[-’'0-9a-zA-ZÀ-ÖØ-öø-ÿ@!*().:zéèà,~ ]{2,500}$", ErrorMessage = "Caractère non autorisés !")]
        [Required(ErrorMessage ="Veuillez saisir une description (voire même sommaire) (500 caractères max)")]
        public string Description { get; set; }
        public string ImagePath { get; set; }
        //[Required]
        [Range(0, 100000, ErrorMessage = "Saissisez un prix entre 0 et 100 000 !")]
        [DisplayName("Prix Unitaire")]
        public double? UnitPrice { get; set; }
        public int? CategoryId { get; set; }
        public virtual Category Category { get; set; }
        public virtual ICollection<CartItem> CartItems { get; set; }
        
    }
}
