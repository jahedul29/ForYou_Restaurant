using ForYou.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForYou.Utility
{
    public class SD
    {
        public const string DefaultFood = "constImage.jpg";

        public const string ManagerUser = "Manager";
        public const string KitcheUser = "Kitchen";
        public const string FrontDestUser = "FrontDesk";
        public const string CustomerEndUser = "Customer";

        public const string ssShoppingCartCount = "ssCartCount";
        public const string ssCouponCode = "ssCouponCode";

        public const string StatusSubmitted = "Submitted";
        public const string StatusInProcess = "Being Prepared";
        public const string StatusReady = "Ready To Pickup";
        public const string StatusCompleted = "Completed";
        public const string StatusCencelled = "Cencelled";

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusRejected = "Rejected";




        public static string ConvertToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;

            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

        public static double DiscountedPrice(Coupon coupon, double OriginalOrderTotal)
        {
            if (coupon == null)
            {
                return OriginalOrderTotal;
            }
            else
            {
                if (coupon.MinimumAmount > OriginalOrderTotal)
                {
                    return OriginalOrderTotal;
                }
                else if(Convert.ToInt32(coupon.CouponType) == (int)Coupon.ECouponType.Dollar)
                {
                    return Math.Round(OriginalOrderTotal - coupon.Discount, 2);
                }
                else
                {
                    if (Convert.ToInt32(coupon.CouponType) == (int)Coupon.ECouponType.Percent)
                    {
                        return Math.Round(OriginalOrderTotal - OriginalOrderTotal*coupon.Discount/100, 2);
                    }
                }
            }
            return OriginalOrderTotal;
        }
    }
}
