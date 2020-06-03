using DbEfModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Packing.Models;
using Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;


namespace Packing.Controllers
{
    public class CargoController : BaseController
    {
        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();
        private readonly CargoService _cargoService;

        public CargoController()
        {
            var lxId = int.Parse(ConfigurationManager.AppSettings["FillList"]);
            var lxId2 = int.Parse(ConfigurationManager.AppSettings["FillList2"]);
            var lxCheckId = int.Parse(ConfigurationManager.AppSettings["LxCheckId"]);
            var cargoService = new CargoService(lxId, lxId2, lxCheckId);
            _cargoService = cargoService;
        }
        #region 展示产品
        public ActionResult Index()
        {
            var t1 = new SelectList(_cargoService.GetCargo(0, 0, 0, LoginUser.pk), "Huom2Id", "CargoName", 0).ToList();
            t1.Insert(0, new SelectListItem() { Text = "全部", Value = "0" });
            ViewBag.CargoName = t1;
            //ViewBag.PrivilegeId = LoginUser.PrivilegeId;
            return View();
        }

        [HttpPost]
        public ActionResult Index(CargoViewModel cargoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetCargo(cargoViewModel.Huom2Id, 0, 0, LoginUser.pk);

            if (!cargoViewModel.CargoName.IsEmpty())
            {
             
            }
            var list = temp.OrderByDescending(c => c.CargoId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }
        #endregion

        #region 明细
        public ActionResult Details(int? id)
        {
            var s = new States();
            var t1 = new SelectList(s.StateCargoList, "Id", "StateCargoName", 0).ToList();
            ViewBag.StateList = t1;
            ViewBag.HuotName = new SelectList(_cargoService.GetUserHuot(LoginUser.pk), "HuotId", "HuotName", 0).ToList();

            CargoViewModel cargo = new CargoViewModel();
            cargo.Huom2Id = id ?? 0;
            cargo.TimeStart = DateTime.Now.AddDays(-1);
            cargo.TimeEnd = DateTime.Now.AddHours(1);
            return View(cargo);
        }

        [HttpPost]
        public ActionResult Details1(CargoViewModel cargoViewModel, int? limit, int? offset)
        {
            _cargoService.CheckCargo(cargoViewModel.Huom2Id);
            var temp = _cargoService.GetCargoLog(cargoViewModel.Huom2Id, 2, cargoViewModel.TimeStart, cargoViewModel.TimeEnd, LoginUser.pk, cargoViewModel.StateId, cargoViewModel.HuotId);

            var list = temp.ToList();


            return Json(new { total = list.Count, rows = list.OrderByDescending(c => c.CargoId).Skip((int)offset).Take((int)limit).ToList() });
        }
        #endregion

        #region 进货
        public ActionResult StorageIncoming()
        {
            return View();
        }

        public ActionResult StorageIncomingCreate(int id, int? cargoAreaId)
        {
            var model = new CargoViewModel()
            {
                Huom2Id = id
            };
            var shipmentList = _cargoService.GetShipment(id);
            var huotList = _cargoService.GetUserHuot(LoginUser.pk);
            ViewBag.HuotName = new SelectList(huotList, "HuotId", "HuotName", 1);
            ViewBag.ShipmentName = new SelectList(shipmentList, "ShipmentId", "ShipmentNo", 1);

            var pack = new List<object>();
            pack.Add(new { Id = 0, Weight = 0 });
            pack.Add(new { Id = 25, Weight = 25 });
            pack.Add(new { Id = 50, Weight = 50 });
            pack.Add(new { Id = 100, Weight = 100 });
            ViewBag.PackName = new SelectList(pack, "Id", "Weight", 1);
            return View(model);
        }

        [HttpPost]
        public ActionResult StorageIncomingCreate(CargoViewModel cargoViewModel)
        {
            if (cargoViewModel.ChangeWeight < 0)
            {
                return Json(new { success = "fail", message = "入库产品不能为负数" });
            }
            if (cargoViewModel.ChangeWeight == 0)
            {
                cargoViewModel.ChangeWeight = cargoViewModel.Weight * cargoViewModel.CargoCount;
                if (cargoViewModel.ChangeWeight == 0)
                {
                    return Json(new { success = "fail", message = "入库产品不能为0" });
                }
            }
            if (Session["LoginUser"] == null)
            {
                return Json(new { success = "fail", message = "请重新登陆" });
            }
            bool ok = _cargoService.AddCargoComeLog(cargoViewModel.Huom2Id, cargoViewModel.HuotId, cargoViewModel.ShipmentNo, cargoViewModel.ChangeWeight, LoginUser.name, cargoViewModel.CargoCount, cargoViewModel.Weight);

            return ok ? Json(new { success = "success", message = "添加产品入库成功" }) : Json(new { success = "fail", message = "失败，批号存在" });
        }


        #endregion

        #region 出货
        public ActionResult StorageShipping()
        {
            return View();
        }

        [HttpPost]
        public ActionResult StorageShipping(CargoInfoViewModel cargoInfoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetCargo(0, 0, 0, LoginUser.pk);


            var list = temp.ToList();
            return Json(new { total = list.Count, rows = list.OrderByDescending(m => m.CargoId).Skip(offset).Take(limit).ToList() });
        }


        public ActionResult StorageShippingCreate(int id, int? cargoAreaId)
        {
            var model = new CargoViewModel()
            {
                Huom2Id = id,
            };
            var huotList = _cargoService.GetUserHuot(LoginUser.pk);
            ViewBag.HuotName = new SelectList(huotList, "HuotId", "HuotName", 1);
          
            ViewBag.CargoInName = new SelectList(_db.CargoInInfoes, "Id", "CargoInName", 3);
            var huotShipment = _cargoService.GetHuotShipments((int)LoginUser.htid, id);
            ViewBag.CargoShipmentNo = new SelectList(huotShipment, "ShipmentId", "ShipmentNo", 3);
            return View(model);
        }

        [HttpPost]
        public ActionResult StorageShippingCreate(CargoViewModel cargoViewModel)
        {
            if (cargoViewModel.ChangeWeight < 0)
            {
                return Json(new { success = "fail", message = "出库产品不能为负数" });
            }
            if (cargoViewModel.ChangeWeight == 0)
            {
                return Json(new { success = "fail", message = "出库产品不能为0" });
            }
            if (Session["LoginUser"] == null)
            {
                return Json(new { success = "fail", message = "请重新登陆" });
            }

            cargoViewModel.CargoInId = 2; // 汽车

            var ok = _cargoService.AddCargoShipLog(cargoViewModel.Huom2Id, cargoViewModel.HuotId, cargoViewModel.ShipmentId, cargoViewModel.ChangeWeight, cargoViewModel.CargoInId, LoginUser.name);

            return ok ? Json(new { success = "success", message = "添加产品出库成功" }) : Json(new { success = "fail", message = "失败,批号不匹配或者重量不对" });

        }

        #endregion

        #region 审核

        public ActionResult Audit()
        {
            var s = new States();
            var t1 = new SelectList(s.StateList, "Id", "StateName", 0).ToList();
            ViewBag.StateList = t1;
            var cargoViewModel = new CargoViewModel();
            cargoViewModel.TimeStart = DateTime.Now.AddDays(-1);
            cargoViewModel.TimeEnd = DateTime.Now.AddHours(1);
            return View(cargoViewModel);
        }

        [HttpPost]
        public ActionResult Audit(CargoViewModel cargoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetCargoLog(0, cargoViewModel.StateId, cargoViewModel.TimeStart, cargoViewModel.TimeEnd, LoginUser.pk, 0, 0);
            var list = temp.OrderByDescending(c => c.CargoId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }
        #endregion

        #region 杜姐
        public ActionResult ShipOrder()
        {
            return View();
        }
        [HttpPost]
        public ActionResult ShipOrder(CargoViewModel cargoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetCargoOutShipment(cargoViewModel.HuotId, LoginUser.pk);
            var list = temp.OrderBy(a => a.IsChecked).ThenByDescending(c => c.cargoOutOrderId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }


        public ActionResult CargoOutOrderConfirm(int id)
        {
            var model = _dbd.pihao.FirstOrDefault(a => a.ID == id);
            var huot = _db.huot.FirstOrDefault(a => a.id == model.HuotId);
            CargoViewModel cargoViewModel = new CargoViewModel();
            cargoViewModel.HuotId = (int)model.HuotId;
            cargoViewModel.HuotName = huot.name;
            cargoViewModel.ShipmentNo = model.PH_NO;
            cargoViewModel.ChangeWeight = (decimal)model.PH_ZL;
            cargoViewModel.cargoOutOrderId = id;
            return View(cargoViewModel);
        }

        [HttpPost]
        public ActionResult CargoOutOrderConfirm(CargoViewModel cargoViewModel)
        {
            var ok = _cargoService.ConfirmCargoOutOrder(cargoViewModel.cargoOutOrderId, LoginUser.pk);
            return ok ? Json(new { success = "success", message = "添加产品出库成功" }) : Json(new { success = "fail", message = "失败" });
        }
        #endregion


        [HttpPost]
        public ActionResult AssLogWeight(CargoViewModel cargoViewModel, int? limit, int? offset)
        {
            var tmp = _cargoService.GetCargoLog(cargoViewModel.Huom2Id, 2, cargoViewModel.TimeStart, cargoViewModel.TimeEnd, LoginUser.pk, cargoViewModel.StateId, cargoViewModel.HuotId);

            var temp = (from modelList in tmp
                select new
                {
                    modelList.Huom2Id,
                    modelList.CargoName,
                    Weight = modelList.IsIncome == false ? (0 - modelList.ChangeWeight) : modelList.ChangeWeight
                });

            var list = (from model in temp
                group model by new
                {
                    model.Huom2Id,
                    model.CargoName,
                }
                into g
                select new
                {
                    g.Key.Huom2Id,
                    g.Key.CargoName,
                    Weight = g.Sum(a => a.Weight)
                });

            return Json(new { total = list.Count(), rows = list.OrderByDescending(c => c.Huom2Id).Skip((int)offset).Take((int)limit).ToList() });

        }


        public string Excel(CargoViewModel cargoViewModel)
        {
            var temp = _cargoService.GetCargoLog(cargoViewModel.Huom2Id, 2, cargoViewModel.TimeStart, cargoViewModel.TimeEnd, LoginUser.pk, cargoViewModel.StateId, cargoViewModel.HuotId);

            var temp2 = temp.Select(a => new AssembleLogExcel
            {
                产品名称 = a.CargoName,
                出入库 = a.IsIncome ? "入库" : "出库",
                重量 = a.ChangeWeight,
                批号 = a.ShipmentNo,
                方式 = a.CargoInName,
                仓库 = a.HuotName,
                描述 = a.Desc,
                录入人 = a.InspectName,
                录入时间 = a.Time,
            });
            var list = temp2.ToList();


            var filePath = ToExcel(list);
            return filePath;
        }



        public string ToExcel<T>(List<T> list)
        {
            string fileName = "/Excel/" + DateTime.Now.ToString("yyyyMMddHHmmssfff") + ".xls";
            string filePath = HttpRuntime.AppDomainAppPath + fileName;

            HSSFWorkbook book = new HSSFWorkbook();
            ISheet sheet1 = book.CreateSheet("sheet1");

            var row = sheet1.CreateRow(0);
            PropertyInfo[] propertyInfos = typeof(T).GetProperties();

            for (int i = 0; i < propertyInfos.Length; i++)
            {
                var temp = row.CreateCell(i);
                temp.SetCellValue(propertyInfos[i].Name);
            }

            for (int i = 0; i < list.Count; i++)
            {
                var row0 = sheet1.CreateRow(i + 1);
                for (int j = 0; j < propertyInfos.Length; j++)
                {
                    var cell0 = row0.CreateCell(j);
                    cell0.SetCellValue(propertyInfos[j].GetValue(list[i], null) == null ? "" : propertyInfos[j].GetValue(list[i], null).ToString());
                }
            }

            MemoryStream memoryStream = new MemoryStream();
            book.Write(memoryStream);
            FileStream fileStream = new FileStream(filePath, FileMode.CreateNew);

            memoryStream.WriteTo(fileStream);
            memoryStream.Dispose();
            fileStream.Dispose();

            return fileName;
        }

    }

  
}