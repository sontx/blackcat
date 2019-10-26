using Newtonsoft.Json;
using System;
using System.Text;

namespace Blackcat.Types
{
    public static class BytesUtils
    {
        public static string ToHexString(this byte[] bytes, int offset, int length, char split = ' ')
        {
            return BitConverter.ToString(bytes, offset, length).Replace('-', split);
        }

        public static string ToHexString(this byte[] bytes, char split = ' ')
        {
            return bytes.ToHexString(0, bytes.Length, split);
        }

        public static int ToInt32(this byte[] bytes, int offset = 0, bool srcIsLittleEndian = false)
        {
            return bytes.ToNumber(offset, srcIsLittleEndian, 4, BitConverter.ToInt32);
        }

        public static uint ToUInt32(this byte[] bytes, int offset = 0, bool srcIsLittleEndian = false)
        {
            return bytes.ToNumber(offset, srcIsLittleEndian, 4, BitConverter.ToUInt32);
        }

        public static short ToInt16(this byte[] bytes, int offset = 0, bool srcIsLittleEndian = false)
        {
            return bytes.ToNumber(offset, srcIsLittleEndian, 2, BitConverter.ToInt16);
        }

        public static ushort ToUInt16(this byte[] bytes, int offset = 0, bool srcIsLittleEndian = false)
        {
            return bytes.ToNumber(offset, srcIsLittleEndian, 2, BitConverter.ToUInt16);
        }

        public static long ToInt64(this byte[] bytes, int offset = 0, bool srcIsLittleEndian = false)
        {
            return bytes.ToNumber(offset, srcIsLittleEndian, 8, BitConverter.ToInt64);
        }

        public static ulong ToUInt64(this byte[] bytes, int offset = 0, bool srcIsLittleEndian = false)
        {
            return bytes.ToNumber(offset, srcIsLittleEndian, 8, BitConverter.ToUInt64);
        }

        public static float ToFloat32(this byte[] bytes, int offset = 0, bool srcIsLittleEndian = false)
        {
            return bytes.ToNumber(offset, srcIsLittleEndian, 4, BitConverter.ToSingle);
        }

        public static double ToFloat64(this byte[] bytes, int offset = 0, bool srcIsLittleEndian = false)
        {
            return bytes.ToNumber(offset, srcIsLittleEndian, 8, BitConverter.ToDouble);
        }

        public static T ToNumber<T>(this byte[] bytes, int offset, bool srcIsLittleEndian, int byteCount, Func<byte[], int, T> convertFunc)
        {
            if (srcIsLittleEndian == BitConverter.IsLittleEndian)
                return convertFunc(bytes, offset);

            var temp = new byte[byteCount];
            Array.Copy(bytes, offset, temp, 0, temp.Length);
            Array.Reverse(temp);
            return convertFunc(temp, 0);
        }

        public static byte[] ToBytes(this int value, bool destIsLittleEndian = false)
        {
            return ToBytes<int>(value, destIsLittleEndian);
        }

        public static byte[] ToBytes(this uint value, bool destIsLittleEndian = false)
        {
            return ToBytes<uint>(value, destIsLittleEndian);
        }

        public static byte[] ToBytes(this short value, bool destIsLittleEndian = false)
        {
            return ToBytes<short>(value, destIsLittleEndian);
        }

        public static byte[] ToBytes(this ushort value, bool destIsLittleEndian = false)
        {
            return ToBytes<ushort>(value, destIsLittleEndian);
        }

        public static byte[] ToBytes(this long value, bool destIsLittleEndian = false)
        {
            return ToBytes<long>(value, destIsLittleEndian);
        }

        public static byte[] ToBytes(this ulong value, bool destIsLittleEndian = false)
        {
            return ToBytes<ulong>(value, destIsLittleEndian);
        }

        public static byte[] ToBytes(this float value, bool destIsLittleEndian = false)
        {
            return ToBytes<float>(value, destIsLittleEndian);
        }

        public static byte[] ToBytes(this double value, bool destIsLittleEndian = false)
        {
            return ToBytes<double>(value, destIsLittleEndian);
        }

        public static byte[] ToBytes<T>(this T value, bool destIsLittleEndian = false)
        {
            byte[] bytes = null;

            if (value is int intValue)
                bytes = BitConverter.GetBytes(intValue);
            else if (value is uint uintValue)
                bytes = BitConverter.GetBytes(uintValue);
            else if (value is short shortValue)
                bytes = BitConverter.GetBytes(shortValue);
            else if (value is ushort ushortValue)
                bytes = BitConverter.GetBytes(ushortValue);
            else if (value is long longValue)
                bytes = BitConverter.GetBytes(longValue);
            else if (value is ulong ulongValue)
                bytes = BitConverter.GetBytes(ulongValue);
            else if (value is float floatValue)
                bytes = BitConverter.GetBytes(floatValue);
            else if (value is double doubleValue)
                bytes = BitConverter.GetBytes(doubleValue);

            if (bytes == null)
            {
                var json = JsonConvert.SerializeObject(value);
                bytes = Encoding.UTF8.GetBytes(json);
            }

            if (destIsLittleEndian != BitConverter.IsLittleEndian)
                Array.Reverse(bytes);

            return bytes;
        }

        public static byte[] CopyRange(this byte[] bytes, int offset, int length)
        {
            var ret = new byte[length];
            Array.Copy(bytes, offset, ret, 0, length);
            return ret;
        }

        public static byte[] Copy(this byte[] bytes, int requiredLength, byte fillMissing)
        {
            byte[] dest = new byte[requiredLength <= 0 ? bytes.Length : requiredLength];
            Fill(dest, fillMissing);
            Array.Copy(bytes, 0, dest, 0, Math.Min(dest.Length, bytes.Length));
            return dest;
        }

        public static byte[] Copy(this byte[] bytes, int requiredLength)
        {
            return Copy(bytes, requiredLength, 0);
        }

        public static byte[] Copy(this byte[] bytes)
        {
            return Copy(bytes, -1);
        }

        public static byte[] Fill(this byte[] bytes, int offset, int length, byte fillValue)
        {
            for (var i = 0; i < length; i++)
            {
                bytes[i + offset] = fillValue;
            }
            return bytes;
        }

        public static byte[] Fill(this byte[] bytes, byte fillValue)
        {
            return Fill(bytes, 0, bytes.Length, fillValue);
        }

        public static byte[] CombineWith(this byte[] bytes1, byte[] bytes2)
        {
            var combined = new byte[bytes1.Length + bytes2.Length];
            Array.Copy(bytes1, 0, combined, 0, bytes1.Length);
            Array.Copy(bytes2, 0, combined, bytes1.Length, bytes2.Length);
            return combined;
        }

        public static byte[] EnsureBitEndian(this byte[] bytes)
        {
            var copied = Copy(bytes);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(copied);
            return copied;
        }

        public static byte[] EnsureLittleEndian(this byte[] bytes)
        {
            var copied = Copy(bytes);
            if (!BitConverter.IsLittleEndian)
                Array.Reverse(copied);
            return copied;
        }

        public static bool AreEqual(this byte[] bytes1, byte[] bytes2)
        {
            if (bytes2 == null)
                return false;

            if (bytes1.Length != bytes2.Length)
                return false;

            for (var i = 0; i < bytes1.Length; i++)
            {
                if (bytes1[i] != bytes2[i])
                    return false;
            }

            return true;
        }
    }
}