using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities.DTO
{
    /// <summary>
    /// Kết quả gọi phân trang/tìm kiếm
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagingResult<T>
    {
        /// <summary>
        /// Danh sách tài sản
        /// </summary>
        public List<T> ListData { get; set; }

        /// <summary>
        /// Tổng số bản ghi
        /// </summary>
        public long? TotalRecords { get; set; }

    }
}
