using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model.Utils
{
    public class ParallelExecutor
    {
        public static List<R> DoWork<T, R>(int range, Func<int, T> getItem, Func<T, R> action)
        {
            var itemCount = range;
            var itemsPerThread = itemCount / Environment.ProcessorCount;
            
            var tasks = new List<Task<List<R>>>();

            for (int p = 0; p < Environment.ProcessorCount; p++)
            {
                var p1 = p;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var lastItemNdx = p1 == Environment.ProcessorCount
                        ? itemCount
                        : (p1 * itemsPerThread + itemsPerThread);

                    var res = new List<R>();

                    for (var i = p1 * itemsPerThread; i < lastItemNdx; i++)
                    {
                        res.Add(action(getItem(i)));
                    }

                    return res;
                }));
            }

            Task.WaitAll(tasks.ToArray());

            var result = new List<R>();

            foreach (var task in tasks)
            {
                result.AddRange(task.Result);
            }

            return result;
        }

        public static void DoWork<T>(int range, Func<int, T> getItem, Action<int, T> action)
        {
            var itemCount = range;
            var itemsPerThread = itemCount / Environment.ProcessorCount;

            var tasks = new List<Task>();

            for (int p = 0; p < Environment.ProcessorCount; p++)
            {
                var p1 = p;
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    var lastItemNdx = p1 == Environment.ProcessorCount
                        ? itemCount
                        : (p1 * itemsPerThread + itemsPerThread);

                  

                    for (var i = p1 * itemsPerThread; i < lastItemNdx; i++)
                    {
                        action(i, getItem(i));
                    }
                }));
            }

            Task.WaitAll(tasks.ToArray());
        }
    }
}
