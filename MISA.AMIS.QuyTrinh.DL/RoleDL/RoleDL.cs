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
        /// <returns>Danh sách vai trò và tổng số bản ghi</returns>
        /// Created by: TienDao (26/12/2022)
        public PagingResult<Role> GetRolesByFilterAndPaging(string keyword, int limit, int offset, string fieldSort, string typeSort)
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

            //Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString))
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
            throw new NotImplementedException();
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
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString))
            {
                var results = mySqlConnection.QueryMultiple(sql: storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                var result1 = results.Read().ToList();
                var result2 = results.Read().ToList()[0];
                if (result1.Count == 0)
                {
                    var role = new Role
                    {
                        RoleID = result2.RoleID,
                        RoleName = result2.RoleName,
                        RoleDescribe = result2.RoleDescribe,
                        RoleStatus = (RoleStatus)result2.RoleStatus,
                    };
                    return role;
                }
                else
                {
                    var role = result1.GroupBy(x => (x.RoleID, x.RoleName, x.RoleDescribe, x.RoleStatus))
                    .Select(g => new Role
                    {
                        RoleID = g.Key.RoleID,
                        RoleName = g.Key.RoleName,
                        RoleDescribe = g.Key.RoleDescribe,
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
            throw new NotImplementedException();
        }

        /// <summary>
        /// Đếm số bản ghi vai trò theo tên
        /// </summary>
        /// <param name="roleName">Tên vai trò</param>
        /// <returns>Số bản ghi</returns>
        /// Created by: TienDao(31/12/2022)
        public int CountRoleByName(string roleName, Guid? roleID)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = Procedure.GET_COUNT_ROLE_BY_NAME;

            //Chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("@RoleName", roleName);
            parameters.Add("@RoleID", roleID);

            //Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString))
            {
                //Thực hiện gọi vào DB
                int numberRecords = mySqlConnection.QueryFirstOrDefault<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);

                return numberRecords;

            }
            throw new NotImplementedException();
        }

        /// <summary>
        /// Thêm mới vai trò
        /// </summary>
        /// <param name="role">Thông tin tổng quan vai trò</param>
        /// <param name="listSubSystemID">Danh sách id phân quyền</param>
        /// <param name="listPremissionID">Danh sách id quyền tương ứng với phân quyền</param>
        /// <returns>Số bản ghi</returns>
        /// Created by: TienDao (31/12/2022)
        public int InsertRole(Role role, List<Guid> listSubSystemID, List<Guid> listPermissionID)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = Procedure.INSERT_ROLE;

            //Chuẩn bị tham số đầu vào
            Guid roleID = Guid.NewGuid();
            var parameters = new DynamicParameters();
            parameters.Add("@RoleID", roleID);
            parameters.Add("@RoleID", roleID);
            parameters.Add("@RoleName", role.RoleName);
            parameters.Add("@RoleDescribe", role.RoleDescribe);
            parameters.Add("@ListSubSystemID", string.Join(",", listSubSystemID));
            parameters.Add("@ListPermissionID", string.Join(",", listPermissionID));
            parameters.Add("@CreatedDate", DateTime.Now);
            parameters.Add("@CreatedBy", "Tiến Đạo");

            int numberOfRowsAffected = 0;
            //Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString))
            {
                mySqlConnection.Open();
                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    try
                    {
                        numberOfRowsAffected = mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                        if (numberOfRowsAffected > 0)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }

                    }
                    catch
                    {
                        transaction.Rollback();

                    }
                    //numberOfRowsAffected = mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                    //if (numberOfRowsAffected == listSubSystemID.Count + 1)
                    //{
                    //    transaction.Commit();
                    //}
                    //else
                    //{
                    //    transaction.Rollback();
                    //}
                }
            }
            return numberOfRowsAffected;
            throw new NotImplementedException();
        }
    }
}
