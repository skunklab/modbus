using System.Collections;
using System.Collections.Generic;

namespace SkunkLab.Modbus.Messaging
{
    public static class BitArrayExtensions
    {
        public static bool[] BitArrayToBoolArray(this BitArray bits)
        {
            List<bool> list = new();
            for (int i = 0; i < bits.Length; i++)
                list.Add(bits.Get(i));

            return list.ToArray();
        }
    }
}
