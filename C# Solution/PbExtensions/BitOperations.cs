namespace Appeon.CSharpPbExtensions
{
    public class BitOperations
    {
        public static int ShiftRightInt(int number, int shift) => number >> shift;
        public static int ShiftLeftInt(int number, int shift) => number << shift;
        public static int BitwiseAndInt(int number1, int number2) => number1 & number2;
        public static int BitwiseOrInt(int number1, int number2) => number1 | number2;
        public static int BitwiseXorInt(int number1, int number2) => number1 ^ number2;
    }
}
