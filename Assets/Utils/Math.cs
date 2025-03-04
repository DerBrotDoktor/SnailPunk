using System;

namespace Utils
{
    public class Math
    {
        public static int MinI(params int[] values)
        {
            if (values == null || values.Length < 2)
            {
                throw new ArgumentException("At least two values must be provided.");
            }

            int minValue = values[0];
            foreach (int value in values)
            {
                if (value < minValue)
                {
                    minValue = value;
                }
            }
            return minValue;
        }
    }
}