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
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver;
using Remotion.Data.UnitTests.DomainObjects.Core.MixedDomains.TestDomain;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Mixins;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class DomainObjectEventManagerTest : ClientTransactionBaseTest
  {
    private Order _order;
    private Order _mixedOrder;
    private HookedDomainObjectMixin _mixin;

    private DomainObjectEventManager _eventManager;
    
    private DomainObjectEventReceiver _eventReceiver;
    private bool _onLoadedCalled;
    private DomainObject _domainObject1;
    private DomainObject _domainObject2;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _order = Order.NewObject ();
      _eventManager = _order.EventManager;

      _eventReceiver = new DomainObjectEventReceiver (_order);
      _order.ProtectedLoaded += delegate { _onLoadedCalled = true; };

      _domainObject1 = Computer.GetObject (DomainObjectIDs.Computer1);
      _domainObject2 = Computer.GetObject (DomainObjectIDs.Computer2);

      using (MixinConfiguration.BuildNew ().ForClass<Order> ().AddMixin<HookedDomainObjectMixin> ().EnterScope ())
      {
        _mixedOrder = Order.NewObject();
      }
      
      _mixin = Mixin.Get<HookedDomainObjectMixin> (_mixedOrder);

    }

    [Test]
    public void EndObjectLoading_NormallyConstructedObject ()
    {
      var eventManager = new DomainObjectEventManager (_order, true);
      eventManager.EndObjectLoading ();
      Assert.That (_onLoadedCalled, Is.True);
      Assert.That (_order.LastLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
    }

    [Test]
    public void EndObjectLoading_MagicallyConstructedObject ()
    {
      var eventManager = new DomainObjectEventManager (_order, false);
      eventManager.EndObjectLoading ();
      Assert.That (_onLoadedCalled, Is.True);
      Assert.That (_order.LastLoadMode, Is.EqualTo (LoadMode.WholeDomainObjectInitialized));
    }

    [Test]
    public void EndObjectLoading_MagicallyConstructedObject_Twice ()
    {
      var eventManager = new DomainObjectEventManager (_order, false);
      eventManager.EndObjectLoading ();
      eventManager.EndObjectLoading ();
      
      Assert.That (_onLoadedCalled, Is.True);
      Assert.That (_order.LastLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
    }

    [Test]
    public void EndObjectLoading_NotifiesMixins ()
    {
      var eventManager = new DomainObjectEventManager (_mixedOrder, false);
      eventManager.EndObjectLoading ();

      Assert.That (_mixin.OnLoadedCalled, Is.True);
      Assert.That (_mixin.OnLoadedLoadMode, Is.EqualTo (LoadMode.WholeDomainObjectInitialized));

      eventManager.EndObjectLoading ();

      Assert.That (_mixin.OnLoadedLoadMode, Is.EqualTo (LoadMode.DataContainerLoadedOnly));
    }

    [Test]
    public void BeginPropertyValueChange ()
    {
      var pv = new PropertyValue (_order.ID.ClassDefinition.GetMandatoryPropertyDefinition (
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderNumber")));
      _eventManager.BeginPropertyValueChange (pv, 1, 2);

      Assert.That (_eventReceiver.HasChangingEventBeenCalled, Is.True);
      Assert.That (_eventReceiver.ChangingPropertyValue, Is.SameAs (pv));
      Assert.That (_eventReceiver.ChangingOldValue, Is.EqualTo (1));
      Assert.That (_eventReceiver.ChangingNewValue, Is.EqualTo (2));
    }

    [Test]
    public void EndPropertyValueChange ()
    {
      var pv = new PropertyValue (_order.ID.ClassDefinition.GetMandatoryPropertyDefinition (
          MappingConfiguration.Current.NameResolver.GetPropertyName (typeof (Order), "OrderNumber")));
      _eventManager.EndPropertyValueChange (pv, 1, 2);

      Assert.That (_eventReceiver.HasChangedEventBeenCalled, Is.True);
      Assert.That (_eventReceiver.ChangedPropertyValue, Is.SameAs (pv));
      Assert.That (_eventReceiver.ChangedOldValue, Is.EqualTo (1));
      Assert.That (_eventReceiver.ChangedNewValue, Is.EqualTo (2));
    }

    [Test]
    public void BeginRelationChange()
    {
      _eventManager.BeginRelationChange ("Hello", _domainObject1, _domainObject2);

      Assert.That (_eventReceiver.HasRelationChangingEventBeenCalled, Is.True);
      Assert.That (_eventReceiver.ChangingRelationPropertyName, Is.EqualTo ("Hello"));
      Assert.That (_eventReceiver.OldRelatedObject, Is.SameAs (_domainObject1));
      Assert.That (_eventReceiver.NewRelatedObject, Is.SameAs (_domainObject2));
    }

    [Test]
    public void EndRelationChange ()
    {
      _eventManager.EndRelationChange ("World");

      Assert.That (_eventReceiver.HasRelationChangedEventBeenCalled, Is.True);
      Assert.That (_eventReceiver.ChangedRelationPropertyName, Is.EqualTo ("World"));
    }

    [Test]
    public void BeginDelete ()
    {
      _eventManager.BeginDelete ();

      Assert.That (_eventReceiver.HasDeletingEventBeenCalled, Is.True);
    }

    [Test]
    public void EndDelete ()
    {
      _eventManager.EndDelete ();

      Assert.That (_eventReceiver.HasDeletedEventBeenCalled, Is.True);
    }

    [Test]
    public void BeginCommit ()
    {
      _eventManager.BeginCommit ();

      Assert.That (_eventReceiver.HasCommittingEventBeenCalled, Is.True);
    }

    [Test]
    public void EndCommit ()
    {
      _eventManager.EndCommit ();

      Assert.That (_eventReceiver.HasCommittedEventBeenCalled, Is.True);
    }

    [Test]
    public void BeginRollback ()
    {
      _eventManager.BeginRollback ();

      Assert.That (_eventReceiver.HasRollingBackEventBeenCalled, Is.True);
    }

    [Test]
    public void EndRollback ()
    {
      _eventManager.EndRollback ();

      Assert.That (_eventReceiver.HasRolledBackEventBeenCalled, Is.True);
    }
  }
}
