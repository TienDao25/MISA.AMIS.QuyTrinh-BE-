using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MISA.AMIS.QuyTrinh.BL.RoleBL;
using MISA.AMIS.QuyTrinh.Common.Entities;
using MISA.AMIS.QuyTrinh.Common.Entities.DTO;
using MISA.AMIS.QuyTrinh.Common.Enum;
using MISA.AMIS.QuyTrinh.Common.Resource;

namespace MISA.AMIS.QuyTrinh.API.Controllers
{
    public class RolesController : BaseController<Role>
    {
        #region Field

        private IRoleBL _roleBL;

        #endregion

        #region Contrustor

        public RolesController(IRoleBL roleBL) : base(roleBL)
        {
            _roleBL = roleBL;
        }

        #endregion

        #region Method

        /// <summary>
        /// API lấy chi tiết bản ghi vai trò
        /// </summary>
        /// <returns>chi tiết 1 vài trò</returns>
        /// CreatedBy: TienDao(22/12/2022)
        [HttpGet("{roleID}")]
        public IActionResult GetRoleDetailByID(Guid roleID)
        {
            try
            {
                Role role = _roleBL.GetRoleDetailByID(roleID);
                if (role != null)
                {
                    return StatusCode(StatusCodes.Status200OK, role);

                }
                return StatusCode(StatusCodes.Status404NotFound, new ErrorResult
                {
                    ErrorCode = AMISErrorCode.GetDetail,
                    DevMsg = Resource.DevMsg_GetDetail,
                    UserMsg = Resource.UserMsg_GetDetail,
                    MoreInfo = Resource.MoreInfo_GetDetail,
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

        /// <summary>
        /// API lấy danh sách vai trò theo bộ lọc và phân trang
        /// </summary>
        /// <param name="keyword">Từ khóa muốn tìm kiếm</param>
        /// <param name="limit">Số bản ghi muốn lấy</param>
        /// <param name="offset">Vị trí của bản ghi bắt đầu lấy</param>
        /// <param name="fieldSort">Trường sắp xếp</param>
        /// <param name="typeSort">Kiểu sắp xếp</param>
        /// <param name="roleStatus">Trạng thái muốn lọc</param>
        /// <returns>Danh sách vai trò và tổng số bản ghi</returns>
        /// Created by: TienDao (26/12/2022)
        [HttpGet("filter")]
        public IActionResult GetRolesByFilterAndPaging(
            [FromQuery] string? keyword,
            [FromQuery] int limit = 10,
            [FromQuery] int offset = 0,
            [FromQuery] string? fieldSort = "",
            [FromQuery] TypeSort typeSort = TypeSort.None,
            [FromQuery] int? roleStatus = (int)RoleStatus.All)
        {
            try
            {
                var result = _roleBL.GetRolesByFilterAndPaging(keyword, limit, offset, fieldSort, typeSort, (RoleStatus)roleStatus);
                //Xử lý kết quả trả về

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

        /// <summary>
        /// Thêm mới vai trò
        /// </summary>
        /// <param name="requestClient">Request client gửi về</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        [HttpPost]
        public IActionResult InsertRole([FromBody] RequestClient requestClient)
        {
            try
            {
                // thực hiện thêm mới dữ liệu
                var result = _roleBL.InsertRole(requestClient);

                //Thành công
                if (result.IsSuccess == true)
                {
                    return StatusCode(StatusCodes.Status201Created);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = AMISErrorCode.InsertError,
                        DevMsg = Resource.DevMsg_InsertError,
                        UserMsg = Resource.UserMsg_InsertError,
                        MoreInfo = result.Data,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

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

        /// <summary>
        /// Cập nhật vai trò
        /// </summary>
        /// <param name="requestClient">Request client gửi về</param>
        /// <returns></returns>
        /// CreatedBy: TienDao (05/01/2023)
        [HttpPut]
        public IActionResult UpdateRole([FromBody] RequestClient requestClient)
        {
            try
            {
                // thực hiện thêm mới dữ liệu
                var result = _roleBL.UpdateRole(requestClient);

                //Thành công
                if (result.IsSuccess == true)
                {
                    return StatusCode(StatusCodes.Status200OK);
                }
                else
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new ErrorResult
                    {
                        ErrorCode = AMISErrorCode.InsertError,
                        DevMsg = Resource.DevMsg_InsertError,
                        UserMsg = Resource.UserMsg_InsertError,
                        MoreInfo = result.Data,
                        TraceId = HttpContext.TraceIdentifier
                    });
                }

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
