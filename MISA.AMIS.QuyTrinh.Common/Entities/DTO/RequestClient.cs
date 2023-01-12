using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities.DTO
{
    /// <summary>
    /// Yêu cầu client
    /// </summary>
    public class RequestClient
    {
        /// <summary>
        /// Loại form
        /// </summary>
        
        public ModeForm ModeForm { get; set; }

        /// <summary>
        /// ID vai trò
        /// </summary>
        public Guid? RoleID { get; set; }

        /// <summary>
        /// Mã vai trò
        /// </summary>
        public string? RoleCode { get; set; }

        /// <summary>
        /// Tên vai trò
        /// </summary> 
        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        public string RoleName { get; set; }

        /// <summary>
        /// Mô tả vai trò
        /// </summary>
        public string? RoleDescription { get; set; }

        /// <summary>
        /// Danh sách quyền 
        /// </summary>
        public List<SubSystemAndPermission>? Permissions { get; set; }
    }
}
