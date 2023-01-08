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
        }

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
        public PagingResult<Role> GetRolesByFilterAndPaging(string keyWord, int limit, int offset, string fieldSort, string typeSort, RoleStatus roleStatus)
        {
            return _roleDL.GetRolesByFilterAndPaging(keyWord, limit, offset, fieldSort, typeSort, roleStatus);
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
        }

        /// <summary>
        /// Check bắt buộc
        /// </summary>
        /// <param name="role">Vai trò</param>
        /// <param name="validateFailures">Mảng lỗi</param>
        /// Created by: TienDao (31/12/2022)
        private static void CheckRequired(RequestClient requestClient, List<string> validateFailures)
        {
            var properties = typeof(RequestClient).GetProperties();

            foreach (var property in properties)
            {
                var propertyValue = property.GetValue(requestClient);
                var requiredAttribute = (RequiredAttribute?)Attribute.GetCustomAttribute(property, typeof(RequiredAttribute));
                if (requiredAttribute != null && string.IsNullOrEmpty(propertyValue?.ToString()))
                {
                    validateFailures.Add(requiredAttribute.ErrorMessage); ;
                }
            }
        }

        /// <summary>
        /// Thêm vai trò
        /// </summary>
        /// <param name="requestClient">Request client gửi về</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        public ResponseService InsertRole(RequestClient requestClient)
        {
            var validateFailures = new List<string>();

            // Kiểm tra bắt buộc
            CheckRequired(requestClient, validateFailures);

            // Kiểm tra trùng tên

            if (IsDulicateRoleName(requestClient.RoleName, null) == true)
            {
                validateFailures.Add(Resource.Error_DulicateRoleName);
            }

            //Trả về controller nếu có lỗi
            if (validateFailures.Count > 0)
            {
                return new ResponseService
                {
                    IsSuccess = false,
                    Data = validateFailures,
                };
            }

            List<SubSystemAndPermission> permissionsAdd = new List<SubSystemAndPermission>();
            List<SubSystemAndPermission> permissionsDelete = new List<SubSystemAndPermission>();

            HandlerPermissions(requestClient.Permissions, permissionsAdd, permissionsDelete);

            int numberOfRowsAffected = _roleDL.InsertRole(requestClient, permissionsAdd, permissionsDelete);

            if (numberOfRowsAffected == permissionsAdd.Count + permissionsDelete.Count + 1)
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
        }

        /// <summary>
        /// Sửa vai trò
        /// </summary>
        /// <param name="requestClient">Request client gửi về</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        public ResponseService UpdateRole(RequestClient requestClient)
        {
            var validateFailures = new List<string>();

            // Kiểm tra bắt buộc
            CheckRequired(requestClient, validateFailures);

            //Check trùng tên
            if (IsDulicateRoleName(requestClient.RoleName, requestClient.RoleID) == true)
            {
                validateFailures.Add(Resource.Error_DulicateRoleName);
            }

            //Trả về controller nếu có lỗi
            if (validateFailures.Count > 0)
            {
                return new ResponseService
                {
                    IsSuccess = false,
                    Data = validateFailures,
                };
            }

            List<SubSystemAndPermission> permissionsAdd = new List<SubSystemAndPermission>();
            List<SubSystemAndPermission> permissionsDelete = new List<SubSystemAndPermission>();

            HandlerPermissions(requestClient.Permissions, permissionsAdd, permissionsDelete);

            int numberOfRowsAffected = _roleDL.UpdateRole(requestClient, permissionsAdd, permissionsDelete);

            if (numberOfRowsAffected == permissionsAdd.Count + permissionsDelete.Count + 1)
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
        }

        /// <summary>
        /// Xử lý/Tách danh sách quyền client gửi về
        /// </summary>
        /// <param name="permissions">Danh sách quyền</param>
        /// <param name="permissionsAdd">Các quyền thêm</param>
        /// <param name="permissionsDelete">Các quyền xóa</param>
        /// CreatedBy: TienDao (05/01/2023)
        private void HandlerPermissions(List<SubSystemAndPermission> permissions, List<SubSystemAndPermission> permissionsAdd, List<SubSystemAndPermission> permissionsDelete)
        {
            permissions.ForEach(permission =>
            {
                if (permission.State == State.Add)
                {
                    permissionsAdd.Add(permission);
                }

                if (permission.State == State.Detele)
                {
                    permissionsDelete.Add(permission);
                }
            });
        }
    }
}
