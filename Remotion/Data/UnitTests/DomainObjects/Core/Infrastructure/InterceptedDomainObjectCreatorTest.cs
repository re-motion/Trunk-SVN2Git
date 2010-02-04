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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.Interception;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Mixins;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class InterceptedDomainObjectCreatorTest : ClientTransactionBaseTest
  {
    [Test]
    public void CreateObjectReference ()
    {
      var order = InterceptedDomainObjectCreator.Instance.CreateObjectReference (DomainObjectIDs.Order1, null);

      Assert.That (order, Is.InstanceOfType (typeof (Order)));
      Assert.That (order.ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void CreateObjectReference_UsesFactoryGeneratedType ()
    {
      var order = InterceptedDomainObjectCreator.Instance.CreateObjectReference (DomainObjectIDs.Order1, null);

      var factory = InterceptedDomainObjectCreator.Instance.Factory;
      Assert.That (factory.WasCreatedByFactory ((((object) order).GetType ())), Is.True);
    }

    [Test]
    public void CreateObjectReference_CallsNoCtor ()
    {
      var order = (Order) InterceptedDomainObjectCreator.Instance.CreateObjectReference (DomainObjectIDs.Order1, null);
      Assert.That (order.CtorCalled, Is.False);
    }

    [Test]
    public void CreateObjectReference_PreparesMixins ()
    {
      var objectID = new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid ());
      var instance = InterceptedDomainObjectCreator.Instance.CreateObjectReference (objectID, null);
      Assert.That (Mixin.Get<MixinAddingPersistentProperties> (instance), Is.Not.Null);
    }

    [Test]
    public void CreateObjectReference_InitializesObjectID ()
    {
      var instance = InterceptedDomainObjectCreator.Instance.CreateObjectReference (DomainObjectIDs.Order1, null);
      Assert.That (instance.ID, Is.EqualTo (DomainObjectIDs.Order1));
    }

    [Test]
    public void CreateObjectReference_BindingTransaction ()
    {
      var transaction = ClientTransaction.CreateRootTransaction ();

      var instance = InterceptedDomainObjectCreator.Instance.CreateObjectReference (DomainObjectIDs.Order1, transaction);

      Assert.That (instance.HasBindingTransaction, Is.True);
      Assert.That (instance.GetBindingTransaction (), Is.SameAs (transaction));
    }

    [Test]
    public void CreateObjectReference_NoBindingTransaction ()
    {
      var instance = InterceptedDomainObjectCreator.Instance.CreateObjectReference (DomainObjectIDs.Order1, null);
      Assert.That (instance.HasBindingTransaction, Is.False);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void CreateObjectReference_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        var objectID = new ObjectID (typeof (TargetClassForPersistentMixin), Guid.NewGuid ());
        InterceptedDomainObjectCreator.Instance.CreateObjectReference (objectID, null);
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
  }
}
