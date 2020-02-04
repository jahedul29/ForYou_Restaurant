using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForYou.Models
{
    public class SubCategory
    {
        [Key]
        public int SubCategoryId { get; set; }

        [Display(Name = "SubCategory Name")]
        [Required]
        public string SubCategoryName { get; set; }

        [Required]
        [Display(Name = "Category Name")]
        public int CategoryId { get; set; }

        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
    }
}
