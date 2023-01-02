using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
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
        /// Thông tin tổng quát vai trò
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// Danh sách ID phân quyền
        /// </summary>
        public List<Guid> ListSubSystemID { get; set; }

        /// <summary>
        /// Danh sách ID quyền tương ứng
        /// </summary>
        public List<Guid> ListPermissionID { get; set; }
    }
}
