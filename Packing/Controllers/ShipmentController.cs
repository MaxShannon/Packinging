using DbEfModel;
using Services;
using System.Linq;
using System.Web.Mvc;

namespace Packing.Controllers
{
    public class ShipmentController : BaseController
    {
        private readonly jhglEntities _db = new jhglEntities();
        private readonly CargoService _cargoService;
        private readonly ShipmentService _shipmentService;
        private readonly HuotService _huotService;
        private readonly Huom2Service _huom2Service;

        public ShipmentController()
        {
            CargoService cargoService = new CargoService();
            _cargoService = cargoService;
            _shipmentService = new ShipmentService();
            _huotService = new HuotService();
            _huom2Service = new Huom2Service();
        }
        public ActionResult Index(int huotId, int cargoId) // id = areaId
        {
            CargoViewModel cargo = new CargoViewModel();
            cargo.HuotId = huotId;
            cargo.CargoId = cargoId;

            return View(cargo);
        }

        [HttpPost]
        public ActionResult Index(CargoViewModel cargoViewModel, int limit, int offset)
        {
            var temp = _cargoService.GetCargoShipmentHuot(cargoViewModel.HuotId, cargoViewModel.CargoId, LoginUser.pk);

            var list = temp.OrderByDescending(c => c.CargoId).ToList();
            return Json(new { total = list.Count, rows = list.Skip(offset).Take(limit).ToList() });
        }

        public ActionResult Exchange(string shipmentNo, int? huotId, int? cargoId)
        {
            var huots = _huotService.GetCargoHuots(shipmentNo);
            ViewBag.HuotName = new SelectList(huots, "id", "name", 1);
            CargoViewModel cargoViewModel = new CargoViewModel();
            cargoViewModel.ShipmentNo = shipmentNo;
            var shipment = _shipmentService.GetShipmentByNo(shipmentNo);
            cargoViewModel.HuotId = huotId ?? 0;
            cargoViewModel.CargoId = cargoId ?? 0;
            cargoViewModel.CargoName = shipment.CargoName;
            return View(cargoViewModel);
        }

        [HttpPost]
        public ActionResult Exchange(CargoViewModel cargoInfo)
        {
            var huotId = cargoInfo.HuotId;
            var newHuotIdId = cargoInfo.NewHuotId;
            var cargoId = cargoInfo.CargoId;
            var weight = cargoInfo.Weight;
            var shipmentNo = cargoInfo.ShipmentNo;
            if (huotId == newHuotIdId)
            {
                return Json(new { success = "fail", message = "新旧货台都一样..." });
            }
            if (newHuotIdId == 0)
            {
                return Json(new { success = "fail", message = "失败,只有这一个货台" });
            }

            if (weight == 0)
            {
                return Json(new { success = "fail", message = "重量为0,没啥意思" });
            }
            var ok = _cargoService.ExchangeShipment(shipmentNo, huotId, newHuotIdId, weight, LoginUser.pk);

            return ok ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "失败" });
        }

        [HttpGet]
        public ActionResult Quality(string shipmentNo, int? huotId, int? cargoId)
        {
            var huom2S = _huom2Service.GetChangeQualityHuoms(shipmentNo);
            ViewBag.HuomName = new SelectList(huom2S, "id", "name", 1);

            CargoViewModel cargoViewModel = new CargoViewModel();
            cargoViewModel.ShipmentNo = shipmentNo;
            var shipment = _shipmentService.GetShipmentByNo(shipmentNo);
            cargoViewModel.HuotId = huotId ?? 0;
            cargoViewModel.CargoId = cargoId ?? 0;
            cargoViewModel.CargoName = shipment.CargoName;
            return View(cargoViewModel);
        }

        [HttpPost]
        public ActionResult Quality(CargoViewModel cargoInfo)
        {
            var huotId = cargoInfo.HuotId;
            var newHuotIdId = cargoInfo.NewHuotId;
            var cargoId = cargoInfo.CargoId;
            var huom2Id = cargoInfo.Huom2Id;
            var weight = cargoInfo.Weight;
            var shipmentNo = cargoInfo.ShipmentNo;

            var ok = _shipmentService.ChangeQuality(cargoInfo.ShipmentNo, huotId,cargoInfo.Huom2Id, LoginUser.pk);

            return ok ? Json(new { success = "success", message = "成功" }) : Json(new { success = "fail", message = "失败" });
        }
    }
}