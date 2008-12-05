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
