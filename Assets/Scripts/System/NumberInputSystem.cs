using System.Globalization;

namespace Assets.Scripts.System
{
    public class NumberInputSystem
    {
        private const NumberStyles Filter = NumberStyles.Integer | NumberStyles.AllowDecimalPoint;
        public float? ValidateNumberInput(string value)
        {
            if (float.TryParse(value, Filter, CultureInfo.InvariantCulture, out var result))
            {
                return result;
            }

            return null;
        }
    }
}