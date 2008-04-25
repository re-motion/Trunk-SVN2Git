using System;
using System.Configuration;
using System.IO;
using Remotion.Configuration;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Queries.Configuration
{
  public class QueryFileElement : ConfigurationElement, INamedConfigurationElement
  {
    public static string GetRootedPath (string path)
    {
      ArgumentUtility.CheckNotNull ("path", path);
      if (Path.IsPathRooted (path))
        return Path.GetFullPath (path);
      else
        return Path.GetFullPath (Path.Combine (ReflectionUtility.GetConfigFileDirectory (), path));
    }

    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    private readonly ConfigurationProperty _queryFileFileNameProperty;

    public QueryFileElement ()
    {
      _queryFileFileNameProperty = new ConfigurationProperty (
          "filename",
          typeof (string),
          null,
          ConfigurationPropertyOptions.IsRequired | ConfigurationPropertyOptions.IsKey);

      _properties.Add (_queryFileFileNameProperty);
    }

    public QueryFileElement (string fileName) : this()
    {
      ArgumentUtility.CheckNotNull ("fileName", fileName);

      FileName = fileName;
    }

    public string FileName
    {
      get { return (string) this[_queryFileFileNameProperty]; }
      protected set { this[_queryFileFileNameProperty] = value; }
    }

    public string RootedFileName
    {
      get { return GetRootedPath (FileName); }
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    string INamedConfigurationElement.Name
    {
      get { return FileName; }
    }
  }
}