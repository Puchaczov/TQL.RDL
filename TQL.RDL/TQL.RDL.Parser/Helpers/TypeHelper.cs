using System;
using System.Linq;
using System.Reflection;

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

        public static int OptionalParameters(this ParameterInfo[] parameters) => parameters.Count(f => f.IsOptional);
    }
}
