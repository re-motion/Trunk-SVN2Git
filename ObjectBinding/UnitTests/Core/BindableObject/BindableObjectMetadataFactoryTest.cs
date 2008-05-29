using System;
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.ObjectBinding.UnitTests.Core.TestDomain;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class BindableObjectMetadataFactoryTest
  {
    public class TestClass
    {
      public int Property
      {
        get { return 0; }
      }
    }

    [Test]
    public void Instantiate_WithMixin ()
    {
      using (MixinConfiguration.BuildNew().ForClass (typeof (BindableObjectMetadataFactory)).AddMixin<MixinStub>().EnterScope())
      {
        BindableObjectMetadataFactory factory = BindableObjectMetadataFactory.Create();
        Assert.That (factory, Is.InstanceOfType (typeof (BindableObjectMetadataFactory)));
        Assert.That (factory, Is.InstanceOfType (typeof (IMixinTarget)));
      }
    }

    [Test]
    public void CreateClassReflector ()
    {
      BindableObjectProvider provider = new BindableObjectProvider();
      IClassReflector classReflector = BindableObjectMetadataFactory.Create().CreateClassReflector (typeof (TestClass), provider);
      Assert.That (classReflector.TargetType, Is.SameAs (typeof (TestClass)));
      Assert.That (classReflector.BusinessObjectProvider, Is.SameAs (provider));
    }

    [Test]
    public void CreatePropertyFinder ()
    {
      IPropertyFinder finder = BindableObjectMetadataFactory.Create().CreatePropertyFinder (typeof (TestClass));
      Assert.AreSame (typeof (ReflectionBasedPropertyFinder), finder.GetType());
      Assert.AreSame (typeof (TestClass), new List<IPropertyInformation> (finder.GetPropertyInfos())[0].DeclaringType);
    }

    [Test]
    public void CreatePropertyReflector ()
    {
      IPropertyInformation property = new PropertyInfoAdapter (typeof (TestClass).GetProperty ("Property"));
      PropertyReflector propertyReflector =
          BindableObjectMetadataFactory.Create().CreatePropertyReflector (typeof (TestClass), property, new BindableObjectProvider());
      Assert.AreSame (typeof (PropertyReflector), propertyReflector.GetType());
      Assert.AreSame (property, propertyReflector.PropertyInfo);
    }
  }
}