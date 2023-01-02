using Dapper;
using MISA.AMIS.QuyTrinh.Common.Const;
using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.Common.Enum;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.DL.SubSystemDL
{
    public class SubSystemDL : ISubSystemDL
    {
        /// <summary>
        /// Lấy danh sách phân quyền
        /// </summary>
        /// <returns>Danh sách phân quyền</returns>
        /// /// Created by: TienDao (28/12/2022)
        public List<SubSystem> GetListSubSystem()
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = Procedure.GET_LIST_PERMISSION;

            //Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString))
            {
                //Thực hiện gọi vào DB
                var result = mySqlConnection.Query(storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);

                List<SubSystem> subSystems = result.GroupBy(x => (x.SubSystemID, x.SubSystemCode, x.SubSystemName, x.SubSystemType))
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
                    }).ToList();

                return subSystems;
            }
            throw new NotImplementedException();
        }
    }
}
