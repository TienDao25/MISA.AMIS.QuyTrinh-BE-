using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Const
{
    public class Procedure
    {
        /// <summary>
        /// Format lấy tất cả bản ghi
        /// </summary>
        public static string GET_ALL = "Proc_{0}_GetAll";

        /// <summary>
        /// Format lấy chi tiết bản ghi theo id
        /// </summary>
        public static string GET_BY_ID = "Proc_{0}_GetById";

        /// <summary>
        /// Procedure lấy chi tiết vai trò theo id
        /// </summary>
        public static string GET_ROLE_DETAIL_BY_ID = "Proc_role_GetById";

        /// <summary>
        /// Procedure lấy danh sách vai trò theo bộ lọc và phân trang
        /// </summary>
        public static string GET_ROLES_BY_FILTER_PAGING = "Proc_role_FindPagingSort";

        /// <summary>
        /// Procedure xóa vai trò
        /// </summary>
        public static string DELETE_RECORD_BY_ID = "Proc_{0}_DeleteByID";

        /// <summary>
        /// Procedure lấy danh sách phân quyền
        /// </summary>
        public static string GET_LIST_PERMISSION = " Proc_permission_GetAllDetail";

        /// <summary>
        /// Procrdure đếm vai trò theo tên
        /// </summary>
        public static string GET_COUNT_ROLE_BY_NAME = "Proc_role_CountRoleByName";

        /// <summary>
        /// Procrdure thêm mới/Sửa vai trò
        /// </summary>
        public static string INSERT_UPDATE_ROLE = "Proc_role_InsertUpdate";

        /// <summary>
        /// Procrdure thêm mới vai trò
        /// </summary>
        public static string INSERT_ROLE = "Proc_role_Insert";

        /// <summary>
        /// Procrdure Sửa vai trò
        /// </summary>
        public static string UPDATE_ROLE = "Proc_role_Update";
    }
}
