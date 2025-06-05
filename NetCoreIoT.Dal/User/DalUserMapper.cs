using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver.Core.Configuration;
using NetCoreIoT.DB;
using NetCoreIoT.Model.User;

namespace NetCoreIoT.Dal.User
{
    public class DalUserMapper: IDalUserMapper
    {
        private MySqlHelper _mysql = new();
        public DalUserMapper() { }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public async Task<SysUser?> GetUserByUsernameAsync(string username)
        {
            const string sql = "SELECT * FROM sys_user WHERE username = @username LIMIT 1";
            var parameters = new Dictionary<string, object>
                {
                {"@username", username},
                };

            var result= await _mysql.ExecuteQueryAsync(sql, parameters);
            if (!(result is null) && result.Rows.Count>0)
            {
                return new SysUser
                {
                    Id = Convert.ToInt32(result.Rows[0]["id"]),
                    Username = result.Rows[0]["username"].ToString()!,
                    Password = result.Rows[0]["password"].ToString()!,
                    Role_Id = Convert.ToInt32(result.Rows[0]["role_id"]),
                    Status = Convert.ToByte(result.Rows[0]["status"]),
                    cerate_time = result.Rows[0]["create_time"].ToString()
                };
            }

            return null;
        }
    }
}
