using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using DbEfModel;
using Packing.Models;
using Services;

namespace Packing.Controllers
{
    public class CargoAreaController : BaseController
    {
        private readonly jhglEntities _db = new jhglEntities();
        private readonly CargoService _cargoService;

        public CargoAreaController()
        {
            var lxId = int.Parse(ConfigurationManager.AppSettings["FillList"]);
            var lxId2 = int.Parse(ConfigurationManager.AppSettings["FillList2"]);
            var lxCheckId = int.Parse(ConfigurationManager.AppSettings["LxCheckId"]);
            CargoService cargoService = new CargoService(lxId, lxId2, lxCheckId);
            _cargoService = cargoService;
        }
        #region 展示产品
        public ActionResult Index()
        {
            //ViewBag.PrivilegeId = LoginUser.PrivilegeId;
            return View();
        }

        [HttpPost]
        public ActionResult Index(CargoViewModel cargoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetUserHuot(LoginUser.pk);

            var list = temp.OrderByDescending(c => c.HuotId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }
        #endregion

        #region 明细
        public ActionResult Details(int? id) // cargoAreaId
        {
            var s = new States();
            var t1 = new SelectList(s.StateCargoList, "Id", "StateCargoName", 0).ToList();
            ViewBag.StateList = t1;
            CargoViewModel cargo = new CargoViewModel();
            cargo.HuotId = id ?? 0;
            //cargo.Id = cargoId;
            cargo.TimeStart = DateTime.Now.AddDays(-1);
            cargo.TimeEnd = DateTime.Now;
            return View(cargo);
        }

        [HttpPost]
        public ActionResult Details1(CargoViewModel cargoViewModel, int? limit, int? offset)
        {
            var temp = _cargoService.GetCargoShipmentHuot(cargoViewModel.HuotId, 0, LoginUser.pk);

            var list = temp.ToList();
            //select SUM(weight) from CargoAreaCargoInfo a
            //    where a.CargoAreaId = 1 and a.Cargoid = 1
            //group by a.shipmentNo
            return Json(new { total = list.Count, rows = list.OrderByDescending(c => c.Weight).Skip((int)offset).Take((int)limit).ToList() });
        }
        #endregion
    }
}