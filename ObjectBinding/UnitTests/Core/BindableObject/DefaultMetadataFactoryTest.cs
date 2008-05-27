using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  [TestFixture]
  public class DefaultMetadataFactoryTest
  {
    public class TestClass
    {
      public int Property { get { return 0; } }
    }

    [Test]
    public void CreatePropertyFinder ()
    {
      IPropertyFinder finder = DefaultMetadataFactory.Instance.CreatePropertyFinder (typeof (TestClass));
      Assert.AreSame (typeof (ReflectionBasedPropertyFinder), finder.GetType());
      Assert.AreSame (typeof (TestClass), new List<IPropertyInformation> (finder.GetPropertyInfos ())[0].DeclaringType);
    }

    [Test]
    public void CreatePropertyReflector ()
    {
      IPropertyInformation property = new PropertyInfoAdapter (typeof (TestClass).GetProperty ("Property"));
      PropertyReflector propertyReflector = DefaultMetadataFactory.Instance.CreatePropertyReflector (typeof (TestClass), property, new BindableObjectProvider());
      Assert.AreSame (typeof (PropertyReflector), propertyReflector.GetType ());
      Assert.AreSame (property, propertyReflector.PropertyInfo);
    }
  }
}