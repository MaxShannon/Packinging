using DbEfModel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services
{
    public class CargoService
    {
        private readonly int _lxId; // 8
        private readonly int _lxId2; // 9
        private readonly int _lxCheckId; // 7
        // private readonly int _dwId;

        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();

        public CargoService(int lxId, int lxId2, int lxCheckId)
        {
            _lxId = lxId;
            _lxId2 = lxId2;
            _lxCheckId = lxCheckId;
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
            var cargo = _db.CargoInfoes.First(c => c.Huom2Id == huom2Id);
            if (CheckShipment(shipmentNo, cargo.Id))
            {
                var shipment = _db.ShipmentInfoes.First(a => a.ShipmentNo == shipmentNo);

                var str = "每袋" + weightType.ToString();
                var cargoLog = new CargoLogInfoes()
                {
                    ChangeWeight = changeWeight,
                    IsIncome = true,
                    InspectTime = DateTime.Now,
                    InspectName = inspectName,
                    StateId = 1,
                    Desc = StateEnum.正在审核.ToString(),//"等待审核",
                    CargoId = cargo.Id,
                    ShipmentId = shipment.Id,
                    HuotId = huotId,
                    Count = count,
                    WeightType = str
                };
                _db.CargoLogInfoes.Add(cargoLog);
                return _db.SaveChanges() > 0;
            }
            return false;
        }

        private bool CheckShipment(string shipmentNo, int cargoId)
        {
            var cargoName = _db.CargoInfoes.Find(cargoId);
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == shipmentNo);
            if (shipment == null)
            {
                // create
                var newShipment = new ShipmentInfoes();
                newShipment.ShipmentNo = shipmentNo;
                newShipment.CargoId = cargoId;
                newShipment.Weight = 0;
                newShipment.CargoName = cargoName.CargoName;
                _db.ShipmentInfoes.Add(newShipment);
                return _db.SaveChanges() > 0;
            }
            else if (shipment.CargoId == cargoId)
            {
                return true;
            }
            else
            {
                return false;
            }
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
                CargoInId = cargoInId,
                HuotId = huotId,
                InspectName = inspectName,
                Desc = ""
            };
            _db.CargoLogInfoes.Add(cargoLog);

            return _db.SaveChanges() > 0;

        }

        public bool PassCargoLog(int id)
        {
            var cargoLog = _db.CargoLogInfoes.Find(id);
            var cargo = _db.CargoInfoes.Find(cargoLog.CargoId);
            cargo.Weight += cargoLog.ChangeWeight;
            cargoLog.Desc = "通过!!!";
            cargoLog.StateId = 2;

            // add shipment
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.Id == cargoLog.ShipmentId);
            if (shipment == null)
            {
                //var shipmentNew = new ShipmentInfoes()
                //{
                //    CargoId = cargo.Id,
                //    Id = cargoLog.ShipmentId,
                //    ShipmentWeight = cargoLog.ChangeWeight,
                //    InspectTime = DateTime.Now,
                //    InspectId = LoginUser.pk
                //};
                //_db.ShipmentInfo.Add(shipmentNew);
                return false;
            }
            else
            {
                shipment.Weight += cargoLog.ChangeWeight;
                _db.Entry(shipment).Property(c => c.Weight).IsModified = true;
            }

            var cargoAreaCargo = _db.ShipmentHuotInfoes.FirstOrDefault(a => a.HuotId == cargoLog.HuotId && a.ShipmentId == cargoLog.ShipmentId);
            var huot = _db.huot.Find(cargoLog.HuotId);

            if (cargoAreaCargo == null)
            {
                // add new 
                var cargoAreaCargoNew = new ShipmentHuotInfoes();
                cargoAreaCargoNew.HuotId = cargoLog.HuotId;
                cargoAreaCargoNew.ShipmentId = cargoLog.ShipmentId;
                cargoAreaCargoNew.Weight = cargoLog.ChangeWeight;
                cargoAreaCargoNew.HuotName = huot.name;
                cargoAreaCargoNew.ShipmentNo = shipment.ShipmentNo;
                _db.ShipmentHuotInfoes.Add(cargoAreaCargoNew);
            }
            else
            {
                // update weight
                cargoAreaCargo.Weight += cargoLog.ChangeWeight;
            }

            var cargoHuotInfoes = _db.CargoHuotInfoes.FirstOrDefault(a => a.CargoId == cargo.Id && a.HuotId == cargoLog.HuotId);
            if (cargoHuotInfoes == null)
            {
                // add
                var newCargoHuot = new CargoHuotInfoes();
                newCargoHuot.CargoId = cargo.Id;
                newCargoHuot.HuotId = cargoLog.HuotId;
                newCargoHuot.Weight = cargoLog.ChangeWeight;
                newCargoHuot.CargoName = cargo.CargoName;
                newCargoHuot.HuotName = huot.name;
                _db.CargoHuotInfoes.Add(newCargoHuot);
            }
            else
            {
                // update
                if (cargoLog.IsIncome == true)
                {
                    cargoHuotInfoes.Weight += cargoLog.ChangeWeight;
                }
                else
                {
                    cargoHuotInfoes.Weight -= cargoLog.ChangeWeight;
                }
                _db.Entry(cargoHuotInfoes).Property(c => c.Weight).IsModified = true;
            }



            //_db.Entry(cargoLog).Property(c => c.StateId).IsModified = true;
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

        public bool RejectCargoLog(int id)
        {
            var cargoLog = _db.CargoLogInfoes.Find(id);
            cargoLog.Desc = "驳回!!!";
            cargoLog.StateId = 3;
            _db.Entry(cargoLog).Property(c => c.Desc).IsModified = true;
            _db.Entry(cargoLog).Property(c => c.StateId).IsModified = true;
            _db.SaveChanges();
            return true;
        }

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

        public IEnumerable<CargoViewModel> GetCargoLog(int huom2Id, int stateId, DateTime timeStart, DateTime timeEnd, string userId, int? isIncome, int? huotId)
        {
            var user = _db.sys_ry.Find(userId);
            IQueryable<CargoViewModel> list;
            if (IsInOut(user))
            {
                list = (
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
                        ShipmentNo = shipmentInfoes.ShipmentNo,
                        CargoInName = cargoInInfoes.CargoInName,
                        HuotName = huot.name,
                        Time = (DateTime)cargoLogInfoes.InspectTime,
                        Desc = cargoLogInfoes.Desc,
                        IsIncome = (bool)cargoLogInfoes.IsIncome,
                        HuotId = huot.id,
                        InspectName = cargoLogInfoes.InspectName,
                    }
               ).Where(a => a.UserId == userId);
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
                        ShipmentNo = shipmentInfoes.ShipmentNo,
                        CargoInName = cargoInInfoes.CargoInName,
                        HuotName = huot.name,
                        Time = (DateTime)cargoLogInfoes.InspectTime,
                        Desc = cargoLogInfoes.Desc,
                        IsIncome = (bool)cargoLogInfoes.IsIncome,
                        HuotId = huot.id,
                        InspectName = cargoLogInfoes.InspectName,
                    }).Where(a => a.UserId == userId);
            }





            if (stateId != 0)
            {
                list = list.Where(a => a.StateId == stateId);
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

        public bool ExchangeShipment(int cargoId, string shipmentNo, int huotId, int newHuotId, decimal weight, string userId)
        {
            var shipment = _db.ShipmentInfoes.First(a => a.ShipmentNo == shipmentNo);
            var user = _db.sys_ry.Find(userId);
            var cargoAreaCargo = _db.ShipmentHuotInfoes.First(a => a.HuotId == huotId && a.ShipmentId == shipment.Id);
            cargoAreaCargo.Weight -= weight;
            if (cargoAreaCargo.Weight < 0)
            {
                return false;
            }
            var newCargoAreaCargo = _db.ShipmentHuotInfoes.FirstOrDefault(a => a.HuotId == newHuotId && a.ShipmentId == shipment.Id);
            var huot = _db.huot.Find(huotId);
            if (newCargoAreaCargo == null)
            {
                // add
                var newCac = new ShipmentHuotInfoes();
                newCac.HuotId = newHuotId;
                newCac.Weight = weight;
                //newCac.InspectId = LoginUser.pk;
                //newCac.InspectTime = DateTime.Now;
                newCac.ShipmentId = shipment.Id;
                newCac.HuotName = huot.name;
                _db.ShipmentHuotInfoes.Add(newCac);
            }
            else
            {
                newCargoAreaCargo.Weight += weight;
                //newCargoAreaCargo.InspectId = LoginUser.pk;
                //newCargoAreaCargo.InspectTime = DateTime.Now;
            }

            //var cargoLogList = new List<CargoLogInfo>();
            var cargoLog = new CargoLogInfoes();
            cargoLog.IsIncome = false;
            cargoLog.ChangeWeight = weight;
            cargoLog.CargoId = cargoId;
            cargoLog.HuotId = huotId;
            cargoLog.ShipmentId = shipment.Id;
            cargoLog.StateId = 3;
            cargoLog.InspectName = user.name;
            cargoLog.InspectTime = DateTime.Now;
            cargoLog.Desc = "转库";
            _db.CargoLogInfoes.Add(cargoLog);
            _db.SaveChanges();

            var cargoLog2 = new CargoLogInfoes();
            cargoLog2.IsIncome = true;
            cargoLog2.ChangeWeight = weight;
            cargoLog2.CargoId = cargoId;
            cargoLog2.HuotId = newHuotId;
            cargoLog2.ShipmentId = shipment.Id;
            cargoLog2.StateId = 3;
            cargoLog2.InspectName = user.name;
            cargoLog2.InspectTime = DateTime.Now;
            cargoLog2.Desc = "转库";
            _db.CargoLogInfoes.Add(cargoLog2);

            return true;
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

            var cargoLog = new CargoLogInfoes()
            {
                IsIncome = false,
                InspectTime = DateTime.Now,
                StateId = 2,
                CargoId = cargo.Id,
                ChangeWeight = (decimal)shipmentView.PH_ZL,
                ShipmentId = shipment.Id,
                CargoInId = 1,
                HuotId = shipmentHuot.HuotId,
                InspectName = shipmentView.qianfa,
                Desc = ""
            };
            _db.CargoLogInfoes.Add(cargoLog);

            _db.SaveChanges();
            _dbd.SaveChanges();
            return true;
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

        private bool IsInOut(sys_ry user)
        {
            if (user.lx == _lxId || user.lx == _lxId2)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public IEnumerable<CargoViewModel> GetCargoOutShipment(int huotId, string loginUserId)
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
                where sys_ry.pk == loginUserId && (sys_ry.lx == _lxId || sys_ry.lx == _lxId2) //&& sys_ry.htid == huotId
                select new CargoViewModel
                {
                    cargoOutOrderId = shipmentView.ID,
                    HuotId = (int)shipmentView.HuotId,
                    ShipmentNo = shipmentView.PH_NO,
                    IsChecked = shipmentView.IsChecked == true,
                    WeightFlag = cheh.weight_flag,
                    HuotName = huot.name,
                    CargoName = huom2.name,
                    TopCargoName = huom.name,
                    WeightDu = (double)shipmentView.PH_ZL,
                    MissDuTime = shipmentView.sj
                });
            var list = temp.ToList();
            return list;
        }
    }
}
