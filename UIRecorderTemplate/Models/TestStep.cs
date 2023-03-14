using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationCLogic.Models
{
    public class TestCase : List<TestStep>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    
    public class TestStep
    {
        public int Step { get; set; }
        public string Description { get; set; }
        public InputParameter InputParameter { get; set; }
        public ExpectedParameter ExpectedParameter { get; set; }
        public object ActualValue { get; set; }
        public bool IsPassed { get; set; }
        public string Note { get; set; }
    }
}
