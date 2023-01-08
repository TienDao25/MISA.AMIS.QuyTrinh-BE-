using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.DL.BaseDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.BL.BaseBL
{
    public class BaseBL<T> : IBaseBL<T>
    {
        #region Field

        private IBaseDL<T> _baseDL;

        #endregion


        #region Contrustor

        public BaseBL(IBaseDL<T> baseDL)
        {
            _baseDL = baseDL;
        }

        #endregion

        #region Method

        /// <summary>
        /// Lấy danh sách tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách tất cả bản ghi</returns>
        /// Created by: TienDao(22/12/2022)
        public IEnumerable<T> GetAllRecords()
        {
            return _baseDL.GetAllRecords();
        }

        /// <summary>
        /// Lấy thông tin 1 bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID của bản ghi muốn lấy</param>
        /// <returns>Thông tin của 1 bản ghi</returns>
        /// Created by: TienDao(22/12/2022)
        public T GetRecordByID(Guid recordID)
        {
            return _baseDL.GetRecordByID(recordID);
        }

        /// <summary>
        /// Xóa/Hủy bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID bản muốn xóa</param>
        /// Created by: TienDao (27/12/2022)
        public ResponseService DeleteRecordByID(Guid recordID)
        {
            int numberOfRowsAffected = _baseDL.DeleteRecordByID(recordID);
            if (numberOfRowsAffected == 1)
            {
                return new ResponseService { IsSuccess = true };
            }
            else
            {
                return new ResponseService { IsSuccess = false };
            }
        }
        #endregion
    }
}
