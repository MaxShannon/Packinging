using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Packing.Models;

namespace Packing.Controllers
{
    public class ProductionOrderController : BaseController
    {
        //private readonly auditEntities _db = new auditEntities();
        //public ActionResult Index(int flowId)
        //{
        //    //ViewBag.FlowName = new SelectList(_db.FlowInfo, "Id", "FlowName", 0);
        //    ProductionOrderViewModel productionOrderViewModel = new ProductionOrderViewModel
        //    {
        //        FlowId = flowId,
        //        FlowNodeName = _db.FlowInfo.Find(flowId)?.FlowName,
        //        TimeStart = DateTime.Now.AddDays(-1),
        //        TimeEnd = DateTime.Now
        //    };
        //    var s = new States();
        //    var t1 = new SelectList(s.StateList, "Id", "StateName", 0).ToList();
        //    ViewBag.StateList = t1;
        //    return View(productionOrderViewModel);
        //}

        //[HttpPost]
        //public ActionResult Index(ProductionOrderViewModel productionOrderViewModel, int limit, int offset)
        //{
        //    var temp = (
        //        from productionOrderInfo in _db.ProductionOrderInfo
        //        join b in _db.FlowNodeInfo on new { CurrentNodeId = (int)productionOrderInfo.CurrentNodeId } equals new
        //        { CurrentNodeId = b.Id }
        //        join cargoLogInfo in _db.CargoLogInfo on productionOrderInfo.CargoLogId equals cargoLogInfo.Id
        //        join cargoInfo in _db.CargoInfo on cargoLogInfo.CargoId equals cargoInfo.Id
        //        join flowInfo in _db.FlowInfo on b.FlowId equals flowInfo.Id
        //        join userInfo in _db.UserInfo on productionOrderInfo.InspectId equals userInfo.Id
        //        select new
        //        {
        //            Id = productionOrderInfo.Id,
        //            Remark = productionOrderInfo.Remark,
        //            InspectId = productionOrderInfo.InspectId,
        //            //.Contains(new { DeptId = b.DeptId }) ? true : false,
        //            FlowId = productionOrderInfo.FlowId,
        //            CurrentNodeId = productionOrderInfo.CurrentNodeId,
        //            Column1 = b.Id,
        //            Column2 = b.FlowId,
        //            FlowNodeName = b.FlowNodeName,
        //            DeptId = b.DeptId,

        //            IsOk = (
        //                from userDeptInfo in _db.UserDeptInfo
        //                where
        //                    userDeptInfo.UserId == LoginUser.pk
        //                select new
        //                {
        //                    userDeptInfo.DeptId
        //                }).Contains(new { DeptId = (int?)b.DeptId }),

        //            cargoLogInfo.CargoId,
        //            cargoLogInfo.ChangeWeight,
        //            cargoInfo.CargoName,
        //            flowInfo.FlowName,
        //            InspectName = userInfo.UserName,
        //            InspectTime = productionOrderInfo.InspectTime,
        //            cargoLogInfo.ShipmentNo,
        //            cargoLogInfo.StateId
        //        });

        //    if (productionOrderViewModel.FlowId != 0)
        //    {
        //        temp = temp.Where(u => u.FlowId == productionOrderViewModel.FlowId);
        //    }

        //    if (productionOrderViewModel.StateId == 1)
        //    {
        //        temp = temp.Where(a => a.IsOk == true);
        //    }
        //    if (productionOrderViewModel.StateId == 2)
        //    {
        //        temp = temp.Where(a => a.IsOk == false);
        //    }
        //    //StateList = new List<State>
        //    //{
        //    //    new State()
        //    //    {
        //    //        Id = 1,
        //    //        StateName = "未审核"
        //    //    },
        //    //    new State()
        //    //    {
        //    //        Id = 2,
        //    //        StateName = "正在审核"
        //    //    },
        //    //    new State()
        //    //    {
        //    //        Id = 3,
        //    //        StateName = "已审核"
        //    //    },
        //    //    new State()
        //    //    {
        //    //        Id = 4,
        //    //        StateName = "已驳回"
        //    //    }
        //    //};

        //    var list = temp.OrderByDescending(u => u.IsOk).ToList();
        //    return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        //}

        //public ActionResult Add(int? id) // cargoId
        //{
        //    ViewBag.FlowName = new SelectList(_db.FlowInfo, "Id", "FlowName", 0);

        //    var cargo = _db.CargoInfo.Find(id);

        //    var model = new CargoInfoViewModel() { Id = cargo.Id, CargoName = cargo.CargoName, Unit = cargo.Unit, Specifications = cargo.Specifications };
        //    ViewBag.CargoAreaName = new SelectList(_db.CargoAreaInfo, "Id", "CargoAreaName", 1);
        //    ViewBag.CargoInName = new SelectList(_db.CargoInInfo, "Id", "CargoInName", 3);
        //    //return View(model);
        //    ProductionOrderViewModel productionOrderViewModel = new ProductionOrderViewModel();
        //    //productionOrderViewModel.CargoId = (int)id;
        //    return View(productionOrderViewModel);
        //}

        //[HttpPost]
        //public ActionResult Add(ProductionOrderViewModel productionOrderViewModel)
        //{
        //    //if (productionOrderViewModel.BuildName.Replace(" ", "").Replace("  ", "").IsEmpty())
        //    //{
        //    //    return Json(new { success = "fail", message = "请输入单位名" });
        //    //}
        //    var buildInfo = new ProductionOrderInfo()
        //    {
        //        //Count = productionOrderViewModel.Count,
        //        CurrentNodeId = GetStartNodeId(productionOrderViewModel.FlowId),
        //        FlowId = productionOrderViewModel.FlowId,
        //        InspectId = LoginUser.pk,
        //        InspectTime = DateTime.Now,
        //        Remark = productionOrderViewModel.Remark
        //    };
        //    //var cargo = _db.ProductionOrderInfo.FirstOrDefault(c => c.BuildName == modelInfo.BuildName);
        //    //if (cargo != null)
        //    //{
        //    //    return Json(new { success = "fail", message = "这个单位已经存在" });
        //    //}
        //    _db.ProductionOrderInfo.Add(buildInfo);

        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "添加成功" }) : Json(new { success = "fail", message = "" });
        //}



        //public ActionResult Audit(int id)
        //{
        //    ViewBag.CurrentNodeName = new SelectList(_db.FlowNodeInfo, "Id", "FlowNodeName", 0);

        //    var pro = _db.ProductionOrderInfo.Find(id);

        //    var model = new ProductionOrderViewModel
        //    {
        //        //Count = pro.Count,
        //        Remark = pro.Remark,
        //        CurrentNodeId = pro.CurrentNodeId,


        //    };

        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Audit(ProductionOrderViewModel productionOrderViewModel)
        //{
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "" });
        //}
    }
}