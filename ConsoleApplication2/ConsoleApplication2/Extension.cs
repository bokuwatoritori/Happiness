using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApplication2
{
    static class Extension
    {
        private static T[] flatten<T>(IEnumerable<T[]> dic)
        {
            return dic.Aggregate((i, j) => i.ToList().Concat(j).ToArray());
        }

        public static T[] Flatten<T>(this IEnumerable<T[]> Container)
        {
            return flatten(Container);
        }
        public static T[] Flatten<Y, T>(this IDictionary<Y, T[]> Container)
        {
            return flatten(Container.OrderBy(i => i.Key).Select(i => i.Value)); //
        }
    }
}
