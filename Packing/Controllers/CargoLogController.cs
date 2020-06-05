using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DbEfModel;
using Packing.Models;
using Services;

namespace Packing.Controllers
{
    public class CargoLogController : BaseController
    {
        private readonly Huom2Service _huom2Service;
        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();
        private readonly CargoService _cargoService;
        private readonly CargoLogService _cargoLogService;

        public CargoLogController()
        {
            var cargoService = new CargoService();
            _cargoService = cargoService;
            _huom2Service = new Huom2Service();
            _cargoLogService = new CargoLogService();
        }
        public ActionResult Index()
        {
            return View();
        }

        #region 明细
        public ActionResult Details(int? id)
        {
            var s = new States();
            var t1 = new SelectList(s.StateCargoList, "Id", "StateCargoName", 0).ToList();
            ViewBag.StateList = t1;
            ViewBag.HuotName = new SelectList(_cargoService.GetUserHuot(LoginUser.pk), "HuotId", "HuotName", 0).ToList();

            CargoViewModel cargo = new CargoViewModel();
            cargo.Huom2Id = id ?? 0;
            cargo.TimeStart = DateTime.Today.AddHours(-6);
            cargo.TimeEnd = DateTime.Today.AddHours(18);
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

        [HttpPost]
        public ActionResult AssLogWeight(CargoViewModel cargoViewModel, int? limit, int? offset)
        {
            var tmp = _cargoService.GetCargoLog(cargoViewModel.Huom2Id, 2, cargoViewModel.TimeStart, cargoViewModel.TimeEnd, LoginUser.pk, cargoViewModel.StateId, cargoViewModel.HuotId);

            #region 小类
            //var temp = (from modelList in tmp
            //            select new
            //            {
            //                modelList.Huom2Id,
            //                modelList.CargoName,
            //                Weight = modelList.IsIncome == false ? (0 - modelList.ChangeWeight) : modelList.ChangeWeight
            //            });

            //var list = (from model in temp
            //            group model by new
            //            {
            //                model.Huom2Id,
            //                model.CargoName,
            //            }
            //    into g
            //            select new
            //            {
            //                g.Key.Huom2Id,
            //                g.Key.CargoName,
            //                Weight = g.Sum(a => a.Weight)
            //            }); 
            #endregion
            #region 大类
            var temp = (from modelList in tmp
                        select new
                        {
                            modelList.HuomId,
                            modelList.HuomName,
                            Weight = modelList.IsIncome == false ? (0 - modelList.ChangeWeight) : modelList.ChangeWeight
                        });

            var tem = (from model in temp
                       group model by new
                       {
                           model.HuomId,
                       }
                into g
                       select new
                       {
                           g.Key.HuomId,
                           Weight = g.Sum(a => a.Weight)
                       });
            var list = tem.Join(_db.huom, a => a.HuomId, b => b.id, (a, b) => new CargoViewModel()
            {
                HuomName = b.name,
                IsIncome = true,
                Weight = a.Weight
            }).ToList();
            #endregion

            return Json(new { total = list.Count(), rows = list.OrderByDescending(c => c.HuomName).Skip((int)offset).Take((int)limit).ToList() });

        }

        [HttpPost]
        public ActionResult AssInOutLogWeight(CargoViewModel cargoViewModel, int? limit, int? offset)
        {
            var tmp = _cargoService.GetCargoLog(cargoViewModel.Huom2Id, 2, cargoViewModel.TimeStart, cargoViewModel.TimeEnd, LoginUser.pk, cargoViewModel.StateId, cargoViewModel.HuotId);

            var temp = (
                from modelList in tmp
                group modelList by new
                {
                    modelList.HuomId,
                    modelList.IsIncome
                }
                into g
                select new
                {
                    g.Key.HuomId,
                    g.Key.IsIncome,
                    Weight = g.Sum(a => a.Weight)
                }
            );

            var list = temp.Join(_db.huom, a => a.HuomId, b => b.id, (a, b) => new CargoViewModel()
            {
                HuomName = b.name,
                IsIncome = a.IsIncome,
                Weight = a.Weight
            }).ToList();
            //list.Add(new CargoViewModel
            //{
            //    Weight = list[0].Weight - list[1].Weight
            //});
            return Json(new { total = list.Count, rows = list.OrderByDescending(c => c.HuomName).Skip((int)offset).Take((int)limit).ToList() });

        }
    }
}