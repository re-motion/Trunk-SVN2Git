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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Data.DomainObjects.Infrastructure;
using System.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.IntegrationTests
{
  public static class PersistableDataObjectMother
  {
    public static PersistableData Create ()
    {
      var domainObject = DomainObjectMother.CreateFakeObject<Order>();
      return new PersistableData (domainObject, StateType.New, DataContainer.CreateNew (domainObject.ID), new IRelationEndPoint[0]);
    }

    public static PersistableData Create (ClientTransaction clientTransaction, DomainObject domainObject)
    {
      var dataManager = ClientTransactionTestHelper.GetIDataManager (clientTransaction);
      var dataContainer = dataManager.GetDataContainerWithoutLoading (domainObject.ID);
      return new PersistableData (
          domainObject,
          domainObject.TransactionContext[clientTransaction].State,
          dataContainer,
          dataContainer.AssociatedRelationEndPointIDs.Select (dataManager.GetRelationEndPointWithoutLoading).Where (ep => ep != null));
    }
  }
}