using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using DbEfModel;
using NPOI.OpenXmlFormats.Dml;
using Packing.Models;
using Services;

namespace Packing.Controllers
{
    public class ShipmentController : BaseController
    {
        private readonly jhglEntities _db = new jhglEntities();
        private readonly CargoService _cargoService;

        public ShipmentController()
        {
            var lxId = int.Parse(ConfigurationManager.AppSettings["FillList"]);
            var lxId2 = int.Parse(ConfigurationManager.AppSettings["FillList2"]);
            var lxCheckId = int.Parse(ConfigurationManager.AppSettings["LxCheckId"]);
            CargoService cargoService = new CargoService(lxId, lxId2,lxCheckId);
            _cargoService = cargoService;
        }
        public ActionResult Index(int cargoAreaId, int cargoId) // id = areaId
        {
            CargoViewModel cargo = new CargoViewModel();
            cargo.HuotId = cargoAreaId;
            cargo.CargoId = cargoId;

            return View(cargo);
        }

        [HttpPost]
        public ActionResult Index(CargoViewModel cargoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetCargoShipmentHuot(cargoViewModel.HuotId, cargoViewModel.CargoId, LoginUser.pk);

            var list = temp.OrderByDescending(c => c.CargoId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }

        public ActionResult Exchange(string shipmentNo, int? cargoAreaId, int? cargoId)
        {
            ViewBag.HuotName = new SelectList(_db.huot, "id", "name", 1);
            CargoViewModel cargoViewModel = new CargoViewModel();
            cargoViewModel.ShipmentNo = shipmentNo;
            cargoViewModel.HuotId = cargoAreaId ?? 0;
            cargoViewModel.CargoId = cargoId ?? 0;
            return View(cargoViewModel);
        }

        [HttpPost]
        public ActionResult Exchange(CargoViewModel cargoInfo)
        {
            var cargoAreaId = cargoInfo.HuotId;
            var newCargoAreaId = cargoInfo.NewHuotId;
            var cargoId = cargoInfo.CargoId;
            var weight = cargoInfo.Weight;
            var shipmentNo = cargoInfo.ShipmentNo;

            var ok = _cargoService.ExchangeShipment(cargoId, shipmentNo, cargoAreaId, newCargoAreaId, weight, LoginUser.pk);

            return ok ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "失败" });
        }
    }
}