using System;
using System.Resources;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Collections;
using Remotion.UnitTests.Globalization.SampleTypes;
using Remotion.Development.UnitTesting;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.UnitTests.Globalization
{
  [TestFixture]
  public class ResourceManagerResolverTest
  {
    private ResourceManagerResolver<MultiLingualResourcesAttribute> _resolver;

    [SetUp]
    public void SetUp ()
    {
      _resolver = new ResourceManagerResolver<MultiLingualResourcesAttribute>();
    }

    [Test]
    public void GetResourceManagers()
    {
      MultiLingualResourcesAttribute[] attributes = new MultiLingualResourcesAttribute[]
          {
            new MultiLingualResourcesAttribute ("One"),
            new MultiLingualResourcesAttribute ("Two")
          };
      ResourceManager[] resourceManagers = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);
      Assert.AreEqual (2, resourceManagers.Length);
      Assert.AreEqual ("One", resourceManagers[0].BaseName);
      Assert.AreEqual ("Two", resourceManagers[1].BaseName);
    }

    [Test]
    public void GetResourceManagers_UsesCache ()
    {
      MultiLingualResourcesAttribute[] attributes = new MultiLingualResourcesAttribute[]
          {
            new MultiLingualResourcesAttribute ("One"),
            new MultiLingualResourcesAttribute ("Two")
          };
      ResourceManager[] resourceManagers1 = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);
      ResourceManager[] resourceManagers2 = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);

      Assert.AreNotSame (resourceManagers1, resourceManagers2);
      Assert.AreSame (resourceManagers1[0], resourceManagers2[0]);
      Assert.AreSame (resourceManagers1[1], resourceManagers2[1]);
      Assert.AreNotSame (resourceManagers1[0], resourceManagers1[1]);
      Assert.AreNotSame (resourceManagers2[0], resourceManagers2[1]);
    }

    [Test]
    public void GetResourceManagers_NoSpecificAssembly ()
    {
      MultiLingualResourcesAttribute[] attributes = new MultiLingualResourcesAttribute[]
          {
            new MultiLingualResourcesAttribute ("One"),
            new MultiLingualResourcesAttribute ("Two")
          };
      ResourceManager[] resourceManagers = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);
      Assert.AreEqual (2, resourceManagers.Length);
      Assert.AreEqual (typeof (object).Assembly, PrivateInvoke.GetNonPublicField (resourceManagers[0], "MainAssembly"));
      Assert.AreEqual (typeof (object).Assembly, PrivateInvoke.GetNonPublicField (resourceManagers[1], "MainAssembly"));
    }

    [Test]
    public void GetResourceManagers_SpecificAssembly ()
    {
      MultiLingualResourcesAttribute[] attributes = new MultiLingualResourcesAttribute[]
          {
            new MultiLingualResourcesAttribute ("One"),
            new MultiLingualResourcesAttribute ("Two")
          };

      PrivateInvoke.InvokeNonPublicMethod (attributes[0], "SetResourceAssembly", typeof (ResourceManagerResolverTest).Assembly);

      ResourceManager[] resourceManagers = _resolver.GetResourceManagers (typeof (object).Assembly, attributes);
      Assert.AreEqual (2, resourceManagers.Length);
      Assert.AreEqual (typeof (ResourceManagerResolverTest).Assembly, PrivateInvoke.GetNonPublicField (resourceManagers[0], "MainAssembly"));
      Assert.AreEqual (typeof (object).Assembly, PrivateInvoke.GetNonPublicField (resourceManagers[1], "MainAssembly"));
    }

    [Test]
    public void FindFirstResourceDefinitions_SuccessOnSameType ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      _resolver.FindFirstResourceDefinitions (typeof (ClassWithMultiLingualResourcesAttributes), false, out definingType, out attributes);
      Assert.AreSame (typeof (ClassWithMultiLingualResourcesAttributes), definingType);
      Assert.AreEqual (3, attributes.Length);
      Assert.That (attributes, Is.EquivalentTo (
          AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (ClassWithMultiLingualResourcesAttributes), false)));
    }

    [Test]
    public void FindFirstResourceDefinitions_DoesNotInherit ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      _resolver.FindFirstResourceDefinitions (typeof (InheritedClassWithMultiLingualResourcesAttributes), false, out definingType, out attributes);
      Assert.AreSame (typeof (InheritedClassWithMultiLingualResourcesAttributes), definingType);
      Assert.AreEqual (2, attributes.Length);
      Assert.That (attributes, Is.EquivalentTo (
          AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (InheritedClassWithMultiLingualResourcesAttributes), false)));
    }

    [Test]
    public void FindFirstResourceDefinitions_SuccessOnBase ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      _resolver.FindFirstResourceDefinitions (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false, out definingType, out attributes);
      Assert.AreSame (typeof (ClassWithMultiLingualResourcesAttributes), definingType);
      Assert.AreEqual (3, attributes.Length);
      Assert.That (attributes, Is.EquivalentTo (
          AttributeUtility.GetCustomAttributes<MultiLingualResourcesAttribute> (typeof (ClassWithMultiLingualResourcesAttributes), false)));
    }

    [Test]
    public void FindFirstResourceDefinitions_NoSuccessNoException ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      _resolver.FindFirstResourceDefinitions (typeof (ClassWithoutMultiLingualResourcesAttributes), true, out definingType, out attributes);
      Assert.IsNull (definingType);
      Assert.IsEmpty (attributes);
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Globalization.SampleTypes."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void FindFirstResourceDefinitions_NoSuccessException ()
    {
      Type definingType;
      MultiLingualResourcesAttribute[] attributes;

      _resolver.FindFirstResourceDefinitions (typeof (ClassWithoutMultiLingualResourcesAttributes), false, out definingType, out attributes);
    }

    [Test]
    public void GetResourceManager_NoDefiningType_NoHierarchy ()
    {
      ResourceManagerSet resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false);
      Assert.AreEqual (2, resourceManagerSet.Count);
      Set<string> names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name);
      Assert.That (names, Is.EquivalentTo (new string[] { "Four", "Five" }));
    }

    [Test]
    public void GetResourceManager_NoDefiningType_NoHierarchy_SuccessOnBase()
    {
      ResourceManagerSet resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false);
      Assert.AreEqual (3, resourceManagerSet.Count);
      Set<string> names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name);
      Assert.That (names, Is.EquivalentTo (new string[] { "One", "Two", "Three" }));
    }

    [Test]
    public void GetResourceManager_NoDefiningType_Hierarchy ()
    {
      ResourceManagerSet resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true);
      Assert.AreEqual (5, resourceManagerSet.Count);
      Set<string> names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name,
          resourceManagerSet[3].Name, resourceManagerSet[4].Name);
      Assert.That (names, Is.EquivalentTo (new string[] { "One", "Two", "Three", "Four", "Five" }));
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Globalization.SampleTypes."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void GetResourceManager_NoDefiningType_NoSuccess ()
    {
      _resolver.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true);
    }

    [Test]
    public void GetResourceManager_DefiningType_NoHierarchy ()
    {
      Type definingType;
      ResourceManagerSet resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), false, out definingType);
      Assert.AreSame (typeof (InheritedClassWithMultiLingualResourcesAttributes), definingType);
      Assert.AreEqual (2, resourceManagerSet.Count);
      Set<string> names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name);
      Assert.That (names, Is.EquivalentTo (new string[] { "Four", "Five" }));
    }

    [Test]
    public void GetResourceManager_DefiningType_NoHierarchy_SuccessOnBase ()
    {
      Type definingType;
      ResourceManagerSet resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithoutMultiLingualResourcesAttributes), false, out definingType);
      Assert.AreSame (typeof (ClassWithMultiLingualResourcesAttributes), definingType);
      Assert.AreEqual (3, resourceManagerSet.Count);
      Set<string> names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name);
      Assert.That (names, Is.EquivalentTo (new string[] { "One", "Two", "Three" }));
    }

    [Test]
    public void GetResourceManager_DefiningType_Hierarchy ()
    {
      Type definingType;
      ResourceManagerSet resourceManagerSet = (ResourceManagerSet) _resolver.GetResourceManager (typeof (InheritedClassWithMultiLingualResourcesAttributes), true, out definingType);
      Assert.AreSame (typeof (InheritedClassWithMultiLingualResourcesAttributes), definingType);
      Assert.AreEqual (5, resourceManagerSet.Count);
      Set<string> names = new Set<string> (resourceManagerSet[0].Name, resourceManagerSet[1].Name, resourceManagerSet[2].Name,
          resourceManagerSet[3].Name, resourceManagerSet[4].Name);
      Assert.That (names, Is.EquivalentTo (new string[] { "One", "Two", "Three", "Four", "Five" }));
    }

    [Test]
    [ExpectedException (typeof (ResourceException), ExpectedMessage = "Type Remotion.UnitTests.Globalization.SampleTypes."
        + "ClassWithoutMultiLingualResourcesAttributes and its base classes do not define the attribute MultiLingualResourcesAttribute.")]
    public void GetResourceManager_DefiningType_NoSuccess ()
    {
      Type definingType;
      _resolver.GetResourceManager (typeof (ClassWithoutMultiLingualResourcesAttributes), true, out definingType);
    }
  }
}