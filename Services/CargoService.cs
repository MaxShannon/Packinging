using DbEfModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class CargoService : BaseService
    {


        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();

        public CargoService()
        {

        }

        public IEnumerable<CargoViewModel> GetCargo(int? Huom2Id, int? huotId, int? shipmentId, string userId)
        {
            var user = _db.sys_ry.Find(userId);
            IQueryable<CargoViewModel> list;
            if (user.lx == _lxId || user.lx == _lxId2)
            {
                list = (
                   from sys_ry in _db.sys_ry
                   join huot in _db.huot on sys_ry.htid equals huot.id
                   join huotHuomInfoes in _db.HuotHuomInfoes on huot.id equals huotHuomInfoes.HuotId
                   join huom in _db.huom on huotHuomInfoes.HuomId equals huom.id
                   join huom2 in _db.huom2 on huom.id equals huom2.huomid
                   join cargoInfoes in _db.CargoInfoes on huom2.id equals cargoInfoes.Huom2Id
                       into cargoInfoesJoin
                   from cargoInfoes in cargoInfoesJoin.DefaultIfEmpty()
                   join cargoHuotInfoes in _db.CargoHuotInfoes on new { CargoId = cargoInfoes.Id, HuotId = huot.id }
                       equals new { CargoId = cargoHuotInfoes.CargoId, HuotId = cargoHuotInfoes.HuotId }
                       into cargoHuotInfoesJoin
                   from cargoHuotInfoes in cargoHuotInfoesJoin.DefaultIfEmpty()
                   select new CargoViewModel
                   {
                       TopCargoName = huom.name,
                       CargoName = huom2.name,
                       Huom2Id = huom2.id,
                       HuotName = huot.name,
                       Weight = cargoHuotInfoes == null ? 0 : cargoHuotInfoes.Weight,
                       FillList = sys_ry.lx == _lxId || sys_ry.lx == _lxId2, // 8,9 包装货台包装都可以
                       UserId = sys_ry.pk,
                       IsOk = huom2.sfyx == 1,
                   }).Where(a => a.UserId == userId && a.IsOk);
            }

            else
            {
                list = (
                    from sys_ry in _db.sys_ry
                    join huot in _db.huot on sys_ry.dwid equals huot.dwid // 就这一句不同
                    join huotHuomInfoes in _db.HuotHuomInfoes on huot.id equals huotHuomInfoes.HuotId
                    join huom in _db.huom on huotHuomInfoes.HuomId equals huom.id
                    //join huotHuomInfoese in _db.huom on EXPR1 equals EXPR2 
                    join huom2 in _db.huom2 on huom.id equals huom2.huomid
                    join cargoInfoes in _db.CargoInfoes on huom2.id equals cargoInfoes.Huom2Id
                        into cargoInfoesJoin
                    from cargoInfoes in cargoInfoesJoin.DefaultIfEmpty()
                    join cargoHuotInfoes in _db.CargoHuotInfoes on new { CargoId = cargoInfoes.Id, HuotId = huot.id }
                     equals new { CargoId = cargoHuotInfoes.CargoId, HuotId = cargoHuotInfoes.HuotId }
                     into cargoHuotInfoesJoin
                    from cargoHuotInfoes in cargoHuotInfoesJoin.DefaultIfEmpty()
                    select new CargoViewModel
                    {
                        TopCargoName = huom.name,
                        CargoName = huom2.name,
                        Huom2Id = huom2.id,
                        HuotName = huot.name,
                        Weight = cargoHuotInfoes == null ? 0 : cargoHuotInfoes.Weight,
                        FillList = sys_ry.lx == _lxId || sys_ry.lx == _lxId2, // 8,9 包装货台包装都可以
                        UserId = sys_ry.pk,
                        IsOk = huom2.sfyx == 1,
                    }).Where(a => a.UserId == userId && a.IsOk);
            }


            if (huotId != 0 && huotId != null)
            {
                list = list.Where(a => a.HuotId == huotId);
            }
            if (shipmentId != 0 && shipmentId != null)
            {
                list = list.Where(a => a.ShipmentId == shipmentId);
            }
            if (Huom2Id != 0 && Huom2Id != null)
            {
                list = list.Where(a => a.Huom2Id == Huom2Id);
            }

            var li = list.ToList();
            return li;
        }
        public IEnumerable<CargoViewModel> GetCargoDetail(int? cargoId, int? huotId, int? shipmentId)
        {
            var list = (
                from shipmentHuotInfoes in _db.ShipmentHuotInfoes
                join huot in _db.huot on shipmentHuotInfoes.HuotId equals huot.id
                join shipmentInfoes in _db.ShipmentInfoes on shipmentHuotInfoes.ShipmentId equals shipmentInfoes.Id
                join cargoInfoes in _db.CargoInfoes on shipmentInfoes.CargoId equals cargoInfoes.Id
                join huom2 in _db.huom2 on cargoInfoes.Huom2Id equals huom2.id
                select new CargoViewModel
                {
                    CargoId = cargoInfoes.Id,
                    ShipmentId = shipmentInfoes.Id,
                    ShipmentNo = shipmentInfoes.ShipmentNo,
                    CargoName = huom2.name,
                    Weight = shipmentHuotInfoes.Weight,
                    HuotId = huot.id,
                    HuotName = huot.name
                });

            if (huotId != 0 && huotId != null)
            {
                list = list.Where(a => a.HuotId == huotId);
            }
            if (shipmentId != 0 && shipmentId != null)
            {
                list = list.Where(a => a.ShipmentId == shipmentId);
            }
            if (cargoId != 0 && cargoId != null)
            {
                list = list.Where(a => a.CargoId == cargoId);
            }

            var li = list.ToList();
            return li;
        }

        public bool AddCargoComeLog(int huom2Id, int huotId, string shipmentNo, decimal changeWeight, string inspectName, int count, decimal weightType)
        {
            CheckCargo(huom2Id);
            var cargo = _db.CargoInfoes.FirstOrDefault(c => c.Huom2Id == huom2Id);

            CheckShipment(shipmentNo, cargo.Id);
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == shipmentNo && a.CargoId == cargo.Id);

            CheckShipmentHuot(shipment.Id, huotId);
            var shipmentHuot = _db.ShipmentHuotInfoes.FirstOrDefault(a => a.ShipmentId == shipment.Id && a.HuotId == huotId);

            CheckCargoHuot(cargo.Id, huotId);
            var cargoHuot = _db.CargoHuotInfoes.FirstOrDefault(a => a.CargoId == cargo.Id && a.HuotId == huotId);

            var cargoLog = new CargoLogInfoes()
            {
                ChangeWeight = changeWeight,
                IsIncome = true,
                InspectTime = DateTime.Now,
                InspectName = inspectName,
                StateId = (int)StateEnum.正在审核,
                Desc = StateEnum.正在审核.ToString(),//"等待审核",
                CargoId = cargo.Id,
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                HuotId = huotId,
                Count = count,
                WeightType = weightType == 0 ? "" : "每袋" + weightType,
            };
            _db.CargoLogInfoes.Add(cargoLog);

            cargo.Weight += changeWeight;
            shipment.Weight += changeWeight;
            shipmentHuot.Weight += changeWeight;
            cargoHuot.Weight += changeWeight;

            return _db.SaveChanges() > 0;
        }

        public bool AddCargoShipLog(int huom2Id, int huotId, int shipmentId, decimal changeWeight, int cargoInId, string inspectName)
        {
            //CheckCargo(huom2Id);
            //var cargo = _db.CargoInfoes.First(c => c.Huom2Id == huom2Id);
            //CheckShipment(shipmentNo, cargo.Id);
            //var shipment = _db.ShipmentInfoes.First(a => a.ShipmentNo == shipmentNo);

            var cargo = _db.CargoInfoes.First(a => a.Huom2Id == huom2Id);
            var shipment = _db.ShipmentInfoes.First(a => a.Id == shipmentId);
            var shipmentHuotInfoes = _db.ShipmentHuotInfoes.First(a => a.ShipmentId == shipment.Id && a.HuotId == huotId);
            var cargoHuot = _db.CargoHuotInfoes.First(a => a.CargoId == cargo.Id && a.HuotId == huotId);

            cargo.Weight -= changeWeight;
            _db.Entry(cargo).Property(c => c.Weight).IsModified = true;
            shipment.Weight -= changeWeight;
            _db.Entry(shipment).Property(c => c.Weight).IsModified = true;
            shipmentHuotInfoes.Weight -= changeWeight;
            _db.Entry(shipmentHuotInfoes).Property(c => c.Weight).IsModified = true;
            cargoHuot.Weight -= changeWeight;
            _db.Entry(cargoHuot).Property(c => c.Weight).IsModified = true;

            if (cargo.Weight < 0 || shipment.Weight < 0 || shipmentHuotInfoes.Weight < 0 || cargoHuot.Weight < 0)
            {
                return false;
            }

            if (shipment.CargoId != cargo.Id)
            {
                return false;
                // return Json(new { success = "fail", message = "当前批号不对应该产品" });
            }

            var cargoLog = new CargoLogInfoes()
            {
                IsIncome = false,
                InspectTime = DateTime.Now,
                StateId = 2,
                CargoId = cargo.Id,
                ChangeWeight = changeWeight,
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CargoInId = cargoInId,
                HuotId = huotId,
                InspectName = inspectName,
                Desc = "火车出库"
            };
            _db.CargoLogInfoes.Add(cargoLog);

            return _db.SaveChanges() > 0;

        }

        public IEnumerable<CargoViewModel> GetShipment(int huom2Id)
        {
            CheckCargo(huom2Id);
            var cargo = _db.CargoInfoes.FirstOrDefault(a => a.Huom2Id == huom2Id);
            var shipmentList = _db.ShipmentInfoes.Where(a => a.CargoId == cargo.Id).Select(a => new CargoViewModel
            {
                ShipmentId = a.Id,
                ShipmentNo = a.ShipmentNo
            }).ToList();
            return shipmentList;
        }



        public IEnumerable<CargoViewModel> GetCargoLog(int huom2Id, int stateId, DateTime timeStart, DateTime timeEnd, string userId, int? isIncome, int? huotId)
        {
            var user = _db.sys_ry.Find(userId);
            IQueryable<CargoViewModel> list;
            if (IsInOut(user))
            {
                var temp = (
                    from cargoLogInfoes in _db.CargoLogInfoes
                    join sys_ry in _db.sys_ry on cargoLogInfoes.HuotId equals sys_ry.htid
                    join cargoInfoes in _db.CargoInfoes on cargoLogInfoes.CargoId equals cargoInfoes.Id
                    join huom2 in _db.huom2 on cargoInfoes.Huom2Id equals huom2.id
                    join huom in _db.huom on huom2.huomid equals huom.id
                    join shipmentInfoes in _db.ShipmentInfoes on cargoLogInfoes.ShipmentId equals shipmentInfoes.Id
                    join cargoInInfoes in _db.CargoInInfoes on cargoLogInfoes.CargoInId equals cargoInInfoes.Id
                        into cargoInInfoesJoin
                    from cargoInInfoes in cargoInInfoesJoin.DefaultIfEmpty()
                    join huot in _db.huot on cargoLogInfoes.HuotId equals huot.id
                    select new CargoViewModel
                    {
                        CargoLogId = cargoLogInfoes.Id,
                        TopCargoName = huom.name,
                        CargoName = huom2.name,
                        Huom2Id = huom2.id,
                        Weight = cargoInfoes == null ? 0 : cargoInfoes.Weight,
                        IsOk = sys_ry.lx == _lxCheckId,
                        UserId = sys_ry.pk,
                        ChangeWeight = cargoLogInfoes.ChangeWeight,
                        StateId = cargoLogInfoes.StateId,
                        ShipmentNo = cargoLogInfoes.ShipmentNo,
                        CargoInName = cargoInInfoes.CargoInName,
                        HuotName = huot.name,
                        Time = (DateTime)cargoLogInfoes.InspectTime,
                        Desc = cargoLogInfoes.Desc,
                        IsIncome = (bool)cargoLogInfoes.IsIncome,
                        HuotId = huot.id,
                        InspectName = cargoLogInfoes.InspectName,
                        HuomId = huom.id
                    }
                );
                list = temp.Where(a => a.UserId == userId);
            }
            else
            {
                //select* from sys_ry
                //    join dw on sys_ry.dwid = dw.id
                //join huot c on sys_ry.dwid = c.dwid
                //join CargoLogInfoes d on c.id = d.HuotId
                //join ShipmentInfoes e on d.ShipmentId = e.Id
                //join CargoInfoes f on d.CargoId = f.Id
                //join huom2 g on f.Huom2Id = g.id
                //join huom h on g.huomid = h.id
                //where sys_ry.id = 31 and d.StateId = 1
                list = (
                    from sys_ry in _db.sys_ry
                    join dw in _db.dw on sys_ry.dwid equals dw.id
                    join huot in _db.huot on sys_ry.dwid equals huot.dwid
                    join cargoLogInfoes in _db.CargoLogInfoes on huot.id equals cargoLogInfoes.HuotId
                    join cargoInInfoes in _db.CargoInInfoes on cargoLogInfoes.CargoInId equals cargoInInfoes.Id
                        into cargoInInfoesJoin
                    from cargoInInfoes in cargoInInfoesJoin.DefaultIfEmpty()
                    join shipmentInfoes in _db.ShipmentInfoes on cargoLogInfoes.ShipmentId equals shipmentInfoes.Id
                    join cargoInfoes in _db.CargoInfoes on cargoLogInfoes.CargoId equals cargoInfoes.Id
                    join huom2 in _db.huom2 on cargoInfoes.Huom2Id equals huom2.id
                    join huom in _db.huom on huom2.huomid equals huom.id
                    select new CargoViewModel
                    {
                        CargoLogId = cargoLogInfoes.Id,
                        TopCargoName = huom.name,
                        CargoName = huom2.name,
                        Huom2Id = huom2.id,
                        Weight = cargoInfoes == null ? 0 : cargoInfoes.Weight,
                        IsOk = sys_ry.lx == _lxCheckId,
                        UserId = sys_ry.pk,
                        ChangeWeight = cargoLogInfoes.ChangeWeight,
                        StateId = cargoLogInfoes.StateId,
                        ShipmentNo = cargoLogInfoes.ShipmentNo,
                        CargoInName = cargoInInfoes.CargoInName,
                        HuotName = huot.name,
                        Time = (DateTime)cargoLogInfoes.InspectTime,
                        Desc = cargoLogInfoes.Desc,
                        IsIncome = (bool)cargoLogInfoes.IsIncome,
                        HuotId = huot.id,
                        InspectName = cargoLogInfoes.InspectName,
                        HuomId = huom.id,
                    }).Where(a => a.UserId == userId);
            }


            if (isIncome == 2)
            {
                list = list.Where(a => !a.IsIncome);
            }
            if (isIncome == 3)
            {
                list = list.Where(a => a.IsIncome);
            }
            list = list.Where(a => a.Time >= timeStart && a.Time <= timeEnd);
            if (huotId != 0)
            {
                list = list.Where(a => a.HuotId == huotId);
            }

            if (huom2Id != 0)
            {
                list = list.Where(a => a.Huom2Id == huom2Id);
            }

            var li = list.OrderByDescending(a => a.CargoLogId).ToList();
            return li;
        }

        public IEnumerable<CargoViewModel> GetCargoShipmentHuot(int huotId, int? cargoId, string userId)
        {
            var user = _db.sys_ry.Find(userId);
            IQueryable<CargoViewModel> list;
            if (IsInOut(user))
            {
                //select* from sys_ry
                //    join huot c on sys_ry.htid = c.id
                //join ShipmentHuotInfoes b on sys_ry.htid = b.HuotId
                //join ShipmentInfoes d on b.ShipmentId = d.Id
                //join CargoInfoes e on d.CargoId = e.Id
                //join huom2 f on e.Huom2Id = f.id
                //join huom g on f.huomid = g.id
                //where sys_ry.id = 31
                // 自己的货台
                list = (
                   from sys_ry in _db.sys_ry
                   join huot in _db.huot on sys_ry.htid equals huot.id
                   join shipmentHuotInfoes in _db.ShipmentHuotInfoes on huot.id equals shipmentHuotInfoes.HuotId
                   join shipmentInfoes in _db.ShipmentInfoes on shipmentHuotInfoes.ShipmentId equals shipmentInfoes.Id
                   join cargoInfoes in _db.CargoInfoes on shipmentInfoes.CargoId equals cargoInfoes.Id
                   join huom2 in _db.huom2 on cargoInfoes.Huom2Id equals huom2.id
                   join huom in _db.huom on huom2.huomid equals huom.id
                   select new CargoViewModel
                   {
                       TopCargoName = huom.name,
                       CargoName = huom2.name,
                       Huom2Id = huom2.id,
                       HuotName = huot.name,
                       Weight = shipmentHuotInfoes == null ? 0 : shipmentHuotInfoes.Weight,
                       FillList = sys_ry.lx == _lxId || sys_ry.lx == _lxId2, // 8,9 包装货台包装都可以
                       UserId = sys_ry.pk,
                       HuotId = huot.id,
                       CargoId = cargoInfoes.Id,
                       ShipmentNo = shipmentInfoes.ShipmentNo,
                       IsOk = true
                   }).Where(a => a.UserId == userId);
            }

            else
            {
                //select* from ShipmentInfoes
                //    join CargoInfoes on ShipmentInfoes.CargoId = CargoInfoes.Id
                //join ShipmentHuotInfoes on ShipmentInfoes.Id = ShipmentHuotInfoes.ShipmentId
                //join huot on ShipmentHuotInfoes.HuotId = huot.id
                //join sys_ry on sys_ry.dwid = huot.dwid
                //where ShipmentHuotInfoes.HuotId = 2 and CargoInfoes.Id = 9 and sys_ry.pk = '1bba0a9b-8ce0-4f82-ad1c-292672a696a4'
                // 很多货台
                list = (
                    from sys_ry in _db.sys_ry
                    join huot in _db.huot on sys_ry.dwid equals huot.dwid
                    join shipmentHuotInfoes in _db.ShipmentHuotInfoes on huot.id equals shipmentHuotInfoes.HuotId
                    join shipmentInfoes in _db.ShipmentInfoes on shipmentHuotInfoes.ShipmentId equals shipmentInfoes.Id
                    join cargoInfoes in _db.CargoInfoes on shipmentInfoes.CargoId equals cargoInfoes.Id
                    join huom2 in _db.huom2 on cargoInfoes.Huom2Id equals huom2.id
                    join huom in _db.huom on huom2.huomid equals huom.id
                    select new CargoViewModel
                    {
                        TopCargoName = huom.name,
                        CargoName = huom2.name,
                        Huom2Id = huom2.id,
                        HuotName = huot.name,
                        Weight = shipmentHuotInfoes == null ? 0 : shipmentHuotInfoes.Weight,
                        FillList = sys_ry.lx == _lxId || sys_ry.lx == _lxId2, // 8,9 包装货台包装都可以
                        UserId = sys_ry.pk,
                        HuotId = huot.id,
                        CargoId = cargoInfoes.Id,
                        ShipmentNo = shipmentInfoes.ShipmentNo,
                        IsOk = true
                    }).Where(a => a.UserId == userId);
            }
            //var list = (
            //    //from shipmentHuotInfoes in _db.ShipmentHuotInfoes
            //    from shipmentHuotInfoes in _db.ShipmentHuotInfoes
            //    join shipmentInfoes in _db.ShipmentInfoes on shipmentHuotInfoes.ShipmentId equals shipmentInfoes.Id
            //    join huot in _db.huot on shipmentHuotInfoes.HuotId equals huot.id
            //    join cargoInfoes in _db.CargoInfoes on shipmentInfoes.CargoId equals cargoInfoes.Id
            //    join huom2 in _db.huom2 on cargoInfoes.Huom2Id equals huom2.id
            //    join huom in _db.huom on huom2.huomid equals huom.id
            //    join cargoView in _db.CargoView on huom.id equals cargoView.huomid
            //    join sys_ry in _db.sys_ry on cargoView.dwid equals sys_ry.dwid
            //    select new CargoViewModel
            //    {
            //        ShipmentNo = shipmentInfoes.ShipmentNo,
            //        HuotId = huot.id,
            //        HuotName = huot.name,
            //        Weight = (decimal)shipmentHuotInfoes.Weight,
            //        CargoName = huom2.name,
            //        TopCargoName = huom.name,
            //        CargoId = cargoInfoes.Id,
            //        IsIncome = sys_ry.lx == _lxId, //权限
            //        UserId = sys_ry.id,
            //        //IsChecked = shipmentHuotInfoes.IsChecked,
            //        //MissDuTime = shipmentHuotInfoes.sj
            //    }).Where(a => a.HuotId == huotId && a.UserId == userId);
            if (cargoId != 0)
            {
                list = list.Where(a => a.CargoId == cargoId);
            }
            if (huotId != 0)
            {
                list = list.Where(a => a.HuotId == huotId);
            }
            //if (shipmentId != 0 && shipmentId != null)
            //{
            //    list = list.Where(a => a.ShipmentId == shipmentId);
            //}

            //if (cargoId != 0 && cargoId != null)
            //{
            //    list = list.Where(a => a.CargoId == cargoId);
            //}

            var li = list.ToList();
            return li;
        }

        public bool ExchangeShipment(string shipmentNo, int huotId, int newHuotId, decimal weight, string userId)
        {
            var shipment = _db.ShipmentInfoes.First(a => a.ShipmentNo == shipmentNo);
            var cargoId = shipment.CargoId;
            var user = _db.sys_ry.Find(userId);
            var cargoAreaCargo = _db.ShipmentHuotInfoes.First(a => a.HuotId == huotId && a.ShipmentId == shipment.Id);
            cargoAreaCargo.Weight -= weight;
            if (cargoAreaCargo.Weight < 0)
            {
                return false;
            }
            var cargoHuot = _db.CargoHuotInfoes.FirstOrDefault(a => a.CargoId == shipment.CargoId && a.HuotId == huotId);
            cargoHuot.Weight -= weight;
            if (cargoHuot.Weight < 0)
            {
                return false;
            }
            CheckCargoHuot(cargoId, newHuotId);
            var newCargoHuot = _db.CargoHuotInfoes.First(a => a.CargoId == cargoId && a.HuotId == newHuotId);
            newCargoHuot.Weight += weight;

            CheckShipmentHuot(shipment.Id, newHuotId);
            var newShipmentHuo = _db.ShipmentHuotInfoes.FirstOrDefault(a => a.HuotId == newHuotId && a.ShipmentId == shipment.Id);
            var huot = _db.huot.Find(newHuotId);
            newShipmentHuo.Weight += weight;

            //var cargoLogList = new List<CargoLogInfo>();
            var cargoLog = new CargoLogInfoes();
            cargoLog.IsIncome = false;
            cargoLog.ChangeWeight = weight;
            cargoLog.CargoId = cargoId;
            cargoLog.HuotId = huotId;
            cargoLog.ShipmentId = shipment.Id;
            cargoLog.ShipmentNo = shipment.ShipmentNo;
            cargoLog.StateId = 3;
            cargoLog.InspectName = user.name;
            cargoLog.InspectTime = DateTime.Now;
            cargoLog.Desc = "转库";
            _db.CargoLogInfoes.Add(cargoLog);

            var cargoLog2 = new CargoLogInfoes();
            cargoLog2.IsIncome = true;
            cargoLog2.ChangeWeight = weight;
            cargoLog2.CargoId = cargoId;
            cargoLog2.HuotId = newHuotId;
            cargoLog2.ShipmentId = shipment.Id;
            cargoLog.ShipmentNo = shipment.ShipmentNo;
            cargoLog2.StateId = 3;
            cargoLog2.InspectName = user.name;
            cargoLog2.InspectTime = DateTime.Now;
            cargoLog2.Desc = "转库";
            _db.CargoLogInfoes.Add(cargoLog2);


            return _db.SaveChanges() >= 2;
        }

        public bool ConfirmCargoOutOrder(int cargoOutOrderId, string userId)
        {
            var shipmentView = _dbd.pihao.FirstOrDefault(a => a.ID == cargoOutOrderId);
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == shipmentView.PH_NO);
            var cargo = _db.CargoInfoes.Find(shipment.CargoId);
            var shipmentHuot = _db.ShipmentHuotInfoes.First(a => a.HuotId == (int)shipmentView.HuotId && a.ShipmentId == shipment.Id);
            var cargoHuot = _db.CargoHuotInfoes.First(a => a.HuotId == shipmentView.HuotId && a.CargoId == cargo.Id);

            shipmentView.InspectId = userId;
            shipmentView.InspectTime = DateTime.Now;
            shipmentView.IsChecked = true;
            _dbd.Entry(shipmentView).Property(c => c.IsChecked).IsModified = true;
            _dbd.Entry(shipmentView).Property(c => c.InspectTime).IsModified = true;
            _dbd.Entry(shipmentView).Property(c => c.InspectId).IsModified = true;

            cargo.Weight -= (decimal)shipmentView.PH_ZL;

            shipment.Weight -= (decimal)shipmentView.PH_ZL;

            shipmentHuot.Weight -= (decimal)shipmentView.PH_ZL;

            cargoHuot.Weight -= (decimal)shipmentView.PH_ZL;

            if (cargo.Weight < 0 || shipment.Weight < 0 || shipmentHuot.Weight < 0 || cargoHuot.Weight < 0)
            {
                return false;
            }

            _db.Entry(cargo).Property(c => c.Weight).IsModified = true;
            _db.Entry(shipment).Property(c => c.Weight).IsModified = true;
            _db.Entry(shipmentHuot).Property(c => c.Weight).IsModified = true;
            _db.Entry(cargoHuot).Property(c => c.Weight).IsModified = true;

            var cargoLog = new CargoLogInfoes()
            {
                IsIncome = false,
                InspectTime = DateTime.Now,
                StateId = 2,
                CargoId = cargo.Id,
                ChangeWeight = (decimal)shipmentView.PH_ZL,
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                CargoInId = 2,
                HuotId = shipmentHuot.HuotId,
                InspectName = shipmentView.qianfa,
                Desc = "汽车出库",
            };
            _db.CargoLogInfoes.Add(cargoLog);


            return _db.SaveChanges() == 5 && _dbd.SaveChanges() == 1;
        }

        public IEnumerable<CargoViewModel> GetUserHuot(string userId)
        {
            var user = _db.sys_ry.Find(userId);
            IQueryable<CargoViewModel> temp;
            if (IsInOut(user))//包装 (入库)
            {
                if (IsCanSeeAll(user))
                {
                    temp = (
                        from sys_ry in _db.sys_ry
                        join huot in _db.huot on sys_ry.dwid equals huot.ssdwid
                        select new CargoViewModel
                        {
                            HuotId = huot.id,
                            HuotName = huot.name,
                            UserId = sys_ry.pk
                        });
                }
                else
                {
                    temp = (
                        from sys_ry in _db.sys_ry
                        join huot in _db.huot on sys_ry.htid equals huot.id
                        select new CargoViewModel
                        {
                            HuotId = huot.id,
                            HuotName = huot.name,
                            UserId = sys_ry.pk
                        });
                }

            }
            else
            {
                temp = (
                    from huot in _db.huot
                    join sys_ry in _db.sys_ry on huot.dwid equals sys_ry.dwid
                    where sys_ry.pk == userId
                    select new CargoViewModel
                    {
                        HuotId = (int)huot.id,
                        HuotName = huot.name,
                        UserId = sys_ry.pk
                    });
            }

            temp = temp.Where(a => a.UserId == userId);
            var list = temp.ToList();
            return list;
        }

        public IEnumerable<CargoViewModel> GetHuotShipments(int huotId, int huom2Id)
        {
            var temp = (
                from shipmentHuotInfoes in _db.ShipmentHuotInfoes
                join shipmentInfoes in _db.ShipmentInfoes on shipmentHuotInfoes.ShipmentId equals shipmentInfoes.Id
                join cargoInfoes in _db.CargoInfoes on shipmentInfoes.CargoId equals cargoInfoes.Id
                select new CargoViewModel
                {
                    HuotId = shipmentHuotInfoes.HuotId,
                    ShipmentId = shipmentInfoes.Id,
                    ShipmentNo = shipmentInfoes.ShipmentNo,
                    Huom2Id = cargoInfoes.Huom2Id
                }).Where(a => a.HuotId == huotId && a.Huom2Id == huom2Id);
            var list = temp.ToList();
            return list;
        }

        public IEnumerable<CargoViewModel> GetCargoOutShipment(string loginUserId)
        {
            var temp = (
                from shipmentView in _db.ShipmentView
                join cheh in _db.cheh on shipmentView.DHLS equals cheh.DHLS
                join huot in _db.huot on shipmentView.HuotId equals huot.id
                join shipmentInfoes in _db.ShipmentInfoes on shipmentView.PH_NO equals shipmentInfoes.ShipmentNo
                join cargoInfoes in _db.CargoInfoes on shipmentInfoes.CargoId equals cargoInfoes.Id
                join huom2 in _db.huom2 on cargoInfoes.Huom2Id equals huom2.id
                join huom in _db.huom on huom2.huomid equals huom.id
                join sys_ry in _db.sys_ry on huot.id equals sys_ry.htid
                where sys_ry.pk == loginUserId && (sys_ry.lx == _lxId || sys_ry.lx == _lxId2) && sys_ry.htid == shipmentView.HuotId && shipmentView.tihuolx == "发货位       "
                select new CargoViewModel
                {
                    CargoOutOrderId = shipmentView.ID,
                    HuotId = (int)shipmentView.HuotId,
                    ShipmentNo = shipmentView.PH_NO,
                    IsChecked = shipmentView.IsChecked == true,
                    WeightFlag = cheh.weight_flag,
                    HuotName = huot.name,
                    CargoName = huom2.name,
                    TopCargoName = huom.name,
                    WeightDu = (double)shipmentView.PH_ZL,
                    MissDuTime = shipmentView.sj,
                    IsOk = true
                });
            var list = temp.ToList();
            return list;
        }
        public IEnumerable<CargoViewModel> GetSelfSellLog(string loginUserId)
        {
            var temp = (
                from shipmentView in _db.ShipmentView
                join cheh in _db.cheh on shipmentView.DHLS equals cheh.DHLS
                join huot in _db.huot on shipmentView.HuotId equals huot.id
                join shipmentInfoes in _db.ShipmentInfoes on shipmentView.PH_NO equals shipmentInfoes.ShipmentNo
                join cargoInfoes in _db.CargoInfoes on shipmentInfoes.CargoId equals cargoInfoes.Id
                join huom2 in _db.huom2 on cargoInfoes.Huom2Id equals huom2.id
                join huom in _db.huom on huom2.huomid equals huom.id
                join sys_ry in _db.sys_ry on huot.id equals sys_ry.htid
                where sys_ry.pk == loginUserId && (sys_ry.lx == _lxId || sys_ry.lx == _lxId2) && sys_ry.htid == shipmentView.HuotId && shipmentView.tihuolx == "自提        "
                select new CargoViewModel
                {
                    CargoOutOrderId = shipmentView.ID,
                    HuotId = (int)shipmentView.HuotId,
                    ShipmentNo = shipmentView.PH_NO,
                    IsChecked = shipmentView.IsChecked == true,
                    WeightFlag = cheh.weight_flag,
                    HuotName = huot.name,
                    CargoName = huom2.name,
                    TopCargoName = huom.name,
                    WeightDu = (double)shipmentView.PH_ZL,
                    MissDuTime = shipmentView.sj,
                    IsOk = true
                });
            var list = temp.ToList();
            return list;
        }

        private bool IsCanSeeAll(sys_ry user)
        {
            if (user.qx == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void CheckShipment(string shipmentNo, int cargoId)
        {
            var cargo = _db.CargoInfoes.Find(cargoId);
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == shipmentNo && a.CargoId == cargo.Id);
            if (shipment == null)
            {
                // create 是否有这个批号
                var newShipment = new ShipmentInfoes();
                newShipment.ShipmentNo = shipmentNo;
                newShipment.CargoId = cargoId;
                newShipment.Weight = 0;
                newShipment.CargoName = cargo.CargoName;
                _db.ShipmentInfoes.Add(newShipment);
                _db.SaveChanges();
            }
        }

        public void CheckShipmentHuot(int shipmentId, int huotId)
        {
            var model = _db.ShipmentHuotInfoes.FirstOrDefault(a => a.ShipmentId == shipmentId && a.HuotId == huotId);
            if (model == null)
            {
                var huot = _db.huot.Find(huotId);
                var shipment = _db.ShipmentInfoes.Find(shipmentId);
                var newModel = new ShipmentHuotInfoes();
                newModel.HuotId = huotId;
                newModel.ShipmentId = shipmentId;
                newModel.HuotName = huot.name;
                newModel.ShipmentNo = shipment.ShipmentNo;
                newModel.Weight = 0;
                _db.ShipmentHuotInfoes.Add(newModel);
                _db.SaveChanges();
            }
        }

        public void CheckCargoHuot(int cargoId, int huotId)
        {
            var model = _db.CargoHuotInfoes.FirstOrDefault(a => a.CargoId == cargoId && a.HuotId == huotId);
            if (model == null)
            {
                var cargo = _db.CargoInfoes.Find(cargoId);
                var huot = _db.huot.Find(huotId);

                var newModel = new CargoHuotInfoes();
                newModel.HuotId = huotId;
                newModel.CargoId = cargoId;
                newModel.CargoName = cargo.CargoName;
                newModel.HuotName = huot.name;
                newModel.Weight = 0;
                _db.CargoHuotInfoes.Add(newModel);
                _db.SaveChanges();
            }
        }
        /// <summary>
        /// cargo中是否有huom2
        /// </summary>
        /// <param name="id">huom2id</param>
        public void CheckCargo(int id)
        {
            if (id == 0)
            {
                return;
            }
            var huom2 = _db.huom2.Find(id);
            var cargo = _db.CargoInfoes.FirstOrDefault(a => a.Huom2Id == id);
            if (cargo == null)
            {
                // create
                var newCargo = new CargoInfoes();
                newCargo.Huom2Id = id;
                newCargo.Weight = 0;
                newCargo.CargoName = huom2.name;
                _db.CargoInfoes.Add(newCargo);
                _db.SaveChanges();
            }
        }

        public bool SelfSellOut(int id, string userId)
        {
            var user = _db.sys_ry.Find(userId);
            var pihao = _dbd.pihao.Find(id);
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == pihao.PH_NO);

            var cargoLog = new CargoLogInfoes()
            {
                ChangeWeight = (decimal)pihao.PH_ZL,
                IsIncome = true,
                InspectTime = DateTime.Now,
                InspectName = user.name,
                StateId = 3,
                Desc = "入库自提",
                CargoId = shipment.CargoId,
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                HuotId = (int)pihao.HuotId,
                Count = 0,
                WeightType = ""
            };
            var cargoLog2 = new CargoLogInfoes()
            {
                ChangeWeight = (decimal)pihao.PH_ZL,
                IsIncome = false,
                InspectTime = DateTime.Now,
                InspectName = user.name,
                StateId = 3,
                Desc = "出库自提",
                CargoId = shipment.CargoId,
                ShipmentNo = shipment.ShipmentNo,
                ShipmentId = shipment.Id,
                HuotId = (int)pihao.HuotId,
                Count = 0,
                WeightType = ""
            };
            _db.CargoLogInfoes.Add(cargoLog);
            _db.CargoLogInfoes.Add(cargoLog2);
            pihao.IsChecked = true;
            pihao.InspectId = userId;
            pihao.InspectTime = DateTime.Now;
            return _db.SaveChanges() == 2 && _dbd.SaveChanges() == 1;
        }

        public IEnumerable<CargoViewModel> GetCargoHuot(int huotId, string loginUserPk)
        {
            var list = (
                from a in _db.CargoHuotInfoes
                join b in _db.CargoInfoes on a.CargoId equals b.Id
                join c in _db.huom2 on b.Huom2Id equals c.id
                join d in _db.huom on c.huomid equals d.id
                where a.HuotId == huotId
                select new CargoViewModel()
                {
                    CargoHuotId = a.Id,
                    HuotId = a.HuotId,
                    CargoId = a.CargoId,
                    CargoName = a.CargoName,
                    HuotName = a.HuotName,
                    TopCargoName = d.name,
                    Weight = a.Weight
                }
            ).ToList();
            return list;
        }
    }
}
