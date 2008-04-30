using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.MixinTestDomain;

namespace Remotion.Data.DomainObjects.UnitTests.Core.Configuration.Mapping.PropertyFinderTests
{
  [TestFixture]
  public class PropertyFinderBaseMixinsTest : PropertyFinderBaseTestBase
  {
    [Test]
    public void FindPropertyInfos_ForInheritanceRoot ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (TargetClassA), true);

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (TargetClassA))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (TargetClassBase), "P0"),
                      GetProperty (typeof (MixinBase), "P0a"),
                      GetProperty (typeof (TargetClassA), "P1"),
                      GetProperty (typeof (TargetClassA), "P2"),
                      GetProperty (typeof (MixinA), "P5"),
                      GetProperty (typeof (MixinC), "P7"),
                      GetProperty (typeof (MixinD), "P8"),
                  }));
    }

    [Test]
    public void FindPropertyInfos_ForDerived ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (TargetClassB), false);

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (TargetClassB))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (TargetClassB), "P3"),
                      GetProperty (typeof (TargetClassB), "P4"),
                      GetProperty (typeof (MixinB), "P6"),
                      GetProperty (typeof (MixinE), "P9"),
                  }));
    }

    [Test]
    [Ignore ("TODO: FS - Implement derived mixins")]
    public void FindPropertyInfos_ForDerivedMixinNotOnBase ()
    {
      PropertyFinderBase propertyFinder = new StubPropertyFinderBase (typeof (TargetClassC), false);

      Assert.That (
          propertyFinder.FindPropertyInfos (CreateReflectionBasedClassDefinition (typeof (TargetClassC))),
          Is.EquivalentTo (
              new PropertyInfo[]
                  {
                      GetProperty (typeof (DerivedMixinNotOnBase), "DerivedMixinProperty"),
                      GetProperty (typeof (MixinNotOnBase), "MixinProperty"),
                  }));
    }
  }
}