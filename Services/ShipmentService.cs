using System;
using System.Linq;
using System.Net.WebSockets;
using DbEfModel;

namespace Services
{
    public class ShipmentService : BaseService
    {
        private readonly CargoService _cargoService;
        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();

        public ShipmentService()
        {
            _cargoService = new CargoService();
        }
        public ShipmentInfoes GetShipmentByNo(string shipmentNo)
        {
            return _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == shipmentNo);
        }

        public bool ChangeQuality(string shipmentNo, int huotId, int newHuom2Id, string userId)
        {
            var user = _db.sys_ry.Find(userId);
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == shipmentNo);
            var cargo = _db.CargoInfoes.Find(shipment.CargoId);
            var cargoHuot = _db.CargoHuotInfoes.FirstOrDefault(a => a.CargoId == cargo.Id && a.HuotId == huotId);
            var shipmentHuot = _db.ShipmentHuotInfoes.FirstOrDefault(a => a.ShipmentId == shipment.Id && a.HuotId == huotId);
            var weight = shipment.Weight;

            // cargo 减库存 加库存
            cargo.Weight -= weight;
            _cargoService.CheckCargo(newHuom2Id);
            var newCargo = _db.CargoInfoes.First(a => a.Huom2Id == newHuom2Id);
            newCargo.Weight += weight;

            // shipment 减库存 加库存
            shipment.Weight -= weight;
            _db.ShipmentInfoes.Remove(shipment);
            _cargoService.CheckShipment(shipmentNo, cargo.Huom2Id);
            var newShipment = _db.ShipmentInfoes.First(a => a.ShipmentNo == shipmentNo && a.CargoId == newCargo.Id);
            newShipment.Weight += weight;

            // cargoHuot 减库存 加库存
            cargoHuot.Weight -= weight;
            _cargoService.CheckCargoHuot(newCargo.Id, huotId);
            var newCargoHuot = _db.CargoHuotInfoes.First(a => a.CargoId == newCargo.Id && a.HuotId == huotId);
            newCargoHuot.Weight += weight;

            // ShipmentHuot 减库存 加库存
            shipmentHuot.Weight -= weight;
            _cargoService.CheckShipmentHuot(newShipment.Id, huotId);
            var newShipmentHuot = _db.ShipmentHuotInfoes.First(a => a.ShipmentId == newShipment.Id && a.HuotId == huotId);
            newShipmentHuot.Weight += weight;
            // CargoLog 加

            var cargoLog = new CargoLogInfoes()
            {
                ChangeWeight = weight,
                IsIncome = false,
                InspectTime = DateTime.Now,
                InspectName = user.name,
                StateId = 2,
                Desc = "调整品质出库",
                CargoId = shipment.CargoId,
                ShipmentId = shipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                HuotId = huotId,
                Count = 0,
                WeightType = ""
            };
            var cargoLog2 = new CargoLogInfoes()
            {
                ChangeWeight = weight,
                IsIncome = true,
                InspectTime = DateTime.Now,
                InspectName = user.name,
                StateId = 2,
                Desc = "调整品质入库",
                CargoId = newCargo.Id,
                ShipmentId = newShipment.Id,
                ShipmentNo = shipment.ShipmentNo,
                HuotId = huotId,
                Count = 0,
                WeightType = ""
            };
            _db.CargoLogInfoes.Add(cargoLog);
            _db.CargoLogInfoes.Add(cargoLog2);
            return _db.SaveChanges() > 0 && _dbd.SaveChanges() == 1;
        }
    }
}