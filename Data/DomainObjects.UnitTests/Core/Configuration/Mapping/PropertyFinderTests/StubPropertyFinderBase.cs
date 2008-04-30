using System;
using System.Collections.Generic;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyFinderTests
{
  public class StubPropertyFinderBase : PropertyFinderBase
  {
    public StubPropertyFinderBase (Type type, bool includeBaseProperties)
      : this (type, includeBaseProperties, PersistentMixinFinder.GetPersistentMixins (type))
    {
    }

    public StubPropertyFinderBase (Type type, bool includeBaseProperties, IEnumerable<Type> persistentMixins)
      : base (type, includeBaseProperties, persistentMixins)
    {
    }
  }
}