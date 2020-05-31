using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForYou.Models
{
    public class OrderDetails
    {
        [Key]
        public int OrderDetailsId { get; set; }

        [Required]
        [Display(Name = "Order Id")]
        public int OrderId { get; set; }

        [NotMapped]
        [ForeignKey("OrderId")]
        public virtual OrderHeader OrderHeader { get; set; }


        [Required]
        public int MenuItemId { get; set; }

        [NotMapped]
        [ForeignKey("MenuItemId")]
        public virtual MenuItem MenuItem { get; set; }

        public int Count { get; set; }

        public string ItemName { get; set; }

        [Required]
        public double Price{ get; set; }

    }
}
