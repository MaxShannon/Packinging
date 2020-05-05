using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Packing.Models
{
    public class FlowLineViewModel// :FlowLineInfo
    {

        [Display(Name = "流程名称")]
        public string FlowName { get; set; }

        [Display(Name = "上一个流程")]
        public string PreFlowNodeName { get; set; }

        [Display(Name = "当前流程")]
        public string FlowNodeName { get; set; }

        [Display(Name = "下一个流程")]
        public string NextFlowNodeName { get; set; }
    }
}