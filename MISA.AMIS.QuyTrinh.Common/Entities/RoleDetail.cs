using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.Common.Entities
{
    /// <summary>
    /// Chi tiết vai trò
    /// </summary>
    public class RoleDetail : BaseEntity
    {
        /// <summary>
        /// ID vai trò
        /// </summary>
        public Guid RoleID { get; set; }

        /// <summary>
        /// ID quyền và phân quyền
        /// </summary>
        public Guid SubSystemPremissionID { get; set; }
    }
}
