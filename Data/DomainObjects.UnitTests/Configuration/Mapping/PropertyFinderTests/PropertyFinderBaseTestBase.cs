using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Configuration.Mapping.PropertyFinderTests
{
  public class PropertyFinderBaseTestBase
  {
    protected PropertyInfo GetProperty (Type type, string propertyName)
    {
      PropertyInfo propertyInfo =
          type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly);
      Assert.That (propertyInfo, Is.Not.Null, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return propertyInfo;
    }

    protected ReflectionBasedClassDefinition CreateReflectionBasedClassDefinition (Type type)
    {
      return new ReflectionBasedClassDefinition (type.Name, type.Name, "TestDomain", type, false, PersistentMixinFinder.GetPersistentMixins (type));
    }
  }
}