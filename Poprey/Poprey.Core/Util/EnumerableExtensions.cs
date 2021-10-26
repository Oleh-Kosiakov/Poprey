using System;
using System.Collections.Generic;
using System.Linq;

namespace Poprey.Core.Util
{
    public static class EnumerableExtensions
    {
        public static int NearestTo(this IEnumerable<int> list, int number)
        {
            var minimumDifference = int.MaxValue;
            var minimumDifferenceValue = 0;

            foreach (var i in list)
            {
                var currentAbsDiff = Math.Abs(number - i);
                if (currentAbsDiff < minimumDifference)
                {
                    minimumDifference = currentAbsDiff;
                    minimumDifferenceValue = i;
                }
            }

            return minimumDifferenceValue;
        }

        public static int GetNextTo(this IEnumerable<int> list, int nextTo)
        {
            return list.SkipWhile(i => i != nextTo).Skip(1).First();
        }
        public static bool HasNextTo(this IEnumerable<int> list, int nextTo)
        {
            return list.SkipWhile(i => i != nextTo).Skip(1).Any();
        }

        public static bool TryGetNextTo(this IEnumerable<int> list, int nextTo, out int next)
        {
            var tempList = list.ToList();

            var hasNext = tempList.ToList().SkipWhile(i => i != nextTo).Skip(1).Any();

            if (!hasNext)
            {
                next = default(int);
                return false;
            }

            next = tempList.SkipWhile(i => i != nextTo).Skip(1).First();
            return true;
        }
    }
}