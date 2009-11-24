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
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  public static class DomainObjectCollectionDataTestHelper
  {
    public static T GetCollectionDataAndCheckType<T> (DomainObjectCollection collection) where T : IDomainObjectCollectionData
    {
      var data = PrivateInvoke.GetNonPublicField (collection, "_dataStrategy");
      Assert.That (data, Is.InstanceOfType (typeof (T)));
      return (T) data;
    }

    public static T GetWrappedDataAndCheckType<T> (ArgumentCheckingCollectionDataDecorator decorator) where T : IDomainObjectCollectionData
    {
      var data = PrivateInvoke.GetNonPublicField (decorator, "_wrappedData");
      Assert.That (data, Is.InstanceOfType (typeof (T)));
      return (T) data;
    }

    public static T GetWrappedDataAndCheckType<T> (EventRaisingCollectionDataDecorator decorator) where T : IDomainObjectCollectionData
    {
      var data = PrivateInvoke.GetNonPublicField (decorator, "_wrappedData");
      Assert.That (data, Is.InstanceOfType (typeof (T)));
      return (T) data;
    }

    public static T GetWrappedDataAndCheckType<T> (ReadOnlyCollectionDataDecorator decorator) where T : IDomainObjectCollectionData
    {
      var data = PrivateInvoke.GetNonPublicField (decorator, "_wrappedData");
      Assert.That (data, Is.InstanceOfType (typeof (T)));
      return (T) data;
    }


    public static T GetActualDataAndCheckType<T> (EndPointDelegatingCollectionData newCollectionDelegatingData) where T : IDomainObjectCollectionData
    {
      var data = PrivateInvoke.GetNonPublicField (newCollectionDelegatingData, "_actualData");
      Assert.That (data, Is.InstanceOfType (typeof (T)));
      return (T) data;
    }

    public static void CheckAssociatedCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType, CollectionEndPoint expectedEndPoint, IDomainObjectCollectionData expectedDataStore)
    {
      // collection => argument checking decorator => end point data => actual data store

      var argCheckingDecorator = GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      Assert.That (argCheckingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var delegator = GetWrappedDataAndCheckType<EndPointDelegatingCollectionData> (argCheckingDecorator);
      Assert.That (delegator.AssociatedEndPoint, Is.SameAs (expectedEndPoint));

      var dataStore = GetActualDataAndCheckType<DomainObjectCollectionData> (delegator);
      Assert.That (dataStore, Is.SameAs (expectedDataStore), "new collection still uses its original data store");
    }

    public static void CheckStandAloneCollectionStrategy (DomainObjectCollection collection, Type expectedRequiredItemType, IDomainObjectCollectionData expectedDataStore)
    {
      // collection => argument decorator => event decorator => actual data store

      var argCheckingDecorator = GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      Assert.That (argCheckingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var eventRaisingDecorator = GetWrappedDataAndCheckType<EventRaisingCollectionDataDecorator> (argCheckingDecorator);
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
      // collection => argument decorator => event decorator => actual data store

      var argCheckingDecorator = GetCollectionDataAndCheckType<ArgumentCheckingCollectionDataDecorator> (collection);
      Assert.That (argCheckingDecorator.RequiredItemType, Is.SameAs (expectedRequiredItemType));

      var eventRaisingDecorator = GetWrappedDataAndCheckType<EventRaisingCollectionDataDecorator> (argCheckingDecorator);
      var eventRaiserAsIndirectRaiser = eventRaisingDecorator.EventRaiser as IndirectDomainObjectCollectionEventRaiser;
      
      if (eventRaiserAsIndirectRaiser == null)
        Assert.That (eventRaisingDecorator.EventRaiser, Is.SameAs (collection));
      else
        Assert.That (eventRaiserAsIndirectRaiser.EventRaiser, Is.SameAs (collection));

      GetWrappedDataAndCheckType<DomainObjectCollectionData> (eventRaisingDecorator);
    }
  }
}