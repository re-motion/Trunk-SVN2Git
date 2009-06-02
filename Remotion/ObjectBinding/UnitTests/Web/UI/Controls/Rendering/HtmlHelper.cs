// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Xml;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Rendering
{
  //public class HtmlHelperBase
  //{
  //      public delegate void AssertThatActualIsEqualToExpected (object actual, object expected, string message, params object[] args);

  //      public HtmlHelperBase (AssertThatActualIsEqualToExpected assertion)
  //  {

  //  }
  //}

  public class HtmlHelper : IDisposable
  {
    public const string WhiteSpace = "&nbsp;";

    public enum AttributeValueCompareMode
    {
      Equal,
      Contains
    }

    private readonly HtmlTextWriter _writer;

    public HtmlTextWriter Writer
    {
      get { return _writer; }
    }

    private readonly MemoryStream _stream;

    protected MemoryStream Stream
    {
      get { return _stream; }
    }

    private readonly StreamReader _reader;

    protected StreamReader Reader
    {
      get { return _reader; }
    }

    public HtmlHelper ()
    {
      _stream = new MemoryStream (4096);
      _writer = new HtmlTextWriter (new StreamWriter (Stream, Encoding.Unicode));
      _reader = new StreamReader (Stream, Encoding.Unicode);
    }

    public string GetDocumentText ()
    {
      Writer.Flush();
      Reader.BaseStream.Seek (0, SeekOrigin.Begin);
      return Reader.ReadToEnd();
    }

    public XmlDocument GetResultDocument ()
    {
      XmlDocument document = new XmlDocument();
      using (TextReader reader = new StringReader (GetDocumentText().Replace ("&nbsp;", "&amp;nbsp;")))
      {
        document.Load (reader);
      }
      return document;
    }

    public void AssertChildElementCount (XmlNode parent, int count)
    {
      int elementCount = 0;
      foreach (XmlNode node in parent.ChildNodes)
      {
        if (node.NodeType == XmlNodeType.Element)
          ++elementCount;
      }
      Assert.AreEqual (count, elementCount);
    }

    public XmlNode GetAssertedChildElement (XmlNode parent, string tag, int index)
    {
      Assert.Greater (
          parent.ChildNodes.Count,
          index,
          string.Format ("Node {0} has only {1} children - index {2} out of range.", parent.Name, parent.ChildNodes.Count, index));

      XmlNode node = parent.ChildNodes[index];

      Assert.AreEqual (
          XmlNodeType.Element,
          node.NodeType,
          string.Format (
              "{0}.ChildNodes[{1}].NodeType is {2}, not {3}.",
              parent.Name,
              index,
              parent.ChildNodes[index].NodeType,
              XmlNodeType.Element));

      Assert.AreEqual (tag, node.Name, "Unexpected element tag.");
      return node;
    }

    public void AssertTextNode (XmlNode parent, string content, int index)
    {
      Assert.Greater (
          parent.ChildNodes.Count,
          index,
          string.Format ("Node {0} has only {1} children - index {2} out of range.", parent.Name, parent.ChildNodes.Count, index));
      Assert.AreEqual (
          XmlNodeType.Text,
          parent.ChildNodes[index].NodeType,
          string.Format (
              "{0}.ChildNodes[{1}].NodeType is {2}, not {3}.",
              parent.Name,
              index,
              parent.ChildNodes[index].NodeType,
              XmlNodeType.Text));
      var node = (XmlText) parent.ChildNodes[index];

      Assert.AreEqual (content, node.InnerText.Trim(), "Unexpected text node content.");
    }

    public void AssertAttribute (XmlNode node, string attributeName, string attributeValue)
    {
      AssertAttribute (node, attributeName, attributeValue, AttributeValueCompareMode.Equal);
    }

    public void AssertAttribute (XmlNode node, string attributeName, string attributeValue, AttributeValueCompareMode mode)
    {
      XmlAttribute attribute = node.Attributes[attributeName];
      Assert.IsNotNull (attribute, string.Format ("Attribute {0}.{1} does not exist.", node.Name, attributeName));

      if (attributeValue != null)
      {
        switch (mode)
        {
          case AttributeValueCompareMode.Equal:
            Assert.AreEqual (attributeValue, attribute.Value, string.Format ("Attribute {0}.{1}", node.Name, attribute.Name));
            break;
          case AttributeValueCompareMode.Contains:
            Assert.IsTrue (
                attribute.Value.Contains (attributeValue),
                string.Format (
                    "Unexpected attribute value in {0}.{1}: should contain {2}, but was {3}",
                    node.Name,
                    attribute.Name,
                    attributeValue,
                    attribute.Value));
            break;
        }
      }
    }

    public void AssertStyleAttribute (XmlNode node, string cssProperty, string cssValue)
    {
      XmlAttribute attribute = node.Attributes["style"];
      Assert.IsNotNull (attribute, string.Format ("Attribute {0}.{1}", node.Name, "style"));

      string stylePart = string.Format ("{0}:{1};", cssProperty, cssValue);
      Assert.IsTrue (
          attribute.Value.Contains (stylePart),
          string.Format ("Attribute {0}.{1} does not contain '{2}' - value is '{3}'.", node.Name, attribute.Name, stylePart, attribute.Value));
    }

    public void AssertIcon (XmlNode parentNode, IBusinessObject businessObject, string imageSourcePart)
    {
      XmlNode img = GetAssertedChildElement (parentNode, "img", 0);
      if (imageSourcePart == null)
      {
        string businessObjectClass = businessObject.BusinessObjectClass.Identifier;
        imageSourcePart = businessObjectClass.Substring (0, businessObjectClass.IndexOf (", "));
      }
      AssertAttribute (img, "src", imageSourcePart, AttributeValueCompareMode.Contains);
      AssertAttribute (img, "width", "16px");
      AssertAttribute (img, "height", "16px");
      AssertAttribute (img, "alt", "");
      AssertStyleAttribute (img, "vertical-align", "middle");
      AssertStyleAttribute (img, "border-style", "none");
    }

    public void AssertNoAttribute (XmlNode node, string attributeName)
    {
      Assert.That (node.Attributes[attributeName], Is.Null);
    }

    public void Dispose ()
    {
      Reader.Dispose();
      Stream.Dispose();
      Writer.Dispose();
    }
  }
}