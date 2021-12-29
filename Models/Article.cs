using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace App.Models
{
    public class Article
    {
        [Key]
        public int Id { get; set; }
        [StringLength(255, MinimumLength = 3, ErrorMessage = "{0} phải nhập từ {2} đến {1}")]
        [Required(ErrorMessage = "Phải Nhập {0}")]
        [Column(TypeName = "nvarchar")]
        [DisplayName("Tiêu Đề")]
        public string Title { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Phải Nhập {0}")]
        [DisplayName("Ngày Tạo")]
        public DateTime Created { get; set; }
        [Column(TypeName = "ntext")]
        [DisplayName("Nội Dung")]
        public string Content { get; set; }
    }
}