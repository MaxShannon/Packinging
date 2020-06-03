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
            }

            return View(auditViewModel);
        }

        [HttpPost]
        public ActionResult Index(AuditViewModel auditViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetCargo(0, 0, 0, LoginUser.pk);
            
            var list = temp.OrderByDescending(u => u.CargoId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }

        public ActionResult Add(int id)
        {
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