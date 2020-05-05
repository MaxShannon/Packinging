using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Packing.Models;

namespace Packing.Controllers
{
    public class FlowController : BaseController
    {
        //private readonly auditEntities _db = new auditEntities();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Index(FlowViewModel flowViewModel, int limit, int offset)
        //{
        //    var temp = from flowInfo in _db.FlowInfo
        //        select
        //            flowInfo;
        //    var list = temp.OrderByDescending(u => u.Id).ToList();
        //    return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        //}

        //public ActionResult Add()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Add(FlowViewModel flowViewModel)
        //{
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "添加成功" }) : Json(new { success = "fail", message = "" });
        //}

        //public ActionResult Delete()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Delete(FlowViewModel flowViewModel)
        //{
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "" });
        //}

        //public ActionResult Edit()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Edit(FlowViewModel flowViewModel)
        //{
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "" });
        //}


    }
}