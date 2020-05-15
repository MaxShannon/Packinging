using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;
using System.Web.WebPages;
using DbEfModel;
using Microsoft.Ajax.Utilities;
using Newtonsoft.Json;
using NPOI.HSSF.Record;
using NPOI.OpenXmlFormats.Wordprocessing;
using Packing.Controllers;
using Packing.Models;
using Services;


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


            //if (cargoViewModel.CargoId != 0)
            //{
            //    temp = temp.Where(c => c.CargoId == cargoViewModel.CargoId);
            //}
            if (!cargoViewModel.CargoName.IsEmpty())
            {
                //temp = temp.Where(c => c.CargoName.Contains(cargoInfoViewModel.CargoName)); // 模糊
                //temp = temp.Where(c => c.CargoName == cargoInfoViewModel.CargoName);
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

        #region 新建产品
        //public ActionResult Create()
        //{
        //    if (!Judge())
        //    {
        //        return Redirect(LogOff);
        //    }
        //    ViewBag.CargoInName = new SelectList(_db.CargoInInfo, "Id", "CargoInName", 1);
        //    ViewBag.CatName = new SelectList(_db.CatInfo, "Id", "CatName", 1);
        //    ViewBag.CargoAreaName = new SelectList(_db.CargoAreaInfo, "Id", "CargoAreaName", 1);
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Create(CargoInfoViewModel cargoInfoViewModel)
        //{
        //    if (cargoInfoViewModel.CargoName.Replace(" ", "").Replace("  ", "").IsEmpty())
        //    {
        //        return Json(new { success = "fail", message = "请输入产品名" });
        //    }
        //    var cargoInfo = new CargoInfo()
        //    {
        //        CargoName = cargoInfoViewModel.CargoName.Replace(" ", "").Replace("  ", ""),
        //        Weight = cargoInfoViewModel.Weight,
        //        CargoCount = 0,
        //        Unit = cargoInfoViewModel.Unit,
        //        Amount = cargoInfoViewModel.Amount,
        //        Specifications = cargoInfoViewModel.Specifications,
        //        CatId1 = cargoInfoViewModel.CatId,

        //        //Area = cargoInfoViewModel.Area,
        //    };
        //    var cargo = _db.CargoInfo.FirstOrDefault(c => c.CargoName == cargoInfo.CargoName && c.Unit == cargoInfo.Unit && c.Specifications == cargoInfo.Specifications && c.Area == cargoInfo.Area);
        //    if (cargo != null)
        //    {
        //        return Json(new { success = "fail", message = "这个产品已经存在" });
        //    }
        //    //var temp = _db.CargoInfo.Add(userInfo);
        //    _db.CargoInfo.Add(cargoInfo);
        //    return _db.SaveChanges() > 0 ? Json(new { success = "success", message = "添加成功" }) : Json(new { success = "fail", message = "" });
        //}
        #endregion

        #region 修改产品信息
        //public ActionResult Edit(int id)
        //{
        //    var cargo = _db.CargoInfo.Find(id);
        //    if (cargo == null)
        //    {
        //        return Redirect("index");
        //    }
        //    ViewData.Model = new CargoInfoViewModel()
        //    {
        //        Id = cargo.Id,
        //        CargoName = cargo.CargoName,
        //        Unit = cargo.Unit,
        //        Amount = cargo.Amount,
        //        Specifications = cargo.Specifications,
        //        Area = cargo.Area
        //    };
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Edit(CargoInfo cargoInfo) ///////////////////////////
        //{
        //    var cargo = _db.CargoInfo.Find(cargoInfo.Id);
        //    if (cargo == null)
        //    {
        //        return Json(new { success = "fail", message = "修改失败，没有当前的产品" });
        //    }
        //    var temp = _db.CargoInfo.FirstOrDefault(c => c.CargoName == cargoInfo.CargoName.Replace(" ", "").Replace("  ", "") && c.Unit == cargoInfo.Unit && c.Specifications == cargoInfo.Specifications && c.Amount == cargoInfo.Amount && c.Area == cargoInfo.Area);
        //    if (temp != null)
        //    {
        //        return Json(new { success = "fail", message = "修改失败，已经有这个产品" });
        //    }
        //    cargo.CargoName = cargoInfo.CargoName.Replace(" ", "").Replace("  ", "");
        //    cargo.Amount = cargoInfo.Amount;
        //    cargo.Unit = cargoInfo.Unit;
        //    cargo.Specifications = cargoInfo.Specifications;
        //    cargo.Area = cargoInfo.Area;
        //    _db.CargoInfo.Attach(cargo);
        //    _db.Entry(cargo).Property(c => c.CargoName).IsModified = true;
        //    _db.Entry(cargo).Property(c => c.Amount).IsModified = true;
        //    _db.Entry(cargo).Property(c => c.Unit).IsModified = true;
        //    _db.Entry(cargo).Property(c => c.Specifications).IsModified = true;
        //    _db.Entry(cargo).Property(c => c.Area).IsModified = true;
        //    return _db.SaveChanges() > 0
        //        ? Json(new { success = "success", message = "修改成功" })
        //        : Json(new { success = "fail", message = "修改失败" });
        //}
        #endregion

        #region 删除产品
        //public ActionResult Delete(int id)
        //{
        //    var cargo = _db.CargoInfo.Find(id);
        //    if (cargo == null)
        //    {
        //        return Redirect("index");
        //    }
        //    var model = new CargoInfoViewModel()
        //    {
        //        Id = cargo.Id,
        //        CargoName = cargo.CargoName,
        //        Unit = cargo.Unit,
        //        Amount = cargo.Amount,
        //        Specifications = cargo.Specifications,
        //        Area = cargo.Area
        //    };
        //    return View(model);
        //}

        //[HttpPost]
        //public ActionResult Delete(CargoInfo cargoInfo)
        //{
        //    var cargo = _db.CargoInfo.Find(cargoInfo.Id);
        //    _db.CargoInfo.Remove(cargo);
        //    return _db.SaveChanges() > 0
        //        ? Json(new { success = "success", message = "删除成功" })
        //        : Json(new { success = "fail", message = "删除失败" });
        //}
        #endregion

        #region 进货
        public ActionResult StorageIncoming()
        {
            //ViewBag.PrivilegeId = LoginUser.PrivilegeId;

            return View();
        }

        //[HttpPost]
        //public ActionResult StorageIncoming(CargoInfoViewModel cargoInfoViewModel, int limit, int offset)
        //{
        //    var temp = (
        //        from cargoLogInfo in _db.CargoLogInfo
        //        join userInfo in _db.UserInfo on cargoLogInfo.InspectId equals userInfo.Id
        //        where
        //            cargoLogInfo.IsIncome == true
        //        select new
        //        {
        //            CargoLogId = cargoLogInfo.Id,
        //            cargoLogInfo.ChangeCount,
        //            cargoLogInfo.InspectTime,
        //            userInfo.UserName,
        //            cargoLogInfo.IsIncome,
        //            cargoLogInfo.Desc,
        //            cargoLogInfo.TakenName,
        //        });
        //    if (cargoInfoViewModel.CargoName != null)
        //    {
        //        //temp = temp.Where(c => c.CargoName.Contains(cargoInfoViewModel.CargoName));
        //    }
        //    if (cargoInfoViewModel.CargoName != null)
        //    {
        //        //temp = temp.Where(c => c.CargoName.Contains(cargoInfoViewModel.CargoName));
        //    }

        //    if (cargoInfoViewModel.TimeEnd == DateTime.MinValue)
        //    {
        //        cargoInfoViewModel.TimeEnd = DateTime.MaxValue;
        //    }
        //    temp = temp.Where(c => cargoInfoViewModel.TimeStart <= c.InspectTime && c.InspectTime < cargoInfoViewModel.TimeEnd);

        //    var list = temp.OrderByDescending(m => m.CargoLogId).ToList();
        //    return Json(new { total = list.Count(), rows = list.Skip(offset).Take(limit).ToList() });
        //}


        public ActionResult StorageIncomingCreate(int id, int? cargoAreaId)
        {
            //var cargo = _db.CargoInfoes.Find(id);
            var model = new CargoViewModel()
            {
                Huom2Id = id
                //CargoName = cargo.CargoName,
                //Unit = cargo.Unit,
                //Specifications = cargo.Specifications,
                //CatId = cargo.CatId1,
                //CargoAreaId = cargoAreaId ?? 0
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
            //ViewBag.CargoInName = new SelectList(_db.CargoInInfo, "Id", "CargoInName", 3);
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
            bool ok = _cargoService.AddCargoComeLog(cargoViewModel.Huom2Id, cargoViewModel.HuotId, cargoViewModel.ShipmentNo, cargoViewModel.ChangeWeight, LoginUser.name);

            return ok ? Json(new { success = "success", message = "添加产品入库成功" }) : Json(new { success = "fail", message = "失败，批号存在" });
        }


        #endregion

        #region 出货
        public ActionResult StorageShipping()
        {
            //ViewBag.PowerInfoId = LoginUser.PrivilegeId;
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
            //ViewBag.HuotName = new SelectList(_db.huot, "id", "name", 1);
            ViewBag.CargoInName = new SelectList(_db.CargoInInfoes, "Id", "CargoInName", 3);
            var huotShipment = _cargoService.GetHuotShipments((int)LoginUser.htid);
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


            var ok = _cargoService.AddCargoShipLog(cargoViewModel.Huom2Id, cargoViewModel.HuotId, cargoViewModel.ShipmentId, cargoViewModel.ChangeWeight, cargoViewModel.CargoInId, LoginUser.name);

            return ok ? Json(new { success = "success", message = "添加产品出库成功" }) : Json(new { success = "fail", message = "失败" });

        }


        #region 修改出库
        //public ActionResult EditStorageShipping(int cargoLogId)
        //{
        //    var cargoLog = _db.CargoLogInfo.Find(cargoLogId);
        //    if (cargoLog == null)
        //    {
        //        return Redirect("StorageShipping");
        //    }
        //    var cargo = _db.CargoInfo.Find(cargoLog.Id);
        //    if (cargo == null)
        //    {
        //        return Redirect("index");
        //    }
        //    //ViewData.Model = new CargoInfoViewModel() { Id = cargo.Id, CargoName = cargo.CargoName, Unit = cargo.Unit, Specifications = cargo.Specifications, Type = cargo.Type, CargoLogId = cargoLog.Id, TakenName = cargoLog.TakenName, Time = cargoLog.Time, Desc = cargoLog.Desc, ProjectId = cargoLog.ProjectId };
        //    //ViewBag.ProjectName = new SelectList(_db.ProjectInfo, "Id", "ProjectName", 1);
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult EditStorageShipping(CargoInfoViewModel cargoLogInfo) ///////////////////////////
        //{
        //    //var cargoLog = _db.CargoLogInfo.Find(cargoLogInfo.CargoLogId);
        //    //if (cargoLog == null)
        //    //{
        //    //    return Json(new { success = "fail", message = "修改失败，没有当前记录" });
        //    //}
        //    //var cargo = _db.CargoInfo.Find(cargoLog.CargoInfoId);
        //    //if (cargo == null)
        //    //{
        //    //    return Json(new { success = "fail", message = "修改失败，没有当前的产品" });
        //    //}

        //    //cargoLog.TakenName = cargoLogInfo.TakenName;
        //    //cargoLog.Time = DateTime.Now;
        //    //cargoLog.ProjectId = cargoLogInfo.ProjectId;
        //    //cargoLog.Desc = cargoLogInfo.Desc;
        //    //_db.CargoLogInfo.Attach(cargoLog);
        //    //_db.Entry(cargoLog).Property(c => c.TakenName).IsModified = true;
        //    //_db.Entry(cargoLog).Property(c => c.Time).IsModified = true;
        //    //_db.Entry(cargoLog).Property(c => c.ProjectId).IsModified = true;
        //    //_db.Entry(cargoLog).Property(c => c.Desc).IsModified = true;
        //    return _db.SaveChanges() > 0
        //        ? Json(new { success = "success", message = "修改成功" })
        //        : Json(new { success = "fail", message = "修改失败" });
        //}
        #endregion
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
            var list = temp.OrderByDescending(c => c.cargoOutOrderId).ToList();
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

        #region 还原

        public ActionResult Back(int cargoLogId)
        {
            //var cargoLog = _db.CargoLogInfo.Find(cargoLogId);
            //if (cargoLog == null)
            //{
            //    return Redirect("index");
            //}
            //var cargoLogViewModel = new CargoInfoViewModel()
            //{
            //    CargoLogId = cargoLog.Id,
            //    ChangeCount = cargoLog.ChangeCount,
            //    CargoName = _db.CargoInfo.Find(cargoLog.CargoInfoId)?.CargoName,
            //    IsIncome = cargoLog.IsIncome == null ? true : (bool)cargoLog.IsIncome
            //};cargoLogViewModel
            return View();
        }

        [HttpPost]
        public ActionResult Back2(int cargoLogId)
        {
            return Json(new { success = "fail", message = "还原失败" });
            //var cargoLog = _db.CargoLogInfo.Find(cargoLogId);
            //if (cargoLog == null)
            //{
            //    return Json(new { success = "fail", message = "还原失败 cargoLog == null" });
            //}
            //var cargo = _db.CargoInfo.Find(cargoLog.CargoInfoId);
            //if (cargo == null)
            //{
            //    return Json(new { success = "fail", message = "还原失败 cargo = null" });
            //}
            //if (cargoLog.IsIncome == true)
            //{
            //    var count = _db.CargoLogInfo.Where(m => m.Id == cargoLogId).ToList().Count;
            //    cargo.CargoCount -= cargoLog.ChangeCount;
            //    //SELECT COUNT(*) FROM dbo.CargoLogInfo
            //    //GROUP BY CargoInfoId
            //    if (cargo.CargoCount < 0)
            //    {
            //        return Json(new { success = "fail", message = "库存为负数还原失败" });
            //    }

            //    if (count == 1)
            //    {
            //        cargo.CanDel = true;
            //    }
            //}
            //if (cargoLog.IsIncome == false)
            //{
            //    cargo.CargoCount += cargoLog.ChangeCount;
            //}

            //var project = _db.ProjectInfo.Find(cargoLog.ProjectId);
            //var projectCount = _db.CargoLogInfo.Where(m => m.ProjectId == project.Id).ToList().Count;
            //if (projectCount == 1)
            //{
            //    project.CanDel = true;
            //}

            //var supply = _db.SupplyInfo.Find(cargoLog.SupplyId);
            //var supplyCount = _db.SupplyInfo.Where(m => m.Id == supply.Id).ToList().Count;
            //if (supplyCount == 1)
            //{
            //    supply.CanDel = true;
            //}

            //_db.Entry(project).Property(m => m.CanDel).IsModified = true;
            //_db.Entry(supply).Property(m => m.CanDel).IsModified = true;

            //_db.Entry(cargo).Property(m => m.CargoCount).IsModified = true;
            //_db.Entry(cargo).Property(m => m.CanDel).IsModified = true;

            //using (var tran = _db.Database.BeginTransaction())
            //{
            //    try
            //    {
            //        if (_db.SaveChanges() > 0)
            //        {
            //            _db.CargoLogInfo.Remove(cargoLog);
            //            if (_db.SaveChanges() > 0)
            //            {
            //                tran.Commit();
            //                return Json(new { success = "success", message = "还原成功" });
            //            }
            //            else
            //            {
            //                tran.Rollback();
            //                return Json(new { success = "fail", message = "还原失败" });
            //            }

            //        }

            //        return Json(new { success = "fail", message = "还原失败" });
            //    }
            //    catch (Exception e)
            //    {
            //        Console.WriteLine(e);
            //        throw;
            //    }
            //}

        }
        #endregion

        #region 项目查询
        //public ActionResult Project()
        //{
        //    //ViewBag.ProjectName = new SelectList(_db.ProjectInfo, "Id", "ProjectName", 1);
        //    //ViewBag.SupplyName = new SelectList(_db.SupplyInfo, "Id", "Name", 1);
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult Project(CargoInfoViewModel cargoInfoViewModel, int limit, int offset)
        //{
        //    //var SQL = @"SELECT [t0].[Id] AS [CargoLogId], [t1].[CargoName],[t1].Id, [t0].[ChangeCount], [t0].[Time], [t2].[ProjectName], [t1].[DelFlag], [t2].[Id] AS [ProjectId] FROM [CargoLogInfo] AS [t0] INNER JOIN [CargoInfo] AS [t1] ON [t0].[CargoInfoId] = [t1].[Id] INNER JOIN [ProjectInfo] AS [t2] ON [t0].[ProjectId] = ([t2].[Id]) ";

        //    //var temp = _db.Database.SqlQuery<CargoInfoViewModel>(SQL);

        //    //var temp = from cargoLogInfo in _db.CargoLogInfo
        //    //           join cargoInfo in _db.CargoInfo on cargoLogInfo.CargoInfoId equals cargoInfo.Id
        //    //           join projectInfo in _db.ProjectInfo on cargoLogInfo.ProjectId equals projectInfo.Id
        //    //           select new { CargoLogId = cargoLogInfo.Id, cargoInfo.CargoName, cargoLogInfo.ChangeCount, cargoLogInfo.Time, projectInfo.ProjectName, cargoInfo.DelFlag, ProjectId = projectInfo.Id };

        //    var temp = (from t0 in _db.CargoLogInfo
        //                    //join t1 in _db.CargoInfo on new { t0.CargoInfoId } equals new { CargoInfoId = t1.Id }
        //                    //join supply in _db.SupplyInfo on  t0.SupplyId equals  supply.Id 
        //                    //join user in _db.UserInfo on t0.UserId equals user.Id
        //                    //join t2 in _db.ProjectInfo on new { ProjectId = (int)t0.ProjectId } equals new { ProjectId = t2.Id }
        //                select new
        //                {
        //                    CargoLogId = t0.Id,
        //                    //t1.CargoName,
        //                    //t1.Id,
        //                    //t0.ChangeCount,
        //                    //t0.Time,
        //                    //t2.ProjectName,
        //                    //t1.DelFlag,
        //                    //ProjectId = t2.Id,
        //                    //t0.Desc,
        //                    //t1.Unit,
        //                    //t1.Specifications,
        //                    //t1.Type,
        //                    //t1.Amount,
        //                    //t0.SupplyId,
        //                    //SupplyName = supply.Name,
        //                    //t0.TakenName,
        //                    //user.UserName
        //                });

        //    if (cargoInfoViewModel.ProjectId != null)
        //    {
        //        //temp = temp.Where(c => c.ProjectId == cargoInfoViewModel.ProjectId);
        //    }

        //    if (cargoInfoViewModel.SupplyId != null)
        //    {
        //        //temp = temp.Where(c => c.SupplyId == cargoInfoViewModel.SupplyId);
        //    }

        //    if (cargoInfoViewModel.TimeEnd == DateTime.MinValue)
        //    {
        //        cargoInfoViewModel.TimeEnd = DateTime.MaxValue;
        //    }
        //    //temp = temp.Where(c => cargoInfoViewModel.TimeStart <= c.Time && c.Time < cargoInfoViewModel.TimeEnd);

        //    var list = temp.OrderByDescending(c => c.CargoLogId).ToList();
        //    return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        //}



        #region 汇总

        public ActionResult Assemble(CargoInfoViewModel cargoInfoViewModel, int limit, int offset) // group by
        {
            //var temp = from cargoLog in _db.CargoLogInfo
            //           join cargo in _db.CargoInfo on new { Id = cargoLog.CargoInfoId } equals new { cargo.Id }
            //           join supply in _db.SupplyInfo on cargoLog.SupplyId equals supply.Id
            //           join project in _db.ProjectInfo on new { Id = (int)cargoLog.ProjectId } equals new { project.Id }
            //           where
            //               cargoLog.IsIncome == false
            //           group new { cargo, project, supply, cargoLog } by new
            //           {
            //               cargo.Id,
            //               cargo.CargoName,
            //               cargo.Unit,
            //               cargo.Specifications,
            //               cargo.Amount,
            //               project.ProjectName,
            //               ProjectId = project.Id,
            //               cargoLog.SupplyId,
            //               SupplyName = supply.Name
            //           }
            //    into g
            //           select new
            //           {
            //               g.Key.Id,
            //               g.Key.CargoName,
            //               g.Key.Unit,
            //               g.Key.Specifications,
            //               g.Key.Amount,
            //               g.Key.ProjectId,
            //               g.Key.ProjectName,
            //               g.Key.SupplyId,
            //               g.Key.SupplyName,
            //               SumCount = (int?)g.Sum(p => p.cargoLog.ChangeCount),
            //               SumAmount = g.Sum(p => (decimal)p.cargoLog.ChangeCount * p.cargo.Amount)
            //           };

            //if (cargoInfoViewModel.ProjectId != null)
            //{
            //    temp = temp.Where(c => c.ProjectId == cargoInfoViewModel.ProjectId);
            //}

            //if (cargoInfoViewModel.SupplyId != null)
            //{
            //    temp = temp.Where(c => c.SupplyId == cargoInfoViewModel.SupplyId);
            //}

            ////if (cargoInfoViewModel.TimeEnd == DateTime.MinValue)
            ////{
            ////    cargoInfoViewModel.TimeEnd = DateTime.MaxValue;
            ////}
            ////temp2 = temp.Where(c => cargoInfoViewModel.TimeStart <= c.Time && c.Time < cargoInfoViewModel.TimeEnd);

            //var list = temp.OrderByDescending(c => c.Id).ToList();
            //return Json(new { total = list.Count, rows = list.Skip((int)offset).Take((int)limit).ToList() });
            return Json("");
        }
        #endregion

        #endregion

        //#region 导出Excel

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
            //decimal wei = 0;
            //foreach (var item in list)
            //{
            //    if (item.出入库 == "入库")
            //    {
            //        wei += item.重量;
            //    }

            //    if (item.出入库 == "出库")
            //    {
            //        wei -= item.重量;
            //    }
            //}
            ////var wei = SumWeightList(cargoInfoViewModel.CargoAreaId, cargoInfoViewModel.Id);



            //list.Add(new AssembleLogExcel { 重量 = wei });

            var filePath = ToExcel(list);
            return filePath;
        }

        //public string ExcelCargo(CargoInfoViewModel cargoInfoViewModel)
        //{
        //    var temp = (from cargoInfo in _db.CargoInfo select new { 产品号 = cargoInfo.Id, 产品名 = cargoInfo.CargoName, 金额 = cargoInfo.Amount, 数量 = cargoInfo.CargoCount, 单位 = cargoInfo.Unit, 规格型号 = cargoInfo.Specifications, 存放区域 = cargoInfo.Area }).Select(cargoInfo => new
        //    {
        //        cargoInfo.产品号,
        //        cargoInfo.产品名,
        //        cargoInfo.金额,
        //        cargoInfo.数量,
        //        cargoInfo.单位,
        //        cargoInfo.规格型号,
        //        cargoInfo.存放区域
        //    });

        //    if (cargoInfoViewModel.Id != 0)
        //    {
        //        temp = temp.Where(c => c.产品号 == cargoInfoViewModel.Id);
        //    }
        //    if (!cargoInfoViewModel.CargoName.IsEmpty())
        //    {
        //        //temp = temp.Where(c => c.CargoName.Contains(cargoInfoViewModel.CargoName)); // 模糊
        //        temp = temp.Where(c => c.产品名 == cargoInfoViewModel.CargoName);
        //    }
        //    var list = temp.OrderByDescending(c => c.产品号).ToList();


        //    var filePath = ToExcel(list);
        //    return filePath;
        //}

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
        //#endregion

        //[HttpPost]
        //public ActionResult AssembleWeight(CargoInfoViewModel cargoInfoViewModel, int? limit, int? offset)
        //{
        //    var temp = (
        //        (from a in _db.CargoLogInfo
        //         join b in _db.CargoInfo on new { CargoId = (int)a.CargoId } equals new { CargoId = b.Id }
        //         where a.CargoAreaId == cargoInfoViewModel.CargoAreaId && a.CargoId == cargoInfoViewModel.Id
        //         select new
        //         {
        //             b.CatId1,
        //             a.ChangeWeight,
        //             a.IsIncome
        //         }));
        //    var list = (
        //        from d in (
        //            (from c in temp
        //             select new
        //             {
        //                 CatId = c.CatId1,
        //                 Weight =
        //                     c.IsIncome == false ? (0 - c.ChangeWeight) : (System.Int64)c.ChangeWeight
        //             }))
        //        group d by new
        //        {
        //            d.CatId
        //        }
        //        into g
        //        select new
        //        {
        //            g.Key.CatId,
        //            Weight = g.Sum(p => p.Weight)
        //        });
        //    var lis = list.Join(_db.CatInfo, a => a.CatId, b => b.Id, (a, b) => new { a.Weight, b.CatName })
        //        .ToList();
        //    //var tt = _db.CargoLogInfo.Join(_db.CargoInfo, cargoLog => cargoLog.CargoInfoId, cargo => cargo.Id, (cargoLog, cargo) => new { ProjectId = cargoLog.ProjectId, CargoLogId = cargoLog.Id, cargoLog.IsIncome, cargoLog.ChangeCount, cargoLog.Time, cargo.CargoName }).GroupJoin(_db.ProjectInfo, c => c.ProjectId, pro => pro.Id, (c, pro) => new { c.CargoLogId, c.IsIncome, c.ChangeCount, c.Time, c.CargoName, pro }).Select(o => new { o.CargoName, o.CargoLogId, o.ChangeCount, o.IsIncome, o.Time, o.pro });

        //    return Json(new { total = lis.Count, data = lis[0].Weight });
        //}

        //public ActionResult AssLogWeight(int? id)
        //{
        //    return View();
        //}

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

        //public decimal SumWeightList(int CargoAreaId, int Id)
        //{
        //    var temp = (
        //        (from a in _db.CargoLogInfo
        //         join b in _db.CargoInfo on new { CargoId = (int)a.CargoId } equals new { CargoId = b.Id }
        //         where a.CargoAreaId == CargoAreaId && a.CargoId == Id
        //         select new
        //         {
        //             b.CatId1,
        //             a.ChangeWeight,
        //             a.IsIncome
        //         }));
        //    var list = (
        //        from d in (
        //            (from c in temp
        //             select new
        //             {
        //                 CatId = c.CatId1,
        //                 Weight =
        //                     c.IsIncome == false ? (0 - c.ChangeWeight) : (System.Int64)c.ChangeWeight
        //             }))
        //        group d by new
        //        {
        //            d.CatId
        //        }
        //        into g
        //        select new
        //        {
        //            g.Key.CatId,
        //            Weight = g.Sum(p => p.Weight)
        //        });
        //    var lis = list.Join(_db.CatInfo, a => a.CatId, b => b.Id, (a, b) => new { a.Weight, b.CatName })
        //        .ToList();
        //    return lis[0].Weight;
        //}

    }

    public class AssembleLogWeight
    {
        public int CatId { get; set; }
        public decimal Weight { get; set; }

        public AssembleLogWeight()
        {
            Weight = 0;
        }
    }

    public class AssembleLogExcel
    {
        public string 产品名称 { get; set; }
        public string 出入库 { get; set; }
        public decimal 重量 { get; set; }
        public string 批号 { get; set; }
        public string 方式 { get; set; }
        public string 仓库 { get; set; }
        public string 描述 { get; set; }
        public string 录入人 { get; set; }
        public DateTime? 录入时间 { get; set; }

        //  cargoLogInfo.ChangeCount,

        //  cargoLogInfo.CargoId,


        //  cargoLogInfo.TakenName,



        //  cargoLogInfo.CargoAreaId
    }
}