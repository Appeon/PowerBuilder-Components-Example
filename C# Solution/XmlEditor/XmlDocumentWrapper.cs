using System.Security;
using System.Xml;

namespace Appeon.ComponentsApp.XmlEditor
{
    public class XmlDocumentWrapper
    {
        public enum NodeType
        {
            Element,
            Text,
            CDATA,
            Comment
        }

        public static XmlDocument? LoadDocument(string filename, out string? error)
        {
            error = null;
            if (!File.Exists(filename))
            {
                error = "No such file";
                return null;
            }

            var document = new XmlDocument();
            try
            {
                document.Load(filename);
                return document;
            }
            catch (Exception e)
            {
                error = (e.InnerException ?? e).Message;
            }

            return null;
        }

        public static XmlDocument? CreateDocument(string filename, string rootElement, out string? error)
        {
            error = null;

            var document = new XmlDocument();

            document.LoadXml($"<{rootElement}/>");

            try
            {
                document.Save(filename);

            }
            catch (Exception e)
            {
                error = e.Message;
                return null;
            }

            return document;
        }

        public static XmlDocument? CreateDocumentFromString(string xml, out string? error)
        {
            error = null;

            var doc = new XmlDocument();
            try
            {
                doc.LoadXml(xml);
                return doc;
            }
            catch (Exception e)
            {
                error = e.Message;
            }

            return null;
        }

        public static XmlNodeChildren? GetChildren(XmlNode element)
        {
            if (!(element?.HasChildNodes ?? false))
                return null;

            var children = new XmlNode[element.ChildNodes.Count];

            int i = 0;
            foreach (var child in element.ChildNodes)
            {
                children[i] = (XmlNode)child;
                ++i;
            }

            return new XmlNodeChildren
            {
                Children = children
            };
        }

        public static NodeType GetNodeType(XmlNode node)
        {
            return node switch
            {
                XmlElement => NodeType.Element,
                XmlText => NodeType.Text,
                XmlCDataSection => NodeType.CDATA,
                XmlComment => NodeType.Comment,
                _ => (NodeType)(-1)
            } + 1;
        }

        public static int GetNodeAttributes(in XmlNode node, out string[]? keys, out string[]? values, out string? error)
        {
            keys = null;
            values = null;
            error = null;

            if (node.Attributes == null)
            {
                error = "Node has no attributes";
                return -1;
            }

            keys = new string[node.Attributes.Count];
            values = new string[node.Attributes.Count];

            try
            {
                for (int i = 0; i < node.Attributes.Count; i++)
                {
                    keys[i] = node.Attributes[i].Name;
                    values[i] = node.Attributes[i].Value;
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            return 1;
        }

        public static int SetNodeAttributes(ref XmlDocument document, ref XmlNode node, string[] keys, string[] values, out string? error)
        {
            error = null;
            if (node.Attributes is null)
            {
                error = "Node has no attributes";
                return -1;
            }

            node.Attributes.RemoveAll();
            for (int i = 0; i < keys.Length; i++)
            {
                var attribute = document.CreateAttribute(keys[i]);
                attribute.Value = values[i];
                node.Attributes.Append(attribute);
            }

            return 1;
        }

        public static int RemoveNode(ref XmlDocument document, ref XmlNode target, out string? error)
        {
            error = null;
            try
            {
                if (target.ParentNode is not null)
                {
                    target.ParentNode.RemoveChild(target);
                    return 1;
                }
                else
                {
                    if (document.DocumentElement is not null)
                        document.RemoveChild(document.DocumentElement);
                    return 1;
                }
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

        }

        public static int InsertNode(
            ref XmlDocument document,
            ref XmlNode node,
            out XmlNode? createdNode,
            int position,
            NodeType childType,
            string @string,
            out string? error)
        {
            error = null;
            createdNode = null;


            try
            {
                //@string = SecurityElement.Escape(@string);
                XmlNode newNode = childType switch
                {
                    NodeType.Element => document.CreateElement(@string),
                    NodeType.Text => document.CreateTextNode(@string
                    ),
                    NodeType.CDATA => document.CreateCDataSection(@string

                        ),
                    NodeType.Comment => document.CreateComment(@string),
                    _ => throw new ArgumentException("Unsupported child type", nameof(childType)),
                };
                switch (position)
                {
                    case < 0:
                        if (node.ParentNode is null)
                            throw new InvalidOperationException("Reference node has no parent");
                        node.ParentNode.InsertBefore(newNode, node);
                        //document.InsertBefore(newNode, node);
                        break;
                    case 0:
                        node.AppendChild(newNode);
                        break;

                    case > 0:
                        if (node.ParentNode is null)
                            throw new InvalidOperationException("Reference node has no parent");
                        node.ParentNode.InsertAfter(newNode, node);
                        //document.InsertAfter(newNode, node);
                        break;
                };

                createdNode = newNode;
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            return 1;

        }

        public static int MoveNodeUp(ref XmlNode node, out string? error)
        {
            error = null;
            if (node.PreviousSibling is var sibling
                && sibling is null
                || sibling is XmlDeclaration)
            {
                return 0;
            }

            if (node.ParentNode is var parentNode
                && parentNode is null
                )
            {
                error = "Cannot move root node";

                return -1;
            }
            try
            {
                parentNode.InsertBefore(node, sibling);
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            return 1;
        }

        public static int MoveNodeDown(ref XmlNode node, out string? error)
        {
            error = null;
            if (node.NextSibling is var sibling
                && sibling is null
                || sibling is XmlDeclaration)
            {
                return 0;
            }

            if (node.ParentNode is var parentNode && parentNode is null)
            {
                error = "Cannot move root node";

                return -1;
            }
            try
            {
                parentNode.InsertAfter(node, sibling);
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            return 1;
        }

        public static int SaveDocument(ref XmlDocument document, string path, out string? error)
        {
            error = null;

            try
            {
                document.Save(path);
            }
            catch (Exception e)
            {
                error = e.Message;
                return -1;
            }

            return 1;
        }

        public static string AsString(XmlDocument document)
        {
            using var stringWriter = new StringWriter();
            using var xmlTextWriter = XmlWriter.Create(stringWriter, new XmlWriterSettings
            {
                Indent = true,
            });

            document.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            return stringWriter.GetStringBuilder().ToString();
        }
    }
}