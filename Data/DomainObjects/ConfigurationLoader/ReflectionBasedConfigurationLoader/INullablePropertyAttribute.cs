namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public interface INullablePropertyAttribute: IMappingAttribute
  {
    bool IsNullable { get; }
  }
}