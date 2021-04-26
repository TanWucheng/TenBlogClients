using System;

namespace UwpApp.Domain
{
    public class EnumParser<T> where T : Enum
    {
        public static T Parse(string enumValue, T defaultValue)
        {
            try
            {
                var resultEnum = (T)Enum.Parse(typeof(T), enumValue, true);
                if (Enum.IsDefined(typeof(T), resultEnum) | resultEnum.ToString().Contains(","))
                {
                    // Console.WriteLine("Converted '{0}' to {1}.", enumValue, resultEnum.ToString());
                    return resultEnum;
                }
                else
                {
                    //Console.WriteLine("{0} is not an underlying value of the Colors enumeration.", enumValue);
                    return defaultValue;
                }
            }
            catch (ArgumentException)
            {
                //Console.WriteLine("{0} is not a member of the Colors enumeration.", enumValue);
                return defaultValue;
            }
        }
    }
}
