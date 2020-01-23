namespace SkunkLab.Modbus.Messaging
{
    public class Crc
    {       

        public static byte[] Compute(byte[] buffer)
        {
            ushort crc = 0xFFFF;

            for (int pos = 0; pos < buffer.Length; pos++)
            {
                crc ^= (ushort)buffer[pos];          // XOR byte into least sig. byte of crc

                for (ushort i = 8; i != 0; i--)
                {    // Loop over each bit
                    if ((crc & 0x01) != 0)
                    {      // If the LSB is set
                        crc >>= 1;                 // Shift right and XOR 0xA001
                        crc ^= 0xA001;
                    }
                    else                            // Else LSB is not set
                        crc >>= 1;                    // Just shift right
                }
            }
           
            return new byte[2] { (byte)(crc & 0x00FF), (byte)((crc >> 8) & 0x00FF) };
        }
    }
}
