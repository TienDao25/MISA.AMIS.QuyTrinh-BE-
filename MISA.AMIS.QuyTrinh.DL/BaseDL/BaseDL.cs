using Dapper;
using MISA.AMIS.QuyTrinh.Common.Const;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.DL.BaseDL
{
    public class BaseDL<T> : IBaseDL<T> where T : class
    {
        /// <summary>
        /// Lấy danh sách tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// Created by: TienDao(22/12/2022)
        public IEnumerable<T> GetAllRecords()
        {
            // Chuẩn bị câu lệnh SQL
            string storedProcedureName = String.Format(Procedure.GET_ALL, typeof(T).Name);

            var records = new List<T>();
            // Chuẩn bị tham số đầu vào

            // Thực hiện gọi vào DB
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString))
            {
                records = (List<T>)mySqlConnection.Query<T>(sql: storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);
            }
            return records;
        }

        /// <summary>
        /// Xóa/Hủy bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID bản muốn xóa</param>
        /// <returns>Số bản ghi xóa</returns>
        /// Created by: TienDao (27/12/2022)
        public int DeleteRecordByID(Guid recordID)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = String.Format(Procedure.DELETE_RECORD_BY_ID, typeof(T).Name);

            //Chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add($"@{typeof(T).Name}ID", recordID);
            parameters.Add("@ModifiedDate", DateTime.Now);
            parameters.Add("@ModifiedBy", "Tiến Đạo");

            int numberOfRowsAffected = 0;
            // Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString))
            {
                mySqlConnection.Open();
                using (var transaction = mySqlConnection.BeginTransaction())
                {
                    try
                    {
                        numberOfRowsAffected = mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);
                        transaction.Commit();

                    }
                    catch
                    {
                        numberOfRowsAffected = 0;
                        transaction.Rollback();
                    }
                }
            }
            return numberOfRowsAffected;
        }

        /// <summary>
        /// Kiểm tra trùng
        /// </summary>
        /// <param name="fieldName">Tên trường</param>
        /// <param name="valueNeedCheck">Giá trị check</param>
        /// <param name="recordID">ID bản ghi</param>
        /// <returns>True: trùng, False: không trùng</</returns>
        public bool CheckDulicate(string fieldName, string valueNeedCheck, Guid? recordID)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = String.Format(Procedure.CHECK_DULICATE, typeof(T).Name, fieldName);

            //Chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add($"@{fieldName}", valueNeedCheck);
            parameters.Add($"@{typeof(T).Name}ID", recordID);


            // Khởi tạo kết nối tới DB MySQL
            using (var mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString))
            {
                int numberRecords = mySqlConnection.QueryFirstOrDefault<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
                if (numberRecords == 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
