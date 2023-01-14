using MISA.AMIS.QuyTrinh.Common.Attributes;
using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MISA.AMIS.QuyTrinh.Common.Entities
{
    /// <summary>
    /// Vai trò tổng quát
    /// </summary>
    public class Role : BaseEntity
    {
        #region Propety

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
        [Unique]
        public string RoleName { get; set; }

        /// <summary>
        /// Mô tả vai trò
        /// </summary>
        public string? RoleDescription { get; set; }

        /// <summary>
        /// Trạng thái vai trò
        /// </summary>
        public RoleStatus RoleStatus { get; set; }

        [SqlIgnore]
        /// <summary>
        /// Danh sách phân quyền và quyền
        /// </summary>
        public List<SubSystem>? ListSubsytemAndPermission { get; set; }

        /// <summary>
        /// Danh sách quyền 
        /// </summary>
        [ManyToMany]
        [SqlIgnore]
        public List<SubSystemAndPermission>? Permissions { get; set; }

        /// <summary>
        /// Loại form
        /// </summary>
        [SqlIgnore]
        public ModeForm ModeForm { get; set; }
        #endregion
    }
}
