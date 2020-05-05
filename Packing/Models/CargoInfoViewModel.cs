using System;
using System.ComponentModel.DataAnnotations;

namespace Packing.Models
{
    public class CargoInfoViewModel
    {
        [Display(Name = "产品编号")]
        public int CargoId { get; set; }

        [Display(Name = "产品记录编号")]
        public int CargoLogId { get; set; }

        [Display(Name = "产品名称")]
        public string CargoName { get; set; }

        public string LastCargoName { get; set; }

        [Display(Name = "产品数量")]
        public int CargoCount { get; set; }

        [Display(Name = "单价")]
        public decimal? Amount { get; set; }

        [Display(Name = "改变数量")]
        //[StringLength(100, ErrorMessage = "{0} 必须至少包含 {2} 个字符。", MinimumLength = 3)]
        //[Compare("0", ErrorMessage = "数量必须大于0")]
        public decimal ChangeCount { get; set; }

        [Display(Name = "库存改变总数")]
        public int ChangeAllCount { get; set; }

        [Display(Name = "是否已经删除")]
        public bool DelFlag { get; set; }

        [Display(Name = "可以删除")]
        public bool CanDel { get; set; }

        [Display(Name = "时间")]
        public DateTime Time { get; set; }

        [Display(Name = "开始时间")]
        public DateTime TimeStart { get; set; }

        [Display(Name = "结束时间")]
        public DateTime TimeEnd { get; set; }

        [Display(Name = "是否入库")]
        public bool IsIncome { get; set; }

        [Display(Name = "项目号")]
        public int? ProjectId { get; set; }

        [Display(Name = "项目名")]
        public string ProjectName { get; set; }

        [Display(Name = "供应商号")]
        public int? SupplyId { get; set; }

        [Display(Name = "供应商名")]
        public string SupplyName { get; set; }

        [Display(Name = "备注")]
        public string Desc { get; set; }

        [Display(Name = "规格")]
        public string Specifications { get; set; }

        [Display(Name = "批号")]
        public string Type { get; set; }

        [Display(Name = "领用人员")]
        public string TakenName { get; set; }

        [Display(Name = "仓库")]
        public string Area { get; set; }

        [Display(Name = "吨/袋")]
        public string Unit { get; set; }

        [Display(Name = "吨数")]
        public decimal Weight { get; set; }

        [Display(Name = "方式")]
        public int CargoInId { get; set; }

        [Display(Name = "方式")]
        public string CargoInName { get; set; }

        [Display(Name = "产品大类")]
        public int CatId { get; set; }

        [Display(Name = "产品大类")]
        public string CatIdName { get; set; }

        [Display(Name = "仓库")]
        public int CargoAreaId { get; set; }
        [Display(Name = "仓库")]
        public string CargoAreaName { get; set; }

        [Display(Name = "批号")]
        public string ShipmentNo { get; set; }
        [Display(Name = "批号")]
        public int ShipmentId { get; set; }

        [Display(Name = "改变重量")]
        public int ChangeWeight { get; set; }
        [Display(Name = "状态")]
        public int StateId { get; set; }

        [Display(Name = "批号")]
        public string NewShipmentNo { get; set; }
        [Display(Name = "新仓库")]
        public int NewCargoAreaId { get; set; }

        public bool IsOk { get; set; }

    }
}