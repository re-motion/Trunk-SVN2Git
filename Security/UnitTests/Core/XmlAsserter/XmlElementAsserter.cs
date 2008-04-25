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
