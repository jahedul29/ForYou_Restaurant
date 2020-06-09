using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForYou.Models.ViewModel
{
    public class OrderHeaderAndOrderDetailsListViewModel
    {
        public IList<OrderHeaderAndOrderDetailsViewModel> OrderList{ get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}
