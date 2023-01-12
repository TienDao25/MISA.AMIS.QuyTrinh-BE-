using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.QuyTrinh.BL.BaseBL;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.Common.Enum;
using MISA.AMIS.QuyTrinh.Common.Resource;

namespace MISA.AMIS.QuyTrinh.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BaseController<T> : ControllerBase
    {
        #region Field

        private IBaseBL<T> _baseBL;

        #endregion

        #region Constructor

        public BaseController(IBaseBL<T> balseBL)
        {
            _baseBL = balseBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// API trả về thông tin tất cả bản ghi
        /// </summary>
        /// <returns>Danh sách thông tin tất cả bản ghi</returns>
        /// CreatedBy:TienDao (22/12/2022)
        [HttpGet]
        public IActionResult GetAllRecords()
        {
            try
            {
                var records = _baseBL.GetAllRecords();


                // Thành công: Trả về dữ liệu cho FE
                if (records != null)
                {
                    return StatusCode(StatusCodes.Status200OK, records);
                }

                // Thất bại: Trả về lỗi
                return StatusCode(StatusCodes.Status200OK, new List<T>());
            }
            //Try catch exception 
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

        /// <summary>
        /// API thêm mới bản ghi
        /// </summary>
        /// <param name="entity">Thông tin cần thêm</param>
        /// <returns>ID bản ghi được thêm</returns>
        /// Author: TienDao (11/01/2023)
        [HttpPost("TestBaseInsert")]
        public virtual IActionResult Insert([FromBody] List<T> entities)
        {
            try
            {
                var result = _baseBL.Insert(entities);
                // Thành công: Trả về dữ liệu cho FE
                if (!result.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = AMISErrorCode.InsertError,
                        DevMsg = Resource.DevMsg_InsertError,
                        UserMsg = Resource.UserMsg_InsertError,
                        MoreInfo = Resource.MoreInfo,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            //Try catch exception 
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

        /// <summary>
        /// API xóa 1 bản ghi theo ID
        /// </summary>
        /// <param name="recordID">ID bản ghi</param>
        /// <returns></returns>
        /// Created by: TienDao (27/12/2022)
        [HttpDelete("{recordID}")]
        public IActionResult DeleteRecordByID([FromRoute] Guid recordID)
        {
            try
            {
                var result = _baseBL.DeleteRecordByID(recordID);


                // Thành công: Trả về dữ liệu cho FE
                if (!result.IsSuccess)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new ErrorResult
                    {
                        ErrorCode = AMISErrorCode.DeleteError,
                        DevMsg = Resource.DevMsg_DeleteError,
                        UserMsg = Resource.UserMsg_DeleteError,
                        MoreInfo = Resource.MoreInfo,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }
                return StatusCode(StatusCodes.Status200OK);
            }
            //Try catch exception 
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

        /// <summary>
        /// API lấy danh sách bản ghi theo bộ lọc và phân trang
        /// </summary>
        /// <returns>Danh sách bản ghi và tổng số bản ghi</returns>
        /// Created by: TienDao (11/01/2023)
        [HttpPost("BaseFilter")]
        public IActionResult GetRecordByFilterAndPaging([FromBody] RequestFilter requestFilter)
        {
            try
            {
                var result = _baseBL.GetRecordByFilterAndPaging(requestFilter.Filter, requestFilter.Limit, requestFilter.Offset, requestFilter.Sort);
                return StatusCode(StatusCodes.Status200OK, result);

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

        #endregion
    }
}
