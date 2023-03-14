using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationCLogic.Enums;

namespace AutomationCLogic.Models
{
    public class ExpectedParameter : ParameterBase
    {
        public ExpectedSourceParamType ExpectedSourceParam1 { get; set; }
        public PropertyType ExpectedSourceParam2 { get; set; }
    }
}
