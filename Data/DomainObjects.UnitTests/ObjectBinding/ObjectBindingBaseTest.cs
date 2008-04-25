using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.ObjectBinding.BindableObject;
using Remotion.Security;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding
{
  public class ObjectBindingBaseTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      SecurityAdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      BindableObjectProvider.SetCurrent (null);
    }

    public override void TearDown ()
    {
      BindableObjectProvider.SetCurrent (null);
      base.TearDown ();
    }

    protected PropertyInfo GetPropertyInfo (Type type, string propertyName)
    {
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.Instance);
      Assert.IsNotNull (propertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return propertyInfo;
    }
  }
}