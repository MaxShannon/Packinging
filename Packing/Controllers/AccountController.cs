using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using DbEfModel;
using Packing.Models;


namespace Packing.Controllers
{
    public class AccountController : BaseController
    {
        private readonly jhglEntities _db = new jhglEntities();

        #region 展示用户

        public ActionResult Index()
        {
            if (!Judge())
            {
                return Redirect(LogOff);
            }
            //ViewBag.PrivilegeName = new SelectList(_db.PrivilegeInfo, "Id", "PrivilegeName", 0);
            return View();
        }

        [HttpPost]
        public ActionResult Index(UserViewModel userInfoViewModel, int limit, int offset)
        {
            var temp = (
                from userInfo in _db.sys_ry
               
                select new
                {
                    userInfo.pk,
                    userInfo.username,
                    //privilegeInfo.PrivilegeName,

                    //deptInfo.DeptName
                }
            );
            //if (userInfoViewModel.Id != 0)
            //{
            //    temp = temp.Where(u => u.Id == userInfoViewModel.Id);
            //}
            //if (!userInfoViewModel.UserName.IsEmpty())
            //{
            //    temp = temp.Where(u => u.UserName.Contains(userInfoViewModel.UserName));
            //}
            var list = temp.OrderByDescending(u => u.pk).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }

        #endregion

        //#region 新建用户

        //public ActionResult Add()
        //{
        //    ViewBag.PrivilegeName = new SelectList(_db.PrivilegeInfo, "Id", "PrivilegeName", 1);
        //    ViewBag.DeptName = new SelectList(_db.DeptInfo, "Id", "DeptName", 1);
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Add(UserViewModel userInfoViewModel)
        //{
        //    if (userInfoViewModel.UserName.IsEmpty() || userInfoViewModel.Password.IsEmpty() || userInfoViewModel.ConfirmPassword.IsEmpty())
        //    {
        //        return Json(new { success = "fail", message = "添加失败，请填写相关信息" });
        //    }
        //    var userInfo = new UserInfo()
        //    {
        //        UserName = userInfoViewModel.UserName,
        //        Password = userInfoViewModel.Password,
        //        PrivilegeId = (int)userInfoViewModel.DeptId,
        //    };

        //    if (userInfoViewModel.Password != userInfoViewModel.ConfirmPassword)
        //    {
        //        return Json(new { success = "fail", message = "2次密码输入不同" });
        //    }
        //    _db.UserInfo.Add(userInfo);
        //    _db.SaveChanges();
        //    var deptUser = new UserDeptInfo();
        //    deptUser.UserId = _db.UserInfo.Max(a => a.Id);
        //    deptUser.DeptId = userInfoViewModel.DeptId;
        //    _db.UserDeptInfo.Add(deptUser);
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "添加成功" }) : Json(new { success = "fail", message = "添加失败" });
        //}

        //#endregion

        //#region 修改用户信息

        //public ActionResult Edit(int id)
        //{
        //    var temp = new SelectList(_db.DeptInfo, "Id", "DeptName", 0).ToList();
        //    ViewBag.DeptName = temp;

        //    var user = _db.UserInfo.Find(id);
        //    if (user == null)
        //    {
        //        return Redirect("index");
        //    }
        //    ViewData.Model = new UserViewModel()
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        PrivilegeId = user.PrivilegeId,
        //        // DeptId = user.DeptId,
        //    };
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Edit(UserViewModel userInfo)
        //{
        //    var user = _db.UserInfo.Find(userInfo.Id);
        //    if (user == null)
        //    {
        //        return Json(new { success = "fail", message = "修改失败，用户名不存在" });
        //    }
        //    user.UserName = userInfo.UserName;
        //    //user.PrivilegeId = userInfo.PrivilegeId;
        //    //user.DeptId = userInfo.DeptId;
        //    _db.UserInfo.Attach(user);
        //    _db.Entry(user).Property(c => c.UserName).IsModified = true;
        //    var userDept = _db.UserDeptInfo.FirstOrDefault(a => a.UserId == userInfo.Id);
        //    userDept.DeptId = userInfo.DeptId;
        //    //_db.Entry(user).Property(c => c.PrivilegeId).IsModified = true;
        //    //_db.Entry(user).Property(c => c.DeptId).IsModified = true;

        //    return _db.SaveChanges() > 0
        //        ? Json(new { success = "success", message = "修改成功" })
        //        : Json(new { success = "fail", message = "修改失败" });
        //}

        //#endregion

        //public ActionResult Details()
        //{
        //    return View();
        //}

        //#region 删除用户

        //public ActionResult Delete(int id)
        //{
        //    var temp = new SelectList(_db.PrivilegeInfo, "Id", "PrivilegeName", 0).ToList();
        //    ViewBag.PrivilegeName = temp;

        //    var user = _db.UserInfo.Find(id);
        //    if (user == null)
        //    {
        //        return Redirect("index");
        //    }
        //    ViewData.Model = new UserViewModel()
        //    {
        //        Id = user.Id,
        //        UserName = user.UserName,
        //        PrivilegeId = user.PrivilegeId,
        //        DeptName = _db.DeptInfo.Join(_db.UserDeptInfo, a => a.Id, b => b.UserId, (a, b) => new { a.DeptName }).FirstOrDefault().DeptName
        //    };
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Delete(UserViewModel userInfo)
        //{
        //    var cargo = _db.UserInfo.Find(userInfo.Id);
        //    if (cargo == null)
        //    {
        //        return Redirect("index");
        //    }

        //    var deptUser = _db.UserDeptInfo.FirstOrDefault(a => a.UserId == userInfo.Id);
        //    _db.UserDeptInfo.Remove(deptUser);
        //    _db.UserInfo.Remove(cargo);
        //    return _db.SaveChanges() > 0
        //        ? Json(new { success = "success", message = "删除成功" })
        //        : Json(new { success = "fail", message = "删除失败" });
        //}

        //#endregion


    }
}