using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCoreIoT.Common;
using NetCoreIoT.Dal.User;
using NetCoreIoT.Model.User;

namespace NetCoreIoT.Services.User
{
    public class UserServices: IUserServices
    {
        private const string secertKey = "iotiot1133557799shengbiasitemask";
        private const string isuser = "systemadmin";
        private const string audience = "api";
        JwtHelper jwt=new JwtHelper();
        private IDalUserMapper _userRepo;
        public UserServices(IDalUserMapper dalUserMapper)
        {
            this._userRepo = dalUserMapper;
        }

        /// <summary>
        /// 返回用户登录信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<SysUser?> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepo.GetUserByUsernameAsync(username);
            if (user == null || user.Status != 1)
                return null;

            // 验证密码（假设使用 bcrypt 加密存储）
            bool passwordMatch = (password==user.Password);
            if (passwordMatch) 
            {
            user.token = jwt.GenerateToken(secertKey, isuser, audience,user.Username);
            }
            return passwordMatch ? user : null;
        }



    }
}
