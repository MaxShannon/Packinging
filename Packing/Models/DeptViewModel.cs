using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Packing.Models
{
    public class DeptViewModel //: DeptInfo
    {
        public int Id { get; set; }
        [Display(Name = "部门")]
        public string DeptName { get; set; }
        public Nullable<int> ActionId { get; set; }
    }
}