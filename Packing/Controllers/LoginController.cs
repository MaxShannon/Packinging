using System.Configuration;
using Packing.Models;
using System.Linq;
using System.Web.Mvc;
using DbEfModel;
using NPOI.OpenXmlFormats.Wordprocessing;

namespace Packing.Controllers
{
    public class LoginController : Controller
    {

        private readonly jhglEntities _db = new jhglEntities();
        private readonly string _userPk;

        public LoginController()
        {
            _userPk = ConfigurationManager.AppSettings["UserPk"];
        }

        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Login(string webtoken)
        {
            var webId = _db.webdddl.Find(webtoken);
            var userInfoNew = _db.sys_ry.Find(webId.c_ry);
            _db.webdddl.Remove(webId);
            _db.SaveChanges();
            if (JudgeLoger(userInfoNew))
            {
                return RedirectToAction("Index", "cargo");
            }
            return Redirect("http://10.28.212.1:8680/");
        }

        [HttpGet]
        public ActionResult PkLogin()
        {
            var userInfoNew = _db.sys_ry.Find(_userPk);
            if (JudgeLoger(userInfoNew))
            {
                return RedirectToAction("Index", "cargo");
            }
            return Redirect("http://10.28.212.1:8680/");
        }

        //[HttpPost]
        //public ActionResult Login(UserViewModel sys_ry)
        //{

        //    var userInfoNew = _db.sys_ry.FirstOrDefault(u => u.username == sys_ry.UserName);
        //    if (userInfoNew != null && userInfoNew.password == sys_ry.Password)
        //    {
        //        Session["LoginUser"] = userInfoNew;

        //        return Json(new { success = "success", message = "登陆成功" });
        //    }
        //    return Json(new { success = "fail", message = "登陆失败" });
        //}

        public ActionResult Check()
        {
            return Json("OK?OK!", JsonRequestBehavior.AllowGet);
        }


        public ActionResult LogOff()
        {
            Session["LoginUser"] = null;

            return RedirectToAction("Index");
        }

        private bool JudgeLoger(sys_ry userInfoNew)
        {
            if (userInfoNew != null)
            {
                if (userInfoNew.lx == 7 || userInfoNew.lx == 8 || userInfoNew.lx == 9)
                {
                    Session["LoginUser"] = userInfoNew;

                    return true;
                }
            }

            return false;
        }
    }
}