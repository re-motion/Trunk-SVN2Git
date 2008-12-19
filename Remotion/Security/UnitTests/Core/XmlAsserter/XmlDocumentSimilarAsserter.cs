// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using System.Xml;

namespace Remotion.Security.UnitTests.Core.XmlAsserter
{
  [CLSCompliant (false)]
  public class XmlDocumentSimilarAsserter : XmlDocumentBaseAsserter
  {
    private NodeStackToXPathConverter _nodeStackToXPathConverter;

    public XmlDocumentSimilarAsserter (XmlDocument expected, XmlDocument actual, string message, params object[] args)
      : base (expected, actual, message, args)
    {
      _nodeStackToXPathConverter = new NodeStackToXPathConverter ();
      _nodeStackToXPathConverter.IncludeNamespaces = true;
    }

    protected override bool CompareDocuments (XmlDocument expectedDocument, XmlDocument actualDocument)
    {
      XmlNode expectedFirstChild = expectedDocument.FirstChild;
      if (expectedFirstChild.NodeType == XmlNodeType.XmlDeclaration)
        expectedFirstChild = expectedDocument.ChildNodes[1];

      if (!ContainsNodeStack (expectedFirstChild, actualDocument))
        return false;

      XmlNode actualFirstChild = actualDocument.FirstChild;
      if (actualFirstChild.NodeType == XmlNodeType.XmlDeclaration)
        actualFirstChild = actualDocument.ChildNodes[1];

      return ContainsNodeStack (actualFirstChild, expectedDocument);
    }

    private bool ContainsNodeStack (XmlNode node, XmlDocument testDocument)
    {
      Stack<XmlNode> nodeStack = GetNodeStack (node);
      string xPathExpression = _nodeStackToXPathConverter.GetXPathExpression (nodeStack);
      if (string.IsNullOrEmpty (xPathExpression))
        return true;

      XmlNodeList nodes = testDocument.SelectNodes (xPathExpression, _nodeStackToXPathConverter.NamespaceManager);
      if (nodes.Count == 0)
      {
        FailureMessage.WriteLine (xPathExpression + " Evaluation failed.");
        FailureMessage.WriteLine ("Node missing in actual document:");
        ShowNodeStack (node, FailureMessage.WriteExpectedLine);

        if (node.ParentNode != null)
        {
          Stack<XmlNode> parentNodeStack = GetNodeStack (node.ParentNode);
          xPathExpression = _nodeStackToXPathConverter.GetXPathExpression (parentNodeStack);
          XmlNodeList actualNodes = testDocument.SelectNodes (xPathExpression, _nodeStackToXPathConverter.NamespaceManager);
          if (actualNodes.Count > 0)
            ShowNodeStack (actualNodes[0], FailureMessage.WriteActualLine);
        }
        return false;
      }

      foreach (XmlNode childNode in node.ChildNodes)
      {
        if (!ContainsNodeStack (childNode, testDocument))
          return false;
      }

      return true;
    }
  }
}
