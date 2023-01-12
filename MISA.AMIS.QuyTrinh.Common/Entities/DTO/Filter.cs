using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities.DTO
{
    /// <summary>
    /// Bộ lọc
    /// </summary>
    public class Filter
    {
        /// <summary>
        /// Điều kiện lọc
        /// </summary>
        public string Relationship { get; set; }

        /// <summary>
        /// Cột tương ứng trong database
        /// </summary>
        public string Column { get; set; }

        /// <summary>
        /// Toán tử so sánh
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Giá trị so sánh
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Các điều kiện con
        /// </summary>
        public SubQuery? SubQuery { get; set; }
    }

    /// <summary>
    /// Các điều kiện con
    /// </summary>
    public class SubQuery
    {
        /// <summary>
        /// Toán tử nối các điều điện con
        /// </summary>
        public string Operator { get; set; }

        /// <summary>
        /// Danh sách điều kiện con
        /// </summary>
        public List<Filter> Detail { get; set; }
    }

    /// <summary>
    /// Cách sắp xếp
    /// </summary>
    public class Sort
    {
        /// <summary>
        /// Trường sắp xếp
        /// </summary>
        public string Selector { get; set; }

        /// <summary>
        /// Kiểu sắp xếp
        /// </summary>
        public TypeSort TypeSort { get; set; }
    }

    /// <summary>
    /// request phân trang tìm kiếm
    /// </summary>
    public class RequestFilter
    {
        /// <summary>
        /// Bộ lọc
        /// </summary>
        public List<Filter>? Filter { get; set; }

        /// <summary>
        /// Số lượng bản ghi / 1 trang
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// Bản ghi bắt đầu
        /// </summary>
        public int Offset { get; set; }

        /// <summary>
        /// Cách sắp xếp
        /// </summary>
        public Sort Sort { get; set; }
    }
}
