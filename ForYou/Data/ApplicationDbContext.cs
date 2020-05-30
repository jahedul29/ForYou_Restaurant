﻿using System;
using System.Collections.Generic;
using System.Text;
using ForYou.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ForYou.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<SubCategory> SubCategories{ get; set; }
        public DbSet<MenuItem> MenuItems{ get; set; }
        public DbSet<Coupon> Coupons{ get; set; }
        public DbSet<ApplicationUser> ApplicationUsers{ get; set; }
        public DbSet<ShoppingCart> ShoppingCart { get; set; }
    }
}
