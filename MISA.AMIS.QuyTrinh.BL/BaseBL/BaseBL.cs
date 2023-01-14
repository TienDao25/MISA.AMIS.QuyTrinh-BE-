﻿using MISA.AMIS.QuyTrinh.Common.Attributes;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.DL.BaseDL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Dapper.SqlMapper;
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
            IDbConnection? connection = null;
            IDbTransaction? transaction = null;

            //Validate dữ liệu
            var validateFailures = new List<string>();

            ValidateData(entities, validateFailures);
            if (validateFailures.Count > 0)
            {
                return new ResponseService()
                {
                    IsSuccess = false,
                    Data = validateFailures
                };
            }

            //Xử lý, cắt ghép chuỗi trước khi lưu
            string queryAdd = "";
            List<string> listAddDetail = new List<string>();
            int numberRows = 0;

            BeforeSave(entities, out queryAdd, listAddDetail, out numberRows);

            //Tạo connection
            connection = _baseDL.CreateDBConnection();
            connection.Open();

            //Tạo transaction
            transaction = connection.BeginTransaction();

            //DoSave() 
            int numberOfRowsAffected = _baseDL.Insert(queryAdd, listAddDetail, numberRows);
            if (numberOfRowsAffected == numberRows)
            {
                return new ResponseService
                {
                    IsSuccess = true
                };
            }
            return new ResponseService
            {
                IsSuccess = false,
            };

            AfterSave();
        }

        /// <summary>
        /// Xử lý, ghép chuỗi trước khi lưu
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="queryAdd"></param>
        /// <param name="listAddDetail"></param>
        /// <param name="numberRows"></param>
        private void BeforeSave(List<T> entities, out string queryAdd, List<string> listAddDetail, out int numberRows)
        {
            Guid newID = Guid.NewGuid();
            List<string> values = new List<string>();
            var listDetail = new List<object>();
            entities.ForEach(entity =>
            {
                //Xử lý, add các giá trị thực thể vào mảng
                List<string> value = BuildListStringForSave(entity, newID, listDetail);

                //Nối các phần tử trong mảng
                values.Add($"({string.Join(",", value.Select(v => (string.Equals(v, "NOW()") || string.Equals(v, "null") ? v : ("'" + v + "'"))))})");
            });

            //Query thêm mới bản ghi cha
            queryAdd = string.Join(",", values);

            //Query thêm mới các bản ghi con
            int count = 0;
            if (listDetail.Count > 0)
            {
                foreach (var detail in listDetail)
                {
                    var collectionDetail = (ICollection)detail;
                    count += collectionDetail.Count;

                    //Xử lý, thêm các giá trị bản ghi detail vào mảng
                    List<string> addDetail = BuildListStringForDetail(newID, collectionDetail);

                    listAddDetail.Add(string.Join(",", addDetail));
                }
            }
            numberRows = count + 1;
        }

        /// <summary>
        /// Xử lý, thêm các giá trị bản ghi detail vào mảng
        /// </summary>
        /// <param name="newID">ID bản ghi cha</param>
        /// <param name="collectionDetail">Object chứa các bản ghi của trường detail</param>
        /// <returns></returns>
        private List<string> BuildListStringForDetail(Guid newID, ICollection collectionDetail)
        {
            List<string> addDetail = new List<string>();
            foreach (var item in collectionDetail)
            {
                List<string> value = new List<string>();
                value.Add(newID.ToString());
                var properties = item.GetType().GetProperties();
                foreach (var property in properties)
                {
                    var propertyName = property.Name;
                    var sqlIgnoreAttribute = (SqlIgnoreAttribute?)Attribute.GetCustomAttribute(property, typeof(SqlIgnoreAttribute));
                    if (sqlIgnoreAttribute == null)
                    {
                        if (propertyName.Equals($"{typeof(T).Name}ID"))
                        {
                            property.SetValue(item, newID, null);
                        }
                        if (propertyName.Equals("ModifiedDate") || propertyName.Equals("ModifiedBy"))
                        {
                            continue;
                        }
                        if (propertyName.Equals($"CreatedDate"))
                        {
                            property.SetValue(item, DateTime.Now, null);
                        }
                        var propertyValue = property.GetValue(item);

                        //Add thêm phần tử vào mảng
                        value = ExpandValue(value, propertyValue);
                    }
                }
                addDetail.Add($"({string.Join(",", value.Select(v => (string.Equals(v, "NOW()") || string.Equals(v, "null") ? v : ("'" + v + "'"))))})");
            }
            return addDetail;
        }

        /// <summary>
        /// Xử lý, Chèn các giá trị cần lưu của thực thể vào mảng.
        /// Thêm các giá trị của trường detail vào mảng
        /// </summary>
        /// <param name="entity">Đối tượng</param>
        /// <param name="newID">ID mới của đối tượng</param>
        /// <param name="listDetail">Danh sách mảng các giá trị của các trường detail</param>
        /// <returns></returns>
        private List<string> BuildListStringForSave(T entity, Guid newID, List<object> listDetail)
        {
            //Lấy danh sách property
            var properties = typeof(T).GetProperties();
            List<string> value = new List<string>();
            foreach (var property in properties)
            {
                var propertyName = property.Name;
                var requiredAttribute = (RequiredAttribute?)Attribute.GetCustomAttribute(property, typeof(RequiredAttribute));
                var manyToManyAttribute = (ManyToManyAttribute?)Attribute.GetCustomAttribute(property, typeof(ManyToManyAttribute));
                var sqlIgnoreAttribute = (SqlIgnoreAttribute?)Attribute.GetCustomAttribute(property, typeof(SqlIgnoreAttribute));
                var uniqueAttribute = (UniqueAttribute?)Attribute.GetCustomAttribute(property, typeof(UniqueAttribute));

                //Lưu các trường không chứa Attribute SqlIgnore
                if (sqlIgnoreAttribute == null)
                {
                    if (propertyName.Equals($"{typeof(T).Name}ID"))
                    {
                        property.SetValue(entity, newID, null);
                    }
                    if (propertyName.Equals("ModifiedDate") || propertyName.Equals("ModifiedBy"))
                    {
                        continue;
                    }
                    if (propertyName.Equals($"CreatedDate"))
                    {
                        property.SetValue(entity, DateTime.Now, null);
                    }
                    var propertyValue = property.GetValue(entity);

                    //Add thêm phần tử vào mảng
                    value = ExpandValue(value, propertyValue);
                }
                else
                {
                    if (manyToManyAttribute != null)
                    {
                        listDetail.Add(property.GetValue(entity));
                    }
                }
            }
            return value;
        }

        /// <summary>
        /// Nối chuối
        /// </summary>
        /// <param name="str">Chuỗi</param>
        /// <param name="propertyValue"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private List<string> ExpandValue(List<string> value, object? propertyValue)
        {
            if (propertyValue != null)
            {
                if (propertyValue.GetType().Name == "DateTime")
                {
                    value.Add("NOW()");
                }
                else if (propertyValue.GetType().IsEnum)
                {
                    value.Add($"{(int)propertyValue}");
                }
                else if (propertyValue == "")
                {
                    value.Add("null");
                }
                else
                {
                    value.Add($"{propertyValue.ToString()}");
                }
            }
            else
            {
                value.Add("null");
            }
            return value;
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

        /// <summary>
        /// Sau khi lưu, dùng để lưu thêm detail
        /// </summary>
        public virtual void AfterSave()
        {
            // do smt
        }

        /// <summary>
        /// Validate dữ liệu
        /// </summary>
        /// <param name="entities">Danh sách đối tượng</param>
        /// <param name="validateFailures">Mảng chứa lỗi</param>
        public virtual void ValidateData(List<T> entities, List<string> validateFailures)
        {
            // do smt
            foreach (T entity in entities)
            {
                var properties = typeof(T).GetProperties();

                foreach (var property in properties)
                {
                    var propertyValue = property.GetValue(entity);
                    var requiredAttribute = (RequiredAttribute?)Attribute.GetCustomAttribute(property, typeof(RequiredAttribute));
                    if (requiredAttribute != null && string.IsNullOrEmpty(propertyValue?.ToString()))
                    {
                        validateFailures.Add(requiredAttribute.ErrorMessage); ;
                    }
                }
            }
        }

        /// <summary>
        /// Lưu đối tượng
        /// </summary>
        public virtual int DoSave(T entity, Guid? id = null)
        {
            if(id== null)
            {
                return 1;
            }
                return 1;
        }
        #endregion
    }
}
