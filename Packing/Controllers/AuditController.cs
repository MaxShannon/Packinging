using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbEfModel;
using Newtonsoft.Json;
using Packing.Models;
using Services;

namespace Packing.Controllers
{
    public class AuditController : BaseController
    {
        private readonly jhglEntities _db = new jhglEntities();
        private readonly CargoService _cargoService;

        public AuditController()
        {
            var lxId = int.Parse(ConfigurationManager.AppSettings["FillList"]);
            var lxId2 = int.Parse(ConfigurationManager.AppSettings["FillList2"]);
            var lxCheckId = int.Parse(ConfigurationManager.AppSettings["LxCheckId"]);
            CargoService cargoService = new CargoService(lxId, lxId2, lxCheckId);
            _cargoService = cargoService;
        }
        public ActionResult Index(int? productionOrderId)
        {
            var auditViewModel = new AuditViewModel();
            if (productionOrderId != null)
            {
                auditViewModel.ProductOrderId = (int)productionOrderId;
                //flowNodeViewModel.FlowName = _db.FlowInfo.Find(flowNodeId)?.FlowName;
            }

            return View(auditViewModel);
        }

        [HttpPost]
        public ActionResult Index(AuditViewModel auditViewModel, int limit, int offset)
        {
            //var temp = (
            //    from flowLineInfo in _db.FlowLineInfo

            //    join flowNodeInfo in _db.FlowNodeInfo on flowLineInfo.CurrentNodeId equals flowNodeInfo.Id

            //    join preFlowNodeInfo in _db.FlowNodeInfo on flowLineInfo.PreNodeId equals preFlowNodeInfo.Id
            //        into preFlowNodeInfoJoin
            //    from preFlowNodeInfo in preFlowNodeInfoJoin.DefaultIfEmpty()

            //    join nextFlowNodeInfo in _db.FlowNodeInfo on flowLineInfo.NextNodeId equals nextFlowNodeInfo.Id
            //        into nextFlowNodeInfoJoin
            //    from nextFlowNodeInfo in nextFlowNodeInfoJoin.DefaultIfEmpty()

            //    select new
            //    {
            //        flowLineInfo.Id,
            //        flowLineInfo.FlowId,
            //        flowLineInfo.CurrentNodeId,
            //        flowLineInfo.PreNodeId,
            //        flowLineInfo.NextNodeId,

            //        CurrentFlowNodeName = flowNodeInfo.FlowNodeName,
            //        PreFlowNodeName = preFlowNodeInfo.FlowNodeName,
            //        NextFlowNodeName = nextFlowNodeInfo.FlowNodeName
            //    });
            var temp = _cargoService.GetCargo(0, 0, 0, LoginUser.pk);
            //var temp = (
            //    from auditInfo in _db.AuditInfo
            //    join productionOrderInfo in _db.ProductionOrderInfo on auditInfo.ProductOrderId equals productionOrderInfo.Id
            //    join flowInfo in _db.FlowInfo on productionOrderInfo.FlowId equals flowInfo.Id
            //    join flowNodeInfo in _db.FlowNodeInfo on productionOrderInfo.CurrentNodeId equals flowNodeInfo.Id
            //    join cargoLogInfo in _db.CargoLogInfo on productionOrderInfo.CargoLogId equals cargoLogInfo.Id
            //    join cargoInfo in _db.CargoInfo on cargoLogInfo.CargoId equals cargoInfo.Id
            //    join userInfo in _db.UserInfo on auditInfo.InspectId equals userInfo.Id
            //    select new
            //    {
            //        auditInfo.Id,
            //        auditInfo.Info,
            //        auditInfo.InspectId,
            //        auditInfo.InspectTime,
            //        auditInfo.ProductOrderId,

            //        productionOrderInfo.FlowId,
            //        // productionOrderInfo.Count,
            //        FlowNodeId = productionOrderInfo.CurrentNodeId,

            //        flowInfo.FlowName,
            //        cargoLogInfo.ChangeWeight,
            //        cargoLogInfo.StateId,
            //        cargoInfo.CargoName,
            //        flowNodeInfo.FlowNodeName,
            //        InspectName = userInfo.UserName
            //    });
            //if (auditViewModel.ProductOrderId != 0)
            //{
            //    temp = temp.Where(a => a.CargoId == auditViewModel.ProductOrderId);
            //}
            //if (flowLineViewModel.FlowId != 0)
            //{
            //    temp = temp.Where(a => a.FlowId == flowLineViewModel.FlowId);
            //}
            var list = temp.OrderByDescending(u => u.CargoId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }

        public ActionResult Add(int id)
        {
            //var pro = _db.ProductionOrderInfo.Find(productionOrderId);
            //var flowNode = _db.FlowNodeInfo.Find(pro?.CurrentNodeId);
            //var cargoLog = _db.CargoLogInfo.Find(pro?.CargoLogId);
            //var cargo = _db.CargoInfo.Find(cargoLog.CargoId);
            //var CargoAreaName = _db.CargoAreaInfo.Find((int)cargoLog.CargoAreaId).CargoAreaName;
            //var auditViewModel = new AuditViewModel
            //{
            //    ProductOrderId = (int)productionOrderId,
            //    FlowNodeName = flowNode?.FlowNodeName,
            //    FlowName = _db.FlowInfo.Find(pro?.FlowId)?.FlowName,
            //    FlowNodeId = flowNode.Id,
            //    CargoName = cargo.CargoName,
            //    ChangeWeight = (decimal)cargoLog.ChangeWeight,
            //    CargoAreaId = (int)cargoLog.CargoAreaId,
            //    CargoAreaName = CargoAreaName,
            //    CargoLogId = cargoLog.Id
            //};auditViewModel
            var cargoViewModel = new CargoViewModel();
            cargoViewModel.CargoLogId = id;
            return View(cargoViewModel);
        }

        [HttpPost]
        public ActionResult Add(CargoViewModel cargoViewModel)
        {

            var cargoLogInfoes = _db.CargoLogInfoes.FirstOrDefault(a => a.Id == cargoViewModel.CargoLogId);
            if (cargoLogInfoes.StateId != 1)
            {
                return Json(new { success = "fail", message = "已经审核过了" });
            }

            bool ok;

            if (cargoViewModel.Pass)
            {
                ok = _cargoService.PassCargoLog(cargoLogInfoes.Id);
            }
            else
            {
                ok = _cargoService.RejectCargoLog(cargoLogInfoes.Id);
            }
            return ok ? Json(new { success = "success", message = "成功"/*, productOrder.FlowId*/ }) : Json(new { success = "fail", message = "" });
        }

        [HttpPost]
        public ActionResult Adds(string datalist)
        {
            var entitys = JsonConvert.DeserializeObject<List<ProductionOrderViewModel>>(datalist);

            using (var tran = _db.Database.BeginTransaction())
            {
                foreach (var item in entitys)
                {
                    if (item.IsOk == false)
                    {
                        var name = item.FlowNodeName;
                        return Json(new { success = "fail", message = name + ",没有权限" });
                    }
                    item.Pass = true; //pass
                    _cargoService.PassCargoLog(item.CargoLogId);
                }
            }

            return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "" });
        }
    }
}