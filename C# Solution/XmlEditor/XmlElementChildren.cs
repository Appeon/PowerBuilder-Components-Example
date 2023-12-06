using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace Appeon.ComponentsApp.XmlEditor
{
    public class XmlNodeChildren
    {
        [DisallowNull]
        public XmlNode[]? Children { get; set; }

        public int ChildrenCount => Children?.Length ?? -1;

        public XmlNode? GetChild(int index) => Children == null ? null : Children[index];
    }
}
