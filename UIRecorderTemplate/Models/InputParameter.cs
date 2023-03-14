using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutomationCLogic.Enums;

namespace AutomationCLogic.Models
{
    public class InputParameter : ParameterBase
    {
        public InputSourceType SourceType { get; set; }
    }
}
