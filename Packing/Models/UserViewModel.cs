using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Packing.Models
{
    public class UserViewModel
    {
        [Display(Name = "密码确认")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "密码和确认密码不匹配。")]
        public string ConfirmPassword { get; set; }

        public int Id { get; set; }

        [Display(Name = "用户名")]
        public string UserName { get; set; }

        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string Mac { get; set; }

        [Display(Name = "手机号")]
        public string Phone { get; set; }

        [Display(Name = "真实姓名")]
        public string RealName { get; set; }

        [Display(Name = "部门")]
        public Nullable<int> DeptId { get; set; }


        [Display(Name = "部门")]
        public string DeptName { get; set; }

        [Display(Name = "权限")]
        public Nullable<int> PrivilegeId { get; set; }

        [Display(Name = "描述")]
        public string Describe { get; set; }
    }
}