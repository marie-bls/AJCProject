using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace AjcProject.Models
{
    public partial class CartItem
    {
        public string ItemId { get; set; }
        public string CartId { get; set; }
        public int Quantity { get; set; }
        public DateTime DateCreated { get; set; }
        public int ProductId { get; set; }

        [NotMapped]
        public bool ToDelete { get; set; }

        public virtual Product Product { get; set; }

        public double PrixTotal
        {
            get { return (double)(Quantity * Product.UnitPrice); }
        }
    }
}
