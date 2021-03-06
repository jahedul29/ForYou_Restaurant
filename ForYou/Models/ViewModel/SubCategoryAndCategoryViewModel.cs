﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForYou.Models.ViewModel
{
    public class SubCategoryAndCategoryViewModel
    {
        public IEnumerable<Category> CategoryList{ get; set; }
        public SubCategory SubCategory { get; set; }
        public List<string> SubCategoryNameList{ get; set; }
        public string StatusMessage { get; set; }
    }
}
