namespace Blackcat.Utils
{
    public static class Checksum
    {
        public static byte[] ComputeCRC16(byte[] bytes, int offset, int length)
        {
            ushort CRCFull = 0xFFFF;
            char CRCLSB;

            for (var i = 0; i < length; i++)
            {
                CRCFull = (ushort)(CRCFull ^ bytes[offset + i]);

                for (int j = 0; j < 8; j++)
                {
                    CRCLSB = (char)(CRCFull & 0x0001);
                    CRCFull = (ushort)((CRCFull >> 1) & 0x7FFF);

                    if (CRCLSB == 1)
                        CRCFull = (ushort)(CRCFull ^ 0xA001);
                }
            }

            var crc16 = new byte[2];
            crc16[1] = (byte)((CRCFull >> 8) & 0xFF);
            crc16[0] = (byte)(CRCFull & 0xFF);
            return crc16;
        }
    }
}