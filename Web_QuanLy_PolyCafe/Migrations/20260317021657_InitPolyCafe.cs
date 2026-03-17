using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Web_QuanLy_PolyCafe.Migrations
{
    /// <inheritdoc />
    public partial class InitPolyCafe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Role = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vouchers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    DiscountType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    DiscountValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageLimit = table.Column<int>(type: "int", nullable: true),
                    UsedCount = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vouchers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Drinks",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CategoryId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Drinks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Drinks_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DiscountAmount = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Orders_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "VoucherDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConditionType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ConditionValue = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherDetails_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrinkHistories",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DrinkId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ImageUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    CategoryId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: true),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrinkHistories_Drinks_DrinkId",
                        column: x => x.DrinkId,
                        principalTable: "Drinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DrinkVariants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DrinkId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VariantName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ExtraPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DrinkVariants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DrinkVariants_Drinks_DrinkId",
                        column: x => x.DrinkId,
                        principalTable: "Drinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VoucherScopes",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VoucherId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApplyType = table.Column<int>(type: "int", nullable: false),
                    DrinkId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VoucherScopes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VoucherScopes_Drinks_DrinkId",
                        column: x => x.DrinkId,
                        principalTable: "Drinks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_VoucherScopes_Vouchers_VoucherId",
                        column: x => x.VoucherId,
                        principalTable: "Vouchers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetails",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    OrderId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DrinkId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    VariantId = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(10,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrderDetails_DrinkVariants_VariantId",
                        column: x => x.VariantId,
                        principalTable: "DrinkVariants",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrderDetails_Drinks_DrinkId",
                        column: x => x.DrinkId,
                        principalTable: "Drinks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetails_Orders_OrderId",
                        column: x => x.OrderId,
                        principalTable: "Orders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Categories",
                columns: new[] { "Id", "Description", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { "CAT01", "Các loại cà phê truyền thống và hiện đại", false, "Cà Phê" },
                    { "CAT02", "Trà sữa Đài Loan, trân châu các loại", false, "Trà Sữa" },
                    { "CAT03", "Sinh tố hoa quả tươi nguyên chất", false, "Sinh Tố" },
                    { "CAT04", "Nước ép trái cây tươi 100%", false, "Nước Ép" },
                    { "CAT05", "Trà kết hợp hoa quả tươi", false, "Trà Trái Cây" },
                    { "CAT06", "Thức uống từ bột trà xanh Nhật Bản", false, "Matcha" },
                    { "CAT07", "Thức uống từ cacao và chocolate", false, "Chocolate" },
                    { "CAT08", "Thức uống xay đá mát lạnh", false, "Đá Xay" },
                    { "CAT09", "Soda kết hợp xi rô nhiều vị", false, "Soda" },
                    { "CAT10", "Cà phê sữa Ý kiểu latte", false, "Latte" },
                    { "CAT11", "Cà phê cappuccino bọt sữa mịn", false, "Cappuccino" },
                    { "CAT12", "Cà phê espresso đậm đặc", false, "Espresso" },
                    { "CAT13", "Cà phê ủ lạnh 12-24 giờ", false, "Cold Brew" },
                    { "CAT14", "Thức uống từ sữa chua", false, "Yogurt" },
                    { "CAT15", "Hoa quả dầm sữa chua, đá bào", false, "Hoa Quả Dầm" },
                    { "CAT16", "Trà từ hoa khô tự nhiên", false, "Trà Hoa" },
                    { "CAT17", "Thức uống kết hợp kem tươi", false, "Kem" },
                    { "CAT18", "Nước dừa tươi và biến tấu", false, "Nước Dừa" },
                    { "CAT19", "Chanh muối, chanh leo các loại", false, "Chanh Muối" },
                    { "CAT20", "Thức uống signature của PolyCafe", false, "Đặc Biệt" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Address", "CreatedAt", "Email", "FullName", "IsActive", "Password", "Phone", "Role" },
                values: new object[,]
                {
                    { "U001", "123 Lê Lợi, Q1, HCM", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin@polycafe.vn", "Admin PolyCafe", true, "admin123", "0900000001", true },
                    { "U002", "45 Nguyễn Huệ, Q1, HCM", new DateTime(2025, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "an@gmail.com", "Nguyễn Văn An", true, "123456", "0901234567", false },
                    { "U003", "78 Trần Hưng Đạo, Q5, HCM", new DateTime(2025, 1, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "bich@gmail.com", "Trần Thị Bích", true, "123456", "0912345678", false },
                    { "U004", "12 Võ Văn Tần, Q3, HCM", new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), "cuong@gmail.com", "Lê Văn Cường", true, "123456", "0923456789", false },
                    { "U005", "99 CMT8, Q10, HCM", new DateTime(2025, 1, 8, 0, 0, 0, 0, DateTimeKind.Unspecified), "dung@gmail.com", "Phạm Thị Dung", true, "123456", "0934567890", false }
                });

            migrationBuilder.InsertData(
                table: "Vouchers",
                columns: new[] { "Id", "Code", "DiscountType", "DiscountValue", "EndDate", "IsActive", "Name", "StartDate", "UsageLimit", "UsedCount" },
                values: new object[,]
                {
                    { "V001", "WELCOME10", "percent", 10m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Chào mừng khách mới", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100, 5 },
                    { "V002", "GIAM20K", "fixed", 20000m, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Giảm 20.000đ đơn từ 100k", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 50, 10 },
                    { "V003", "POLY15", "percent", 15m, new DateTime(2025, 2, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "PolyCafe 15%", new DateTime(2025, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 200, 80 },
                    { "V004", "FREESHIP", "fixed", 15000m, new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Miễn phí giao hàng", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 300, 45 },
                    { "V005", "TETHOLIDAY", "percent", 20m, new DateTime(2025, 2, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Khuyến mãi Tết 2025", new DateTime(2025, 1, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), 500, 320 },
                    { "V006", "BIRTHDAY30", "percent", 30m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Sinh nhật giảm 30%", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0 },
                    { "V007", "COMBO50K", "fixed", 50000m, new DateTime(2025, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Giảm 50k đơn combo", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100, 20 },
                    { "V008", "SUMMER25", "percent", 25m, new DateTime(2025, 8, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Summer Sale 25%", new DateTime(2025, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 400, 0 },
                    { "V009", "LOYAL5", "percent", 5m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Khách thân thiết 5%", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9999, 150 },
                    { "V010", "FIRSTAPP", "fixed", 30000m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Lần đầu đặt app", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0 },
                    { "V011", "FLASH20", "percent", 20m, new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "Flash Sale 20%", new DateTime(2025, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 50, 50 },
                    { "V012", "WEEKDAY10", "percent", 10m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Ngày thường giảm 10%", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9999, 200 },
                    { "V013", "MORNING15K", "fixed", 15000m, new DateTime(2025, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Buổi sáng giảm 15k", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 200, 60 },
                    { "V014", "GROUP100K", "fixed", 100000m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Nhóm đặt giảm 100k", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 30, 5 },
                    { "V015", "ECO10", "percent", 10m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Giảm 10% mang ly riêng", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9999, 90 },
                    { "V016", "REVIEW20K", "fixed", 20000m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Đánh giá 5 sao giảm 20k", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 500, 30 },
                    { "V017", "NEWMENU", "percent", 10m, new DateTime(2025, 4, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Thử menu mới giảm 10%", new DateTime(2025, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100, 0 },
                    { "V018", "RAINY10K", "fixed", 10000m, new DateTime(2025, 10, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Ngày mưa giảm 10k", new DateTime(2025, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 9999, 0 },
                    { "V019", "MIDNIGHT", "percent", 15m, new DateTime(2025, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), true, "Đêm khuya giảm 15%", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100, 10 },
                    { "V020", "POLY2025", "percent", 25m, new DateTime(2025, 1, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), false, "PolyCafe kỷ niệm 2025", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1000, 750 }
                });

            migrationBuilder.InsertData(
                table: "Drinks",
                columns: new[] { "Id", "CategoryId", "Description", "ImageUrl", "IsAvailable", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { "D001", "CAT01", "Cà phê phin truyền thống pha với sữa đặc và đá", "caphe_sua_da.jpg", true, false, "Cà Phê Sữa Đá", 35000m },
                    { "D002", "CAT01", "Cà phê sữa tỉ lệ sữa nhiều hơn, thơm béo", "bac_xiu.jpg", true, false, "Bạc Xỉu", 35000m },
                    { "D003", "CAT02", "Trà sữa Đài Loan với trân châu đen dẻo dai", "trasua_tranchau.jpg", true, false, "Trà Sữa Trân Châu Đen", 45000m },
                    { "D004", "CAT02", "Trà sữa kết hợp matcha Nhật Bản thơm ngon", "trasua_matcha.jpg", true, false, "Trà Sữa Matcha", 50000m },
                    { "D005", "CAT03", "Sinh tố bơ sáp nguyên chất, béo thơm", "sinhto_bo.jpg", true, false, "Sinh Tố Bơ", 55000m },
                    { "D006", "CAT03", "Xoài cát Hòa Lộc ngọt thơm xay mịn", "sinhto_xoai.jpg", true, false, "Sinh Tố Xoài", 50000m },
                    { "D007", "CAT04", "Cam vàng tươi nguyên chất, giàu vitamin C", "nuocep_cam.jpg", true, false, "Nước Ép Cam", 45000m },
                    { "D008", "CAT04", "Dưa hấu đỏ tươi ngọt mát", "nuocep_duahau.jpg", true, false, "Nước Ép Dưa Hấu", 40000m },
                    { "D009", "CAT05", "Trà hoa đào kết hợp cam và sả tươi", "tradao_camsa.jpg", true, false, "Trà Đào Cam Sả", 50000m },
                    { "D010", "CAT05", "Trà xanh kết hợp vải và nhãn tươi mát", "travai_nhan.jpg", true, false, "Trà Vải Nhãn", 48000m },
                    { "D011", "CAT06", "Bột matcha Nhật pha với sữa tươi và đá", "matcha_latte.jpg", true, false, "Matcha Latte Đá", 55000m },
                    { "D012", "CAT06", "Matcha xay đá mịn, thêm kem tươi phủ trên", "matcha_daxay.jpg", true, false, "Matcha Đá Xay", 60000m },
                    { "D013", "CAT07", "Chocolate Bỉ đun nóng, thêm marshmallow", "chocolate_nong.jpg", true, false, "Chocolate Nóng", 50000m },
                    { "D014", "CAT07", "Chocolate đá lạnh mát, béo ngậy", "chocolate_da.jpg", true, false, "Chocolate Đá", 50000m },
                    { "D015", "CAT08", "Cà phê xay đá caramel, phủ whipping cream", "frappuccino.jpg", true, false, "Frappuccino Caramel", 65000m },
                    { "D016", "CAT09", "Soda xi rô việt quất màu tím quyến rũ", "soda_vietquat.jpg", true, false, "Soda Việt Quất", 40000m },
                    { "D017", "CAT10", "Espresso pha với sữa hấp và xi rô vani", "latte_vani.jpg", true, false, "Latte Vani", 55000m },
                    { "D018", "CAT13", "Cold brew ủ lạnh 20 giờ, thêm nước cam tươi", "coldbrew_cam.jpg", true, false, "Cold Brew Cam", 60000m },
                    { "D019", "CAT19", "Chanh leo tươi, muối hồng, đá bào mát lạnh", "chanhleo_muoi.jpg", true, false, "Chanh Leo Muối", 38000m },
                    { "D020", "CAT20", "Thức uống bí mật của PolyCafe - vị đặc trưng không nơi nào có", "poly_signature.jpg", true, false, "PolyCafe Signature", 75000m }
                });

            migrationBuilder.InsertData(
                table: "Orders",
                columns: new[] { "Id", "DiscountAmount", "OrderDate", "Status", "TotalPrice", "UserId", "VoucherId" },
                values: new object[,]
                {
                    { "ORD001", 0m, new DateTime(2025, 1, 10, 8, 30, 0, 0, DateTimeKind.Unspecified), "Completed", 80000m, "U002", null },
                    { "ORD002", 9000m, new DateTime(2025, 1, 11, 9, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 90000m, "U003", "V001" },
                    { "ORD003", 20000m, new DateTime(2025, 1, 12, 10, 15, 0, 0, DateTimeKind.Unspecified), "Completed", 105000m, "U004", "V002" },
                    { "ORD004", 0m, new DateTime(2025, 1, 13, 14, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 70000m, "U005", null },
                    { "ORD005", 15000m, new DateTime(2025, 1, 14, 15, 30, 0, 0, DateTimeKind.Unspecified), "Completed", 120000m, "U002", "V004" },
                    { "ORD006", 0m, new DateTime(2025, 1, 15, 8, 0, 0, 0, DateTimeKind.Unspecified), "Pending", 55000m, "U003", null },
                    { "ORD007", 0m, new DateTime(2025, 1, 16, 11, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 160000m, "U004", null },
                    { "ORD008", 0m, new DateTime(2025, 1, 17, 16, 0, 0, 0, DateTimeKind.Unspecified), "Cancelled", 75000m, "U005", null },
                    { "ORD009", 40000m, new DateTime(2025, 1, 18, 9, 30, 0, 0, DateTimeKind.Unspecified), "Completed", 200000m, "U002", "V005" },
                    { "ORD010", 0m, new DateTime(2025, 1, 19, 10, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 85000m, "U003", null },
                    { "ORD011", 11000m, new DateTime(2025, 1, 20, 13, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 110000m, "U004", "V001" },
                    { "ORD012", 0m, new DateTime(2025, 1, 21, 14, 30, 0, 0, DateTimeKind.Unspecified), "Pending", 95000m, "U005", null },
                    { "ORD013", 0m, new DateTime(2025, 1, 22, 8, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 45000m, "U002", null },
                    { "ORD014", 20000m, new DateTime(2025, 1, 23, 17, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 135000m, "U003", "V002" },
                    { "ORD015", 6000m, new DateTime(2025, 1, 24, 9, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 60000m, "U004", "V009" },
                    { "ORD016", 36000m, new DateTime(2025, 1, 25, 10, 30, 0, 0, DateTimeKind.Unspecified), "Completed", 180000m, "U005", "V005" },
                    { "ORD017", 0m, new DateTime(2025, 1, 26, 11, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 50000m, "U002", null },
                    { "ORD018", 15000m, new DateTime(2025, 1, 27, 14, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 145000m, "U003", "V004" },
                    { "ORD019", 11250m, new DateTime(2025, 1, 28, 19, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 75000m, "U004", "V019" },
                    { "ORD020", 56250m, new DateTime(2025, 1, 29, 10, 0, 0, 0, DateTimeKind.Unspecified), "Completed", 225000m, "U005", "V020" }
                });

            migrationBuilder.InsertData(
                table: "VoucherDetails",
                columns: new[] { "Id", "ConditionType", "ConditionValue", "Description", "VoucherId" },
                values: new object[,]
                {
                    { "VD001", "MinOrder", 50000m, "Đơn tối thiểu 50.000đ", "V001" },
                    { "VD002", "MinOrder", 100000m, "Đơn tối thiểu 100.000đ", "V002" },
                    { "VD003", "MinOrder", 80000m, "Đơn tối thiểu 80.000đ", "V003" },
                    { "VD004", "MinOrder", 60000m, "Đơn tối thiểu 60.000đ", "V004" },
                    { "VD005", "MinOrder", 100000m, "Đơn tối thiểu 100.000đ", "V005" },
                    { "VD006", "BirthdayMonth", 1m, "Áp dụng trong tháng sinh nhật", "V006" },
                    { "VD007", "MinOrder", 150000m, "Đơn combo tối thiểu 150.000đ", "V007" },
                    { "VD008", "MinOrder", 70000m, "Đơn tối thiểu 70.000đ", "V008" },
                    { "VD009", "MinOrder", 0m, "Không giới hạn đơn hàng", "V009" },
                    { "VD010", "FirstOrder", 1m, "Chỉ áp dụng đơn hàng đầu tiên", "V010" },
                    { "VD011", "MinOrder", 50000m, "Flash sale đơn từ 50k", "V011" },
                    { "VD012", "DayOfWeek", 5m, "Thứ 2 đến thứ 6", "V012" },
                    { "VD013", "TimeRange", 9m, "Áp dụng 6h-9h sáng", "V013" },
                    { "VD014", "MinOrder", 300000m, "Đơn nhóm tối thiểu 300.000đ", "V014" },
                    { "VD015", "EcoCup", 1m, "Mang ly cá nhân khi đến quán", "V015" },
                    { "VD016", "ReviewRequired", 5m, "Đánh giá 5 sao lần đặt trước", "V016" },
                    { "VD017", "NewItem", 1m, "Áp dụng khi đặt món trong menu mới", "V017" },
                    { "VD018", "MinOrder", 40000m, "Đơn tối thiểu 40.000đ", "V018" },
                    { "VD019", "TimeRange", 22m, "Áp dụng từ 22h - 24h", "V019" },
                    { "VD020", "MinOrder", 50000m, "Kỷ niệm thành lập quán", "V020" }
                });

            migrationBuilder.InsertData(
                table: "VoucherScopes",
                columns: new[] { "Id", "ApplyType", "DrinkId", "VoucherId" },
                values: new object[,]
                {
                    { "VS001", 0, null, "V001" },
                    { "VS002", 0, null, "V002" },
                    { "VS003", 0, null, "V003" },
                    { "VS004", 0, null, "V004" },
                    { "VS005", 0, null, "V005" },
                    { "VS006", 0, null, "V006" },
                    { "VS007", 0, null, "V007" },
                    { "VS008", 0, null, "V008" },
                    { "VS009", 0, null, "V009" },
                    { "VS010", 0, null, "V010" },
                    { "VS014", 0, null, "V014" },
                    { "VS015", 0, null, "V015" },
                    { "VS016", 0, null, "V016" },
                    { "VS018", 0, null, "V018" },
                    { "VS019", 0, null, "V019" },
                    { "VS020", 0, null, "V020" }
                });

            migrationBuilder.InsertData(
                table: "DrinkHistories",
                columns: new[] { "Id", "CategoryId", "ChangedAt", "Description", "DrinkId", "ImageUrl", "IsAvailable", "IsDeleted", "Name", "Price" },
                values: new object[,]
                {
                    { "DH001", "CAT01", new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cà phê phin truyền thống", "D001", "caphe_sua_da.jpg", true, false, "Cà Phê Sữa Đá", 30000m },
                    { "DH002", "CAT01", new DateTime(2025, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cà phê sữa tỉ lệ sữa nhiều hơn", "D002", "bac_xiu.jpg", true, false, "Bạc Xỉu", 30000m },
                    { "DH003", "CAT02", new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trà sữa Đài Loan", "D003", "trasua_tranchau.jpg", true, false, "Trà Sữa Trân Châu Đen", 40000m },
                    { "DH004", "CAT02", new DateTime(2025, 1, 11, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trà sữa matcha Nhật", "D004", "trasua_matcha.jpg", true, false, "Trà Sữa Matcha", 45000m },
                    { "DH005", "CAT03", new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Sinh tố bơ sáp", "D005", "sinhto_bo.jpg", true, false, "Sinh Tố Bơ", 50000m },
                    { "DH006", "CAT03", new DateTime(2025, 1, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Xoài cát Hòa Lộc", "D006", "sinhto_xoai.jpg", true, false, "Sinh Tố Xoài", 45000m },
                    { "DH007", "CAT04", new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cam vàng nguyên chất", "D007", "nuocep_cam.jpg", true, false, "Nước Ép Cam", 40000m },
                    { "DH008", "CAT04", new DateTime(2025, 1, 13, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dưa hấu đỏ tươi", "D008", "nuocep_duahau.jpg", true, false, "Nước Ép Dưa Hấu", 35000m },
                    { "DH009", "CAT05", new DateTime(2025, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trà hoa đào", "D009", "tradao_camsa.jpg", true, false, "Trà Đào Cam Sả", 45000m },
                    { "DH010", "CAT05", new DateTime(2025, 1, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Trà xanh vải nhãn", "D010", "travai_nhan.jpg", true, false, "Trà Vải Nhãn", 43000m },
                    { "DH011", "CAT06", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Matcha sữa đá", "D011", "matcha_latte.jpg", true, false, "Matcha Latte Đá", 50000m },
                    { "DH012", "CAT06", new DateTime(2025, 1, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Matcha xay đá", "D012", "matcha_daxay.jpg", true, false, "Matcha Đá Xay", 55000m },
                    { "DH013", "CAT07", new DateTime(2025, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chocolate Bỉ nóng", "D013", "chocolate_nong.jpg", true, false, "Chocolate Nóng", 45000m },
                    { "DH014", "CAT07", new DateTime(2025, 1, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chocolate đá lạnh", "D014", "chocolate_da.jpg", true, false, "Chocolate Đá", 45000m },
                    { "DH015", "CAT08", new DateTime(2025, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cà phê xay caramel", "D015", "frappuccino.jpg", true, false, "Frappuccino Caramel", 60000m },
                    { "DH016", "CAT09", new DateTime(2025, 1, 17, 0, 0, 0, 0, DateTimeKind.Unspecified), "Soda việt quất", "D016", "soda_vietquat.jpg", true, false, "Soda Việt Quất", 35000m },
                    { "DH017", "CAT10", new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Latte xi rô vani", "D017", "latte_vani.jpg", true, false, "Latte Vani", 50000m },
                    { "DH018", "CAT13", new DateTime(2025, 1, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cold brew cam tươi", "D018", "coldbrew_cam.jpg", true, false, "Cold Brew Cam", 55000m },
                    { "DH019", "CAT19", new DateTime(2025, 1, 19, 0, 0, 0, 0, DateTimeKind.Unspecified), "Chanh leo muối đá", "D019", "chanhleo_muoi.jpg", true, false, "Chanh Leo Muối", 33000m },
                    { "DH020", "CAT20", new DateTime(2025, 1, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Thức uống bí mật cũ", "D020", "poly_signature.jpg", true, false, "PolyCafe Signature", 70000m }
                });

            migrationBuilder.InsertData(
                table: "DrinkVariants",
                columns: new[] { "Id", "DrinkId", "ExtraPrice", "IsActive", "VariantName" },
                values: new object[,]
                {
                    { "DV001", "D001", 0m, true, "Size S" },
                    { "DV002", "D001", 5000m, true, "Size M" },
                    { "DV003", "D001", 10000m, true, "Size L" },
                    { "DV004", "D003", 0m, true, "Ít đường" },
                    { "DV005", "D003", 0m, true, "Không đường" },
                    { "DV006", "D003", 8000m, true, "Thêm trân châu" },
                    { "DV007", "D005", 0m, true, "Không đường" },
                    { "DV008", "D005", 5000m, true, "Thêm sữa đặc" },
                    { "DV009", "D011", 0m, true, "Matcha vị nhẹ" },
                    { "DV010", "D011", 5000m, true, "Matcha vị đậm" },
                    { "DV011", "D015", 5000m, true, "Thêm caramel drizzle" },
                    { "DV012", "D015", 10000m, true, "Thêm extra espresso" },
                    { "DV013", "D017", 10000m, true, "Sữa hạnh nhân" },
                    { "DV014", "D017", 10000m, true, "Sữa yến mạch" },
                    { "DV015", "D020", 0m, true, "Size S" },
                    { "DV016", "D020", 15000m, true, "Size L" },
                    { "DV017", "D009", 0m, true, "Ít đá" },
                    { "DV018", "D009", 0m, true, "Không đá" },
                    { "DV019", "D012", 8000m, true, "Thêm kem tươi" },
                    { "DV020", "D013", 5000m, true, "Thêm marshmallow" }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "Id", "DrinkId", "OrderId", "Quantity", "UnitPrice", "VariantId" },
                values: new object[,]
                {
                    { "OD004", "D009", "ORD003", 1, 50000m, null },
                    { "OD005", "D007", "ORD004", 1, 45000m, null },
                    { "OD006", "D019", "ORD004", 1, 38000m, null },
                    { "OD009", "D002", "ORD006", 1, 35000m, null },
                    { "OD013", "D004", "ORD010", 1, 50000m, null },
                    { "OD014", "D016", "ORD010", 1, 40000m, null },
                    { "OD015", "D018", "ORD011", 1, 60000m, null },
                    { "OD016", "D006", "ORD012", 2, 50000m, null }
                });

            migrationBuilder.InsertData(
                table: "VoucherScopes",
                columns: new[] { "Id", "ApplyType", "DrinkId", "VoucherId" },
                values: new object[,]
                {
                    { "VS011", 1, "D020", "V011" },
                    { "VS012", 1, "D001", "V012" },
                    { "VS013", 1, "D002", "V013" },
                    { "VS017", 1, "D019", "V017" }
                });

            migrationBuilder.InsertData(
                table: "OrderDetails",
                columns: new[] { "Id", "DrinkId", "OrderId", "Quantity", "UnitPrice", "VariantId" },
                values: new object[,]
                {
                    { "OD001", "D001", "ORD001", 2, 40000m, "DV002" },
                    { "OD002", "D003", "ORD002", 2, 53000m, "DV006" },
                    { "OD003", "D005", "ORD003", 1, 60000m, "DV008" },
                    { "OD007", "D015", "ORD005", 1, 70000m, "DV011" },
                    { "OD008", "D011", "ORD005", 1, 60000m, "DV010" },
                    { "OD010", "D020", "ORD007", 2, 90000m, "DV016" },
                    { "OD011", "D013", "ORD008", 1, 55000m, "DV020" },
                    { "OD012", "D015", "ORD009", 2, 75000m, "DV012" },
                    { "OD017", "D001", "ORD013", 1, 35000m, "DV001" },
                    { "OD018", "D012", "ORD014", 2, 68000m, "DV019" },
                    { "OD019", "D017", "ORD015", 1, 65000m, "DV013" },
                    { "OD020", "D020", "ORD016", 2, 75000m, "DV015" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_DrinkHistories_DrinkId",
                table: "DrinkHistories",
                column: "DrinkId");

            migrationBuilder.CreateIndex(
                name: "IX_Drinks_CategoryId",
                table: "Drinks",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_DrinkVariants_DrinkId",
                table: "DrinkVariants",
                column: "DrinkId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_DrinkId",
                table: "OrderDetails",
                column: "DrinkId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_OrderId",
                table: "OrderDetails",
                column: "OrderId");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_VariantId",
                table: "OrderDetails",
                column: "VariantId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserId",
                table: "Orders",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_VoucherId",
                table: "Orders",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherDetails_VoucherId",
                table: "VoucherDetails",
                column: "VoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherScopes_DrinkId",
                table: "VoucherScopes",
                column: "DrinkId");

            migrationBuilder.CreateIndex(
                name: "IX_VoucherScopes_VoucherId",
                table: "VoucherScopes",
                column: "VoucherId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DrinkHistories");

            migrationBuilder.DropTable(
                name: "OrderDetails");

            migrationBuilder.DropTable(
                name: "VoucherDetails");

            migrationBuilder.DropTable(
                name: "VoucherScopes");

            migrationBuilder.DropTable(
                name: "DrinkVariants");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Drinks");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Vouchers");

            migrationBuilder.DropTable(
                name: "Categories");
        }
    }
}
