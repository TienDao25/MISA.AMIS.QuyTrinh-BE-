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
            if (CheckDulicate("RoleName", requestClient.RoleName, null) == true)
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

            //Tách danh sách mảng theo state
            HandlerPermissions(requestClient.ModeForm, requestClient.Permissions, permissionsAdd, null);

            int numberOfRowsAffected = _roleDL.InsertRole(requestClient, permissionsAdd);

            if (numberOfRowsAffected == permissionsAdd.Count + 1)
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
            if (CheckDulicate("RoleName", requestClient.RoleName, requestClient.RoleID) == true)
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

            //Tách danh sách mảng
            HandlerPermissions(requestClient.ModeForm, requestClient.Permissions, permissionsAdd, permissionsDelete);

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
        private void HandlerPermissions(ModeForm modeForm, List<SubSystemAndPermission> permissions, List<SubSystemAndPermission> permissionsAdd, List<SubSystemAndPermission>? permissionsDelete)
        {
            if (modeForm == ModeForm.Add || modeForm == ModeForm.Dulicate)
            {
                permissions.ForEach(permission =>
                {
                    permissionsAdd.Add(permission);
                });
            }

            if (modeForm == ModeForm.Update)
            {
                permissions.ForEach(permission =>
                {
                    if (permission.State == State.Add)
                    {
                        permissionsAdd.Add(permission);
                    }

                    if (permission.State == State.Detele)
                    {
                        permissionsDelete?.Add(permission);
                    }
                });
            }

        }

        /// <summary>
        /// Xử lý trước khi lưu, thêm validate tên trùng
        /// </summary>
        /// <param name="entities">Danh sách đối tượng</param>
        /// <param name="validateFailures">Mảng chứa lỗi</param>
        public override void BeforeSave(List<Role> entities, List<string> validateFailures)
        {
            base.BeforeSave(entities, validateFailures);
            //Kiểm tra trùng tên trong list
            if (entities.GroupBy(x => x.RoleName).Any(g => g.Count() > 1))
            {
                validateFailures.Add(Resource.Error_DulicateRoleName);
                return;
            }

            foreach (Role role in entities)
            {
                //Kiểm tra trùng tên với các bản ghi trong DB
                if (CheckDulicate("RoleName", role.RoleName, null) == true)
                {
                    validateFailures.Add(Resource.Error_DulicateRoleName);
                    return;
                }
            }
        }

        public override int DoSave(List<Role> entities)
        {
            base.DoSave(entities);
            return 0;
        }
    }
}
