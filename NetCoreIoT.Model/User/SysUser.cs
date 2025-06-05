using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCoreIoT.Model.User
{
    public class SysUser
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int Role_Id { get; set; }
        public byte Status { get; set; }

        public string cerate_time { get; set; }

        public string token { get; set; }
    }
}
