using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutomationCLogic.Extensions
{
    public static class EnumExtensions
    {
        public static T ParseEnum<T>(string value) where T : struct
        {
            if (string.IsNullOrEmpty(value))
            {
                value = "NONE";
            }
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}
