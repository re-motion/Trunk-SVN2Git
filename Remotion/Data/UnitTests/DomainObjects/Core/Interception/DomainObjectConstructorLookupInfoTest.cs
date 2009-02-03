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