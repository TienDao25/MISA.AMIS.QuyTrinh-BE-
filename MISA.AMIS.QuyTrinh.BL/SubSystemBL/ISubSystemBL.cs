using MISA.AMIS.QuyTrinh.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.BL.SubSystemBL
{
    public interface ISubSystemBL
    {
        /// <summary>
        /// Lấy danh sách phân quyền
        /// </summary>
        /// <returns>Danh sách phân quyền</returns>
        /// /// Created by: TienDao (28/12/2022)
        public List<SubSystem> GetListSubSystem();
    }
}
