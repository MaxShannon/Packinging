using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Packing.Models
{
    public class FlowNodeViewModel// : FlowNodeInfo
    {

        [Display(Name = "流程名称")]
        public string FlowName { get; set; }

    }
}