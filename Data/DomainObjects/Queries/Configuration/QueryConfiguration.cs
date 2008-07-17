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
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Represents the current query configuration.
  /// </summary>
  public class QueryConfiguration : ExtendedConfigurationSection
  {
    private const string c_defaultConfigurationFile = "queries.xml";

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();
    private readonly ConfigurationProperty _queryFilesProperty;

    private readonly DoubleCheckedLockingContainer<QueryDefinitionCollection> _queries;

    public QueryConfiguration ()
    {
      _queries = new DoubleCheckedLockingContainer<QueryDefinitionCollection> (delegate { return LoadAllQueryDefinitions (); });

      _queryFilesProperty = new ConfigurationProperty (
          "queryFiles",
          typeof (ConfigurationElementCollection<QueryFileElement>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_queryFilesProperty);
    }

    public QueryConfiguration (params string[] configurationFiles) : this()
    {
      ArgumentUtility.CheckNotNull ("configurationFiles", configurationFiles);

      for (int i = 0; i < configurationFiles.Length; i++)
      {
        string configurationFile = configurationFiles[i];
        QueryFileElement element = new QueryFileElement (configurationFile);
        QueryFiles.Add (element);
      }
    }

    private QueryDefinitionCollection LoadAllQueryDefinitions ()
    {
      if (QueryFiles.Count == 0)
        return new QueryConfigurationLoader (GetDefaultQueryFilePath()).GetQueryDefinitions ();
      else
      {
        QueryDefinitionCollection result = new QueryDefinitionCollection ();

        for (int i = 0; i < QueryFiles.Count; i++)
        {
          QueryConfigurationLoader loader = new QueryConfigurationLoader (QueryFiles[i].RootedFileName);
            QueryDefinitionCollection queryDefinitions = loader.GetQueryDefinitions ();
          try
          {
            result.Merge (queryDefinitions);
          }
          catch (DuplicateQueryDefinitionException ex)
          {
            string message = string.Format ("File '{0}' defines a duplicate for query definition '{1}'.", QueryFiles[i].RootedFileName,
              ex.QueryDefinition.ID);
            throw new ConfigurationException (message);
          }
        }
        return result;
      }
    }

    public string GetDefaultQueryFilePath ()
    {
      List<string> potentialPaths = GetPotentialDefaultQueryFilePaths();

      string path = null;
      foreach (string potentialPath in potentialPaths)
      {
        if (File.Exists (potentialPath))
        {
          if (path != null)
          {
            string message = string.Format ("Two default query configuration files found: '{0}' and '{1}'.", path, potentialPath);
            throw new ConfigurationException (message);
          }
          path = potentialPath;
        }
      }

      if (path == null)
      {
        string message = string.Format ("No default query file found. Searched for one of the following files:\n{0}",
            SeparatedStringBuilder.Build ("\n", potentialPaths));
        throw new ConfigurationException (message);
      }
      return path;
    }

    private List<string> GetPotentialDefaultQueryFilePaths ()
    {
      List<string> potentialPaths = new List<string> ();
      potentialPaths.Add (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, c_defaultConfigurationFile));
      if (AppDomain.CurrentDomain.RelativeSearchPath != null)
      {
        foreach (string part in AppDomain.CurrentDomain.RelativeSearchPath.Split (';'))
        {
          string absoluteSearchPath = Path.GetFullPath (Path.Combine (AppDomain.CurrentDomain.BaseDirectory, part));
          potentialPaths.Add (Path.Combine (absoluteSearchPath, c_defaultConfigurationFile));
        }
      }
      return potentialPaths;
    }

    public ConfigurationElementCollection<QueryFileElement> QueryFiles
    {
      get { return (ConfigurationElementCollection<QueryFileElement>) this[_queryFilesProperty]; }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public QueryDefinitionCollection QueryDefinitions
    {
      get { return _queries.Value; }
    }
  }
}
