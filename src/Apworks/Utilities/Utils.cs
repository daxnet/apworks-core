using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Apworks.Utilities
{
    public static class Utils
    {
        private static Lazy<Type[]> SimpleTypesInternal = new Lazy<Type[]>(() =>
        {
            var types = new[]
                           {
                              typeof (Enum),
                              typeof (string),
                              typeof (char),
                              typeof (Guid),

                              typeof (bool),
                              typeof (byte),
                              typeof (short),
                              typeof (int),
                              typeof (long),
                              typeof (float),
                              typeof (double),
                              typeof (decimal),

                              typeof (sbyte),
                              typeof (ushort),
                              typeof (uint),
                              typeof (ulong),

                              typeof (DateTime),
                              typeof (DateTimeOffset),
                              typeof (TimeSpan),
                          };


            var nullableTypes = from t in types
                                where t != typeof(Enum) && t != typeof(string)
                                select typeof(Nullable<>).MakeGenericType(t);

            return types.Concat(nullableTypes).ToArray();
        });

        public static bool IsSimpleType(this Type src)
        {
            if (src == null)
            {
                throw new ArgumentNullException(nameof(src));
            }

            return src.GetTypeInfo().IsEnum ||
                (src.GetTypeInfo().IsGenericType &&
                    src.GetTypeInfo().GetGenericTypeDefinition() == typeof(Nullable<>) &&
                    src.GetTypeInfo().GetGenericArguments().First().GetTypeInfo().IsEnum) ||
                SimpleTypesInternal.Value.Contains(src);
        }
    }
}
