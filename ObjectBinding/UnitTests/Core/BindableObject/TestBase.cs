using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Security;

namespace Remotion.ObjectBinding.UnitTests.Core.BindableObject
{
  public class TestBase
  {
    [SetUp]
    public virtual void SetUp ()
    {
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      BindableObjectProvider.SetCurrent (null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      BindableObjectProvider.SetCurrent (null);      
    }

    protected IPropertyInformation GetPropertyInfo (Type type, string propertyName)
    {
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      Assert.IsNotNull (propertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return new PropertyInfoAdapter (propertyInfo);
    }

    protected Type GetUnderlyingType (PropertyReflector reflector)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (reflector, "GetUnderlyingType");
    }

    protected PropertyBase.Parameters GetPropertyParameters (IPropertyInformation property, BindableObjectProvider provider)
    {
      PropertyReflector reflector = new PropertyReflector (property, provider);
      return (PropertyBase.Parameters) PrivateInvoke.InvokeNonPublicMethod (reflector, "CreateParameters", GetUnderlyingType (reflector));
    }
  }
}