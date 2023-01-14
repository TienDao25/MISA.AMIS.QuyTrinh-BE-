using MISA.AMIS.QuyTrinh.Common.Attributes;
using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities
{
    /// <summary>
    /// Phân quyền - Quyền tương ứng kèm trạng thái thêm/xóa/giữ nguyên
    /// </summary>
    public class SubSystemAndPermission : BaseEntity
    {
        /// <summary>
        /// ID phân quyền
        /// </summary>
        public Guid SubSystemID { get; set; }

        /// <summary>
        /// Mã phân quyền
        /// </summary>
        [SqlIgnore]
        public string SubSystemCode { get; set; }

        /// <summary>
        /// ID quyền
        /// </summary>
        public Guid PermissionID { get; set; }

        /// <summary>
        /// Mã quyền
        /// </summary>
        [SqlIgnore]
        public string PermissionCode { get; set; }

        /// <summary>
        /// Trạng thái phân quyền - quyền
        /// </summary>
        [SqlIgnore]
        public State State { get; set; }

    }
}
