using System;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.Infrastructure.Annotations;

namespace ShoeStore.Models
{
    public class ShoeStoreDBContext : DbContext
    {
        public ShoeStoreDBContext() : base("MyConnectionString") { }

        public DbSet<Users> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<Products> Products { get; set; }
        public DbSet<ProductVariants> ProductVariants { get; set; }
        public DbSet<ProductImages> ProductImages { get; set; }
        public DbSet<Reviews> Reviews { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Orders> Orders { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<Cart> Cart { get; set; }
        public DbSet<PasswordResetToken> PasswordResetTokens { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Users>()
                .Property(u => u.createdAt).HasColumnType("datetime");
            modelBuilder.Entity<Users>()
                .Property(u => u.lastLogin).HasColumnType("datetime");
            modelBuilder.Entity<UserRoles>()
                .Property(ur => ur.AssignedAt).HasColumnType("datetime");
            modelBuilder.Entity<Orders>()
                .Property(o => o.OrderDate).HasColumnType("datetime");
            modelBuilder.Entity<Reviews>()
                .Property(r => r.ReviewDate).HasColumnType("datetime");
            modelBuilder.Entity<Cart>()
                .Property(c => c.AddedAt).HasColumnType("datetime");
            modelBuilder.Entity<Products>()
                .Property(p => p.CreatedAt).HasColumnType("datetime");
            modelBuilder.Entity<PasswordResetToken>()
                .Property(t => t.CreatedAt).HasColumnType("datetime");
            modelBuilder.Entity<PasswordResetToken>()
                .Property(t => t.ExpiresAt).HasColumnType("datetime");
            modelBuilder.Entity<PasswordResetToken>()
                .Property(t => t.UsedAt).HasColumnType("datetime");

            modelBuilder.Entity<Products>()
                .Property(p => p.Price).HasPrecision(12, 2);
            modelBuilder.Entity<Products>()
                .Property(p => p.PromotionPrice).HasPrecision(12, 2);
            modelBuilder.Entity<Products>()
                .Property(p => p.Rating).HasPrecision(3, 2);
            modelBuilder.Entity<ProductVariants>()
                .Property(v => v.PriceOverride).HasPrecision(12, 2);
            modelBuilder.Entity<OrderItems>()
                .Property(oi => oi.UnitPrice).HasPrecision(12, 2);
            modelBuilder.Entity<Orders>()
                .Property(o => o.TotalAmount).HasPrecision(12, 2);

            modelBuilder.Entity<ProductVariants>()
                .Property(v => v.ProductID).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UQ_Product_Size_Color", 1) { IsUnique = true }));
            modelBuilder.Entity<ProductVariants>()
                .Property(v => v.SizeValue).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UQ_Product_Size_Color", 2) { IsUnique = true }));
            modelBuilder.Entity<ProductVariants>()
                .Property(v => v.ColorName).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UQ_Product_Size_Color", 3) { IsUnique = true }));

            modelBuilder.Entity<Reviews>()
                .Property(r => r.ProductID).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UQ_Review_User_Product", 1) { IsUnique = true }));
            modelBuilder.Entity<Reviews>()
                .Property(r => r.UserID).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UQ_Review_User_Product", 2) { IsUnique = true }));

            modelBuilder.Entity<Cart>()
                .Property(c => c.UserID).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UQ_Cart_User_Variant", 1) { IsUnique = true }));
            modelBuilder.Entity<Cart>()
                .Property(c => c.VariantID).HasColumnAnnotation(
                    IndexAnnotation.AnnotationName,
                    new IndexAnnotation(new IndexAttribute("UQ_Cart_User_Variant", 2) { IsUnique = true }));

            modelBuilder.Entity<Products>()
                .HasRequired(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ProductVariants>()
                .HasRequired(v => v.Product)
                .WithMany(p => p.ProductVariants)
                .HasForeignKey(v => v.ProductID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ProductImages>()
                .HasRequired(pi => pi.Product)
                .WithMany(p => p.ProductImages)
                .HasForeignKey(pi => pi.ProductID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Reviews>()
                .HasRequired(r => r.Product)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProductID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Reviews>()
                .HasRequired(r => r.Users)
                .WithMany(u => u.Reviews)  
                .HasForeignKey(r => r.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Address>()
                .HasRequired(a => a.Users)
                .WithMany(u => u.Addresses)  
                .HasForeignKey(a => a.UserID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Orders>()
                .HasRequired(o => o.Users)
                .WithMany(u => u.Orders)  
                .HasForeignKey(o => o.UserID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<OrderItems>()
                .HasRequired(oi => oi.Order)
                .WithMany(o => o.OrderItems)
                .HasForeignKey(oi => oi.OrderID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<OrderItems>()
                .HasRequired(oi => oi.ProductVariant)
                .WithMany()
                .HasForeignKey(oi => oi.VariantID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Cart>()
                .HasRequired(c => c.Users)
                .WithMany(u => u.Cart)  
                .HasForeignKey(c => c.UserID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<Cart>()
                .HasRequired(c => c.ProductVariant)
                .WithMany()
                .HasForeignKey(c => c.VariantID)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserRoles>()
                .HasRequired(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.UserID)
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<UserRoles>()
                .HasRequired(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleID)
                .WillCascadeOnDelete(true);

            base.OnModelCreating(modelBuilder);
        }
    }
}
