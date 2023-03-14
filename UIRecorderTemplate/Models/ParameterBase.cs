using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationCLogic.Enums;

namespace AutomationCLogic.Models
{
    public class ParameterBase
    {
        public ActionType ActionType { get; set; }
        public object Source { get; set; }
        public object Value { get; set; }
    }
}
