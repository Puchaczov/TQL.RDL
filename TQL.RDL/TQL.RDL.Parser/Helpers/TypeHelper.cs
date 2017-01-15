using System;

namespace RDL.Parser.Helpers
{
    public static class TypeHelper
    {
        public static string GetTypeName(this Type type) => GetUnderlyingNullable(type).Name;

        public static Type GetUnderlyingNullable(this Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);

            var isNullableType = nullableType != null;

            return isNullableType ? nullableType : type;
        }
    }
}
