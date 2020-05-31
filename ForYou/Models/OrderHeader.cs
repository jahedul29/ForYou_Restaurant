using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ForYou.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderId { get; set; }
    
        [Display(Name ="User Id")]
        [Required]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public virtual ApplicationUser ApplicationUser { get; set; }

        [Required]
        public DateTime OrderDate{ get; set; }

        [Required]
        public double OrderTotalOriginal { get; set; }

        [Required]
        [Display(Name ="Order Total")]
        [DisplayFormat(DataFormatString ="{0:C}")]
        public double OrderTotal { get; set; }

        [Required]
        [Display(Name ="PickUp Time")]
        public DateTime PickUpTime { get; set; }
        
        [NotMapped]
        [Required]
        public DateTime PickUpDate { get; set; }

        [Display(Name ="Coupon Code")]
        public string CouponCode { get; set; }

        public double CouponCodeDiscount { get; set; }

        public string Status { get; set; }

        public string PaymentStatus { get; set; }

        public string Comments { get; set; }

        [Display(Name ="Pick Up Name")]
        public string PickUpName { get; set; }

        [Required]
        [Display(Name ="Phone Number")]
        public string PhoneNumber { get; set; }

        public string TransactionId{ get; set; }


    }
}
