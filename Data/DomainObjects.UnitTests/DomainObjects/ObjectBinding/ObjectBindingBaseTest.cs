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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.Security;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  public class ObjectBindingBaseTest : ClientTransactionBaseTest
  {
    public override void SetUp ()
    {
      base.SetUp ();
      AdapterRegistry.Instance.SetAdapter (typeof (IObjectSecurityAdapter), null);
      BusinessObjectProvider.SetProvider (typeof(BindableDomainObjectProviderAttribute), null);
    }

    public override void TearDown ()
    {
      BusinessObjectProvider.SetProvider (typeof (BindableDomainObjectProviderAttribute), null);
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
