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
using Remotion.Data.DomainObjects.DataManagement.ObjectEndPointDataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.ObjectEndPointDataManagement
{
  [TestFixture]
  public class RealObjectEndPointTest : ClientTransactionBaseTest
  {
    private RealObjectEndPoint _endPoint;
    private IRelationEndPointLazyLoader _lazyLoaderStub;

    public override void SetUp ()
    {
      base.SetUp ();

      var endPointID = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      _endPoint = RelationEndPointObjectMother.CreateRealObjectEndPoint (endPointID);
      _lazyLoaderStub = MockRepository.GenerateStub<IRelationEndPointLazyLoader>();
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "End point ID must refer to a non-virtual end point.\r\nParameter name: id")]
    public void Initialize_VirtualDefinition ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.Order1, "OrderTicket");
      var foreignKeyDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      new RealObjectEndPoint (ClientTransactionMock, id, foreignKeyDataContainer, _lazyLoaderStub);
    }

    [Test]
    [ExpectedException (typeof (ArgumentException), ExpectedMessage =
        "The foreign key data container must be compatible with the end point definition.\r\nParameter name: foreignKeyDataContainer")]
    public void Initialize_InvalidDataContainer ()
    {
      var id = RelationEndPointObjectMother.CreateRelationEndPointID (DomainObjectIDs.OrderTicket1, "Order");
      var foreignKeyDataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      new RealObjectEndPoint (ClientTransactionMock, id, foreignKeyDataContainer, _lazyLoaderStub);
    }

    [Test]
    public void OppositeObjectID_Get_FromProperty ()
    {
      Assert.That (_endPoint.OppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));
      _endPoint.ForeignKeyProperty.Value = DomainObjectIDs.Order2;

      Assert.That (_endPoint.OppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void OppositeObjectID_Get_DoesNotRaisePropertyReadEvents ()
    {
      var listenerMock = MockRepository.GenerateMock<IClientTransactionListener> ();
      ClientTransactionMock.AddListener (listenerMock);

      Dev.Null = _endPoint.OppositeObjectID;

      listenerMock.AssertWasNotCalled (mock => mock.PropertyValueReading (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<DataContainer>.Is.Anything, 
          Arg<PropertyValue>.Is.Anything, 
          Arg<ValueAccess>.Is.Anything));
      listenerMock.AssertWasNotCalled (mock => mock.PropertyValueRead (
          Arg<ClientTransaction>.Is.Anything, 
          Arg<DataContainer>.Is.Anything,
          Arg<PropertyValue>.Is.Anything,
          Arg<object>.Is.Anything,
          Arg<ValueAccess>.Is.Anything));
    }

    [Test]
    public void OppositeObjectID_Set_ToProperty ()
    {
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.Not.EqualTo (DomainObjectIDs.Order2));

      ObjectEndPointTestHelper.SetOppositeObjectID (_endPoint, DomainObjectIDs.Order2);
      
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void OriginalOppositeObjectID_Get_FromProperty ()
    {
      Assert.That (_endPoint.OriginalOppositeObjectID, Is.Not.EqualTo (DomainObjectIDs.Order2));

      PrivateInvoke.SetNonPublicField (_endPoint.ForeignKeyProperty, "_originalValue", DomainObjectIDs.Order2);

      Assert.That (_endPoint.OriginalOppositeObjectID, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void HasChanged_FromProperty ()
    {
      Assert.That (_endPoint.HasChanged, Is.False);

      _endPoint.ForeignKeyProperty.Value = DomainObjectIDs.Order2;

      Assert.That (_endPoint.HasChanged, Is.True);
    }

    [Test]
    public void HasBeenTouched_FromProperty ()
    {
      Assert.That (_endPoint.HasBeenTouched, Is.False);

      _endPoint.ForeignKeyProperty.Touch();

      Assert.That (_endPoint.HasBeenTouched, Is.True);
    }

    [Test]
    public void Touch_ToProperty ()
    {
      Assert.That (_endPoint.ForeignKeyProperty.HasBeenTouched, Is.False);

      _endPoint.Touch ();

      Assert.That (_endPoint.ForeignKeyProperty.HasBeenTouched, Is.True);
    }

    [Test]
    public void Commit_ToProperty ()
    {
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.EqualTo (DomainObjectIDs.Order1));

      _endPoint.ForeignKeyProperty.Value = DomainObjectIDs.Order2;
      Assert.That (_endPoint.ForeignKeyProperty.HasChanged, Is.True);

      _endPoint.Commit ();

      Assert.That (_endPoint.ForeignKeyProperty.HasChanged, Is.False);
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.EqualTo (DomainObjectIDs.Order2));
    }

    [Test]
    public void Rollback_ToProperty ()
    {
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.EqualTo (DomainObjectIDs.Order1));

      _endPoint.ForeignKeyProperty.Value = DomainObjectIDs.Order2;
      Assert.That (_endPoint.ForeignKeyProperty.HasChanged, Is.True);

      _endPoint.Rollback ();

      Assert.That (_endPoint.ForeignKeyProperty.HasChanged, Is.False);
      Assert.That (_endPoint.ForeignKeyProperty.Value, Is.EqualTo (DomainObjectIDs.Order1));
    }
  }
}