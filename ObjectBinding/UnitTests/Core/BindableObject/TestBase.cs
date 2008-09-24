/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
