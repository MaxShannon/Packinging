using System;
using System.Collections.Generic;
using System.Linq;
using DbEfModel;

namespace Services
{
    public class CargoLogService : BaseService
    {

        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();

        public CargoLogService()
        {

        }

        public List<CargoViewModel> GetCargoLogList(int huom2Id, int stateId, DateTime timeStart, DateTime timeEnd, string userId, int? isIncome, int? huotId)
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
                        ShipmentNo = cargoLogInfoes.ShipmentNo,
                        CargoInName = cargoInInfoes.CargoInName,
                        HuotName = huot.name,
                        Time = (DateTime)cargoLogInfoes.InspectTime,
                        Desc = cargoLogInfoes.Desc,
                        IsIncome = (bool)cargoLogInfoes.IsIncome,
                        HuotId = huot.id,
                        InspectName = cargoLogInfoes.InspectName,
                        IsChecked = cargoLogInfoes.StateId == (int)StateEnum.通过,
                        HuomId = huom.id,
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
                        ShipmentNo = cargoLogInfoes.ShipmentNo,
                        CargoInName = cargoInInfoes.CargoInName,
                        HuotName = huot.name,
                        Time = (DateTime)cargoLogInfoes.InspectTime,
                        Desc = cargoLogInfoes.Desc,
                        IsIncome = (bool)cargoLogInfoes.IsIncome,
                        HuotId = huot.id,
                        InspectName = cargoLogInfoes.InspectName,
                        IsChecked = cargoLogInfoes.StateId == (int)StateEnum.通过,
                        HuomId = huom.id,
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


        public bool PassCargoLog(int id)
        {
            var cargoLog = _db.CargoLogInfoes.Find(id);

            cargoLog.Desc = "审核通过";
            cargoLog.StateId = 2;

            return _db.SaveChanges() > 0;

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
    }
}