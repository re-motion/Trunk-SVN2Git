using System;
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Design
{
  public class FakeDesignModeMappingNameResolver : IMappingNameResolver
  {
    public string GetPropertyName (PropertyInfo property)
    {
      throw new System.NotImplementedException();
    }

    public string GetPropertyName (Type originalDeclaringType, string shortPropertyName)
    {
      throw new System.NotImplementedException();
    }

    public PropertyInfo GetProperty (Type concreteType, string propertyName)
    {
      throw new System.NotImplementedException();
    }
  }
}