﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDL.Parser.Helpers
{
    public static class TypeHelper
    {
        public static string GetTypeName(this Type type) => GetUnderlyingNullable(type).Name;

        public static Type GetUnderlyingNullable(this Type type)
        {
            var nullableType = Nullable.GetUnderlyingType(type);

            bool isNullableType = nullableType != null;

            if (isNullableType)
                return nullableType;
            else
                return type;
        }
    }
}
