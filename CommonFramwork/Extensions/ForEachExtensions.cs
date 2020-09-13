using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonFramwork.Extensions
{
    public static class ForEachExtensions
    {
        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var i in collection)
            {
                action(i);
            }

            return collection;
        }
    }
}
