using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public static class General
    {
        public static string ByteArrToStr(this IEnumerable<byte> input)
        {
            var arr = input.ToArray();
            if (!BitConverter.IsLittleEndian)
            {
                Array.Reverse(arr);
            }
            return "0x" + BitConverter.ToString(arr).Replace("-", "");
        }
    }
}
