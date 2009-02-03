// Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// All rights reserved.

using System;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Configuration;
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
  public class FactoryBasedDomainObjectCreatorTest : ClientTransactionBaseTest
  {
    [Test]
    public void CreateWithDataContainer_UsesFactoryGeneratedType ()
    {
      var dataContainer = CreateDataContainer (typeof (Order));
      var order = FactoryBasedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (order, Is.InstanceOfType (typeof (Order)));
      var factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Assert.That (factory.WasCreatedByFactory ((((object) order).GetType ())), Is.True);
    }

    [Test]
    public void CreateWithDataContainer_CallsNoCtor ()
    {
      var dataContainer = CreateDataContainer (typeof (Order));
      var order = (Order) FactoryBasedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (order.CtorCalled, Is.False);
    }

    [Test]
    public void CreateWithDataContainer_PreparesMixins ()
    {
      var dataContainer = CreateDataContainer (typeof (TargetClassForPersistentMixin));
      var instance = FactoryBasedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (Mixin.Get <MixinAddingPersistentProperties>(instance), Is.Not.Null);
    }

    [Test]
    public void CreateWithDataContainer_SetsDataContainerDomainObject ()
    {
      var dataContainer = CreateDataContainer (typeof (Order));
      var instance = FactoryBasedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (dataContainer.DomainObject, Is.SameAs (instance));
    }

    [Test]
    public void CreateWithDataContainer_InitializesDomainObject ()
    {
      var dataContainer = CreateDataContainer (typeof (Order));
      var instance = FactoryBasedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      Assert.That (instance.ID, Is.EqualTo(dataContainer.ID));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void CreateWithDataContainer_ValidatesMixinConfiguration ()
    {
      var dataContainer = CreateDataContainer (typeof (TargetClassForPersistentMixin));
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        FactoryBasedDomainObjectCreator.Instance.CreateWithDataContainer (dataContainer);
      }
    }

    [Test]
    public void GetConstructorLookupInfo_UsesFactoryGeneratedType ()
    {
      var info = FactoryBasedDomainObjectCreator.Instance.GetConstructorLookupInfo (typeof (Order));
      var factory = DomainObjectsConfiguration.Current.MappingLoader.DomainObjectFactory;
      Assert.That (factory.WasCreatedByFactory (info.DefiningType), Is.True);
    }

    [Test]
    public void GetConstructorLookupInfo_SpecifiesCorrectPublicType ()
    {
      var info = (DomainObjectConstructorLookupInfo) FactoryBasedDomainObjectCreator.Instance.GetConstructorLookupInfo (typeof (Order));
      Assert.That (info.PublicDomainObjectType, Is.EqualTo (typeof (Order)));
    }

    [Test]
    public void GetConstructorLookupInfo_BindingFlags ()
    {
      var info = FactoryBasedDomainObjectCreator.Instance.GetConstructorLookupInfo (typeof (Order));
      Assert.That (info.BindingFlags, Is.EqualTo (BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = "mixin", MatchType = MessageMatch.Contains)]
    public void GetConstructorLookupInfo_ValidatesMixinConfiguration ()
    {
      using (MixinConfiguration.BuildNew ().EnterScope ())
      {
        FactoryBasedDomainObjectCreator.Instance.GetConstructorLookupInfo(typeof (TargetClassForPersistentMixin));
      }
    }

    private DataContainer CreateDataContainer (Type type)
    {
      return (DataContainer) PrivateInvoke.InvokeNonPublicMethod (ClientTransactionMock, "CreateNewDataContainer", type);
    }
  }
}