using Dapper;
using MISA.AMIS.QuyTrinh.Common.Const;
using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.Common.Enum;
using MISA.AMIS.QuyTrinh.DL.BaseDL;
using MySqlConnector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.DL.RoleDL
{
    public class RoleDL : BaseDL<Role>, IRoleDL
    {
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
        public PagingResult<Role> GetRolesByFilterAndPaging(string keyword, int limit, int offset, string fieldSort, TypeSort typeSort, RoleStatus roleStatus)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = Procedure.GET_ROLES_BY_FILTER_PAGING;

            //Chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("@Keyword", keyword);
            parameters.Add("@Limit", limit);
            parameters.Add("@Offset", offset);
            parameters.Add("@FieldSort", fieldSort);
            parameters.Add("@TypeSort", typeSort);
            parameters.Add("@roleStatus", roleStatus);

            //Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = CreateDBConnection())
            {
                //Thực hiện gọi vào DB
                var results = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                var roles = results.Read<Role>().ToList();
                long TotalRecords = results.Read<long>().Single();

                //Xử lý kết quả trả về
                if (roles != null)
                {
                    return new PagingResult<Role>
                    {
                        ListData = roles,
                        TotalRecords = TotalRecords
                    };
                }
                // không có bản ghi nào trong db
                return new PagingResult<Role>
                {
                    ListData = new List<Role>(),
                    TotalRecords = 0
                };

            }
        }

        /// <summary>
        /// Lấy chi tiết 1 vai trò
        /// </summary>
        /// <param name="roleID">ID vai trò</param>
        /// <returns>1 bản ghi vai trò đẩy đủ</returns>
        /// Created by: TienDao(22/12/2022)
        public Role GetRoleDetailByID(Guid roleID)
        {
            // Chuẩn bị câu lệnh SQL
            string storedProcedureName = Procedure.GET_ROLE_DETAIL_BY_ID;

            // Chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("@RoleID", roleID);
            // Thực hiện gọi vào DB
            using (var mySqlConnection = CreateDBConnection())
            {
                var results = mySqlConnection.QueryMultiple(sql: storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                var result1 = results.Read().ToList();
                var result2 = results.Read().ToList()[0];

                //Nếu DB vai trò không có phân quyền nào
                if (result1.Count == 0)
                {
                    var role = new Role
                    {
                        RoleID = result2.RoleID,
                        RoleName = result2.RoleName,
                        RoleDescription = result2.RoleDescribe,
                        RoleStatus = (RoleStatus)result2.RoleStatus,
                    };
                    return role;
                }
                // DB trả về vai trò có phân quyền
                // Thực hiện:
                // - Gộp các quyền theo phân quyền
                // - Gộp các phân quyền theo vai trò
                else
                {
                    var role = result1.GroupBy(x => (x.RoleID, x.RoleName, x.RoleDescribe, x.RoleStatus))
                    .Select(g => new Role
                    {
                        RoleID = g.Key.RoleID,
                        RoleName = g.Key.RoleName,
                        RoleDescription = g.Key.RoleDescribe,
                        RoleStatus = (RoleStatus)g.Key.RoleStatus,
                        ListSubsytemAndPermission = g.GroupBy(x => (x.SubSystemID, x.SubSystemCode, x.SubSystemName, x.SubSystemType))
                            .Select(g => new SubSystem
                            {
                                SubSystemID = g.Key.SubSystemID,
                                SubSystemCode = g.Key.SubSystemCode,
                                SubSystemName = g.Key.SubSystemName,
                                SubSystemType = (SubSystemType)g.Key.SubSystemType,
                                ListPermissions = g.GroupBy(x => x)
                                    .Select(g => new Permission
                                    {
                                        PermissionID = g.Key.PermissionID,
                                        PermissionCode = g.Key.PermissionCode,
                                        PermissionName = g.Key.PermissionName,
                                    }).ToList()
                            }).ToList()
                    }).ToList()[0];
                    return role;
                }

            }
        }

        /// <summary>
        /// DL Thêm vai trò
        /// </summary>
        /// <param name="requestClient">request client (dùng để lấy cá thông tin cố định (tên, mô tả))</param>
        /// <param name="permissionsAdd">Danh sách quyền thêm</param>
        /// <param name="permissionsDelete">Danh sách quyền xóa</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        public int InsertRole(RequestClient requestClient, List<SubSystemAndPermission> permissionsAdd)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = Procedure.INSERT_ROLE;

            //Chuẩn bị tham số đầu vào
            Guid roleID = Guid.NewGuid();

            string roleName = requestClient.RoleName;
            string roleCode = requestClient.RoleCode;
            string roleDescription = requestClient.RoleDescription;
            string presentTime = "NOW()";
            //string user = requestClient.User;
            string user = "Tiến Đạo";

            //Chuỗi danh sách Quyền - Phân quyền thêm mới
            string listSubPerAdd = "";

            int index = 0;

            //Thực hiện nối chuỗi
            permissionsAdd.ForEach((permission) =>
            {
                listSubPerAdd += "('" + roleID + "','" + permission.SubSystemID + "','"
                    + permission.PermissionID + "'," + presentTime + ",'" + user + "')";
                if (index < permissionsAdd.Count - 1)
                {
                    listSubPerAdd += ",";
                }
                index++;
            });

            var parameters = new DynamicParameters();
            parameters.Add("@RoleID", roleID);
            parameters.Add("@RoleName", roleName);
            parameters.Add("@RoleDescription", roleDescription);
            parameters.Add("@CreatedDate", DateTime.Now);
            parameters.Add("@CreatedBy", user);

            parameters.Add("@ListSubPerAdd", listSubPerAdd);

            int numberOfRowsAffected = 0;
            //Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = CreateDBConnection())
            {
                mySqlConnection.Open();
                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    try
                    {
                        numberOfRowsAffected = mySqlConnection.QueryFirst<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                        if (numberOfRowsAffected == permissionsAdd.Count + 1)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        transaction.Rollback();
                    }
                }
            }
            return numberOfRowsAffected;
        }

        /// <summary>
        /// DL Sửa vai trò
        /// </summary>
        /// <param name="requestClient">request client (dùng để lấy cá thông tin cố định (tên, mô tả))</param>
        /// <param name="permissionsAdd">Danh sách quyền thêm</param>
        /// <param name="permissionsDelete">Danh sách quyền xóa</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        public int UpdateRole(RequestClient requestClient, List<SubSystemAndPermission> permissionsAdd, List<SubSystemAndPermission> permissionsDelete)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = Procedure.UPDATE_ROLE;

            //Chuẩn bị tham số đầu vào
            Guid roleID = (Guid)requestClient.RoleID;

            string roleName = requestClient.RoleName;
            string roleCode = requestClient.RoleCode;
            string roleDescription = requestClient.RoleDescription;
            string presentTime = "NOW()";
            string user = "Tiến Đạo";

            //Chuỗi danh sách Quyền - Phân quyền thêm mới
            string listSubPerAdd = "";

            //Chuỗi danh sách Quyền - Phân quyền xóa
            string listSubPerDelete = "";

            int index = 0;
            permissionsAdd.ForEach((permission) =>
            {
                listSubPerAdd += "('" + roleID + "','" + permission.SubSystemID + "','"
                    + permission.PermissionID + "'," + presentTime + ",'" + user + "')";
                if (index < permissionsAdd.Count - 1)
                {
                    listSubPerAdd += ",";
                }
                index++;
            });
            index = 0;
            permissionsDelete.ForEach((permission) =>
            {
                listSubPerDelete += "('" + permission.SubSystemID + "','" + permission.PermissionID + "')";
                if (index < permissionsDelete.Count - 1)
                {
                    listSubPerDelete += ",";
                }
                index++;
            });

            var parameters = new DynamicParameters();
            parameters.Add("@RoleID", roleID);
            parameters.Add("@RoleName", roleName);
            parameters.Add("@RoleDescription", roleDescription);
            parameters.Add("@ModifiedDate", DateTime.Now);
            parameters.Add("@ModifiedBy", user);
            parameters.Add("@ListSubPerAdd", listSubPerAdd);
            parameters.Add("@ListSubPerDelete", listSubPerDelete);

            int numberOfRowsAffected = 0;
            //Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = CreateDBConnection())
            {
                mySqlConnection.Open();
                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    try
                    {
                        numberOfRowsAffected = mySqlConnection.QueryFirstOrDefault<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                        if (numberOfRowsAffected == permissionsDelete.Count + permissionsAdd.Count + 1)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        transaction.Rollback();
                    }
                }
            }
            return numberOfRowsAffected;
        }
    }
}
