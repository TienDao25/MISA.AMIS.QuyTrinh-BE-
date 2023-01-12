using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.Common.Enum;
using MISA.AMIS.QuyTrinh.Common.Resource;
using MISA.AMIS.QuyTrinh.BL.SubSystemBL;

namespace MISA.AMIS.QuyTrinh.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SubSystemsController : ControllerBase
    {
        #region Field

        private ISubSystemBL _subSystemBL;

        #endregion

        #region Contrustor

        public SubSystemsController(ISubSystemBL subSystemBL)
        {
            _subSystemBL = subSystemBL;
        }

        #endregion

        /// <summary>
        /// API lấy danh sách phân quyền
        /// </summary>
        /// <returns>Danh sách phân quyền</returns>
        /// CreatedBy: TienDao(22/12/2022)
        [HttpGet]
        public IActionResult GetListSubSystem()
        {
            try
            {
                List<SubSystem> subSystems = _subSystemBL.GetListSubSystem();
                if (subSystems != null)
                {
                    return StatusCode(StatusCodes.Status200OK, subSystems);

                }
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResult
                {
                    ErrorCode = AMISErrorCode.GetAll,
                    DevMsg = Resource.DevMsg_GetAll,
                    UserMsg = Resource.UserMsg_GetAll,
                    MoreInfo = Resource.MoreInfo_GetAll,
                    TraceId = HttpContext.TraceIdentifier
                });

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResult
                {
                    ErrorCode = AMISErrorCode.Exception,
                    DevMsg = Resource.DevMsg_Exception,
                    UserMsg = Resource.UserMsg_Exception,
                    MoreInfo = Resource.MoreInfo_Exception,
                    TraceId = HttpContext.TraceIdentifier
                });
            }
        }

    }
}
