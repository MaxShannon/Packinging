using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Mapping;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Packing.Models;

namespace Packing.Controllers
{
    public class FlowNodeController : BaseController
    {
        //private readonly auditEntities _db = new auditEntities();

        //public ActionResult Index(int? flowId)
        //{
        //    var flowNode = new FlowNodeViewModel();
        //    if (flowId != null)
        //    {
        //        flowNode.FlowId = (int)flowId;
        //        flowNode.FlowName = _db.FlowInfo.Find(flowId)?.FlowName;
        //    }
        //    return View(flowNode);
        //}

        //[HttpPost]
        //public ActionResult Index(FlowNodeViewModel flowNodeViewModel, int limit, int offset)
        //{
        //    var temp = (
        //        from flowNodeInfo in _db.FlowNodeInfo
        //        join deptInfo in _db.DeptInfo on flowNodeInfo.DeptId equals deptInfo.Id
        //        select new
        //        {
        //            flowNodeInfo.Id,
        //            flowNodeInfo.FlowNodeName,

        //            deptInfo.DeptName
        //        });
        //    if (flowNodeViewModel.FlowId != 0)
        //    {
        //        temp = temp.Where(a => a.Id == flowNodeViewModel.FlowId);
        //    }
        //    var list = temp.OrderByDescending(u => u.Id).ToList();
        //    return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        //}

        //public ActionResult Add(int flowId)
        //{
        //    ViewBag.DeptName = new SelectList(_db.DeptInfo, "Id", "DeptName", 0);
        //    var flowNodeInfo = new FlowNodeViewModel();
        //    flowNodeInfo.FlowId = flowId;
        //    //flowNodeInfo.FlowName = _db.flo
        //    return View(flowNodeInfo);
        //}

        //[HttpPost]
        //public ActionResult Add(FlowNodeViewModel flowNodeViewModel)
        //{
        //    var flowNodeInfo = new FlowNodeInfo()
        //    {
        //        FlowNodeName = flowNodeViewModel.FlowNodeName,
        //        DeptId = flowNodeViewModel.DeptId,
        //        FlowId = flowNodeViewModel.FlowId
        //    };
        //    _db.FlowNodeInfo.Add(flowNodeInfo);
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "添加成功" }) : Json(new { success = "fail", message = "" });
        //}

        //public ActionResult Delete()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Delete(FlowNodeViewModel flowNodeViewModel)
        //{
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "" });
        //}

        //public ActionResult Edit()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Edit(FlowNodeViewModel flowNodeViewModel)
        //{
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "" });
        //}
    }
}