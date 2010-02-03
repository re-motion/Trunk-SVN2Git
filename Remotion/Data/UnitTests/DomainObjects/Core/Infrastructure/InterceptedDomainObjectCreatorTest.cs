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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class InterceptedDomainObjectCreatorTest : ClientTransactionBaseTest
  {
    [Test]
    public void CreateWithDataContainer_UsesFactoryGeneratedType ()
    {
      var dataContainer = CreateDataContainer (typeof (Order), ClientTransactionMock);
      var order = InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (order, Is.InstanceOfType (typeof (Order)));
      var factory = InterceptedDomainObjectCreator.Instance.Factory;
      Assert.That (factory.WasCreatedByFactory ((((object) order).GetType ())), Is.True);
    }

    [Test]
    public void CreateWithDataContainer_CallsNoCtor ()
    {
      var dataContainer = CreateDataContainer (typeof (Order), ClientTransactionMock);
      var order = (Order) InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (order.CtorCalled, Is.False);
    }

    [Test]
    public void CreateWithDataContainer_PreparesMixins ()
    {
      var dataContainer = CreateDataContainer (typeof (TargetClassForPersistentMixin), ClientTransactionMock);
      var instance = InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (Mixin.Get <MixinAddingPersistentProperties>(instance), Is.Not.Null);
    }

    [Test]
    public void CreateWithDataContainer_SetsDataContainerDomainObject ()
    {
      var dataContainer = CreateDataContainer (typeof (Order), ClientTransactionMock);
      var instance = InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (dataContainer.DomainObject, Is.SameAs (instance));
    }

    [Test]
    public void CreateWithDataContainer_InitializesDomainObject ()
    {
      var dataContainer = CreateDataContainer (typeof (Order), ClientTransactionMock);
      var instance = InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (instance.ID, Is.EqualTo(dataContainer.ID));
    }

    [Test]
    public void CreateWithDataContainer_EnlistsDomainObject ()
    {
      var dataContainer = CreateDataContainer (typeof (Order), ClientTransactionMock);
      var instance = InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (dataContainer.ClientTransaction.IsEnlisted (instance), Is.True);
    }

    [Test]
    public void CreateWithDataContainer_BindsDomainObjectToBindingClientTransaction ()
    {
      var transaction = ClientTransaction.CreateBindingTransaction ();
      var dataContainer = CreateDataContainer (typeof (Order), transaction);

      var instance = InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (dataContainer.ClientTransaction.IsEnlisted (instance), Is.True);
      Assert.That (instance.HasBindingTransaction, Is.True);
      Assert.That (instance.GetBindingTransaction(), Is.SameAs (transaction));
    }

    [Test]
    public void CreateWithDataContainer_DoesntBindDomainObjectToOtherTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();
      var dataContainer = CreateDataContainer (typeof (Order), transaction);

      var instance = InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (dataContainer.ClientTransaction.IsEnlisted (instance), Is.True);
      Assert.That (instance.HasBindingTransaction, Is.False);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void CreateWithDataContainer_ValidatesMixinConfiguration ()
    {
      var dataContainer = CreateDataContainer (typeof (TargetClassForPersistentMixin), ClientTransactionMock);
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        InterceptedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      }
    }

    [Test]
    public void GetConstructorLookupInfo_UsesFactoryGeneratedType ()
    {
      var info = InterceptedDomainObjectCreator.Instance.GetConstructorLookupInfo (typeof (Order));
      var factory = InterceptedDomainObjectCreator.Instance.Factory;
      Assert.That (factory.WasCreatedByFactory (info.DefiningType), Is.True);
    }

    [Test]
    public void GetConstructorLookupInfo_SpecifiesCorrectPublicType ()
    {
      var info = (DomainObjectConstructorLookupInfo) InterceptedDomainObjectCreator.Instance.GetConstructorLookupInfo (typeof (Order));
      Assert.That (info.PublicDomainObjectType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void GetConstructorLookupInfo_BindingFlags ()
    {
      var info = InterceptedDomainObjectCreator.Instance.GetConstructorLookupInfo (typeof (Order));
      Assert.That (info.BindingFlags, Is.EqualTo (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void GetConstructorLookupInfo_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        InterceptedDomainObjectCreator.Instance.GetConstructorLookupInfo(typeof (TargetClassForPersistentMixin));
      }
    }

    private DataContainer CreateDataContainer (Type type, ClientTransaction clientTransaction)
    {
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (clientTransaction, "CreateNewDataContainer", type);
    }
  }
}
