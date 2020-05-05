using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using Packing.Models;


namespace Packing.Controllers
{
    public class DeptController : BaseController
    {
        //private readonly auditEntities _db = new auditEntities();

        //#region 展示

        //public ActionResult Index()
        //{
        //    if (!Judge())
        //    {
        //        return Redirect(LogOff);
        //    }
        //    //ViewBag.PrivilegeName = new SelectList(_db.PrivilegeInfo, "Id", "PrivilegeName", 0);
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Index(UserViewModel userInfoViewModel, int limit, int offset)
        //{
        //    var temp = (
        //        from deptInfo in _db.DeptInfo
        //        select deptInfo
        //    );
        //    //if (userInfoViewModel.Id != 0)
        //    //{
        //    //    temp = temp.Where(u => u.Id == userInfoViewModel.Id);
        //    //}
        //    //if (!userInfoViewModel.UserName.IsEmpty())
        //    //{
        //    //    temp = temp.Where(u => u.UserName.Contains(userInfoViewModel.UserName));
        //    //}
        //    var list = temp.OrderByDescending(u => u.Id).ToList();
        //    return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        //}

        //#endregion

        //#region 新建

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
        //        PrivilegeId = userInfoViewModel.PrivilegeId,
        //    };

        //    if (userInfoViewModel.Password != userInfoViewModel.ConfirmPassword)
        //    {
        //        return Json(new { success = "fail", message = "2次密码输入不同" });
        //    }
        //    _db.UserInfo.Add(userInfo);
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "添加成功" }) : Json(new { success = "fail", message = "添加失败" });
        //}

        //#endregion

        //#region 修改信息

        //public ActionResult Edit(int id)
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
        //    user.PrivilegeId = userInfo.PrivilegeId;
        //    //user.DeptId = userInfo.DeptId;
        //    _db.UserInfo.Attach(user);
        //    _db.Entry(user).Property(c => c.UserName).IsModified = true;
        //    _db.Entry(user).Property(c => c.PrivilegeId).IsModified = true;
        //    //_db.Entry(user).Property(c => c.DeptId).IsModified = true;

        //    return _db.SaveChanges() > 0
        //        ? Json(new { success = "success", message = "修改成功" })
        //        : Json(new { success = "fail", message = "修改失败" });
        //}

        //#endregion



        //#region 删除

        //public ActionResult Delete(int id)
        //{
        //    var temp = new SelectList(_db.PrivilegeInfo, "Id", "PrivilegeName", 0).ToList();
        //    ViewBag.PrivilegeName = temp;

        //    var dept = _db.DeptInfo.Find(id);
        //    if (dept == null)
        //    {
        //        return Redirect("index");
        //    }
        //    ViewData.Model = new DeptViewModel()
        //    {
        //        Id = dept.Id,
        //        DeptName = dept.DeptName,

        //    };
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Delete(DeptViewModel userInfo)
        //{
        //    var dept = _db.DeptInfo.Find(userInfo.Id);
        //    if (dept == null)
        //    {
        //        return Redirect("index");
        //    }

        //    var cargoDept = _db.UserDeptInfo.FirstOrDefault(a => a.DeptId == dept.Id && a.UserId == userInfo.Id);
        //    if (cargoDept != null)
        //    {
        //        return Json(new {success = "fail", message = "删除失败"});
        //    }
        //    //_db.UserDeptInfo.Remove(cargoDept);
        //    _db.DeptInfo.Remove(dept);
        //    return _db.SaveChanges() > 0
        //        ? Json(new { success = "success", message = "删除成功" })
        //        : Json(new { success = "fail", message = "删除失败" });
        //}

        //#endregion

        //public ActionResult Detail(int id)
        //{

        //    var userDeptViewModel = new UserDeptViewModel
        //    {
        //        DeptId = id
        //    };
        //    return View(userDeptViewModel);
        //}

        //[HttpPost]
        //public ActionResult Detail(UserDeptViewModel userDeptViewModel, int limit, int offset)
        //{
        //    var temp = (
        //        from userInfo in _db.UserInfo
        //        join userDeptInfo in _db.UserDeptInfo on userInfo.Id equals userDeptInfo.UserId
        //        join deptInfo in _db.DeptInfo on userDeptInfo.DeptId equals deptInfo.Id
        //        where deptInfo.Id == userDeptViewModel.DeptId
        //        select new
        //        {
        //            userDeptInfo.Id,
        //            userInfo.UserName,
        //            userInfo.RealName,
        //            deptInfo.DeptName
        //        });

        //    var list = temp.OrderByDescending(u => u.Id).ToList();
        //    return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        //}

    }
}