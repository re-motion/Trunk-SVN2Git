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
using System.IO;
using System.Xml;
using System.Xml.Schema;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Queries.Configuration;
using Remotion.Data.DomainObjects.Schemas;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader
{
  public class QueryConfigurationLoader : BaseFileLoader
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    //TODO: resolve parameter
    public QueryConfigurationLoader (string configurationFile)
    {
      try
      {
        base.Initialize (
            configurationFile,
            SchemaLoader.Queries,
            true,
            PrefixNamespace.QueryConfigurationNamespace);
      }
      catch (ConfigurationException e)
      {
        throw CreateQueryConfigurationException (
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
      catch (XmlSchemaException e)
      {
        throw CreateQueryConfigurationException (
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
      catch (XmlException e)
      {
        throw CreateQueryConfigurationException (
            e, "Error while reading query configuration: {0} File: '{1}'.", e.Message, Path.GetFullPath (configurationFile));
      }
    }

    // methods and properties

    public QueryDefinitionCollection GetQueryDefinitions ()
    {
      QueryDefinitionCollection queries = new QueryDefinitionCollection();
      FillQueryDefinitions (queries);
      return queries;
    }

    private void FillQueryDefinitions (QueryDefinitionCollection queries)
    {
      XmlNodeList queryNodeList = Document.SelectNodes (FormatXPath ("{0}:queries/{0}:query"), NamespaceManager);

      foreach (XmlNode queryNode in queryNodeList)
        queries.Add (GetQueryDefinition (queryNode));
    }

    private QueryDefinition GetQueryDefinition (XmlNode queryNode)
    {
      string queryID = queryNode.SelectSingleNode ("@id", NamespaceManager).InnerText;
      string queryTypeAsString = queryNode.SelectSingleNode ("@type", NamespaceManager).InnerText;
      QueryType queryType = (QueryType) Enum.Parse (typeof (QueryType), queryTypeAsString, true);

      XmlNode node = queryNode.SelectSingleNode (FormatXPath ("{0}:storageGroupType"), NamespaceManager);
      string storageProviderID;
      if (node != null)
        storageProviderID = GetStorageProviderID (node.InnerText);
      else
        storageProviderID = GetStorageProviderID (null);

      string statement = queryNode.SelectSingleNode (FormatXPath ("{0}:statement"), NamespaceManager).InnerText;

      Type collectionType = LoaderUtility.GetOptionalType (queryNode, FormatXPath ("{0}:collectionType"), NamespaceManager);

      if (queryType == QueryType.Scalar && collectionType != null)
        throw CreateQueryConfigurationException ("A scalar query '{0}' must not specify a collectionType.", queryID);

      return new QueryDefinition (queryID, storageProviderID, statement, queryType, collectionType);
    }

    private string FormatXPath (string xPath)
    {
      return NamespaceManager.FormatXPath (xPath, PrefixNamespace.QueryConfigurationNamespace.Uri);
    }

    //TODO: COMMONS-842
    //TODO: Move type resolving to storagegrouplist and unify with ClassReflector
    //TODO: Test for DefaultStorageProvider
    private string GetStorageProviderID (string storageGroupName)
    {
      var defaultStorageProviderDefinition = DomainObjectsConfiguration.Current.Storage.DefaultStorageProviderDefinition;
      if (storageGroupName == null)
      {
        //TODO COMMONS-783: Test exception
        if (defaultStorageProviderDefinition == null)
          throw DomainObjectsConfiguration.Current.Storage.CreateMissingDefaultProviderException ("File: " + ConfigurationFile);
        return defaultStorageProviderDefinition.Name;
      }

      Type storageGroupType = TypeUtility.GetType (storageGroupName, true);
      string canonicalStorageGroupName = TypeUtility.GetPartialAssemblyQualifiedName (storageGroupType);
      StorageGroupElement storageGroup = DomainObjectsConfiguration.Current.Storage.StorageGroups[canonicalStorageGroupName];
      if (storageGroup == null)
      {
        //TODO COMMONS-783: Test exception
        if (defaultStorageProviderDefinition == null)
          throw DomainObjectsConfiguration.Current.Storage.CreateMissingDefaultProviderException ("File: " + ConfigurationFile);
        return defaultStorageProviderDefinition.Name;
      }
      return storageGroup.StorageProviderName;
    }

    private QueryConfigurationException CreateQueryConfigurationException (string message, params object[] args)
    {
      return CreateQueryConfigurationException (null, message, args);
    }

    private QueryConfigurationException CreateQueryConfigurationException (Exception inner, string message, params object[] args)
    {
      return new QueryConfigurationException (string.Format (message, args), inner);
    }
  }
}
