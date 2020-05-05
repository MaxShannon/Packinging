using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbEfModel;

namespace Packing.Controllers
{
    public class BaseController : Controller
    {
        public bool IsCheck = true;
        public sys_ry LoginUser { get; set; }
        public string LogOff = "/Login/LogOff";

        protected override void OnActionExecuting(ActionExecutingContext filterExecutingContext)
        {
            base.OnActionExecuting(filterExecutingContext);

            if (IsCheck)
            {
                if (filterExecutingContext.HttpContext.Session["loginUser"] == null)
                {
                    filterExecutingContext.HttpContext.Response.Redirect("Login/Index");
                }
                else
                {
                    LoginUser = filterExecutingContext.HttpContext.Session["loginUser"] as sys_ry;
                    ViewData["UserName"] = LoginUser == null ? null : LoginUser.name;
                }
            }
            //LoginUser = new UserInfo
            //{
            //    PrivilegeId = 1,
            //    UserName =  "max"
            //};
        }

        protected bool Judge()
        {
            //if (LoginUser.PrivilegeId == 0 || LoginUser.PrivilegeId == 1 || LoginUser.PrivilegeId == 2)
            //{
            //    return true;
            //}
            //return false;
            return true;
        }

        public static int GetStartNodeId(int? flowId)
        {
            return 1;
        }
    }

}