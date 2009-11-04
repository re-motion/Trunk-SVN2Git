// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using System.Xml;

namespace Remotion.Security.UnitTests.Core.XmlAsserter
{
  [CLSCompliant (false)]
  public class XmlDocumentEqualAsserter : XmlDocumentBaseAsserter
  {
    private XmlnsAttributeHandler _xmlnsAttributeFilter;

    public XmlDocumentEqualAsserter (XmlDocument expected, XmlDocument actual, string message, params object[] args)
        : base (expected, actual, message, args)
    {
      _xmlnsAttributeFilter = new XmlnsAttributeHandler ();
    }

    protected override bool CompareDocuments (XmlDocument expectedDocument, XmlDocument actualDocument)
    {
      return CompareNodes (expectedDocument, actualDocument);
    }

    protected bool CompareNodes (XmlNode expectedParentNode, XmlNode actualParentNode)
    {
      if (expectedParentNode.ChildNodes.Count != actualParentNode.ChildNodes.Count)
      {
        FailureMessage.WriteLine ("Child node lengths differ:");
        FailureMessage.WriteExpectedLine (expectedParentNode.ChildNodes.Count.ToString ());
        FailureMessage.WriteActualLine (actualParentNode.ChildNodes.Count.ToString ());
        SetFailureMessage (expectedParentNode, actualParentNode);

        return false;
      }

      for (int i = 0; i < expectedParentNode.ChildNodes.Count; i++)
      {
        XmlNode expectedNode = expectedParentNode.ChildNodes[i];
        XmlNode actualNode = actualParentNode.ChildNodes[i];

        if (!AreNodesEqual (expectedNode, actualNode))
        {
          FailureMessage.WriteLine ("Nodes differ:");
          SetFailureMessage (expectedNode, actualNode);
          return false;
        }

        if (!CompareNodes (expectedNode, actualNode))
          return false;
      }

      return true;
    }

    protected virtual bool AreNodesEqual (XmlNode expected, XmlNode actual)
    {
      return expected.NamespaceURI == actual.NamespaceURI
          && expected.LocalName == actual.LocalName
          && AreNodeAttributesEqual (expected.Attributes, actual.Attributes)
          && expected.Value == actual.Value;
    }

    protected virtual bool AreNodeAttributesEqual (XmlAttributeCollection expectedAttributes, XmlAttributeCollection actualAttributes)
    {
      if (expectedAttributes == null && actualAttributes == null)
        return true;

      if (expectedAttributes == null || actualAttributes == null)
        return false;

      _xmlnsAttributeFilter.Handle (expectedAttributes);
      _xmlnsAttributeFilter.Handle (actualAttributes);

      if (expectedAttributes.Count != actualAttributes.Count)
        return false;

      for (int i = 0; i < expectedAttributes.Count; i++)
      {
        if (!AreAttributesEqual (expectedAttributes[i], actualAttributes[i]))
          return false;
      }

      return true;
    }

    protected bool AreAttributesEqual (XmlAttribute expected, XmlAttribute actual)
    {
      return expected.NamespaceURI == actual.NamespaceURI
          && expected.LocalName == actual.LocalName
          && expected.Value == actual.Value;
    }

    protected void SetFailureMessage (XmlNode expectedNode, XmlNode actualNode)
    {
      ShowNodeStack (expectedNode, FailureMessage.WriteExpectedLine);
      ShowNodeStack (actualNode, FailureMessage.WriteActualLine);
    }
  }
}
