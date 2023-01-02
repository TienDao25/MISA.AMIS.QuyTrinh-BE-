using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities.DTO
{
    /// <summary>
    /// Kết quả trả về
    /// </summary>
    public class ResponseService
    {
        /// <summary>
        /// Trạng thái của response
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Thông tin trả về của response
        /// </summary>
        public List<string>? Data { get; set; }
    }
}
