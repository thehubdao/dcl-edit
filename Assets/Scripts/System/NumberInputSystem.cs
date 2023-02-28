using System.Globalization;

namespace Assets.Scripts.System
{
    public class NumberInputSystem
    {
        private const NumberStyles Filter = NumberStyles.Integer | NumberStyles.AllowDecimalPoint;
        public float? ValidateNumberInput(string value)
        {
            value = value.Replace(',', '.');
            value = RemoveExcessDecimalSeparators(value);
            if (float.TryParse(value, Filter, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }

        /// <summary>
        /// Checks if there is more than one decimal separator (dot) and removes the excess ones.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        string RemoveExcessDecimalSeparators(string value)
        {
            int decimalPointCount = 0;
            for (int i = 0; i < value.Length; i++)
            {
                if (value[i] == '.')
                {
                    decimalPointCount++;
                    if (decimalPointCount > 1)
                    {
                        value = value.Remove(i, 1);
                    }
                }
            }

            return value;
        }
    }
}