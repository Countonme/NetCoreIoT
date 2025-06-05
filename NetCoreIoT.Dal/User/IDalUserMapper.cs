using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NetCoreIoT.Model.User;

namespace NetCoreIoT.Dal.User
{
    public interface IDalUserMapper
    {
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public Task<SysUser?> GetUserByUsernameAsync(string username);
    }
}