using MISA.AMIS.QuyTrinh.BL.BaseBL;
using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.BL.RoleBL
{
    public interface IRoleBL  : IBaseBL<Role>
    {
        /// <summary>
        /// Lấy chi tiết 1 vai trò
        /// </summary>
        /// <param name="RoleID">ID vai trò</param>
        /// <returns>1 bản ghi vai trò đẩy đủ</returns>
        /// Created by: TienDao(22/12/2022)
        public Role GetRoleDetailByID(Guid RoleID);

        /// <summary>
        /// lấy danh sách vai trò theo bộ lọc và phân trang
        /// </summary>
        /// <param name="keyword">Từ khóa muốn tìm kiếm</param>
        /// <param name="limit">Số bản ghi muốn lấy</param>
        /// <param name="offset">Vị trí của bản ghi bắt đầu lấy</param>
        /// <param name="fieldSort">Trường sắp xếp</param>
        /// <param name="typeSort">Kiểu sắp xếp</param>
        /// <param name="roleStatus">Trạng thái muốn lọc</param>
        /// <returns>Danh sách vai trò và tổng số bản ghi</returns>
        /// Created by: TienDao (26/12/2022)
        public PagingResult<Role> GetRolesByFilterAndPaging(string keyWord, int limit, int offset, string fieldSort, TypeSort typeSort, RoleStatus roleStatus);

        /// <summary>
        /// Thêm vai trò
        /// </summary>
        /// <param name="requestClient">Request client gửi về</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        public ResponseService InsertRole(RequestClient requestClient);

        /// <summary>
        /// Sửa vai trò
        /// </summary>
        /// <param name="requestClient">Request client gửi về</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        public ResponseService UpdateRole(RequestClient requestClient);
    }
}
