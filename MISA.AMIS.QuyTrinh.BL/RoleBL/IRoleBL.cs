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
        /// <returns>Danh sách vai trò và tổng số bản ghi</returns>
        /// Created by: TienDao (26/12/2022)
        public PagingResult<Role> GetRolesByFilterAndPaging(string keyWord, int limit, int offset, string fieldSort, string typeSort);

        /// <summary>
        /// Kiểm tra tên vai trò trùng
        /// </summary>
        /// <param name="roleName">Tên vai trò</param>
        /// <returns>True: trùng, False: không trùng</returns>
        /// Created by: TienDao(31/12/2022)
        public bool IsDulicateRoleName(string roleName, Guid? roleID);

        /// <summary>
        /// Thêm mới vai trò
        /// </summary>
        /// <param name="role">Thông tin tổng quan vai trò</param>
        /// <param name="listSubSystemID">Danh sách id phân quyền</param>
        /// <param name="listPermissionID">Danh sách id quyền tương ứng với phân quyền</param>
        /// <returns>Số bản ghi</returns>
        /// Created by: TienDao (31/12/2022)
        public ResponseService InsertUpdateDulicateRole(ModeForm modeForm, Role role, List<Guid> listSubSystemID, List<Guid> listPermissionID);
    }
}
