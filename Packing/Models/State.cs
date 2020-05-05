using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Packing.Models
{
    public class State
    {
        public int Id { get; set; }
        public string StateName { get; set; }
    }

    public class StateCargo
    {
        public int Id { get; set; }
        public string StateCargoName { get; set; }
    }

    public class States
    {
        public List<State> StateList { get; set; }
        public List<StateCargo> StateCargoList { get; set; }

        public States()
        {
            StateList = new List<State>
            {
                new State()
                {
                    Id = 1,
                    StateName = "要审核"
                },
                new State()
                {
                    Id = 2,
                    StateName = "通过"
                },
                new State()
                {
                    Id = 3,
                    StateName = "驳回"
                },
                //new State()
                //{
                //    Id = 4,
                //    StateName = "已驳回"
                //}
            };

            StateCargoList = new List<StateCargo>
            {
                new StateCargo()
                {
                    Id = 1,
                    StateCargoName = "出入库"
                },
                new StateCargo()
                {
                    Id = 2,
                    StateCargoName = "出库"
                },
                new StateCargo()
                {
                    Id = 3,
                    StateCargoName = "入库"
                }
            };
        }
    }
}