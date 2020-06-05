using System.Collections.Generic;
using System.Linq;
using DbEfModel;

namespace Services
{
    public class HuotService : BaseService
    {
        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();

        public List<huot> GetCargoHuots(string shipmentNo)
        {
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == shipmentNo);
            var cargo = _db.CargoInfoes.Find(shipment.CargoId);
            var huom2 = _db.huom2.Find(cargo.Huom2Id);
            var lists = _db.HuotHuomInfoes.Where(a => a.HuomId == huom2.huomid).ToList();
            var list = lists.Select(a => a.HuotId).ToList();
            var tt = from a in _db.huot where list.Contains(a.id) select a;
            return tt.Where(a=>a.sfyx == 1).ToList();
        }
    }
}