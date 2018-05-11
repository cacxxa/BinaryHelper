using System.Collections.Generic;
using System.Linq;

namespace BinaryHelper
{
    static class Extensions
    {
        public static void Skip(this IEnumerator<byte> data, int count)
        {
            while (count > 0 && data.MoveNext()) count--;
        }

        public static IEnumerable<byte> Take(this IEnumerator<byte> data, int count)
        {
            while (count > 0 && data.MoveNext())
            {
                yield return data.Current;
                if (--count == 0) yield break;
            }

            yield return 0;
        }

        public static byte[] ToArray(this IEnumerator<byte> data, int count, bool reverse = false, int typeSize = 0)
        {
            int len = typeSize > 0 ? typeSize : count;

            byte[] result = new byte[len];
            List<byte> result2 = null;

            if (count == 0)
            {
                count = int.MaxValue;
                result2 = new List<byte>();
            }

            for (int i = 0; i < count; i++)
            {
                if (!data.MoveNext())
                {
                    break;
                }

                if (result2 != null)
                {
                    if (reverse) result2.Prepend(data.Current);
                    else result2.Add(data.Current);
                }
                else
                {
                    if (i >= len) continue;
                    if (reverse) result[count - 1 - i] = data.Current;
                    else result[i] = data.Current;
                }
            }

            return result2 != null ? result2.ToArray() : result;
        }
    }
}