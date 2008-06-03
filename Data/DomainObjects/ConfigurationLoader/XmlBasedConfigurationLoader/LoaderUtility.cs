/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.IO;
using System.Xml;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public static class LoaderUtility
  {
    public static Type GetType (string typeName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("typeName", typeName);

      return TypeUtility.GetType (typeName.Trim (), true);    
    }

    public static Type GetType (XmlNode node)
    {
      ArgumentUtility.CheckNotNull ("node", node);

      return GetType (node.InnerText);    
    }

    public static Type GetType (XmlNode node, string xPath, XmlNamespaceManager namespaceManager)
    {
      ArgumentUtility.CheckNotNull ("node", node);
      ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);
      ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);

      return GetType (node.SelectSingleNode (xPath, namespaceManager));
    }

    public static Type GetOptionalType (XmlNode selectionNode, string xPath, XmlNamespaceManager namespaceManager)
    {
      ArgumentUtility.CheckNotNull ("selectionNode", selectionNode);
      ArgumentUtility.CheckNotNullOrEmpty ("xPath", xPath);
      ArgumentUtility.CheckNotNull ("namespaceManager", namespaceManager);
    
      XmlNode typeNode = selectionNode.SelectSingleNode (xPath, namespaceManager);

      if (typeNode != null)
        return GetType (typeNode);
      else
        return null;
    }

    public static string GetConfigurationFileName (string appSettingKey, string defaultFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("appSettingKey", appSettingKey);
      ArgumentUtility.CheckNotNullOrEmpty ("defaultFileName", defaultFileName);

      string fileName = ConfigurationWrapper.Current.GetAppSetting (appSettingKey, false);
      if (fileName != null)
        return fileName;

      return Path.Combine (ReflectionUtility.GetConfigFileDirectory (), defaultFileName);
    }
  }
}
