// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
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
      SampleBindableMixinDomainObject instance = SampleBindableMixinDomainObject.NewObject();
      var instanceAsIBusinessObject = (IBusinessObject) instance;

      Assert.IsNull (instanceAsIBusinessObject.GetProperty ("Int32"));

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (0, instance.Int32);
        Assert.AreEqual (0, instanceAsIBusinessObject.GetProperty ("Int32"));
      }

      instanceAsIBusinessObject.SetProperty ("Int32", 1);
      Assert.AreEqual (1, instance.Int32);
      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
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

      using (ClientTransactionMock.CreateSubTransaction().EnterDiscardingScope())
      {
        Assert.AreEqual (0, instance.Int32);
        Assert.AreEqual (0, instanceAsIBusinessObject.GetProperty ("Int32"));
      }
    }

    [Test]
    public void GetPropertyDefinitions ()
    {
      var provider = BindableObjectProvider.GetProviderForBindableObjectType (typeof (SampleBindableMixinDomainObject));
      var bindableObjectClass = provider.GetBindableObjectClass (typeof (SampleBindableMixinDomainObject));

      var properties = bindableObjectClass.GetPropertyDefinitions();

      var propertiesByName = Array.ConvertAll (properties, property => property.Identifier);
      Assert.That (propertiesByName, Is.EquivalentTo (new[] { "Name", "Int32" }));
    }
  }
}