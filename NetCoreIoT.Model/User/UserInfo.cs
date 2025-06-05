using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace NetCoreIoT.Model.User
{
    public class UserInfo
    {
        [BsonId] // 主键
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("username")]
        [Required(ErrorMessage = "用户名不能为空")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "用户名长度应在3到50个字符之间")]
        public string Username { get; set; }

        [BsonElement("email")]
        [Required(ErrorMessage = "邮箱不能为空")]
        [EmailAddress(ErrorMessage = "邮箱格式不正确")]
        public string Email { get; set; }

        [BsonElement("age")]
        [Range(0, 120, ErrorMessage = "年龄必须在0到120之间")]
        public int Age { get; set; }

        [BsonElement("phone")]
        [Phone(ErrorMessage = "电话号码格式不正确")]
        public string Phone { get; set; }

        [BsonElement("roles")]
        public List<string> Roles { get; set; } = new List<string>();


        [BsonElement("notes")]
        public string Notes { get; set; }

        [BsonElement("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [BsonElement("is_active")]
        public bool IsActive { get; set; } = true;
    }
}
