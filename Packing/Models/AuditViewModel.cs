using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Packing.Models;

namespace Packing.Models
{
    public class AuditViewModel// : AuditInfo
    {

        public int Id { get; set; }
        public int ProductOrderId { get; set; }
        public int FlowNodeId { get; set; }
        [Display(Name = "意见")]
        public string Info { get; set; }
        public int InspectId { get; set; }
        public System.DateTime InspectTime { get; set; }


        [Display(Name = "流程名称")]
        public string FlowName { get; set; }

        public int FlowId { get; set; }
        [Display(Name = "名称")]
        public string FlowNodeName { get; set; }
        public int? DeptId { get; set; }

        public bool Pass { get; set; }

        public int CargoLogId { get; set; }
        [Display(Name = "产品")]
        public string CargoName { get; set; }

        [Display(Name = "改变重量")]
        public decimal ChangeWeight { get; set; }
        [Display(Name = "批号")]
        public decimal ShipmentNo { get; set; }
        public int CargoAreaId { get; set; }
        [Display(Name = "仓库")]
        public string CargoAreaName { get; set; }
    }

}