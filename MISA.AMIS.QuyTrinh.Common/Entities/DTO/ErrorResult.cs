using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities.DTO
{
    /// <summary>
    /// Kết quả trả về FE khi có lỗi
    /// </summary>
    public class ErrorResult
    {
        #region Property

        /// <summary>
        /// Mã lỗi
        /// </summary>
        public AMISErrorCode ErrorCode { get; set; }

        /// <summary>
        /// Message trả về cho dev
        /// </summary>
        public string DevMsg { get; set; }

        /// <summary>
        /// Message trả về cho người dùng hiểu
        /// </summary>
        public string UserMsg { get; set; }

        /// <summary>
        /// Thông tin chi tiết về lỗi
        /// </summary>
        public object MoreInfo { get; set; }

        /// <summary>
        /// TraceId
        /// </summary>
        public string TraceId { get; set; }

        #endregion
    }
}
