using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DbEfModel;

namespace Services
{
    public class Huom2Service : BaseService
    {
        private readonly jhglEntities _db = new jhglEntities();
        private readonly ZDCZ_SLEntities _dbd = new ZDCZ_SLEntities();

        public Huom2Service()
        {

        }

        public huom2 GetHuom2(int id)
        {
            return _db.huom2.Find(id);
        }

        public IEnumerable<huom2> GetChangeQualityHuoms(string shipmentNo)
        {
            var shipment = _db.ShipmentInfoes.FirstOrDefault(a => a.ShipmentNo == shipmentNo);
            var cargo = _db.CargoInfoes.Find(shipment.CargoId);
            var huom2 = _db.huom2.Find(cargo.Huom2Id);
            var huom2s = _db.huom2.Where(a => a.huomid == huom2.huomid && a.sfyx == 1);
            return huom2s.ToList();
        }
    }
}