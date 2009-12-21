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
using System.Collections.Generic;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.UnitTests.DomainObjects.Factories;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  public static class RelationEndPointObjectMother
  {
    public static CollectionEndPoint CreateCollectionEndPoint (
        RelationEndPointID endPointID,
        IEnumerable<DomainObject> initialContents)
    {
      return CreateCollectionEndPoint (endPointID, new RootCollectionEndPointChangeDetectionStrategy (), ClientTransaction.Current, initialContents);
    }

    public static CollectionEndPoint CreateCollectionEndPoint (
        RelationEndPointID endPointID, 
        ICollectionEndPointChangeDetectionStrategy changeDetectionStrategy, 
        ClientTransaction transaction, 
        IEnumerable<DomainObject> initialContents)
    {
      var newCollectionEndPoint = new CollectionEndPoint (
          transaction,
          endPointID,
          changeDetectionStrategy,
          initialContents);
      return newCollectionEndPoint;
    }

    public static RealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID)
    {
      var domainObject = RepositoryAccessor.GetObject (endPointID.ObjectID, true);
      var dataContainer = (DataContainer) PrivateInvoke.InvokeNonPublicMethod (ClientTransaction.Current, "GetDataContainer", domainObject);
      return CreateRealObjectEndPoint (endPointID, dataContainer);
    }

    public static RealObjectEndPoint CreateRealObjectEndPoint (RelationEndPointID endPointID, DataContainer dataContainer)
    {
      return new RealObjectEndPoint (ClientTransaction.Current, endPointID, dataContainer);
    }

    public static VirtualObjectEndPoint CreateVirtualObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      return new VirtualObjectEndPoint (ClientTransaction.Current, endPointID, oppositeObjectID);
    }

    public static ObjectEndPoint CreateObjectEndPoint (RelationEndPointID endPointID, ObjectID oppositeObjectID)
    {
      if (endPointID.Definition.IsVirtual)
        return CreateVirtualObjectEndPoint (endPointID, oppositeObjectID);
      else
      {
        var endPoint = CreateRealObjectEndPoint (endPointID);
        endPoint.OppositeObjectID = oppositeObjectID;
        endPoint.Commit ();
        return endPoint;
      }
    }


    public static RelationEndPointID CreateRelationEndPointID (ObjectID objectID, string shortPropertyName)
    {
      return new RelationEndPointID (objectID, objectID.ClassDefinition.ClassType.FullName + "." + shortPropertyName);
    }

    public static CollectionEndPoint CreateCollectionEndPoint_Customer1_Orders (params Order[] initialContents)
    {
      var customerEndPointID = CreateRelationEndPointID (new DomainObjectIDs().Customer1, "Orders");
      return CreateCollectionEndPoint (customerEndPointID, initialContents);
    }

  }
}