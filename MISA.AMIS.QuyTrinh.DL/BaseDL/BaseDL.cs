using Dapper;
using MISA.AMIS.QuyTrinh.Common.Const;
using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.DL.BaseDL
{
    public class BaseDL<T> : IBaseDL<T> where T : class
    {
        public IDbConnection? mySqlConnection = null;

        /// <summary>
        /// Khởi tạo connection tới database
        /// </summary>
        /// <returns>New DB connection</returns>
        public IDbConnection CreateDBConnection()
        {
            return new MySqlConnection(DataBaseContext.ConnectionString);
        }

        /// <summary>
        /// Khởi tạo và mở connection tới database
        /// </summary>
        public void OpenDB()
        {
            mySqlConnection = new MySqlConnection(DataBaseContext.ConnectionString);
            mySqlConnection.Open();
        }

        /// <summary>
        /// Đóng connection tới database
        /// </summary>
        public void CloseDB()
        {
            if (mySqlConnection != null)
            {
                mySqlConnection.Close();
            }
        }

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
            OpenDB();
            records = (List<T>)mySqlConnection.Query<T>(sql: storedProcedureName, commandType: System.Data.CommandType.StoredProcedure);
            CloseDB();
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
            OpenDB();
            using (var transaction = mySqlConnection.BeginTransaction())
            {
                try
                {
                    numberOfRowsAffected = mySqlConnection.Execute(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);
                    transaction.Commit();
                    CloseDB();

                }
                catch
                {
                    numberOfRowsAffected = 0;
                    transaction.Rollback();
                    CloseDB();

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
            OpenDB();
            int numberRecords = mySqlConnection.QueryFirstOrDefault<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            CloseDB();
            if (numberRecords == 0)
            {
                return false;
            }
            return true;

        }

        /// <summary>
        /// API lấy danh sách bản ghi theo bộ lọc và phân trang
        /// </summary>
        /// <returns>Danh sách bản ghi và tổng số bản ghi</returns>
        /// Created by: TienDao (11/01/2023)
        public PagingResult<T> GetRecordByFilterAndPaging(string queryWhere, string paging)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = String.Format(Procedure.FILTER_PAGING, typeof(T).Name);

            //Chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add("@QueryWhere", queryWhere);
            parameters.Add("@Paging", paging);

            // Khởi tạo kết nối tới DB MySQL
            OpenDB();
            var results = mySqlConnection.QueryMultiple(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure);
            
            var roles = results.Read<T>().ToList();
            long TotalRecords = results.Read<long>().Single();
            CloseDB();
            //Xử lý kết quả trả về
            if (roles != null)
            {
                return new PagingResult<T>
                {
                    ListData = roles,
                    TotalRecords = TotalRecords
                };
            }
            // không có bản ghi nào trong db
            return new PagingResult<T>
            {
                ListData = new List<T>(),
                TotalRecords = 0
            };
        }

        /// <summary>
        /// Thêm mới bản ghi
        /// </summary>
        /// <param name="queryAdd">Bản ghi cha</param>
        /// <param name="listAddDetail">Danh sách thông tin bản ghi con</param>
        /// <param name="numberRows">Tổng số bản ghi cần thực hiện</param>
        /// <returns>ID bản ghi được thêm</returns>
        /// Author: TienDao (11/01/2023)
        public int Insert(string queryAdd, List<string> listAddDetail, int numberRows)
        {
            //Chuẩn bị câu lệnh SQL
            string storedProcedureName = String.Format(Procedure.INSERT, typeof(T).Name);

            //Chuẩn bị tham số đầu vào
            var parameters = new DynamicParameters();
            parameters.Add($"@QueryAdd", queryAdd);
            for (int i = 0; i < listAddDetail.Count; i++)
            {
                parameters.Add($"@QueryAddDetail{i}", listAddDetail[i]);
            }
            int numberOfRowsAffected = 0;
            // Khởi tạo kết nối tới DB MySQL
            OpenDB();
            using (var transaction = mySqlConnection.BeginTransaction())
            {
                try
                {
                    numberOfRowsAffected = mySqlConnection.QueryFirst<int>(storedProcedureName, parameters, commandType: System.Data.CommandType.StoredProcedure, transaction: transaction);

                    if (numberOfRowsAffected == numberRows)
                    {
                        transaction.Commit();
                        CloseDB();
                    }
                    else
                    {
                        transaction.Rollback();
                        numberOfRowsAffected = 0;
                        CloseDB();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    transaction.Rollback();
                    CloseDB();
                }
            }

            return numberOfRowsAffected;
        }
    }
}
