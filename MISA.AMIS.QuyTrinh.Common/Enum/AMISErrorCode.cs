using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Enum
{
    /// <summary>
    /// Mã lỗ
    /// </summary>
    public enum AMISErrorCode
    {
        /// <summary>
        /// Lỗi gặp exception
        /// </summary>
        Exception = 1,

        /// <summary>
        /// Lỗi trùng
        /// </summary>
        Duplicate = 2,

        /// <summary>
        /// Lỗi thêm mới bản ghi
        /// </summary>
        InsertError = 3,

        /// <summary>
        /// Lỗi cập nhật/sửa bản ghi
        /// </summary>
        UpdateError = 4,

        /// <summary>
        /// Lỗi xóa bản ghi       
        /// </summary>
        DeleteError = 5,

        /// <summary>
        /// Lỗi validate
        /// </summary>
        Validate = 6,

        /// <summary>
        /// Lỗi lấy bản ghi
        /// </summary>
        GetDetail = 7,

        /// <summary>
        /// Lỗi lấy danh sách
        /// </summary>
        GetAll = 8
    }
}
