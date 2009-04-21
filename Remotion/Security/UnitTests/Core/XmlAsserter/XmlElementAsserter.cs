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
using System.Xml;
using NUnit.Framework;

namespace Remotion.Security.UnitTests.Core.XmlAsserter
{
  [CLSCompliant (false)]
#pragma warning disable 612,618 // Asserters are obsolete
  public class XmlElementAsserter : AbstractAsserter
#pragma warning restore 612,618
  {
    private string _expectedLocalName;
    private string _expectedNamespace;
    private XmlNode _actualNode;

    public XmlElementAsserter (string expectedNamespace, string expectedLocalName, XmlNode actualNode, string message, params object[] args)
      : base (message, args)
    {
      _expectedLocalName = expectedLocalName;
      _expectedNamespace = expectedNamespace;
      _actualNode = actualNode;
    }

    public override bool Test ()
    {
      return _actualNode != null 
          && _actualNode.NodeType == XmlNodeType.Element
          && _actualNode.NamespaceURI.Equals (_expectedNamespace)
          && _actualNode.LocalName.Equals (_expectedLocalName);
    }

    public override string Message
    {
      get
      {
        FailureMessage.DisplayExpectedValue (_expectedNamespace + ":" + _expectedLocalName);
        FailureMessage.DisplayActualValue (_actualNode.NamespaceURI + ":" + _actualNode.LocalName);
        return FailureMessage.ToString ();
      }
    }
  }
}
