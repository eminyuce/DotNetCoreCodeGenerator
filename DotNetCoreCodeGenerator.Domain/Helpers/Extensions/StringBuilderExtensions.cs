using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotNetCodeGenerator.Domain.Helpers.Extensions
{
    static class StringBuilderExtensions
    {
        public static StringBuilder AppendLineFormat(this StringBuilder self, string format, params object[] arguments)
        {
            string value = string.Format(format, arguments);
            self.AppendLine(value);

            return self;
        }

        public static StringBuilder AppendWhen(this StringBuilder self, bool condition, params string[] values)
        {
            if (condition)
                foreach (var value in values)
                    self.Append(value);

            return self;
        }

        public static StringBuilder AppendWhen(this StringBuilder self, bool condition, params char[] values)
        {
            if (condition)
                foreach (var value in values)
                    self.Append(value);

            return self;
        }

        public static StringBuilder AppendFormatWhen(this StringBuilder self, bool condition, string format, params object[] args)
        {
            return condition
                ? self.AppendFormat(format, args)
                : self;
        }

        public static StringBuilder AppendIf(this StringBuilder self, bool condition, string ifTrue, string ifFalse)
        {
            return condition
                ? self.Append(ifTrue)
                : self.Append(ifFalse);
        }

        public static StringBuilder BimapIf(this StringBuilder self, bool condition,
            Func<StringBuilder, StringBuilder> ifTrue, Func<StringBuilder, StringBuilder> ifFalse)
        {
            return condition
                ? ifTrue(self)
                : ifFalse(self);
        }

        public static StringBuilder MapIf(this StringBuilder self, bool condition,
            Func<StringBuilder, StringBuilder> ifTrue)
        {
            return condition
                ? ifTrue(self)
                : self;
        }

        public static StringBuilder AppendIfNotEmpty(this StringBuilder self, params string[] values)
        {
            foreach (var value in values)
                if (value.Length > 0)
                    self.Append(value);

            return self;
        }

        public static string SafeToString(this StringBuilder self)
        {
            return self == null
                ? string.Empty
                : self.ToString();
        }

        public static int SafeLength(this StringBuilder self)
        {
            return self == null ? 0 : self.Length;
        }

        public static StringBuilder TrimEnd(this StringBuilder self, char c)
        {
            return self.Length > 0
                ? self.Remove(self.Length - 1, 1)
                : self;
        }

        public static StringBuilder TrimEndIfMatch(this StringBuilder self, char c)
        {
            if (self.Length > 0)
                if (self[self.Length - 1] == c)
                    self.Remove(self.Length - 1, 1);

            return self;
        }

        public static StringBuilder TrimEndIfMatchWhen(this StringBuilder self, bool condition, char c)
        {
            return condition
                ? self.TrimEndIfMatch(c)
                : self;
        }

        public static int TrailingSpaces(this StringBuilder self)
        {
            var bound = self.Length - 1;
            if (self.Length == 0) return 0;
            if (self[bound] != ' ') return 0;
            var c = 0;
            for (var i = bound; i <= bound; i--)
            {
                if (i < 0) break;
                if (self[i] != ' ') break;
                c++;
            }
            return c;
        }
    }
}
