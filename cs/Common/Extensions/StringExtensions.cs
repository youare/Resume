using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Extensions
{
    public static class StringExtensions
    {
        public static Optional<T> ToEnum<T>(this string source)
            where T: struct, IConvertible
        {
            if(Enum.IsDefined(typeof(T), source)){
                Enum.TryParse(source, out T data);
                return Optional<T>.Some(data);
            }
            return Optional<T>.Empty;
        }

        public static string FromPascalToSnakeCase(this string source)
        {
            var result = string.Concat(source.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
            return result;
        }
    }
}
