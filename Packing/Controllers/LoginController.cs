using Packing.Models;
using System.Linq;
using System.Web.Mvc;
using DbEfModel;

namespace Packing.Controllers
{
    public class LoginController : Controller
    {

        private readonly jhglEntities _db = new jhglEntities();

 
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(UserViewModel sys_ry)
        {
        
            var userInfoNew = _db.sys_ry.FirstOrDefault(u => u.username == sys_ry.UserName);
            if (userInfoNew != null && userInfoNew.password == sys_ry.Password)
            {
                Session["LoginUser"] = userInfoNew;

                return Json(new { success = "success", message = "登陆成功" });
            }
            return Json(new { success = "fail", message = "登陆失败" });
        }

        public ActionResult LogOff()
        {
            Session["LoginUser"] = null;

            return RedirectToAction("Index");
        }


    }
}