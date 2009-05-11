﻿using System;
using System.IO;
using System.Text;
using System.Web.UI;
using HtmlAgilityPack;
using NUnit.Framework;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.Renderers
{
  public class HtmlHelper
  {
    public const string WhiteSpace = "&nbsp;";

    public enum AttributeValueCompareMode { Equal, Contains }

    private byte[] Buffer { get; set; }
    public HtmlTextWriter Writer { get; private set; }
    protected MemoryStream Stream { get; private set; }
    protected StreamReader Reader { get; private set; }

    public void InitializeStream ()
    {
      Buffer = new byte[1 * 1024];
      Stream = new MemoryStream (Buffer);
      StreamWriter innerWriter = new StreamWriter (Stream, Encoding.Unicode);
      Writer = new HtmlTextWriter (innerWriter);
      Reader = new StreamReader (Stream, Encoding.Unicode);
    }

    public HtmlDocument GetResultDocument ()
    {
      Writer.Flush ();
      Reader.BaseStream.Seek (0, SeekOrigin.Begin);
      HtmlDocument document = new HtmlDocument ();
      document.Load (Reader);
      return document;
    }

    public HtmlNode GetAssertedChildElement (HtmlNode parent, string tag, int index, bool ignoreNonElementNodes)
    {
      HtmlNode node = null;
      if (ignoreNonElementNodes)
      {
        int elementIndex = 0;
        foreach (HtmlNode childNode in parent.ChildNodes)
        {
          if (childNode.NodeType == HtmlNodeType.Element)
          {
            if (elementIndex == index)
            {
              node = childNode;
              break;
            }
            ++elementIndex;
          }
        }
        if (node == null)
          Assert.Fail (String.Format ("Node {0} has only {1} child elements - index {2} out of range.", parent.Name, elementIndex, index));
      }
      else
      {
        Assert.Less (
            index,
            parent.ChildNodes.Count,
            String.Format ("Node {0} has only {1} children - index {2} out of range.", parent.Name, parent.ChildNodes.Count, index));

        node = parent.ChildNodes[index];

        Assert.AreEqual (HtmlNodeType.Element, node.NodeType,
        String.Format (
                "{0}.ChildNodes[{1}].NodeType is {2}, not {3}.",
                parent.Name,
                index,
                parent.ChildNodes[index].NodeType,
                HtmlNodeType.Text));
      }
      
      Assert.AreEqual (tag, node.Name, "Unexpected element tag.");
      return node;
    }

    public void AssertTextNode (HtmlNode parent, string content, int index, bool ignoreNonTextNodes)
    {
      HtmlTextNode node = null;
      if (ignoreNonTextNodes)
      {
        int textNodeIndex = 0;
        foreach (HtmlNode childNode in parent.ChildNodes)
        {
          if (childNode.NodeType == HtmlNodeType.Text)
          {
            if (textNodeIndex == index)
            {
              node = (HtmlTextNode)childNode;
              break;
            }
            ++textNodeIndex;
          }
        }
        if (node == null)
          Assert.Fail (String.Format ("Node {0} has only {1} child elements - index {2} out of range.", parent.Name, textNodeIndex, index));
      }
      else
      {
        Assert.Less (
            index,
            parent.ChildNodes.Count,
            String.Format ("Node {0} has only {1} children - index {2} out of range.", parent.Name, parent.ChildNodes.Count, index));
        Assert.AreEqual (
            HtmlNodeType.Text,
            parent.ChildNodes[index].NodeType,
            String.Format (
                "{0}.ChildNodes[{1}].NodeType is {2}, not {3}.",
                parent.Name,
                index,
                parent.ChildNodes[index].NodeType,
                HtmlNodeType.Text));
        node = (HtmlTextNode)parent.ChildNodes[index];
      }
      Assert.AreEqual (content, node.Text, "Unexpected text node content.");
    }

    public void AssertAttribute (HtmlNode node, string attributeName, string attributeValue)
    {
      AssertAttribute (node, attributeName, attributeValue, AttributeValueCompareMode.Equal);
    }

    public void AssertAttribute (HtmlNode node, string attributeName, string attributeValue, AttributeValueCompareMode mode)
    {
      HtmlAttribute attribute = node.Attributes[attributeName];
      Assert.IsNotNull (attribute, String.Format ("Attribute {0}.{1}", node.Name, attributeName));

      if (attributeValue != null)
      {
        switch (mode)
        {
          case AttributeValueCompareMode.Equal:
            Assert.AreEqual (attributeValue, attribute.Value, String.Format ("Attribute {0}.{1}", node.Name, attribute.Name));
            break;
          case AttributeValueCompareMode.Contains:
            Assert.IsTrue (
                attribute.Value.Contains (attributeValue),
                String.Format (
                    "Unexpected attribute value in {0}.{1}: should contain {2}, but was {3}",
                    node.Name,
                    attribute.Name,
                    attributeValue,
                    attribute.Value));
            break;
        }
      }
    }

    public void AssertStyleAttribute (HtmlNode node, string cssProperty, string cssValue)
    {
      HtmlAttribute attribute = node.Attributes["style"];
      Assert.IsNotNull (attribute, String.Format ("Attribute {0}.{1}", node.Name, "style"));

      string stylePart = String.Format ("{0}:{1};", cssProperty, cssValue);
      Assert.IsTrue (attribute.Value.Contains (stylePart), String.Format ("Attribute {0}.{1}", node.Name, attribute.Name));
    }
  }
}
