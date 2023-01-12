﻿using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.Common.Enum;
using MISA.AMIS.QuyTrinh.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.DL.RoleDL
{
    public interface IRoleDL : IBaseDL<Role>
    {
        /// <summary>
        /// Lấy chi tiết 1 vai trò
        /// </summary>
        /// <param name="roleID">ID vai trò</param>
        /// <returns>1 bản ghi vai trò đẩy đủ</returns>
        /// Created by: TienDao(22/12/2022)
        public Role GetRoleDetailByID(Guid roleID);

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
        /// DL Thêm vai trò
        /// </summary>
        /// <param name="requestClient">request client (dùng để lấy cá thông tin cố định (tên, mô tả))</param>
        /// <param name="permissionsAdd">Danh sách quyền thêm</param>
        /// <param name="permissionsDelete">Danh sách quyền xóa</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        public int InsertRole(RequestClient requestClient, List<SubSystemAndPermission> permissionsAdd);

        /// <summary>
        /// DL Sửa vai trò
        /// </summary>
        /// <param name="requestClient">request client (dùng để lấy cá thông tin cố định (tên, mô tả))</param>
        /// <param name="permissionsAdd">Danh sách quyền thêm</param>
        /// <param name="permissionsDelete">Danh sách quyền xóa</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        public int UpdateRole(RequestClient requestClient, List<SubSystemAndPermission> permissionsAdd, List<SubSystemAndPermission> permissionsDelete);
    }
}
