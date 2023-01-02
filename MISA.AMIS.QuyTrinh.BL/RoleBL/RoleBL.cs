using MISA.AMIS.QuyTrinh.BL.BaseBL;
using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.Common.Enum;
using MISA.AMIS.QuyTrinh.Common.Resource;
using MISA.AMIS.QuyTrinh.DL.RoleDL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.BL.RoleBL
{
    public class RoleBL : BaseBL<Role>, IRoleBL
    {
        #region Field

        private IRoleDL _roleDL;

        #endregion


        #region Contrustor

        public RoleBL(IRoleDL roleDL) : base(roleDL)
        {
            _roleDL = roleDL;
        }

        #endregion


        /// <summary>
        /// Lấy chi tiết 1 vai trò
        /// </summary>
        /// <param name="RoleID">ID vai trò</param>
        /// <returns>1 bản ghi vai trò đẩy đủ</returns>
        /// Created by: TienDao(22/12/2022)
        public Role GetRoleDetailByID(Guid RoleID)
        {
            return _roleDL.GetRoleDetailByID(RoleID);

            throw new NotImplementedException();
        }

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
        public PagingResult<Role> GetRolesByFilterAndPaging(string keyWord, int limit, int offset, string fieldSort, string typeSort)
        {
            return _roleDL.GetRolesByFilterAndPaging(keyWord, limit, offset, fieldSort, typeSort);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Kiểm tra tên vai trò trùng
        /// </summary>
        /// <param name="roleName">Tên vai trò</param>
        /// <returns>True: trùng, False: không trùng</returns>
        /// Created by: TienDao(31/12/2022)
        public bool IsDulicateRoleName(string roleName, Guid? roleID)
        {
            int countRole = _roleDL.CountRoleByName(roleName, roleID);
            if (countRole == 0)
            {
                return false;
            }
            return true;
            throw new NotImplementedException();
        }

        /// <summary>
        /// Check bắt buộc
        /// </summary>
        /// <param name="role">Vai trò</param>
        /// <param name="validateFailures">Mảng lỗi</param>
        /// Created by: TienDao (31/12/2022)
        private static void CheckRequired(Role role, List<string> validateFailures)
        {
            var properties = typeof(Role).GetProperties();

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(role);
                var requiredAttribute = (RequiredAttribute?)Attribute.GetCustomAttribute(property, typeof(RequiredAttribute));
                if (requiredAttribute != null && string.IsNullOrEmpty(propertyValue?.ToString()))
                {
                    validateFailures.Add(requiredAttribute.ErrorMessage); ;
                }
            }
        }

        public ResponseService InsertUpdateDulicateRole(ModeForm modeForm, Role role, List<Guid> listSubSystemID, List<Guid> listPermissionID)
        {
            var validateFailures = new List<string>();

            int numberOfRowsAffected = 0;

            CheckRequired(role, validateFailures);

            if (modeForm == ModeForm.Add || modeForm == ModeForm.Dulicate)
            {
                if (IsDulicateRoleName(role.RoleName, null) == true)
                {
                    validateFailures.Add(Resource.Error_DulicateRoleName);
                }

                if (validateFailures.Count > 0)
                {
                    return new ResponseService
                    {
                        IsSuccess = false,
                        Data = validateFailures,
                    };
                }

                numberOfRowsAffected = _roleDL.InsertRole(role, listSubSystemID, listPermissionID);

            }
            if (modeForm == ModeForm.Update)
            {

            }

            //if (numberOfRowsAffected == listPermissionID.Count + 1)
            if (numberOfRowsAffected > 0)
            {
                return new ResponseService
                {
                    IsSuccess = true
                };
            }
            else
            {
                return new ResponseService
                {
                    IsSuccess = false
                };
            }

            throw new NotImplementedException();
        }
    }
}
