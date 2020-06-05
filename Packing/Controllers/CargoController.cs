using DbEfModel;
using Newtonsoft.Json;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using Packing.Models;
using Services;
using System;
using System.Collections.Generic;
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
        private readonly Huom2Service _huom2Service;
        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();
        private readonly CargoService _cargoService;
        private readonly CargoLogService _cargoLogService;

        public CargoController()
        {
            var cargoService = new CargoService();
            _cargoService = cargoService;
            _huom2Service = new Huom2Service();
            _cargoLogService = new CargoLogService();
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

        #region 进货
        public ActionResult StorageIncoming()
        {
            return View();
        }

        [HttpGet]
        public ActionResult StorageIncomingCreate(int id, int? huot)
        {
            var huom2 = _huom2Service.GetHuom2(id);
            var model = new CargoViewModel()
            {
                Huom2Id = id,
                CargoName = huom2.name
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


        public ActionResult StorageShippingCreate(int id, int? huot)
        {
            var huom2 = _huom2Service.GetHuom2(id);
            var model = new CargoViewModel()
            {
                Huom2Id = id,
                CargoName = huom2.name
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

            cargoViewModel.CargoInId = 1; // 火车

            var ok = _cargoService.AddCargoShipLog(cargoViewModel.Huom2Id, cargoViewModel.HuotId, cargoViewModel.ShipmentId, cargoViewModel.ChangeWeight, cargoViewModel.CargoInId, LoginUser.name);

            return ok ? Json(new { success = "success", message = "添加产品出库成功" }) : Json(new { success = "fail", message = "失败,批号不匹配或者重量不对" });

        }

        #endregion

        #region 杜姐
        public ActionResult ShipOrder()
        {
            CargoViewModel cargo = new CargoViewModel();
            cargo.TimeStart = DateTime.Today.AddHours(-6);
            cargo.TimeEnd = DateTime.Today.AddHours(18);
            return View(cargo);
        }
        [HttpPost]
        public ActionResult ShipOrder(CargoViewModel cargoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetCargoOutShipment(LoginUser.pk);
            var list = temp.OrderBy(a => a.IsChecked).ThenByDescending(c => c.CargoOutOrderId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }

        [HttpPost]
        public ActionResult CargoOutOrdersConfirm(string datalist)
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
                    if (!item.IsChecked && !_cargoService.ConfirmCargoOutOrder(item.CargoOutOrderId, LoginUser.pk))
                    {
                        tran.Rollback();
                        return Json(new { success = "fail", message = "失败" });
                    }

                }
            }

            return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "失败" });
        }

        [HttpPost]
        public ActionResult CargoOutOrderConfirm(int id)
        {
            var ok = _cargoService.ConfirmCargoOutOrder(id, LoginUser.pk);
            return ok ? Json(new { success = "success", message = "添加产品出库成功" }) : Json(new { success = "fail", message = "失败,库存不足" });
        }
        #endregion

        #region 自提出库单
        public ActionResult SelfSell()
        {
            CargoViewModel cargo = new CargoViewModel();
            cargo.TimeStart = DateTime.Today.AddHours(-6);
            cargo.TimeEnd = DateTime.Today.AddHours(18);
            return View(cargo);
        }
        [HttpPost]
        public ActionResult SelfSell(CargoViewModel cargoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetSelfSellLog(LoginUser.pk);
            var list = temp.OrderBy(a => a.IsChecked).ThenByDescending(c => c.CargoOutOrderId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }

        [HttpPost]
        public ActionResult SelfSellLog(int id) // cargoOutOrderid
        {
            var ok = _cargoService.SelfSellOut(id, LoginUser.pk);
            return ok ? Json(new { success = "success", message = "添加产品自提成功" }) : Json(new { success = "fail", message = "失败" });
        }

        [HttpPost]
        public ActionResult SelfSellLogs(string datalist)
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
                    if (!item.IsChecked && !_cargoService.SelfSellOut(item.CargoOutOrderId, LoginUser.pk))
                    {
                        tran.Rollback();
                        return Json(new { success = "fail", message = "失败" });
                    }

                }
            }

            return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "失败" });
        }
        #endregion

        
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