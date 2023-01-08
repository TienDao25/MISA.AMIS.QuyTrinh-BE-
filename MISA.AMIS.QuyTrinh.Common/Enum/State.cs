using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Enum
{
    /// <summary>
    /// Trạng thái phân quyền - quyền
    /// </summary>
    public enum State
    {
        /// <summary>
        /// Giữ nguyên
        /// </summary>
        None = 0,

        /// <summary>
        /// Thêm
        /// </summary>
        Add = 1,

        /// <summary>
        /// Xóa
        /// </summary>
        Detele = 2,
    }
}
