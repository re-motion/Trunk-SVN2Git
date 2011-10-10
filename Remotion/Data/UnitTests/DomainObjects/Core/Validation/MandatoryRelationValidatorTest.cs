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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Validation;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Validation
{
  [TestFixture]
  public class MandatoryRelationValidatorTest : StandardMappingTest
  {
    private MandatoryRelationValidator _validator;
    private IDataManager _dataManagerMock;

    private IRelationEndPoint _endPointMock;

    private OrderItem _orderItem1;

    public override void SetUp ()
    {
      base.SetUp ();
      
      _dataManagerMock = MockRepository.GenerateStrictMock<IDataManager>();
      _validator = new MandatoryRelationValidator (_dataManagerMock);

      _endPointMock = MockRepository.GenerateStrictMock<IRelationEndPoint>();

      _orderItem1 = DomainObjectMother.CreateFakeObject<OrderItem> (DomainObjectIDs.OrderItem1);
    }

    [Test]
    public void Validate_CompleteRelationEndPoint ()
    {
      _dataManagerMock
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (RelationEndPointID.Create (_orderItem1, oi => oi.Order)))
          .Return (_endPointMock);
      _dataManagerMock.Replay ();

      _endPointMock.Stub (stub => stub.IsDataComplete).Return (true);
      _endPointMock.Expect (mock => mock.ValidateMandatory());
      _endPointMock.Replay();

      _validator.Validate (_orderItem1);

      _endPointMock.VerifyAllExpectations();
    }

    [Test]
    public void Validate_IncompleteRelationEndPoint ()
    {
      _dataManagerMock
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (RelationEndPointID.Create (_orderItem1, oi => oi.Order)))
          .Return (_endPointMock);
      _dataManagerMock.Replay ();

      _endPointMock.Stub (stub => stub.IsDataComplete).Return (false);
      _endPointMock.Replay ();

      _validator.Validate (_orderItem1);

      _endPointMock.AssertWasNotCalled (mock => mock.ValidateMandatory ());
    }

    [Test]
    public void Validate_NonLoadedRelationEndPoint ()
    {
      _dataManagerMock
          .Stub (stub => stub.GetRelationEndPointWithoutLoading (RelationEndPointID.Create (_orderItem1, oi => oi.Order)))
          .Return (null);
      _dataManagerMock.Replay ();

      _validator.Validate (_orderItem1);
    }

    [Test]
    public void Validate_ChecksAllMandatoryEndPointIDs ()
    {
      var order1 = DomainObjectMother.CreateFakeObject<Order> (DomainObjectIDs.Order1);

      var mandatoryEndPoint1 = RelationEndPointID.Create (order1, oi => oi.OrderItems);
      Assert.That (mandatoryEndPoint1.Definition.IsMandatory, Is.True);
      var mandatoryEndPoint2 = RelationEndPointID.Create (order1, oi => oi.OrderTicket);
      Assert.That (mandatoryEndPoint2.Definition.IsMandatory, Is.True);
      var mandatoryEndPoint3 = RelationEndPointID.Create (order1, oi => oi.Official);
      Assert.That (mandatoryEndPoint3.Definition.IsMandatory, Is.True);
      var mandatoryEndPoint4 = RelationEndPointID.Create (order1, oi => oi.Customer);
      Assert.That (mandatoryEndPoint4.Definition.IsMandatory, Is.True);

      _dataManagerMock.Expect (mock => mock.GetRelationEndPointWithoutLoading (mandatoryEndPoint1)).Return (null);
      _dataManagerMock.Expect (mock => mock.GetRelationEndPointWithoutLoading (mandatoryEndPoint2)).Return (null);
      _dataManagerMock.Expect (mock => mock.GetRelationEndPointWithoutLoading (mandatoryEndPoint3)).Return (null);
      _dataManagerMock.Expect (mock => mock.GetRelationEndPointWithoutLoading (mandatoryEndPoint4)).Return (null);
      _dataManagerMock.Replay();

      _validator.Validate (order1);

      _dataManagerMock.VerifyAllExpectations();
    }

    [Test]
    public void Validate_ChecksNoNonMandatoryEndPointIDs ()
    {
      var computer = DomainObjectMother.CreateFakeObject<Computer> (DomainObjectIDs.Computer1);
      var nonMandatoryEndPoint = RelationEndPointID.Create (computer, oi => oi.Employee);
      Assert.That (nonMandatoryEndPoint.Definition.IsMandatory, Is.False);

      _dataManagerMock.Replay ();

      _validator.Validate (computer);

      _dataManagerMock.AssertWasNotCalled (mock => mock.GetRelationEndPointWithoutLoading (nonMandatoryEndPoint));
    }


    [Test]
    public void Validate_IntegrationTest_RelationsOk ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        var orderTicket = OrderTicket.NewObject ();
        orderTicket.Order = Order.NewObject();
        var validator = new MandatoryRelationValidator (ClientTransactionTestHelper.GetIDataManager (ClientTransaction.Current));

        Assert.That (() => validator.Validate (orderTicket), Throws.Nothing);
      }
    }

    [Test]
    public void Validate_IntegrationTest_RelationsNotOk ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope())
      {
        var orderTicket = OrderTicket.NewObject();
        var validator = new MandatoryRelationValidator (ClientTransactionTestHelper.GetIDataManager (ClientTransaction.Current));

        Assert.That (
            () => validator.Validate (orderTicket),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.Matches (
                @"Mandatory relation property 'Remotion\.Data\.UnitTests\.DomainObjects\.TestDomain\.OrderTicket\.Order' of domain object "
                + @"'OrderTicket|.*|System\.Guid' cannot be null."));
      }
    }
  }
}