using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForYou.Models.ViewModel
{
    public class MenuItemViewModel
    {
        public MenuItem MenuItem { get; set; }
        public IEnumerable<Category> CategoryList { get; set; }
        public IEnumerable<SubCategory> SubCategoryList { get; set; }
    }
}
