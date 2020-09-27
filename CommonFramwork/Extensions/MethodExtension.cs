using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CommonFramework.Extensions
{
    public static class MethodExtension
    {
        /// <summary>
        /// 打印方法调用堆栈
        /// </summary>
        /// <param name="methodBase"></param>
        /// <returns></returns>
        public static string GetFullCallStack(this MethodBase methodBase)
        {
            var stackTrace = new System.Diagnostics.StackTrace(true);
            Console.WriteLine("------------------------------------------------------------------");
            Console.WriteLine("StackTrace ToString Information:" + stackTrace.ToString());
            var stackTraces = stackTrace.GetFrames();
            return string.Join(" <-- ", stackTraces.Select(a => a.GetMethod().Name));
        }
    }

    public class MethodHelper
    {
        public static MethodBase GetCurrentMethod()
        {
            return MethodBase.GetCurrentMethod();
        }
    }
}
