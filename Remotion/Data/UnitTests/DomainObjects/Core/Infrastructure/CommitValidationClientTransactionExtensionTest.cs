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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class CommitValidationClientTransactionExtensionTest : ClientTransactionBaseTest
  {
    private CommitValidationClientTransactionExtension _extension;

    public override void SetUp ()
    {
      base.SetUp ();

      _extension = new CommitValidationClientTransactionExtension ();
    }

    [Test]
    public void DefaultKey ()
    {
      Assert.That (CommitValidationClientTransactionExtension.DefaultKey, Is.EqualTo (typeof (CommitValidationClientTransactionExtension).FullName));
    }

    [Test]
    public void Key ()
    {
      Assert.That (_extension.Key, Is.EqualTo (CommitValidationClientTransactionExtension.DefaultKey));
    }

    [Test]
    public void CommitValidate_MandatoryRelation_NotSet ()
    {
      var objectWithMandatoryRelations = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      objectWithMandatoryRelations.Order = null;

      Assert.That (
          () => _extension.CommitValidate (ClientTransactionMock, Array.AsReadOnly<DomainObject> (new[] { objectWithMandatoryRelations })),
          Throws.TypeOf<MandatoryRelationNotSetException> ().With.Message.EqualTo (
            "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' of domain object "
            + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be null."));
    }

    [Test]
    public void CommitValidate_MandatoryRelation_Set ()
    {
      var objectWithMandatoryRelations = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      Assert.That (objectWithMandatoryRelations.Order, Is.Not.Null);

      Assert.That (
          () => _extension.CommitValidate (ClientTransactionMock, Array.AsReadOnly<DomainObject> (new[] { objectWithMandatoryRelations })),
          Throws.Nothing);
    }

    [Test]
    public void CommitValidate_NonMandatoryRelation_NotSet ()
    {
      var objectWithoutMandatoryRelations = Computer.GetObject (DomainObjectIDs.Computer1);
      objectWithoutMandatoryRelations.Employee = null;

      Assert.That (
          () => _extension.CommitValidate (ClientTransactionMock, Array.AsReadOnly<DomainObject> (new[] { objectWithoutMandatoryRelations })),
          Throws.Nothing);
    }

    [Test]
    public void CommitValidate_NonMandatoryRelation_Set ()
    {
      var objectWithoutMandatoryRelations = Computer.GetObject (DomainObjectIDs.Computer1);
      Assert.That (objectWithoutMandatoryRelations.Employee, Is.Not.Null);

      Assert.That (
          () => _extension.CommitValidate (ClientTransactionMock, Array.AsReadOnly<DomainObject> (new[] { objectWithoutMandatoryRelations })),
          Throws.Nothing);
    }

    [Test]
    public void CommitValidate_NonLoadedObject ()
    {
      var objectWithMandatoryRelations = OrderItem.GetObject (DomainObjectIDs.OrderItem1);
      objectWithMandatoryRelations.Order = null;

      var subTransaction = ClientTransactionMock.CreateSubTransaction ();
      using (subTransaction.EnterDiscardingScope ())
      {
        Assert.That (
            () => _extension.CommitValidate (subTransaction, Array.AsReadOnly<DomainObject> (new[] { objectWithMandatoryRelations })),
            Throws.Nothing);

        objectWithMandatoryRelations.EnsureDataAvailable();

        Assert.That (
            () => _extension.CommitValidate (subTransaction, Array.AsReadOnly<DomainObject> (new[] { objectWithMandatoryRelations })),
            Throws.TypeOf<MandatoryRelationNotSetException>().With.Message.EqualTo (
                "Mandatory relation property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Order' of domain object "
                + "'OrderItem|2f4d42c7-7ffa-490d-bfcd-a9101bbf4e1a|System.Guid' cannot be null."));
      }
    }
  }
}