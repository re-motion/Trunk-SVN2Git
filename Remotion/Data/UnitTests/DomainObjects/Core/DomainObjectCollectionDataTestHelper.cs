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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class DomainObjectCollectionDataTestHelper
  {
    public static T GetDataStrategyAndCheckType<T> (DomainObjectCollection collection) where T : IDomainObjectCollectionData
    {
      var data = PrivateInvoke.GetNonPublicField (collection, "_dataStrategy");
      Assert.That (data, Is.InstanceOfType (typeof (T)));
      return (T) data;
    }

    public static void SetDataStrategy (DomainObjectCollection collection, IDomainObjectCollectionData dataStrategy)
    {
      PrivateInvoke.SetNonPublicField (collection, "_dataStrategy", dataStrategy);
    }

    public static T GetWrappedDataAndCheckType<T> (DomainObjectCollectionDataDecoratorBase decorator) where T : IDomainObjectCollectionData
    {
      var data = PrivateInvoke.GetNonPublicField (decorator, "_wrappedData");
      Assert.That (data, Is.InstanceOfType (typeof (T)));
      return (T) data;
    }
    
    public static IDomainObjectCollectionData GetData (EndPointDelegatingCollectionData delegatingData)
    {
      var data = (IDomainObjectCollectionData) PrivateInvoke.GetNonPublicField (delegatingData, "_endPointData");
      return data;
    }

    public static void CheckAssociatedCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType, ICollectionEndPoint expectedEndPoint)
    {
      // collection => checking checking decorator => end point data => actual data store

      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      Assert.That (checkingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var delegator = GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (checkingDecorator);
      Assert.That (delegator.AssociatedEndPoint, Is.SameAs (expectedEndPoint));

      var data = GetData (delegator);
      Assert.That (data, Is.SameAs (((ICollectionEndPointDataKeeper) PrivateInvoke.GetNonPublicField (expectedEndPoint, "_dataKeeper")).CollectionData));
    }

    public static void CheckAssociatedCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType, ICollectionEndPoint expectedEndPoint, IDomainObjectCollectionData expectedDataStore)
    {
      // collection => checking checking decorator => end point data => actual data store

      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      Assert.That (checkingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var delegator = GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (checkingDecorator);
      Assert.That (delegator.AssociatedEndPoint, Is.SameAs (expectedEndPoint));
      
      var data = GetData (delegator);
      Assert.That (data, Is.SameAs (((ICollectionEndPointDataKeeper) PrivateInvoke.GetNonPublicField (expectedEndPoint, "_dataKeeper")).CollectionData));

      if (expectedDataStore != null)
        Assert.That (data, Is.SameAs (expectedDataStore), "new collection still uses its original data store");
    }

    public static void CheckStandAloneCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType, IDomainObjectCollectionData expectedDataStore)
    {
      // collection => checking decorator => event decorator => actual data store

      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      Assert.That (checkingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var eventRaisingDecorator = GetWrappedDataAndCheckType<EventRaisingCollectionDataDecorator> (checkingDecorator);
      var eventRaiserAsIndirectRaiser = eventRaisingDecorator.EventRaiser as IndirectDomainObjectCollectionEventRaiser;

      if (eventRaiserAsIndirectRaiser == null)
        Assert.That (eventRaisingDecorator.EventRaiser, Is.SameAs (collection));
      else
        Assert.That (eventRaiserAsIndirectRaiser.EventRaiser, Is.SameAs (collection));

      var dataStore = GetWrappedDataAndCheckType<DomainObjectCollectionData> (eventRaisingDecorator);
      Assert.That (dataStore, Is.SameAs (expectedDataStore));
    }

    public static void CheckStandAloneCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType)
    {
      // collection => checking decorator => event decorator => actual data store

      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      Assert.That (checkingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var eventRaisingDecorator = GetWrappedDataAndCheckType<EventRaisingCollectionDataDecorator> (checkingDecorator);
      var eventRaiserAsIndirectRaiser = eventRaisingDecorator.EventRaiser as IndirectDomainObjectCollectionEventRaiser;
      
      if (eventRaiserAsIndirectRaiser == null)
        Assert.That (eventRaisingDecorator.EventRaiser, Is.SameAs (collection));
      else
        Assert.That (eventRaiserAsIndirectRaiser.EventRaiser, Is.SameAs (collection));

      GetWrappedDataAndCheckType<DomainObjectCollectionData> (eventRaisingDecorator);
    }

    public static void CheckReadOnlyCollectionStrategy (DomainObjectCollection collection, bool expectedIsGetDataStoreAllowed)
    {
      // collection => read-only decorator => actual data store

      var readOnlyDecorator = GetDataStrategyAndCheckType<ReadOnlyCollectionDataDecorator> (collection);
      Assert.That (readOnlyDecorator.IsGetDataStoreAllowed, Is.EqualTo (expectedIsGetDataStoreAllowed));

      GetWrappedDataAndCheckType<DomainObjectCollectionData> (readOnlyDecorator);
    }

    public static void MakeCollectionReadOnly (DomainObjectCollection collection)
    {
      // strip off all decorators
      var checkingDecorator = GetDataStrategyAndCheckType<ModificationCheckingCollectionDataDecorator> (collection);
      var originalStrategy = GetWrappedDataAndCheckType<IDomainObjectCollectionData> (checkingDecorator);
      if (originalStrategy is EventRaisingCollectionDataDecorator)
        originalStrategy = GetWrappedDataAndCheckType<IDomainObjectCollectionData> ((EventRaisingCollectionDataDecorator) originalStrategy);

      var newStrategy = new ReadOnlyCollectionDataDecorator (originalStrategy, true);
      SetDataStrategy (collection, newStrategy);
    }

    public static ICollectionEndPoint GetAssociatedEndPoint (DomainObjectCollection collection)
    {
      var strategy = GetDataStrategyAndCheckType<IDomainObjectCollectionData> (collection);
      return strategy.AssociatedEndPoint;
    }
  }
}