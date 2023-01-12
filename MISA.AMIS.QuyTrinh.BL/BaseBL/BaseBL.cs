using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.DL.BaseDL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

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
        /// Thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Thông tin cần thêm</param>
        /// <returns>ID bản ghi được thêm</returns>
        /// Author: TienDao (11/01/2023)
        public virtual ResponseService Insert(List<T> entities)
        {
            return new ResponseService { };
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

        /// <summary>
        /// Kiểm tra trùng
        /// </summary>
        /// <param name="fieldName">Tên trường</param>
        /// <param name="valueNeedCheck">Giá trị check</param>
        /// <param name="recordID">ID bản ghi</param>
        /// <returns>True: trùng, False: không trùng</</returns>
        public bool CheckDulicate(string fieldName, string valueNeedCheck, Guid? recordID)
        {
            return _baseDL.CheckDulicate(fieldName, valueNeedCheck, recordID);
        }

        /// <summary>
        /// API lấy danh sách bản ghi theo bộ lọc và phân trang
        /// </summary>
        /// <returns>Danh sách bản ghi và tổng số bản ghi</returns>
        /// Created by: TienDao (11/01/2023)
        public PagingResult<T> GetRecordByFilterAndPaging(List<Filter> filter, int limit, int offset, Sort sort)
        {
            string queryWhere = "";
            string paging = "";
            if (filter.Count > 0)
            {
                queryWhere = ConCatQueryFilter(filter, queryWhere);
            }
            else
            {
                queryWhere = " 1=1";
            }
            if (sort.TypeSort != Common.Enum.TypeSort.None)
            {
                queryWhere += $" ORDER BY {sort.Selector} {sort.TypeSort}";
            }
            paging += $" LIMIT {limit} OFFSET {offset}";
            return _baseDL.GetRecordByFilterAndPaging(queryWhere, paging);
        }

        /// <summary>
        /// Nối chuỗi query
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="queryWhere"></param>
        /// <returns></returns>
        private static string ConCatQueryFilter(List<Filter>? filter, string queryWhere)
        {
            int index = 0;
            foreach (Filter f in filter)
            {
                if (!(string.IsNullOrEmpty(f.Column) && string.IsNullOrEmpty(f.Value)
                    && string.IsNullOrEmpty(f.Relationship) && string.IsNullOrEmpty(f.Operator)))
                {
                    f.Column = RemoveSpace(f.Column);
                    f.Operator = RemoveSpace(f.Operator);
                    f.Operator = RemoveSpace(f.Operator);
                    f.Value = SanitizeInput(f.Value);

                    queryWhere += (index == 0 ? " " : f.Relationship) + " " + f.Column + " " + f.Operator;
                    queryWhere = CaseOperator(queryWhere, f);
                }

                if (f.SubQuery != null)
                {
                    queryWhere += (string.IsNullOrEmpty(f.Column) == true ? "" : f.SubQuery.Operator) + " (" + ConCatQueryFilter(f.SubQuery.Detail, queryWhere) + ") ";
                }
                index++;
            }

            return queryWhere;
        }

        /// <summary>
        /// Xử lý nối câu phần toán tử
        /// </summary>
        /// <param name="queryWhere">chuỗi</param>
        /// <param name="f">bộ lọc</param>
        /// <returns></returns>
        private static string CaseOperator(string queryWhere, Filter f)
        {
            if (string.Equals(f.Operator, "like", StringComparison.OrdinalIgnoreCase))
            {
                queryWhere += " '%" + f.Value + "%' ";
            }
            else if (string.Equals(f.Operator, "in", StringComparison.OrdinalIgnoreCase))
            {
                string[] value = f.Value.Split(',');
                queryWhere += $" ( {string.Join(",", value.Select(v => "'" + v + "'"))}  ) ";
            }
            else
            {
                queryWhere += " " + f.Value + " ";
            }

            return queryWhere;
        }

        /// <summary>
        /// Xóa khoảng trắng
        /// </summary>
        /// <param name="str">Chuỗi cần xử ký</param>
        /// <returns>Chuỗi đã xóa khoảng trắng</returns>
        private static string RemoveSpace(string str)
        {
            return str.Replace(" ", "");
        }

        /// <summary>
        /// Xóa ký tự đặc biệt
        /// </summary>
        /// <param name="input">Chuỗi cần xử lý</param>
        /// <returns>Chuỗi đã xía ký tự đặc biệt</returns>
        private static string SanitizeInput(string input)
        {
            // Danh sách kí tự cần loại bỏ
            string[] dangerousChars = new string[] { "'", "--", "/*", "*/", ";", "%", "_", "=" };
            // loại bỏ các ký tự
            foreach (string dangerousChar in dangerousChars)
            {
                input = input.Replace(dangerousChar, "");
            }
            // Trả về input đã khử
            return input;
        }
        #endregion
    }
}
