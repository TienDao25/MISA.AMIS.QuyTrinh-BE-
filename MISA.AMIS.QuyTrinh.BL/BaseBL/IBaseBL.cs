using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.BL.BaseBL
{
    public interface IBaseBL<T>
    {
        /// <summary>
        /// Lấy danh sách tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// Created by: TienDao(22/12/2022)
        public IEnumerable<T> GetAllRecords();

        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Thông tin cần thêm</param>
        /// <returns>ID bản ghi được thêm</returns>
        /// Author: TienDao (11/01/2023)
        public ResponseService Insert(List<T> entities);

        /// <summary>
        /// Xóa/Hủy bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID bản muốn xóa</param>
        /// Created by: TienDao (27/12/2022)
        public ResponseService DeleteRecordByID(Guid recordID);

        /// <summary>
        /// Kiểm tra trùng
        /// </summary>
        /// <param name="fieldName">Tên trường</param>
        /// <param name="valueNeedCheck">Giá trị check</param>
        /// <param name="recordID">ID bản ghi</param>
        /// <returns>True: trùng, False: không trùng</</returns>
        public bool CheckDulicate(string fieldName, string valueNeedCheck, Guid? recordID);

        /// <summary>
        /// API lấy danh sách bản ghi theo bộ lọc và phân trang
        /// </summary>
        /// <returns>Danh sách bản ghi và tổng số bản ghi</returns>
        /// Created by: TienDao (11/01/2023)
        public PagingResult<T> GetRecordByFilterAndPaging(List<Filter>? filter, int limit, int offset, Sort sort);
    }
}
