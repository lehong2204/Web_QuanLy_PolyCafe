    using Microsoft.EntityFrameworkCore;
    using Web_QuanLy_PolyCafe.Models;

    namespace Web_QuanLy_PolyCafe.Data
    {
        public class PolyCafeDbContext : DbContext
        {
            public PolyCafeDbContext(DbContextOptions<PolyCafeDbContext> options)
                : base(options) { }

            public DbSet<User> Users { get; set; }
            public DbSet<Category> Categories { get; set; }
            public DbSet<Drink> Drinks { get; set; }
            public DbSet<DrinkVariant> DrinkVariants { get; set; }
            public DbSet<DrinkHistory> DrinkHistories { get; set; }
            public DbSet<Voucher> Vouchers { get; set; }
            public DbSet<VoucherDetail> VoucherDetails { get; set; }
            public DbSet<VoucherScope> VoucherScopes { get; set; }
            public DbSet<Order> Orders { get; set; }
            public DbSet<OrderDetail> OrderDetails { get; set; }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

                // ===================================================
                // ==================== USERS (5) ====================
                // ===================================================
                modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        Id = "U001",
                        FullName = "Admin PolyCafe",
                        Email = "admin@polycafe.vn",
                        Password = "admin123",
                        Phone = "0900000001",
                        Address = "123 Lê Lợi, Q1, HCM",
                        Role = true,
                        CreatedAt = new DateTime(2025, 1, 1),
                        IsActive = true
                    },
                    new User
                    {
                        Id = "U002",
                        FullName = "Nguyễn Văn An",
                        Email = "an@gmail.com",
                        Password = "123456",
                        Phone = "0901234567",
                        Address = "45 Nguyễn Huệ, Q1, HCM",
                        Role = false,
                        CreatedAt = new DateTime(2025, 1, 5),
                        IsActive = true
                    },
                    new User
                    {
                        Id = "U003",
                        FullName = "Trần Thị Bích",
                        Email = "bich@gmail.com",
                        Password = "123456",
                        Phone = "0912345678",
                        Address = "78 Trần Hưng Đạo, Q5, HCM",
                        Role = false,
                        CreatedAt = new DateTime(2025, 1, 6),
                        IsActive = true
                    },
                    new User
                    {
                        Id = "U004",
                        FullName = "Lê Văn Cường",
                        Email = "cuong@gmail.com",
                        Password = "123456",
                        Phone = "0923456789",
                        Address = "12 Võ Văn Tần, Q3, HCM",
                        Role = false,
                        CreatedAt = new DateTime(2025, 1, 7),
                        IsActive = true
                    },
                    new User
                    {
                        Id = "U005",
                        FullName = "Phạm Thị Dung",
                        Email = "dung@gmail.com",
                        Password = "123456",
                        Phone = "0934567890",
                        Address = "99 CMT8, Q10, HCM",
                        Role = false,
                        CreatedAt = new DateTime(2025, 1, 8),
                        IsActive = true
                    }
                );

                // ===================================================
                // ================ CATEGORIES (20) ==================
                // ===================================================
                modelBuilder.Entity<Category>().HasData(
                    new Category { Id = "CAT01", Name = "Cà Phê", Description = "Các loại cà phê truyền thống và hiện đại", IsDeleted = false },
                    new Category { Id = "CAT02", Name = "Trà Sữa", Description = "Trà sữa Đài Loan, trân châu các loại", IsDeleted = false },
                    new Category { Id = "CAT03", Name = "Sinh Tố", Description = "Sinh tố hoa quả tươi nguyên chất", IsDeleted = false },
                    new Category { Id = "CAT04", Name = "Nước Ép", Description = "Nước ép trái cây tươi 100%", IsDeleted = false },
                    new Category { Id = "CAT05", Name = "Trà Trái Cây", Description = "Trà kết hợp hoa quả tươi", IsDeleted = false },
                    new Category { Id = "CAT06", Name = "Matcha", Description = "Thức uống từ bột trà xanh Nhật Bản", IsDeleted = false },
                    new Category { Id = "CAT07", Name = "Chocolate", Description = "Thức uống từ cacao và chocolate", IsDeleted = false },
                    new Category { Id = "CAT08", Name = "Đá Xay", Description = "Thức uống xay đá mát lạnh", IsDeleted = false },
                    new Category { Id = "CAT09", Name = "Soda", Description = "Soda kết hợp xi rô nhiều vị", IsDeleted = false },
                    new Category { Id = "CAT10", Name = "Latte", Description = "Cà phê sữa Ý kiểu latte", IsDeleted = false },
                    new Category { Id = "CAT11", Name = "Cappuccino", Description = "Cà phê cappuccino bọt sữa mịn", IsDeleted = false },
                    new Category { Id = "CAT12", Name = "Espresso", Description = "Cà phê espresso đậm đặc", IsDeleted = false },
                    new Category { Id = "CAT13", Name = "Cold Brew", Description = "Cà phê ủ lạnh 12-24 giờ", IsDeleted = false },
                    new Category { Id = "CAT14", Name = "Yogurt", Description = "Thức uống từ sữa chua", IsDeleted = false },
                    new Category { Id = "CAT15", Name = "Hoa Quả Dầm", Description = "Hoa quả dầm sữa chua, đá bào", IsDeleted = false },
                    new Category { Id = "CAT16", Name = "Trà Hoa", Description = "Trà từ hoa khô tự nhiên", IsDeleted = false },
                    new Category { Id = "CAT17", Name = "Kem", Description = "Thức uống kết hợp kem tươi", IsDeleted = false },
                    new Category { Id = "CAT18", Name = "Nước Dừa", Description = "Nước dừa tươi và biến tấu", IsDeleted = false },
                    new Category { Id = "CAT19", Name = "Chanh Muối", Description = "Chanh muối, chanh leo các loại", IsDeleted = false },
                    new Category { Id = "CAT20", Name = "Đặc Biệt", Description = "Thức uống signature của PolyCafe", IsDeleted = false }
                );

                // ===================================================
                // =================== DRINKS (20) ===================
                // ===================================================
                modelBuilder.Entity<Drink>().HasData(
                    new Drink { Id = "D001", Name = "Cà Phê Sữa Đá", Description = "Cà phê phin truyền thống pha với sữa đặc và đá", ImageUrl = "caphe_sua_da.jpg", Price = 35000, IsAvailable = true, CategoryId = "CAT01", IsDeleted = false },
                    new Drink { Id = "D002", Name = "Bạc Xỉu", Description = "Cà phê sữa tỉ lệ sữa nhiều hơn, thơm béo", ImageUrl = "bac_xiu.jpg", Price = 35000, IsAvailable = true, CategoryId = "CAT01", IsDeleted = false },
                    new Drink { Id = "D003", Name = "Trà Sữa Trân Châu Đen", Description = "Trà sữa Đài Loan với trân châu đen dẻo dai", ImageUrl = "trasua_tranchau.jpg", Price = 45000, IsAvailable = true, CategoryId = "CAT02", IsDeleted = false },
                    new Drink { Id = "D004", Name = "Trà Sữa Matcha", Description = "Trà sữa kết hợp matcha Nhật Bản thơm ngon", ImageUrl = "trasua_matcha.jpg", Price = 50000, IsAvailable = true, CategoryId = "CAT02", IsDeleted = false },
                    new Drink { Id = "D005", Name = "Sinh Tố Bơ", Description = "Sinh tố bơ sáp nguyên chất, béo thơm", ImageUrl = "sinhto_bo.jpg", Price = 55000, IsAvailable = true, CategoryId = "CAT03", IsDeleted = false },
                    new Drink { Id = "D006", Name = "Sinh Tố Xoài", Description = "Xoài cát Hòa Lộc ngọt thơm xay mịn", ImageUrl = "sinhto_xoai.jpg", Price = 50000, IsAvailable = true, CategoryId = "CAT03", IsDeleted = false },
                    new Drink { Id = "D007", Name = "Nước Ép Cam", Description = "Cam vàng tươi nguyên chất, giàu vitamin C", ImageUrl = "nuocep_cam.jpg", Price = 45000, IsAvailable = true, CategoryId = "CAT04", IsDeleted = false },
                    new Drink { Id = "D008", Name = "Nước Ép Dưa Hấu", Description = "Dưa hấu đỏ tươi ngọt mát", ImageUrl = "nuocep_duahau.jpg", Price = 40000, IsAvailable = true, CategoryId = "CAT04", IsDeleted = false },
                    new Drink { Id = "D009", Name = "Trà Đào Cam Sả", Description = "Trà hoa đào kết hợp cam và sả tươi", ImageUrl = "tradao_camsa.jpg", Price = 50000, IsAvailable = true, CategoryId = "CAT05", IsDeleted = false },
                    new Drink { Id = "D010", Name = "Trà Vải Nhãn", Description = "Trà xanh kết hợp vải và nhãn tươi mát", ImageUrl = "travai_nhan.jpg", Price = 48000, IsAvailable = true, CategoryId = "CAT05", IsDeleted = false },
                    new Drink { Id = "D011", Name = "Matcha Latte Đá", Description = "Bột matcha Nhật pha với sữa tươi và đá", ImageUrl = "matcha_latte.jpg", Price = 55000, IsAvailable = true, CategoryId = "CAT06", IsDeleted = false },
                    new Drink { Id = "D012", Name = "Matcha Đá Xay", Description = "Matcha xay đá mịn, thêm kem tươi phủ trên", ImageUrl = "matcha_daxay.jpg", Price = 60000, IsAvailable = true, CategoryId = "CAT06", IsDeleted = false },
                    new Drink { Id = "D013", Name = "Chocolate Nóng", Description = "Chocolate Bỉ đun nóng, thêm marshmallow", ImageUrl = "chocolate_nong.jpg", Price = 50000, IsAvailable = true, CategoryId = "CAT07", IsDeleted = false },
                    new Drink { Id = "D014", Name = "Chocolate Đá", Description = "Chocolate đá lạnh mát, béo ngậy", ImageUrl = "chocolate_da.jpg", Price = 50000, IsAvailable = true, CategoryId = "CAT07", IsDeleted = false },
                    new Drink { Id = "D015", Name = "Frappuccino Caramel", Description = "Cà phê xay đá caramel, phủ whipping cream", ImageUrl = "frappuccino.jpg", Price = 65000, IsAvailable = true, CategoryId = "CAT08", IsDeleted = false },
                    new Drink { Id = "D016", Name = "Soda Việt Quất", Description = "Soda xi rô việt quất màu tím quyến rũ", ImageUrl = "soda_vietquat.jpg", Price = 40000, IsAvailable = true, CategoryId = "CAT09", IsDeleted = false },
                    new Drink { Id = "D017", Name = "Latte Vani", Description = "Espresso pha với sữa hấp và xi rô vani", ImageUrl = "latte_vani.jpg", Price = 55000, IsAvailable = true, CategoryId = "CAT10", IsDeleted = false },
                    new Drink { Id = "D018", Name = "Cold Brew Cam", Description = "Cold brew ủ lạnh 20 giờ, thêm nước cam tươi", ImageUrl = "coldbrew_cam.jpg", Price = 60000, IsAvailable = true, CategoryId = "CAT13", IsDeleted = false },
                    new Drink { Id = "D019", Name = "Chanh Leo Muối", Description = "Chanh leo tươi, muối hồng, đá bào mát lạnh", ImageUrl = "chanhleo_muoi.jpg", Price = 38000, IsAvailable = true, CategoryId = "CAT19", IsDeleted = false },
                    new Drink { Id = "D020", Name = "PolyCafe Signature", Description = "Thức uống bí mật của PolyCafe - vị đặc trưng không nơi nào có", ImageUrl = "poly_signature.jpg", Price = 75000, IsAvailable = true, CategoryId = "CAT20", IsDeleted = false }
                );

                // ===================================================
                // =============== DRINK VARIANTS (20) ===============
                // ===================================================
                modelBuilder.Entity<DrinkVariant>().HasData(
                    new DrinkVariant { Id = "DV001", DrinkId = "D001", VariantName = "Size S", ExtraPrice = 0, IsActive = true },
                    new DrinkVariant { Id = "DV002", DrinkId = "D001", VariantName = "Size M", ExtraPrice = 5000, IsActive = true },
                    new DrinkVariant { Id = "DV003", DrinkId = "D001", VariantName = "Size L", ExtraPrice = 10000, IsActive = true },
                    new DrinkVariant { Id = "DV004", DrinkId = "D003", VariantName = "Ít đường", ExtraPrice = 0, IsActive = true },
                    new DrinkVariant { Id = "DV005", DrinkId = "D003", VariantName = "Không đường", ExtraPrice = 0, IsActive = true },
                    new DrinkVariant { Id = "DV006", DrinkId = "D003", VariantName = "Thêm trân châu", ExtraPrice = 8000, IsActive = true },
                    new DrinkVariant { Id = "DV007", DrinkId = "D005", VariantName = "Không đường", ExtraPrice = 0, IsActive = true },
                    new DrinkVariant { Id = "DV008", DrinkId = "D005", VariantName = "Thêm sữa đặc", ExtraPrice = 5000, IsActive = true },
                    new DrinkVariant { Id = "DV009", DrinkId = "D011", VariantName = "Matcha vị nhẹ", ExtraPrice = 0, IsActive = true },
                    new DrinkVariant { Id = "DV010", DrinkId = "D011", VariantName = "Matcha vị đậm", ExtraPrice = 5000, IsActive = true },
                    new DrinkVariant { Id = "DV011", DrinkId = "D015", VariantName = "Thêm caramel drizzle", ExtraPrice = 5000, IsActive = true },
                    new DrinkVariant { Id = "DV012", DrinkId = "D015", VariantName = "Thêm extra espresso", ExtraPrice = 10000, IsActive = true },
                    new DrinkVariant { Id = "DV013", DrinkId = "D017", VariantName = "Sữa hạnh nhân", ExtraPrice = 10000, IsActive = true },
                    new DrinkVariant { Id = "DV014", DrinkId = "D017", VariantName = "Sữa yến mạch", ExtraPrice = 10000, IsActive = true },
                    new DrinkVariant { Id = "DV015", DrinkId = "D020", VariantName = "Size S", ExtraPrice = 0, IsActive = true },
                    new DrinkVariant { Id = "DV016", DrinkId = "D020", VariantName = "Size L", ExtraPrice = 15000, IsActive = true },
                    new DrinkVariant { Id = "DV017", DrinkId = "D009", VariantName = "Ít đá", ExtraPrice = 0, IsActive = true },
                    new DrinkVariant { Id = "DV018", DrinkId = "D009", VariantName = "Không đá", ExtraPrice = 0, IsActive = true },
                    new DrinkVariant { Id = "DV019", DrinkId = "D012", VariantName = "Thêm kem tươi", ExtraPrice = 8000, IsActive = true },
                    new DrinkVariant { Id = "DV020", DrinkId = "D013", VariantName = "Thêm marshmallow", ExtraPrice = 5000, IsActive = true }
                );

                // ===================================================
                // ============== DRINK HISTORIES (20) ===============
                // ===================================================
                modelBuilder.Entity<DrinkHistory>().HasData(
                    new DrinkHistory { Id = "DH001", DrinkId = "D001", Name = "Cà Phê Sữa Đá", Description = "Cà phê phin truyền thống", ImageUrl = "caphe_sua_da.jpg", Price = 30000, CategoryId = "CAT01", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 10) },
                    new DrinkHistory { Id = "DH002", DrinkId = "D002", Name = "Bạc Xỉu", Description = "Cà phê sữa tỉ lệ sữa nhiều hơn", ImageUrl = "bac_xiu.jpg", Price = 30000, CategoryId = "CAT01", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 10) },
                    new DrinkHistory { Id = "DH003", DrinkId = "D003", Name = "Trà Sữa Trân Châu Đen", Description = "Trà sữa Đài Loan", ImageUrl = "trasua_tranchau.jpg", Price = 40000, CategoryId = "CAT02", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 11) },
                    new DrinkHistory { Id = "DH004", DrinkId = "D004", Name = "Trà Sữa Matcha", Description = "Trà sữa matcha Nhật", ImageUrl = "trasua_matcha.jpg", Price = 45000, CategoryId = "CAT02", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 11) },
                    new DrinkHistory { Id = "DH005", DrinkId = "D005", Name = "Sinh Tố Bơ", Description = "Sinh tố bơ sáp", ImageUrl = "sinhto_bo.jpg", Price = 50000, CategoryId = "CAT03", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 12) },
                    new DrinkHistory { Id = "DH006", DrinkId = "D006", Name = "Sinh Tố Xoài", Description = "Xoài cát Hòa Lộc", ImageUrl = "sinhto_xoai.jpg", Price = 45000, CategoryId = "CAT03", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 12) },
                    new DrinkHistory { Id = "DH007", DrinkId = "D007", Name = "Nước Ép Cam", Description = "Cam vàng nguyên chất", ImageUrl = "nuocep_cam.jpg", Price = 40000, CategoryId = "CAT04", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 13) },
                    new DrinkHistory { Id = "DH008", DrinkId = "D008", Name = "Nước Ép Dưa Hấu", Description = "Dưa hấu đỏ tươi", ImageUrl = "nuocep_duahau.jpg", Price = 35000, CategoryId = "CAT04", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 13) },
                    new DrinkHistory { Id = "DH009", DrinkId = "D009", Name = "Trà Đào Cam Sả", Description = "Trà hoa đào", ImageUrl = "tradao_camsa.jpg", Price = 45000, CategoryId = "CAT05", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 14) },
                    new DrinkHistory { Id = "DH010", DrinkId = "D010", Name = "Trà Vải Nhãn", Description = "Trà xanh vải nhãn", ImageUrl = "travai_nhan.jpg", Price = 43000, CategoryId = "CAT05", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 14) },
                    new DrinkHistory { Id = "DH011", DrinkId = "D011", Name = "Matcha Latte Đá", Description = "Matcha sữa đá", ImageUrl = "matcha_latte.jpg", Price = 50000, CategoryId = "CAT06", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 15) },
                    new DrinkHistory { Id = "DH012", DrinkId = "D012", Name = "Matcha Đá Xay", Description = "Matcha xay đá", ImageUrl = "matcha_daxay.jpg", Price = 55000, CategoryId = "CAT06", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 15) },
                    new DrinkHistory { Id = "DH013", DrinkId = "D013", Name = "Chocolate Nóng", Description = "Chocolate Bỉ nóng", ImageUrl = "chocolate_nong.jpg", Price = 45000, CategoryId = "CAT07", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 16) },
                    new DrinkHistory { Id = "DH014", DrinkId = "D014", Name = "Chocolate Đá", Description = "Chocolate đá lạnh", ImageUrl = "chocolate_da.jpg", Price = 45000, CategoryId = "CAT07", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 16) },
                    new DrinkHistory { Id = "DH015", DrinkId = "D015", Name = "Frappuccino Caramel", Description = "Cà phê xay caramel", ImageUrl = "frappuccino.jpg", Price = 60000, CategoryId = "CAT08", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 17) },
                    new DrinkHistory { Id = "DH016", DrinkId = "D016", Name = "Soda Việt Quất", Description = "Soda việt quất", ImageUrl = "soda_vietquat.jpg", Price = 35000, CategoryId = "CAT09", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 17) },
                    new DrinkHistory { Id = "DH017", DrinkId = "D017", Name = "Latte Vani", Description = "Latte xi rô vani", ImageUrl = "latte_vani.jpg", Price = 50000, CategoryId = "CAT10", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 18) },
                    new DrinkHistory { Id = "DH018", DrinkId = "D018", Name = "Cold Brew Cam", Description = "Cold brew cam tươi", ImageUrl = "coldbrew_cam.jpg", Price = 55000, CategoryId = "CAT13", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 18) },
                    new DrinkHistory { Id = "DH019", DrinkId = "D019", Name = "Chanh Leo Muối", Description = "Chanh leo muối đá", ImageUrl = "chanhleo_muoi.jpg", Price = 33000, CategoryId = "CAT19", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 19) },
                    new DrinkHistory { Id = "DH020", DrinkId = "D020", Name = "PolyCafe Signature", Description = "Thức uống bí mật cũ", ImageUrl = "poly_signature.jpg", Price = 70000, CategoryId = "CAT20", IsAvailable = true, IsDeleted = false, ChangedAt = new DateTime(2025, 1, 20) }
                );

                // ===================================================
                // ================== VOUCHERS (20) ==================
                // ===================================================
                modelBuilder.Entity<Voucher>().HasData(
                    new Voucher { Id = "V001", Code = "WELCOME10", Name = "Chào mừng khách mới", DiscountType = "percent", DiscountValue = 10, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 100, UsedCount = 5, IsActive = true },
                    new Voucher { Id = "V002", Code = "GIAM20K", Name = "Giảm 20.000đ đơn từ 100k", DiscountType = "fixed", DiscountValue = 20000, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 6, 30), UsageLimit = 50, UsedCount = 10, IsActive = true },
                    new Voucher { Id = "V003", Code = "POLY15", Name = "PolyCafe 15%", DiscountType = "percent", DiscountValue = 15, StartDate = new DateTime(2025, 2, 1), EndDate = new DateTime(2025, 2, 28), UsageLimit = 200, UsedCount = 80, IsActive = false },
                    new Voucher { Id = "V004", Code = "FREESHIP", Name = "Miễn phí giao hàng", DiscountType = "fixed", DiscountValue = 15000, StartDate = new DateTime(2025, 1, 15), EndDate = new DateTime(2025, 3, 31), UsageLimit = 300, UsedCount = 45, IsActive = true },
                    new Voucher { Id = "V005", Code = "TETHOLIDAY", Name = "Khuyến mãi Tết 2025", DiscountType = "percent", DiscountValue = 20, StartDate = new DateTime(2025, 1, 25), EndDate = new DateTime(2025, 2, 5), UsageLimit = 500, UsedCount = 320, IsActive = false },
                    new Voucher { Id = "V006", Code = "BIRTHDAY30", Name = "Sinh nhật giảm 30%", DiscountType = "percent", DiscountValue = 30, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 1, UsedCount = 0, IsActive = true },
                    new Voucher { Id = "V007", Code = "COMBO50K", Name = "Giảm 50k đơn combo", DiscountType = "fixed", DiscountValue = 50000, StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2025, 3, 31), UsageLimit = 100, UsedCount = 20, IsActive = true },
                    new Voucher { Id = "V008", Code = "SUMMER25", Name = "Summer Sale 25%", DiscountType = "percent", DiscountValue = 25, StartDate = new DateTime(2025, 6, 1), EndDate = new DateTime(2025, 8, 31), UsageLimit = 400, UsedCount = 0, IsActive = true },
                    new Voucher { Id = "V009", Code = "LOYAL5", Name = "Khách thân thiết 5%", DiscountType = "percent", DiscountValue = 5, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 9999, UsedCount = 150, IsActive = true },
                    new Voucher { Id = "V010", Code = "FIRSTAPP", Name = "Lần đầu đặt app", DiscountType = "fixed", DiscountValue = 30000, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 1, UsedCount = 0, IsActive = true },
                    new Voucher { Id = "V011", Code = "FLASH20", Name = "Flash Sale 20%", DiscountType = "percent", DiscountValue = 20, StartDate = new DateTime(2025, 2, 14), EndDate = new DateTime(2025, 2, 14), UsageLimit = 50, UsedCount = 50, IsActive = false },
                    new Voucher { Id = "V012", Code = "WEEKDAY10", Name = "Ngày thường giảm 10%", DiscountType = "percent", DiscountValue = 10, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 9999, UsedCount = 200, IsActive = true },
                    new Voucher { Id = "V013", Code = "MORNING15K", Name = "Buổi sáng giảm 15k", DiscountType = "fixed", DiscountValue = 15000, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 6, 30), UsageLimit = 200, UsedCount = 60, IsActive = true },
                    new Voucher { Id = "V014", Code = "GROUP100K", Name = "Nhóm đặt giảm 100k", DiscountType = "fixed", DiscountValue = 100000, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 30, UsedCount = 5, IsActive = true },
                    new Voucher { Id = "V015", Code = "ECO10", Name = "Giảm 10% mang ly riêng", DiscountType = "percent", DiscountValue = 10, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 9999, UsedCount = 90, IsActive = true },
                    new Voucher { Id = "V016", Code = "REVIEW20K", Name = "Đánh giá 5 sao giảm 20k", DiscountType = "fixed", DiscountValue = 20000, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 500, UsedCount = 30, IsActive = true },
                    new Voucher { Id = "V017", Code = "NEWMENU", Name = "Thử menu mới giảm 10%", DiscountType = "percent", DiscountValue = 10, StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2025, 4, 30), UsageLimit = 100, UsedCount = 0, IsActive = true },
                    new Voucher { Id = "V018", Code = "RAINY10K", Name = "Ngày mưa giảm 10k", DiscountType = "fixed", DiscountValue = 10000, StartDate = new DateTime(2025, 5, 1), EndDate = new DateTime(2025, 10, 31), UsageLimit = 9999, UsedCount = 0, IsActive = true },
                    new Voucher { Id = "V019", Code = "MIDNIGHT", Name = "Đêm khuya giảm 15%", DiscountType = "percent", DiscountValue = 15, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 12, 31), UsageLimit = 100, UsedCount = 10, IsActive = true },
                    new Voucher { Id = "V020", Code = "POLY2025", Name = "PolyCafe kỷ niệm 2025", DiscountType = "percent", DiscountValue = 25, StartDate = new DateTime(2025, 1, 1), EndDate = new DateTime(2025, 1, 7), UsageLimit = 1000, UsedCount = 750, IsActive = false }
                );

                // ===================================================
                // ============== VOUCHER DETAILS (20) ===============
                // ===================================================
                modelBuilder.Entity<VoucherDetail>().HasData(
                    new VoucherDetail { Id = "VD001", VoucherId = "V001", ConditionType = "MinOrder", ConditionValue = 50000, Description = "Đơn tối thiểu 50.000đ" },
                    new VoucherDetail { Id = "VD002", VoucherId = "V002", ConditionType = "MinOrder", ConditionValue = 100000, Description = "Đơn tối thiểu 100.000đ" },
                    new VoucherDetail { Id = "VD003", VoucherId = "V003", ConditionType = "MinOrder", ConditionValue = 80000, Description = "Đơn tối thiểu 80.000đ" },
                    new VoucherDetail { Id = "VD004", VoucherId = "V004", ConditionType = "MinOrder", ConditionValue = 60000, Description = "Đơn tối thiểu 60.000đ" },
                    new VoucherDetail { Id = "VD005", VoucherId = "V005", ConditionType = "MinOrder", ConditionValue = 100000, Description = "Đơn tối thiểu 100.000đ" },
                    new VoucherDetail { Id = "VD006", VoucherId = "V006", ConditionType = "BirthdayMonth", ConditionValue = 1, Description = "Áp dụng trong tháng sinh nhật" },
                    new VoucherDetail { Id = "VD007", VoucherId = "V007", ConditionType = "MinOrder", ConditionValue = 150000, Description = "Đơn combo tối thiểu 150.000đ" },
                    new VoucherDetail { Id = "VD008", VoucherId = "V008", ConditionType = "MinOrder", ConditionValue = 70000, Description = "Đơn tối thiểu 70.000đ" },
                    new VoucherDetail { Id = "VD009", VoucherId = "V009", ConditionType = "MinOrder", ConditionValue = 0, Description = "Không giới hạn đơn hàng" },
                    new VoucherDetail { Id = "VD010", VoucherId = "V010", ConditionType = "FirstOrder", ConditionValue = 1, Description = "Chỉ áp dụng đơn hàng đầu tiên" },
                    new VoucherDetail { Id = "VD011", VoucherId = "V011", ConditionType = "MinOrder", ConditionValue = 50000, Description = "Flash sale đơn từ 50k" },
                    new VoucherDetail { Id = "VD012", VoucherId = "V012", ConditionType = "DayOfWeek", ConditionValue = 5, Description = "Thứ 2 đến thứ 6" },
                    new VoucherDetail { Id = "VD013", VoucherId = "V013", ConditionType = "TimeRange", ConditionValue = 9, Description = "Áp dụng 6h-9h sáng" },
                    new VoucherDetail { Id = "VD014", VoucherId = "V014", ConditionType = "MinOrder", ConditionValue = 300000, Description = "Đơn nhóm tối thiểu 300.000đ" },
                    new VoucherDetail { Id = "VD015", VoucherId = "V015", ConditionType = "EcoCup", ConditionValue = 1, Description = "Mang ly cá nhân khi đến quán" },
                    new VoucherDetail { Id = "VD016", VoucherId = "V016", ConditionType = "ReviewRequired", ConditionValue = 5, Description = "Đánh giá 5 sao lần đặt trước" },
                    new VoucherDetail { Id = "VD017", VoucherId = "V017", ConditionType = "NewItem", ConditionValue = 1, Description = "Áp dụng khi đặt món trong menu mới" },
                    new VoucherDetail { Id = "VD018", VoucherId = "V018", ConditionType = "MinOrder", ConditionValue = 40000, Description = "Đơn tối thiểu 40.000đ" },
                    new VoucherDetail { Id = "VD019", VoucherId = "V019", ConditionType = "TimeRange", ConditionValue = 22, Description = "Áp dụng từ 22h - 24h" },
                    new VoucherDetail { Id = "VD020", VoucherId = "V020", ConditionType = "MinOrder", ConditionValue = 50000, Description = "Kỷ niệm thành lập quán" }
                );

                // ===================================================
                // ============== VOUCHER SCOPES (20) ================
                // ===================================================
                modelBuilder.Entity<VoucherScope>().HasData(
                    new VoucherScope { Id = "VS001", VoucherId = "V001", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS002", VoucherId = "V002", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS003", VoucherId = "V003", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS004", VoucherId = "V004", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS005", VoucherId = "V005", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS006", VoucherId = "V006", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS007", VoucherId = "V007", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS008", VoucherId = "V008", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS009", VoucherId = "V009", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS010", VoucherId = "V010", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS011", VoucherId = "V011", ApplyType = 1, DrinkId = "D020" },
                    new VoucherScope { Id = "VS012", VoucherId = "V012", ApplyType = 1, DrinkId = "D001" },
                    new VoucherScope { Id = "VS013", VoucherId = "V013", ApplyType = 1, DrinkId = "D002" },
                    new VoucherScope { Id = "VS014", VoucherId = "V014", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS015", VoucherId = "V015", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS016", VoucherId = "V016", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS017", VoucherId = "V017", ApplyType = 1, DrinkId = "D019" },
                    new VoucherScope { Id = "VS018", VoucherId = "V018", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS019", VoucherId = "V019", ApplyType = 0, DrinkId = null },
                    new VoucherScope { Id = "VS020", VoucherId = "V020", ApplyType = 0, DrinkId = null }
                );

                // ===================================================
                // =================== ORDERS (20) ===================
                // ===================================================
                modelBuilder.Entity<Order>().HasData(
                    new Order { Id = "ORD001", UserId = "U002", OrderDate = new DateTime(2025, 1, 10, 8, 30, 0), TotalPrice = 80000, DiscountAmount = 0, Status = "Completed", VoucherId = null },
                    new Order { Id = "ORD002", UserId = "U003", OrderDate = new DateTime(2025, 1, 11, 9, 0, 0), TotalPrice = 90000, DiscountAmount = 9000, Status = "Completed", VoucherId = "V001" },
                    new Order { Id = "ORD003", UserId = "U004", OrderDate = new DateTime(2025, 1, 12, 10, 15, 0), TotalPrice = 105000, DiscountAmount = 20000, Status = "Completed", VoucherId = "V002" },
                    new Order { Id = "ORD004", UserId = "U005", OrderDate = new DateTime(2025, 1, 13, 14, 0, 0), TotalPrice = 70000, DiscountAmount = 0, Status = "Completed", VoucherId = null },
                    new Order { Id = "ORD005", UserId = "U002", OrderDate = new DateTime(2025, 1, 14, 15, 30, 0), TotalPrice = 120000, DiscountAmount = 15000, Status = "Completed", VoucherId = "V004" },
                    new Order { Id = "ORD006", UserId = "U003", OrderDate = new DateTime(2025, 1, 15, 8, 0, 0), TotalPrice = 55000, DiscountAmount = 0, Status = "Pending", VoucherId = null },
                    new Order { Id = "ORD007", UserId = "U004", OrderDate = new DateTime(2025, 1, 16, 11, 0, 0), TotalPrice = 160000, DiscountAmount = 0, Status = "Completed", VoucherId = null },
                    new Order { Id = "ORD008", UserId = "U005", OrderDate = new DateTime(2025, 1, 17, 16, 0, 0), TotalPrice = 75000, DiscountAmount = 0, Status = "Cancelled", VoucherId = null },
                    new Order { Id = "ORD009", UserId = "U002", OrderDate = new DateTime(2025, 1, 18, 9, 30, 0), TotalPrice = 200000, DiscountAmount = 40000, Status = "Completed", VoucherId = "V005" },
                    new Order { Id = "ORD010", UserId = "U003", OrderDate = new DateTime(2025, 1, 19, 10, 0, 0), TotalPrice = 85000, DiscountAmount = 0, Status = "Completed", VoucherId = null },
                    new Order { Id = "ORD011", UserId = "U004", OrderDate = new DateTime(2025, 1, 20, 13, 0, 0), TotalPrice = 110000, DiscountAmount = 11000, Status = "Completed", VoucherId = "V001" },
                    new Order { Id = "ORD012", UserId = "U005", OrderDate = new DateTime(2025, 1, 21, 14, 30, 0), TotalPrice = 95000, DiscountAmount = 0, Status = "Pending", VoucherId = null },
                    new Order { Id = "ORD013", UserId = "U002", OrderDate = new DateTime(2025, 1, 22, 8, 0, 0), TotalPrice = 45000, DiscountAmount = 0, Status = "Completed", VoucherId = null },
                    new Order { Id = "ORD014", UserId = "U003", OrderDate = new DateTime(2025, 1, 23, 17, 0, 0), TotalPrice = 135000, DiscountAmount = 20000, Status = "Completed", VoucherId = "V002" },
                    new Order { Id = "ORD015", UserId = "U004", OrderDate = new DateTime(2025, 1, 24, 9, 0, 0), TotalPrice = 60000, DiscountAmount = 6000, Status = "Completed", VoucherId = "V009" },
                    new Order { Id = "ORD016", UserId = "U005", OrderDate = new DateTime(2025, 1, 25, 10, 30, 0), TotalPrice = 180000, DiscountAmount = 36000, Status = "Completed", VoucherId = "V005" },
                    new Order { Id = "ORD017", UserId = "U002", OrderDate = new DateTime(2025, 1, 26, 11, 0, 0), TotalPrice = 50000, DiscountAmount = 0, Status = "Completed", VoucherId = null },
                    new Order { Id = "ORD018", UserId = "U003", OrderDate = new DateTime(2025, 1, 27, 14, 0, 0), TotalPrice = 145000, DiscountAmount = 15000, Status = "Completed", VoucherId = "V004" },
                    new Order { Id = "ORD019", UserId = "U004", OrderDate = new DateTime(2025, 1, 28, 19, 0, 0), TotalPrice = 75000, DiscountAmount = 11250, Status = "Completed", VoucherId = "V019" },
                    new Order { Id = "ORD020", UserId = "U005", OrderDate = new DateTime(2025, 1, 29, 10, 0, 0), TotalPrice = 225000, DiscountAmount = 56250, Status = "Completed", VoucherId = "V020" }
                );

                // ===================================================
                // =============== ORDER DETAILS (20) ================
                // ===================================================
                modelBuilder.Entity<OrderDetail>().HasData(
                    new OrderDetail { Id = "OD001", OrderId = "ORD001", DrinkId = "D001", VariantId = "DV002", Quantity = 2, UnitPrice = 40000 },
                    new OrderDetail { Id = "OD002", OrderId = "ORD002", DrinkId = "D003", VariantId = "DV006", Quantity = 2, UnitPrice = 53000 },
                    new OrderDetail { Id = "OD003", OrderId = "ORD003", DrinkId = "D005", VariantId = "DV008", Quantity = 1, UnitPrice = 60000 },
                    new OrderDetail { Id = "OD004", OrderId = "ORD003", DrinkId = "D009", VariantId = null, Quantity = 1, UnitPrice = 50000 },
                    new OrderDetail { Id = "OD005", OrderId = "ORD004", DrinkId = "D007", VariantId = null, Quantity = 1, UnitPrice = 45000 },
                    new OrderDetail { Id = "OD006", OrderId = "ORD004", DrinkId = "D019", VariantId = null, Quantity = 1, UnitPrice = 38000 },
                    new OrderDetail { Id = "OD007", OrderId = "ORD005", DrinkId = "D015", VariantId = "DV011", Quantity = 1, UnitPrice = 70000 },
                    new OrderDetail { Id = "OD008", OrderId = "ORD005", DrinkId = "D011", VariantId = "DV010", Quantity = 1, UnitPrice = 60000 },
                    new OrderDetail { Id = "OD009", OrderId = "ORD006", DrinkId = "D002", VariantId = null, Quantity = 1, UnitPrice = 35000 },
                    new OrderDetail { Id = "OD010", OrderId = "ORD007", DrinkId = "D020", VariantId = "DV016", Quantity = 2, UnitPrice = 90000 },
                    new OrderDetail { Id = "OD011", OrderId = "ORD008", DrinkId = "D013", VariantId = "DV020", Quantity = 1, UnitPrice = 55000 },
                    new OrderDetail { Id = "OD012", OrderId = "ORD009", DrinkId = "D015", VariantId = "DV012", Quantity = 2, UnitPrice = 75000 },
                    new OrderDetail { Id = "OD013", OrderId = "ORD010", DrinkId = "D004", VariantId = null, Quantity = 1, UnitPrice = 50000 },
                    new OrderDetail { Id = "OD014", OrderId = "ORD010", DrinkId = "D016", VariantId = null, Quantity = 1, UnitPrice = 40000 },
                    new OrderDetail { Id = "OD015", OrderId = "ORD011", DrinkId = "D018", VariantId = null, Quantity = 1, UnitPrice = 60000 },
                    new OrderDetail { Id = "OD016", OrderId = "ORD012", DrinkId = "D006", VariantId = null, Quantity = 2, UnitPrice = 50000 },
                    new OrderDetail { Id = "OD017", OrderId = "ORD013", DrinkId = "D001", VariantId = "DV001", Quantity = 1, UnitPrice = 35000 },
                    new OrderDetail { Id = "OD018", OrderId = "ORD014", DrinkId = "D012", VariantId = "DV019", Quantity = 2, UnitPrice = 68000 },
                    new OrderDetail { Id = "OD019", OrderId = "ORD015", DrinkId = "D017", VariantId = "DV013", Quantity = 1, UnitPrice = 65000 },
                    new OrderDetail { Id = "OD020", OrderId = "ORD016", DrinkId = "D020", VariantId = "DV015", Quantity = 2, UnitPrice = 75000 }
                );
            }
        }
    }