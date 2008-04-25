using System;
using System.Configuration;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader.XmlBasedConfigurationLoader;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  /// <summary>
  /// Represents the current query configuration.
  /// </summary>
  public class QueryConfiguration : ExtendedConfigurationSection
  {
    public static readonly string DefaultConfigurationFile = QueryFileElement.GetRootedPath ("queries.xml");

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
        return new QueryConfigurationLoader (DefaultConfigurationFile).GetQueryDefinitions ();
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