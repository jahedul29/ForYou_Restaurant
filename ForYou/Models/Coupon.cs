using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ForYou.Models
{
    public class Coupon
    {
        [Key]
        public int CouponId { get; set; }
        [Required]
        public string CouponName { get; set; }
        [Required]
        public string CouponType { get; set; }
        public enum ECouponType { Percent=0,Dollar=1}
        [Required]
        public double Discount { get; set; }
        [Required]
        public double MinimumAmount { get; set; }
        [Required]
        public byte[] Picture { get; set; }
        [Required]
        public bool IsActive { get; set; }
    }
}
