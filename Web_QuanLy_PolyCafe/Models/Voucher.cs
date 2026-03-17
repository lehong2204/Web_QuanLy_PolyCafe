using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_QuanLy_PolyCafe.Models
{
    public class Voucher
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Mã voucher không được để trống")]
        [StringLength(50, ErrorMessage = "Mã voucher tối đa 50 ký tự")]
        [Display(Name = "Mã voucher")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Tên voucher không được để trống")]
        [StringLength(150, ErrorMessage = "Tên voucher tối đa 150 ký tự")]
        [Display(Name = "Tên voucher")]
        public string Name { get; set; }

        /// <summary>percent | fixed</summary>
        [Required(ErrorMessage = "Loại giảm giá không được để trống")]
        [StringLength(20, ErrorMessage = "Loại giảm giá tối đa 20 ký tự")]
        [Display(Name = "Loại giảm giá (percent/fixed)")]
        public string DiscountType { get; set; }

        [Required(ErrorMessage = "Giá trị giảm không được để trống")]
        [Range(0, 99999999.99, ErrorMessage = "Giá trị giảm phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Giá trị giảm")]
        public decimal DiscountValue { get; set; }

        [Required(ErrorMessage = "Ngày bắt đầu không được để trống")]
        [Display(Name = "Ngày bắt đầu")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Ngày kết thúc không được để trống")]
        [Display(Name = "Ngày kết thúc")]
        public DateTime EndDate { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Giới hạn sử dụng phải lớn hơn 0")]
        [Display(Name = "Giới hạn sử dụng")]
        public int? UsageLimit { get; set; }

        [Display(Name = "Đã dùng")]
        public int UsedCount { get; set; } = 0;

        [Display(Name = "Kích hoạt")]
        public bool IsActive { get; set; } = true;

        // Navigation
        public ICollection<VoucherDetail>? VoucherDetails { get; set; }
        public ICollection<VoucherScope>? VoucherScopes { get; set; }
        public ICollection<Order>? Orders { get; set; }
    }
}