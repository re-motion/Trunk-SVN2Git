// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
//
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
//
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
//
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
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), null);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectWithIdentityProviderAttribute), null);
    }

    [TearDown]
    public virtual void TearDown ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableObjectProviderAttribute), null);
      BusinessObjectProvider.SetProvider (typeof (BindableObjectWithIdentityProviderAttribute), null);
    }

    protected IPropertyInformation GetPropertyInfo (Type type, string propertyName)
    {
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      Assert.IsNotNull (propertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, type);

      return new PropertyInfoAdapter (propertyInfo);
    }

    protected IPropertyInformation GetPropertyInfo (Type type, Type interfaceType, string propertyName)
    {
      PropertyInfo interfacePropertyInfo = interfaceType.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      Assert.IsNotNull (interfacePropertyInfo, "Property '{0}' was not found on type '{1}'.", propertyName, interfaceType);
      PropertyInfo propertyInfo = type.GetProperty (propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
      if (propertyInfo == null)
      {
        Type interfaceTypeDefinition = interfaceType.IsGenericType ? interfaceType.GetGenericTypeDefinition() : interfaceType;
        string explicitName = interfaceTypeDefinition.FullName.Replace ("`1", "<T>") + "." + interfacePropertyInfo.Name;
        propertyInfo = type.GetProperty (explicitName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        Assert.IsNotNull (propertyInfo, "Property '{0}' (or '{1}') was not found on type '{2}'.", propertyName, explicitName, type);
      }

      return new PropertyInfoAdapter (propertyInfo, interfacePropertyInfo);
    }

    protected Type GetUnderlyingType (PropertyReflector reflector)
    {
      return (Type) PrivateInvoke.InvokeNonPublicMethod (reflector, "GetUnderlyingType");
    }

    protected PropertyBase.Parameters GetPropertyParameters (IPropertyInformation property, BindableObjectProvider provider)
    {
      PropertyReflector reflector = PropertyReflector.Create(property, provider);
      return (PropertyBase.Parameters) PrivateInvoke.InvokeNonPublicMethod (reflector, "CreateParameters", GetUnderlyingType (reflector));
    }
  }
}
