using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCoreIoT.Model.User;

namespace NetCoreIoT.Services.User
{
    public interface IUserServices
    {
        /// <summary>
        /// 返回用户登录信息
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public  Task<SysUser?> AuthenticateAsync(string username, string password);
    }
}
