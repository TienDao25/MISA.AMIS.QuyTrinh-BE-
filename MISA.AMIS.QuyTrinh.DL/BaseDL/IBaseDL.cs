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
    public interface IBaseDL<T>
    {
        /// <summary>
        /// Lấy danh sách tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// Created by: TienDao(22/12/2022)
        public IEnumerable<T> GetAllRecords();

        /// <summary>
        /// Lấy thông tin 1 bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID của bản ghi muốn lấy</param>
        /// <returns>Thông tin của 1 bản ghi</returns>
        /// Created by: TienDao(22/12/2022)
        public T GetRecordByID(Guid recordID);

        /// <summary>
        /// Xóa/Hủy bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID bản muốn xóa</param>
        /// <returns>Số bản ghi xóa</returns>
        /// Created by: TienDao (27/12/2022)
        public int DeleteRecordByID(Guid recordID);


        /// <summary>
        /// Mở kết nối DB
        /// </summary>
        public void OpenDB();

    }
}
