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
// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception
{
  [TestFixture]
  public class DomainObjectConstructorLookupInfoTest : ClientTransactionBaseTest
  {
    public InterceptedDomainObjectTypeFactory Factory
    {
      get { return SetUpFixture.Factory; }
    }

    [Test]
    public void Initialization ()
    {
      var concreteDomainObjectType = Factory.GetConcreteDomainObjectType (typeof (Order));
      var lookupInfo = new DomainObjectConstructorLookupInfo (typeof (Order), concreteDomainObjectType, BindingFlags.Public | BindingFlags.Instance);

      Assert.That (lookupInfo.DefiningType, Is.SameAs (concreteDomainObjectType));
      Assert.That (lookupInfo.PublicDomainObjectType, Is.SameAs (typeof (Order)));
      Assert.That (lookupInfo.BindingFlags, Is.EqualTo (BindingFlags.Public | BindingFlags.Instance));
    }

    [Test]
    public void GetDelegate ()
    {
      var concreteDomainObjectType = Factory.GetConcreteDomainObjectType (typeof (Order));
      var lookupInfo = new DomainObjectConstructorLookupInfo (typeof (Order), concreteDomainObjectType, BindingFlags.Public | BindingFlags.Instance);
      var del = lookupInfo.GetDelegate (typeof (Func<Order>));
      Assert.That (del, Is.InstanceOfType (typeof (Func<Order>)));
      Assert.That (((Func<Order>) del) (), Is.InstanceOfType (concreteDomainObjectType));
    }

    [Test]
    [ExpectedException (typeof (MissingMethodException), ExpectedMessage = "Type Remotion.Data.UnitTests.DomainObjects.TestDomain.Order does not " 
        + "support the requested constructor with signature (System.Int32, System.Int32, System.Int32).")]
    public void GetDelegate_InvalidArgTypes ()
    {
      var concreteDomainObjectType = Factory.GetConcreteDomainObjectType (typeof (Order));
      var lookupInfo = new DomainObjectConstructorLookupInfo (typeof (Order), concreteDomainObjectType, BindingFlags.Public | BindingFlags.Instance);
      lookupInfo.GetDelegate (typeof (Func<int, int, int, Order>));
    }

    [Test]
    public void Integration_GetTypesafeConstructorInvoker ()
    {
      var concreteDomainObjectType = Factory.GetConcreteDomainObjectType (typeof (Order));
      var lookupInfo = new DomainObjectConstructorLookupInfo (typeof (Order), concreteDomainObjectType,
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      var order = (Order) ParamList.Empty.InvokeConstructor (lookupInfo);
      Assert.IsNotNull (order);
      Assert.AreSame (concreteDomainObjectType, ((object) order).GetType ());
    }

    [Test]
    public void Integration_WithConstructors ()
    {
      var concreteDomainObjectType = Factory.GetConcreteDomainObjectType (typeof (DOWithConstructors));
      var lookupInfo = new DomainObjectConstructorLookupInfo (typeof (DOWithConstructors), concreteDomainObjectType, 
          BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

      var instance = (DOWithConstructors) ParamList.Create ("17", "4").InvokeConstructor (lookupInfo);
      Assert.IsNotNull (instance);
      Assert.AreEqual ("17", instance.FirstArg);
      Assert.AreEqual ("4", instance.SecondArg);
    }
  }
}
