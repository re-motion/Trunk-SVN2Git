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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class PropertyIntegrationTest : ObjectBindingBaseTest
  {
    [Test]
    public void TestPropertyAccess ()
    {
      BindableSampleDomainObject instance = BindableSampleDomainObject.NewObject ();
      IBusinessObject instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.IsNull (instanceAsIBusinessObject.GetProperty ("Int32"));

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (0, instance.Int32);
        Assert.AreEqual (0, instanceAsIBusinessObject.GetProperty ("Int32"));
      }

      instanceAsIBusinessObject.SetProperty ("Int32", 1);
      Assert.AreEqual (1, instance.Int32);
      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (1, instance.Int32);
        Assert.AreEqual (1, instanceAsIBusinessObject.GetProperty ("Int32"));
      }

      instance.Int32 = 2;
      Assert.AreEqual (2, instanceAsIBusinessObject.GetProperty ("Int32"));
      Assert.AreEqual ("2", instanceAsIBusinessObject.GetPropertyString ("Int32"));

      instance.Int32 = 1;
      Assert.AreEqual (1, instanceAsIBusinessObject.GetProperty ("Int32"));

      instance.Int32 = 0;
      Assert.AreEqual (0, instanceAsIBusinessObject.GetProperty ("Int32"));

      using (ClientTransactionMock.CreateSubTransaction ().EnterDiscardingScope ())
      {
        Assert.AreEqual (0, instance.Int32);
        Assert.AreEqual (0, instanceAsIBusinessObject.GetProperty ("Int32"));
      }
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      BindableObjectClass bindableObjectClass = BindableObjectProvider.GetBindableObjectClass (typeof (BindableSampleDomainObject));
      IBusinessObjectProperty[] properties = bindableObjectClass.GetPropertyDefinitions ();
      string[] propertiesByName =
          Array.ConvertAll<IBusinessObjectProperty, string> (properties, delegate (IBusinessObjectProperty property) { return property.Identifier; });
      Assert.That (propertiesByName, Is.EquivalentTo (new string[] { "Name", "Int32" }));
    }
  }
}
