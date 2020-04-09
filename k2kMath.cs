
using System;
using System.Collections.Generic;
using System.Text;


namespace k2kLib
{
    public class k2kMath
    {
        public static double GetDistance(double x1, double y1, double x2, double y2) { return GetDistance(x2 - x1, y2 - y1); }
        public static double GetDistance(double x1, double y1, double z1, double x2, double y2, double z2) { return GetDistance(x2 - x1, y2 - y1, z2 - z1); }

        public static double GetDistance(double dx, double dy) { return Math.Sqrt(dx * dx + dy * dy); }
        public static double GetDistance(double dx, double dy, double dz) { return Math.Sqrt(dx * dx + dy * dy + dz * dz); }


        public static double GetAngle(double x1, double y1, double z1, double x2, double y2, double z2)
        {
            double n1 = GetDistance(x1, y1, z1);
            double n2 = GetDistance(x2, y2, z2);
            double dot = x1 * x2 + y1 * y2 + z1 * z2;
            return Math.Acos(dot / Math.Sqrt(n1 * n2));
        }
        public static double GetAngle(double x1, double y1, double x2, double y2) { return GetAngle(x1, y1, 0, x2, y2, 0); }


        public static double Clamp(double value, double min, double max) { return Math.Min(Math.Max(min, value), max); }
        public static float Clamp(float value, float min, float max) { return Math.Min(Math.Max(min, value), max); }
        public static int Clamp(int value, int min, int max) { return Math.Min(Math.Max(min, value), max); }

        public static double SmoothStep(double value, double min, double max)
        {
            double t = Clamp((value - min) / (max - min), 0.0, 1.0);
            return t * t * (3.0 - 2.0 * t);
        }
        public static int IntStep(double value, double min, double max, int IntMax)
        {
            double t = Clamp((value - min) / (max - min), 0.0, 1.0);
            return (int)(t * IntMax);
        }

        public static void Swap<T>(T a, T b)
        {
            T tmp;
            tmp = a;
            a = b;
            b = a;
        }

        static k2kMath()
        {
            Base32Chars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        }
        static char[] Base32Chars;
        public static string ToBase32(int num) { return ToBase32((ulong)num); }
        public static string ToBase32(int num,int len) { return ToBase32((ulong)num,len); }
        public static string ToBase32(ulong num, int len)
        {
            int i;
            var sb = new StringBuilder();
            var ret = ToBase32(num);

            for (i = ret.Length; i < len; ++i)
            {
                sb.Append("0");
            }
            return sb.Append(ret).ToString();
        }
        public static string ToBase32(ulong num)
        {
            var sb = new StringBuilder();
            while (num != 0)
            {
                sb.Insert(0, Base32Chars[num & 0x1F]);
                num >>= 5;
            }
            return sb.ToString();
        }

        public static T GetMedian<T>(T[] data)
        {
            var tmp = (T[])data.Clone();
            Array.Sort<T>(tmp);
            return tmp[tmp.Length / 2];
        }
    }
}
