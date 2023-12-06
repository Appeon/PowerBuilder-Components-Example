using System.Collections;
using System.Text;

namespace Appeon.CSharpPbExtensions
{
    public class ObjectExtensions
    {
        public static string ToString(object obj)
        {
            if (obj is string str)
                return str;
            if (obj is IEnumerable enumerable)
            {
                bool enumerableHasElements = false;
                var sb = new StringBuilder();
                foreach (var item in enumerable)
                {
                    enumerableHasElements = true;
                    sb.Append(ToString(item));
                    sb.Append(", ");
                }
                if (enumerableHasElements)
                    sb.Length -= 2;
                return sb.ToString();
            }
            return obj.ToString();
        }
    }
}
