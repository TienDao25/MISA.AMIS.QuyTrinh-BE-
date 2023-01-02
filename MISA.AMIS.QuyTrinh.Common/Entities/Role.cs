using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities
{
    public class Role : BaseEntity
    {
        #region Propety

        /// <summary>
        /// ID vai trò
        /// </summary>
        public Guid RoleID { get; set; }

        /// <summary>
        /// Mã vai trò
        /// </summary>
        public string RoleCode { get; set; }

        /// <summary>
        /// Tên vai trò
        /// </summary> 
        [Required(ErrorMessage = "Tên vai trò không được để trống")]
        public string RoleName { get; set; }

        /// <summary>
        /// Mô tả vai trò
        /// </summary>
        public string RoleDescribe { get; set; }

        /// <summary>
        /// Trạng thái vai trò
        /// </summary>
        public RoleStatus RoleStatus { get; set; }

        /// <summary>
        /// Danh sách phân quyền và quyền
        /// </summary>
        public List<SubSystem> ListSubsytemAndPermission { get; set; }
        
        #endregion
    }
}
