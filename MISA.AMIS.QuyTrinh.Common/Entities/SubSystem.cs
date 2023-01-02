using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities
{
    /// <summary>
    /// Các phân quyền
    /// </summary>
    public class SubSystem : BaseEntity
    {
        #region Propety

        /// <summary>
        /// ID phân quyền
        /// </summary>
        public Guid SubSystemID { get; set; }

        /// <summary>
        /// Mã phân quyền
        /// </summary>
        public string SubSystemCode { get; set; }

        /// <summary>
        /// Tên phân quyền
        /// </summary> 
        public string SubSystemName { get; set; }

        /// <summary>
        /// Loại phân quyền
        /// </summary>
        public SubSystemType SubSystemType { get; set; }

        /// <summary>
        /// Danh sách các quyền
        /// </summary>
        public List<Permission>? ListPermissions { get; set; }

        #endregion
    }
}
