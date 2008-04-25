using System;
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public class ConfigurationNamespaceManager : XmlNamespaceManager
  {
    // types

    // static members and constants

    // member fields

    private PrefixNamespace[] _configurationNamespaces;

    // construction and disposing

    public ConfigurationNamespaceManager (
        XmlDocument document, 
        PrefixNamespace[] configurationNamespaces)
        : base (document.NameTable)
    {
      ArgumentUtility.CheckNotNull ("document", document);
      ArgumentUtility.CheckNotNull ("configurationNamespaces", configurationNamespaces);

      foreach (PrefixNamespace configurationNamespace in configurationNamespaces)
      {
        base.AddNamespace (configurationNamespace.Prefix, configurationNamespace.Uri);
      }

      _configurationNamespaces = configurationNamespaces;
    }

    // methods and properties

    public PrefixNamespace[] ConfigurationNamespaces
    {
      get { return _configurationNamespaces; }
    }

    public PrefixNamespace this[string uri]
    {
      get
      {
        ArgumentUtility.CheckNotNullOrEmpty ("uri", uri);

        foreach (PrefixNamespace configurationNamespace in _configurationNamespaces)
        {
          if (configurationNamespace.Uri == uri)
            return configurationNamespace;
        }

        throw new IndexOutOfRangeException (string.Format ("Uri '{0}' could not be found.", uri));
      }
    }

    public string FormatXPath (string xPath, params string[] uris)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);

      string formattedXPath = xPath;
      for (int i = 0; i < uris.Length; i++)
      {
        formattedXPath = formattedXPath.Replace ("{" + i.ToString () + "}", this[uris[i]].Prefix);   
      }

      return formattedXPath;
    }
  }
}
