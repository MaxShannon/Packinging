using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;

namespace Packing.Models
{
    public class ProductionOrderViewModel
    {
        [Display(Name = "流程")]
        public string FlowNodeName { get; set; }

        public int Id { get; set; }
        public int? Count { get; set; }
        public string Remark { get; set; }
        public int InspectId { get; set; }
        public DateTime? InspectTime { get; set; }
        public int FlowId { get; set; }
        public int CurrentNodeId { get; set; }

        public int CargoLogId { get; set; }
        [Display(Name = "产品")]
        public string CargoName { get; set; }

        [Display(Name = "改变重量")]
        public decimal ChangeWeight { get; set; }
        [Display(Name = "批号")]
        public string ShipmentNo { get; set; }

        public bool Pass { get; set; }
        public bool IsOk { get; set; }

        [Display(Name = "开始时间")]
        public DateTime TimeStart { get; set; }
        [Display(Name = "开始时间")]
        public DateTime TimeEnd { get; set; }

        public int StateId { get; set; }

        public int CargoOutOrderId { get; set; }
        public bool IsChecked { get; set; }
    }
}