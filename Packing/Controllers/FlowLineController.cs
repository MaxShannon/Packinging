using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Packing.Models;

namespace Packing.Controllers
{
    public class FlowLineController : BaseController
    {
        //private readonly auditEntities _db = new auditEntities();

        //public ActionResult Index(int? flowId)
        //{

        //    var flowLine = new FlowLineViewModel();
        //    if (flowId != null)
        //    {
        //        flowLine.FlowId = (int)flowId;
        //        flowLine.FlowName = _db.FlowInfo.Find(flowId)?.FlowName;
        //    }

        //    return View(flowLine);
        //}

        //[HttpPost]
        //public ActionResult Index(FlowLineViewModel flowLineViewModel, int limit, int offset)
        //{
        //    var temp = (
        //        from flowLineInfo in _db.FlowLineInfo

        //        join flowNodeInfo in _db.FlowNodeInfo on flowLineInfo.CurrentNodeId equals flowNodeInfo.Id

        //        join preFlowNodeInfo in _db.FlowNodeInfo on flowLineInfo.PreNodeId equals preFlowNodeInfo.Id
        //            into preFlowNodeInfoJoin
        //        from preFlowNodeInfo in preFlowNodeInfoJoin.DefaultIfEmpty()

        //        join nextFlowNodeInfo in _db.FlowNodeInfo on flowLineInfo.NextNodeId equals nextFlowNodeInfo.Id
        //            into nextFlowNodeInfoJoin
        //        from nextFlowNodeInfo in nextFlowNodeInfoJoin.DefaultIfEmpty()

        //        select new
        //        {
        //            flowLineInfo.Id,
        //            flowLineInfo.FlowId,
        //            flowLineInfo.CurrentNodeId,
        //            flowLineInfo.PreNodeId,
        //            flowLineInfo.NextNodeId,

        //            CurrentFlowNodeName = flowNodeInfo.FlowNodeName,
        //            PreFlowNodeName = preFlowNodeInfo.FlowNodeName,
        //            NextFlowNodeName = nextFlowNodeInfo.FlowNodeName
        //        });

        //    if (flowLineViewModel.FlowId != 0)
        //    {
        //        temp = temp.Where(a => a.FlowId == flowLineViewModel.FlowId);
        //    }
        //    var list = temp.OrderByDescending(u => u.Id).ToList();
        //    return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        //}

        //public ActionResult Add(int flowId)
        //{
        //    ViewBag.PreFlowNodeName = new SelectList(_db.FlowNodeInfo,"Id","FlowNodeName",0);
        //    ViewBag.CurrentFlowNodeName = new SelectList(_db.FlowNodeInfo,"Id","FlowNodeName",0);
        //    ViewBag.NextFlowNodeName = new SelectList(_db.FlowNodeInfo,"Id","FlowNodeName",0);
        //    var flowLineViewModel = new FlowLineViewModel()
        //    {
        //        FlowId = flowId
        //    };
        //    return View(flowLineViewModel);
        //}

        //[HttpPost]
        //public ActionResult Add(FlowLineViewModel flowLineViewModel)
        //{


        //    var flowLine = new FlowLineInfo
        //    {
        //        CurrentNodeId = flowLineViewModel.CurrentNodeId,
        //        FlowId = flowLineViewModel.FlowId,
        //        NextNodeId = flowLineViewModel.NextNodeId,
        //        PreNodeId = flowLineViewModel.PreNodeId
        //    };
        //    _db.FlowLineInfo.Add(flowLine);
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