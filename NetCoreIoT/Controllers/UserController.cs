using Microsoft.AspNetCore.Mvc;
using NetCoreIoT.Model.User;
using NetCoreIoT.Services.User;

namespace NetCoreIoT.Controllers
{
    public class UserController : Controller
    {
        private IUserServices _user;

        public UserController(IUserServices _user) 
        {
            this._user = _user;
        
        
        }

        /// <summary>
        /// 返回用户登录状态
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<object>>> Login([FromBody] LoginRequest request)
        {
            var user = await _user.AuthenticateAsync(request.Username, request.Password);
            if (user == null)
                return ApiResponse<object>.Fail("用户名或密码错误", 401);

            var result = new
            {
                user.Id,
                user.Username,
                user.token
            };

            return ApiResponse<object>.Success(result);
        }

    }
}
