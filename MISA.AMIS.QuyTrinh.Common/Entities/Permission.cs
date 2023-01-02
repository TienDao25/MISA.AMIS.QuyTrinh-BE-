using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities
{
    /// <summary>
    /// Quyền
    /// </summary>
    public class Permission : BaseEntity
    {
        #region Propety

        /// <summary>
        /// ID quyền
        /// </summary>
        public Guid PermissionID { get; set; }

        /// <summary>
        /// Mã quyền
        /// </summary>
        public string PermissionCode { get; set; }

        /// <summary>
        /// Tên quyền
        /// </summary> 
        public string PermissionName { get; set; }

        #endregion
    }
}
