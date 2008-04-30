using System;
using System.Collections.Generic;
using System.Reflection;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.UnitTests.TestDomain;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyReflectorTests
{
  public class BaseTest: StandardMappingTest
  {
    protected PropertyReflector CreatePropertyReflector<T> (string property)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("property", property);

      Type type = typeof (T);
      PropertyInfo propertyInfo = type.GetProperty (property, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
      ReflectionBasedClassDefinition classDefinition;
      if (typeof (DomainObject).IsAssignableFrom (type))
        classDefinition = new ReflectionBasedClassDefinition (type.Name, type.Name, c_testDomainProviderID, type, true,
          new List<Type> ());
      else
        classDefinition = new ReflectionBasedClassDefinition ("Order", "Order", c_testDomainProviderID, typeof (Order), false,
          new List<Type> ());

      return new PropertyReflector (classDefinition, propertyInfo);
    }
  }
}