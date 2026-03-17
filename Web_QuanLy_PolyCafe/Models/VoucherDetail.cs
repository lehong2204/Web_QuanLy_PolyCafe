using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Web_QuanLy_PolyCafe.Models
{
    public class VoucherDetail
    {
        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [Required(ErrorMessage = "Voucher không được để trống")]
        [StringLength(50)]
        [Display(Name = "Voucher")]
        public string VoucherId { get; set; }

        [Required(ErrorMessage = "Loại điều kiện không được để trống")]
        [StringLength(50, ErrorMessage = "Loại điều kiện tối đa 50 ký tự")]
        [Display(Name = "Loại điều kiện")]
        public string ConditionType { get; set; }

        [Required(ErrorMessage = "Giá trị điều kiện không được để trống")]
        [Range(0, 99999999.99, ErrorMessage = "Giá trị điều kiện phải lớn hơn hoặc bằng 0")]
        [Column(TypeName = "decimal(10,2)")]
        [Display(Name = "Giá trị điều kiện")]
        public decimal ConditionValue { get; set; }

        [StringLength(255, ErrorMessage = "Mô tả tối đa 255 ký tự")]
        [Display(Name = "Mô tả điều kiện")]
        public string? Description { get; set; }

        // Navigation
        public Voucher? Voucher { get; set; }
    }
}