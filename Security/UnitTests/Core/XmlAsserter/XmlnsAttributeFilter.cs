using System;
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Security.UnitTests.Core.XmlAsserter
{
  public class XmlnsAttributeEventArgs : EventArgs
  {
    private string _namespaceUri;
    private string _prefix;
    private bool _isDefaultNamespace;

    public XmlnsAttributeEventArgs (string namespaceUri, string prefix)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("namespaceUri", namespaceUri);

      _namespaceUri = namespaceUri;
      _prefix = prefix;
      _isDefaultNamespace = string.IsNullOrEmpty (prefix);
    }

    public string NamespaceUri
    {
      get { return _namespaceUri; }
    }

    public string Prefix
    {
      get { return _prefix; }
    }

    public bool IsDefaultNamespace
    {
      get { return _isDefaultNamespace; }
    }
  }

  public delegate void XmlnsAttributeEventHandler (object sender, XmlnsAttributeEventArgs args);

  public class XmlnsAttributeHandler
  {
    public event XmlnsAttributeEventHandler XmlnsAttributeFound;

    private bool _filter;

    public XmlnsAttributeHandler ()
    {
      _filter = true;
    }

    public bool Filter
    {
      get { return _filter; }
      set { _filter = value; }
    }

    public bool IsXmlnsAttribute (XmlAttribute attribute)
    {
      if (attribute.NamespaceURI != "http://www.w3.org/2000/xmlns/")
        return false;

      if (attribute.LocalName == "xmlns")
        return true;

      return attribute.Prefix == "xmlns";
    }

    public void Handle (XmlAttributeCollection attributes)
    {
      for (int i = attributes.Count - 1; i >= 0; i--)
      {
        if (IsXmlnsAttribute (attributes[i]))
        {
          ExtractNamespaceInformation (attributes[i]);

          if (_filter)
            attributes.Remove (attributes[i]);
        }
      }
    }

    private void ExtractNamespaceInformation (XmlAttribute attribute)
    {
      if (attribute.LocalName == "xmlns")
        OnXmlnsAttributeFound (new XmlnsAttributeEventArgs (attribute.Value, null));

      if (attribute.Prefix == "xmlns")
        OnXmlnsAttributeFound (new XmlnsAttributeEventArgs (attribute.Value, attribute.LocalName));
    }

    protected virtual void OnXmlnsAttributeFound (XmlnsAttributeEventArgs args)
    {
      if (XmlnsAttributeFound != null)
        XmlnsAttributeFound (this, args);
    }
  }
}
