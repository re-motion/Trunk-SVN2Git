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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class SubClientTransactionTest : ClientTransactionBaseTest
  {
    private SubClientTransaction _subTx;

    [SetUp]
    public override void SetUp ()
    {
      base.SetUp ();

      _subTx = (SubClientTransaction) ClientTransactionMock.CreateSubTransaction ();
    }

    [Test]
    public void CollectionEndPointChangeDetectionStrategy ()
    {
      var dataManager = (DataManager) PrivateInvoke.GetNonPublicProperty (_subTx, "DataManager");
      Assert.That (dataManager.RelationEndPointMap.CollectionEndPointChangeDetectionStrategy, 
          Is.InstanceOfType (typeof (SubCollectionEndPointChangeDetectionStrategy)));
    }

    [Test]
    public void Enlist_UsesParentManager ()
    {
      var order = DomainObjectMother.CreateObjectInOtherTransaction<Order> ();
      Assert.That (_subTx.IsEnlisted (order), Is.False);
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.False);

      _subTx.EnlistDomainObject (order);
      Assert.That (_subTx.IsEnlisted (order), Is.True);
      Assert.That (ClientTransactionMock.IsEnlisted (order), Is.True);
    }
  }
}