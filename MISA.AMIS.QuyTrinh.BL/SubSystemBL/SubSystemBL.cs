using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.DL.SubSystemDL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MISA.AMIS.QuyTrinh.BL.SubSystemBL
{
    public class SubSystemBL : ISubSystemBL
    {
        #region Field

        private ISubSystemDL _subSystemDL;

        #endregion


        #region Contrustor

        public SubSystemBL(ISubSystemDL subSystemDL)
        {
            _subSystemDL = subSystemDL;
        }

        #endregion

        /// <summary>
        /// Lấy danh sách phân quyền
        /// </summary>
        /// <returns>Danh sách phân quyền</returns>
        /// /// Created by: TienDao (28/12/2022)
        public List<SubSystem> GetListSubSystem()
        {
            return _subSystemDL.GetListSubSystem();
            throw new NotImplementedException();
        }
    }
}
