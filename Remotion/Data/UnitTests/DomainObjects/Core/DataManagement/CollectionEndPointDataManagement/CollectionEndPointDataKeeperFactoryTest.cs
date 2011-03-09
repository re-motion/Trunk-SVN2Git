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
using Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement;
using Remotion.Data.DomainObjects.Mapping;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement
{
  [TestFixture]
  public class CollectionEndPointDataKeeperFactoryTest : ClientTransactionBaseTest
  {
    private IRelationEndPointProvider _relationEndPointProvider;
    private CollectionEndPointDataKeeperFactory _factory;

    public override void SetUp ()
    {
      base.SetUp();

      _relationEndPointProvider = MockRepository.GenerateStub<IRelationEndPointProvider>();
      _factory = new CollectionEndPointDataKeeperFactory (ClientTransactionMock, _relationEndPointProvider);
      
    }

    [Test]
    public void Create ()
    {
      var relationEndPointID = RelationEndPointID.Create (
          DomainObjectIDs.Customer1, "Remotion.Data.UnitTests.DomainObjects.TestDomain.Customer.Orders");
      var comparer = SortedPropertyComparer.CreateCompoundComparer (
          ((VirtualRelationEndPointDefinition) relationEndPointID.Definition).GetSortExpression().SortedProperties, ClientTransactionMock.DataManager);

      var result = _factory.Create (relationEndPointID, comparer);

      Assert.That (result, Is.TypeOf (typeof (CollectionEndPointDataKeeper)));
      Assert.That (((CollectionEndPointDataKeeper) result).EndPointID, Is.SameAs(relationEndPointID));
      Assert.That (((CollectionEndPointDataKeeper) result).SortExpressionBasedComparer, Is.SameAs (comparer));
    }
  }
}