using System;

namespace Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader
{
  public abstract class NullablePropertyAttribute: Attribute, INullablePropertyAttribute
  {
    private bool _isNullable = true;

    protected NullablePropertyAttribute()
    {
    }

    public bool IsNullable
    {
      get { return _isNullable; }
      set { _isNullable = value; }
    }
  }
}