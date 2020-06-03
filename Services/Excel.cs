using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using DbEfModel;

namespace Services
{
    public class Excel
    {
       


      
    }

    public class AssembleLogWeight
    {
        public int CatId { get; set; }
        public decimal Weight { get; set; }

        public AssembleLogWeight()
        {
            Weight = 0;
        }
    }

    public class AssembleLogExcel
    {
        public string 产品名称 { get; set; }
        public string 出入库 { get; set; }
        public decimal 重量 { get; set; }
        public string 批号 { get; set; }
        public string 方式 { get; set; }
        public string 仓库 { get; set; }
        public string 描述 { get; set; }
        public string 录入人 { get; set; }
        public DateTime? 录入时间 { get; set; }


    }


}