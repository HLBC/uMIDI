using System;
namespace uMIDI.Utility
{
    public class BitByBit
    {
        private BitByBit() { }

        public static byte LeftNib(byte b)
        {
            return (byte)(b >> 4);
        }

        public static byte RightNib(byte b)
        {
            return (byte)(b & 0x0F);
        }

        // Converts a 4-bit nibble to a hexidecimal character (0->F)
        // ex: 00000100 -> '4'
        // ex: 01001111 -> 'F'
        public static char Nib2Hex(byte b)
        {
            b = (byte)(0x0F & b);
            if (Between(b, 0, 9))
                return (char)('0' + b);
            else if (Between(b, 10, 15))
                return (char)('A' + b - 10);
            else
                return '0';
        }

        private static bool Between(int i, int low, int high)
        {
            return low <= i && i <= high;
        }

        public static bool IsFrontBitsOn(byte b, int numOfBits)
        {
            if (0 < numOfBits && numOfBits <= 8)
                return 0xFF == (b | ClearFront(0xFF, numOfBits));
            else
                return false;
        }

        public static long Concat(long num, byte toConcat)
        {
            return ConcatIgnoreFront(num, toConcat, 0);
        }

        public static long ConcatIgnoreFront(long num, byte toConcat, int numOfBits)
        {
            if (0 <= numOfBits && numOfBits < 8)
                return (num << (8 - numOfBits)) + ClearFront(toConcat, numOfBits);
            else if (8 == numOfBits)
                return num;
            else
                throw new ArgumentException("Number of bits out of range [0,8].");
        }

        public static byte ClearFront(byte b, int numOfBits)
        {
            return (byte)((byte)(b << numOfBits) >> numOfBits);
        }

        public static byte ClearBack(byte b, int numOfBits)
        {
            return (byte)((byte)(b >> numOfBits) << numOfBits);
        }
    }
}
