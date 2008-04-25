using System;
using System.Configuration;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.Mapping.Configuration
{
  public class MappingLoaderConfiguration: ExtendedConfigurationSection
  {
    private readonly ConfigurationPropertyCollection _properties = new ConfigurationPropertyCollection();

    private readonly ConfigurationProperty _mappingLoaderProperty;

    private readonly ConfigurationProperty _domainObjectFactoryProperty;
    private readonly DoubleCheckedLockingContainer<IDomainObjectFactory> _domainObjectFactory;

    public MappingLoaderConfiguration()
    {
      _mappingLoaderProperty = new ConfigurationProperty (
          "loader",
          typeof (TypeElement<IMappingLoader, MappingReflector>),
          null,
          ConfigurationPropertyOptions.None);

      _domainObjectFactory =
          new DoubleCheckedLockingContainer<IDomainObjectFactory> (delegate { return DomainObjectFactoryElement.CreateInstance(); });
      _domainObjectFactoryProperty = new ConfigurationProperty (
          "domainObjectFactory",
          typeof (TypeElement<IDomainObjectFactory, InterceptedDomainObjectFactory>),
          null,
          ConfigurationPropertyOptions.None);

      _properties.Add (_mappingLoaderProperty);
      _properties.Add (_domainObjectFactoryProperty);
    }

    protected override ConfigurationPropertyCollection Properties
    {
      get { return _properties; }
    }

    public IMappingLoader CreateMappingLoader()
    {
      return MappingLoaderElement.CreateInstance();
    }

    public Type MappingLoaderType
    {
      get { return MappingLoaderElement.Type; }
      set { MappingLoaderElement.Type = value; }
    }

    protected TypeElement<IMappingLoader> MappingLoaderElement
    {
      get { return (TypeElement<IMappingLoader>) this[_mappingLoaderProperty]; }
    }

    public IDomainObjectFactory DomainObjectFactory
    {
      get { return _domainObjectFactory.Value; }
      set { _domainObjectFactory.Value = value; }
    }

    protected TypeElement<IDomainObjectFactory> DomainObjectFactoryElement
    {
      get { return (TypeElement<IDomainObjectFactory>) this[_domainObjectFactoryProperty]; }
    }
  }
}