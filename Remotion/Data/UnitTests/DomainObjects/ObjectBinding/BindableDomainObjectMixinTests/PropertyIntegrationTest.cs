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
      SampleBindableMixinDomainObject instance = SampleBindableMixinDomainObject.NewObject ();
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
      BindableObjectClass bindableObjectClass = BindableObjectProvider.GetBindableObjectClass (typeof (SampleBindableMixinDomainObject));
      IBusinessObjectProperty[] properties = bindableObjectClass.GetPropertyDefinitions ();
      string[] propertiesByName =
          Array.ConvertAll<IBusinessObjectProperty, string> (properties, delegate (IBusinessObjectProperty property) { return property.Identifier; });
      Assert.That (propertiesByName, Is.EquivalentTo (new string[] { "Name", "Int32" }));
    }
  }
}
