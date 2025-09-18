using CoreLib.Application.Common.Exceptions;
using System.Globalization;
using System.Reflection;
using System.Xml.Serialization;

namespace CoreLib.Application.Common.Utility
{
    public static class TypeTableCodeHelper
    {
        public static TEnumType ToEnumFromCode<TEnumType>(this string stringValue) where TEnumType : struct, IConvertible
        {
            return GetEnumFromCode<TEnumType>(stringValue);
        }

        public static string? GetTypeTableCode(this Enum value)
        {
            return value == null ? null : GetCodeEnum(value);
        }

        public static Dictionary<TEnumType, TEntityType> GetEntityDictionary<TEnumType, TEntityType>(Func<string, TEntityType> getEntityFunc)
            where TEnumType : struct, IConvertible
            where TEntityType : class
        {
            var values = new Dictionary<TEnumType, TEntityType>();

            foreach (TEnumType enumValue in Enum.GetValues(typeof(TEnumType)))
            {
                Enum value = (Enum)(object)enumValue;
                var typeCode = value.GetTypeTableCode();
                values[enumValue] = getEntityFunc(typeCode ?? string.Empty);
            }
            return values;
        }

        public static string? GetTypeTableCode<EnumType>(this EnumType value) where EnumType : struct, IConvertible
        {
            if (!typeof(EnumType).IsEnum)
            {
                throw new ArgumentException("must be an enumerated type");
            }

            Enum enumValue = (Enum)(object)value;

            return GetCodeEnum(enumValue);
        }

        public static string? GetTypeTableDisplayName(this Enum value)
        {
            return value == null ? null : GetCodeEnumDisplayName(value);
        }

        public static string? GetTypeTableDisplayName<EnumType>(this EnumType value) where EnumType : struct, IConvertible
        {
            if (!typeof(EnumType).IsEnum)
            {
                throw new ArgumentException("must be an enumerated type");
            }

            Enum enumValue = (Enum)(object)value;

            return GetCodeEnumDisplayName(enumValue);
        }

        public static string GetTypeTableDisplayNameOrEnumName<EnumType>(this EnumType value) where EnumType : struct, IConvertible
        {
            if (!typeof(EnumType).IsEnum)
            {
                throw new ArgumentException("must be an enumerated type");
            }

            Enum enumValue = (Enum)(object)value;

            var displayName = GetCodeEnumDisplayName(enumValue);
            return displayName ?? enumValue.ToString();
        }

        public static Enum[] GetEnumValues(Type enumType)
        {
            Guard.NotNull(enumType, nameof(enumType));

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Enum Type required");
            }

            var enumValues = new List<Enum>();

            var values = enumType.GetMembers();
            foreach (var value in values)
            {
                TypeTableCodeAttribute? typeCodeAttribute = value.GetCustomAttribute(typeof(TypeTableCodeAttribute)) as TypeTableCodeAttribute;
                if (typeCodeAttribute != null)
                {
                    var enumValue = (Enum)Enum.Parse(enumType, value.Name);
                    enumValues.Add(enumValue);
                }
            }

            return enumValues.ToArray();
        }

        private static string? GetCodeEnum(Enum value)
        {
            var enumType = value.GetType();
            var enumValue = enumType.GetMember(value.ToString());
            if (enumValue == null || enumValue.Length == 0)
            {
                return null;
            }

            TypeTableCodeAttribute? typeCodeAttribute = enumValue[0].GetCustomAttribute(typeof(TypeTableCodeAttribute)) as TypeTableCodeAttribute;
            return typeCodeAttribute?.Code;
        }

        private static string? GetCodeEnumDisplayName(Enum value)
        {
            var enumType = value.GetType();
            var enumValue = enumType.GetMember(value.ToString());
            if (enumValue == null || enumValue.Length == 0)
            {
                return null;
            }

            TypeTableCodeAttribute? typeCodeAttribute = enumValue[0].GetCustomAttribute(typeof(TypeTableCodeAttribute)) as TypeTableCodeAttribute;
            return typeCodeAttribute?.DisplayName;
        }

        public static string? GetXmlEnumAttribute<EnumType>(this EnumType value) where EnumType : struct, IConvertible
        {
            var enumType = value.GetType();
            var enumValue = enumType.GetMember(value.ToString(CultureInfo.InvariantCulture) ?? string.Empty);
            if (enumValue == null || enumValue.Length == 0)
            {
                return null;
            }

            XmlEnumAttribute? typeCodeAttribute = enumValue[0].GetCustomAttribute(typeof(XmlEnumAttribute)) as XmlEnumAttribute;
            if (typeCodeAttribute == null)
            {
                return null;
            }

            return typeCodeAttribute.Name;
        }

        public static Enum GetEnumFromCode(Type enumType, string code)
        {
            Guard.NotNull(enumType, nameof(enumType));

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Enum Type required");
            }

            var values = enumType.GetMembers();
            foreach (var value in values)
            {
                TypeTableCodeAttribute? typeCodeAttribute = value.GetCustomAttribute(typeof(TypeTableCodeAttribute)) as TypeTableCodeAttribute;
                if (typeCodeAttribute != null && typeCodeAttribute.Code == code)
                {
                    return (Enum)Enum.Parse(enumType, value.Name);
                }
            }
            throw new NotFoundException("Enum value not found: " + code);
        }

        public static TEnumType GetEnumFromCode<TEnumType>(string code) where TEnumType : struct, IConvertible
        {
            var enumType = typeof(TEnumType);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Enum Type required");
            }

            var values = enumType.GetMembers();
            foreach (var value in values)
            {
                TypeTableCodeAttribute? typeCodeAttribute = value.GetCustomAttribute(typeof(TypeTableCodeAttribute)) as TypeTableCodeAttribute;
                if (typeCodeAttribute != null && typeCodeAttribute.Code == code)
                {
                    return (TEnumType)Enum.Parse(enumType, value.Name);
                }
            }
            throw new NotFoundException("Enum value not found: " + code);
        }
    }
}
