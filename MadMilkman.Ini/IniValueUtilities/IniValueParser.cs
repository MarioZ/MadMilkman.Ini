using System;

namespace MadMilkman.Ini
{
    delegate bool TryParseDelegate<TDelegate>(string value, out TDelegate result);

    internal static class IniValueParser<T>
    {
        private static TryParseDelegate<T> parser;

        static IniValueParser() { InitializeParser(typeof(T)); }

        private static void InitializeParser(Type type)
        {
            if (type.IsEnum) SetParser<T>(EnumTryParse);
            else if (type == typeof(TimeSpan)) SetParser<TimeSpan>(TimeSpan.TryParse);
            else
                switch (Type.GetTypeCode(type))
                {
                    case TypeCode.Boolean: SetParser<bool>(bool.TryParse); break;
                    case TypeCode.Byte: SetParser<byte>(byte.TryParse); break;
                    case TypeCode.SByte: SetParser<sbyte>(sbyte.TryParse); break;
                    case TypeCode.Int16: SetParser<short>(short.TryParse); break;
                    case TypeCode.UInt16: SetParser<ushort>(ushort.TryParse); break;
                    case TypeCode.Int32: SetParser<int>(int.TryParse); break;
                    case TypeCode.UInt32: SetParser<uint>(uint.TryParse); break;
                    case TypeCode.Int64: SetParser<long>(long.TryParse); break;
                    case TypeCode.UInt64: SetParser<ulong>(ulong.TryParse); break;
                    case TypeCode.Single: SetParser<float>(float.TryParse); break;
                    case TypeCode.Double: SetParser<double>(double.TryParse); break;
                    case TypeCode.Decimal: SetParser<decimal>(decimal.TryParse); break;
                    case TypeCode.Char: SetParser<char>(char.TryParse); break;
                    case TypeCode.DateTime: SetParser<DateTime>(DateTime.TryParse); break;
                    case TypeCode.String: SetParser<string>((string value, out string result) => { result = value; return true; }); break;
                }
        }

        private static void SetParser<TParser>(TryParseDelegate<TParser> parser)
        {
            IniValueParser<TParser>.parser = parser;
        }

        private static bool EnumTryParse<TEnum>(string value, out TEnum result)
        {
            /* REMARKS: To support case insensitivity instead of Enum.IsDefined use Enum.GetNames
             *          to achieve case insensitive string comparison between enum's names and value. */

            Type type = typeof(TEnum);
            if (Enum.IsDefined((type), value))
            {
                result = (TEnum)Enum.Parse(type, value);
                return true;
            }

            result = default(TEnum);
            return false;
        }

        public static bool TryParse(string value, out T result)
        {
            if (parser == null)
                throw new NotSupportedException();

            return parser(value, out result);
        }
    }
}
