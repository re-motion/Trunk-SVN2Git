/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System.Configuration;
using System.Xml;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Configuration
{
  /// <summary>
  /// The <see cref="ConfigurationHelper"/> is a ulitilty class designed to deserialize xml-fragments into configuration elements.
  /// </summary>
  public static class ConfigurationHelper
  {
    public static void DeserializeElement (ConfigurationElement configurationElement, string xmlFragment)
    {
      ArgumentUtility.CheckNotNull ("configurationElement", configurationElement);
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      using (XmlTextReader reader = new XmlTextReader (xmlFragment, XmlNodeType.Document, null))
      {
        reader.WhitespaceHandling = WhitespaceHandling.None;
        reader.IsStartElement();
        PrivateInvoke.InvokeNonPublicMethod (configurationElement, "DeserializeElement", reader, false);
      }
    }

    public static void DeserializeSection (ConfigurationSection configurationSection, string xmlFragment)
    {
      ArgumentUtility.CheckNotNull ("configurationSection", configurationSection);
      ArgumentUtility.CheckNotNullOrEmpty ("xmlFragment", xmlFragment);

      using (XmlTextReader reader = new XmlTextReader (xmlFragment, XmlNodeType.Document, null))
      {
        reader.WhitespaceHandling = WhitespaceHandling.None;
        PrivateInvoke.InvokeNonPublicMethod (configurationSection, "DeserializeSection", reader);
      }
    }
  }
}
