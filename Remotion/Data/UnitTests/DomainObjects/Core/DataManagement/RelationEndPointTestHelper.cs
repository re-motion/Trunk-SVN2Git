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
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints.CollectionEndPoints;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  public class RelationEndPointTestHelper
  {
    public static ICollectionEndPointDataKeeper GetCollectionEndPointDataKeeper (CollectionEndPoint collectionEndPoint)
    {
      return (ICollectionEndPointDataKeeper) PrivateInvoke.GetNonPublicField (collectionEndPoint, "_dataKeeper");
    }

    public static DataContainer CreateNewDataContainer (RelationEndPointID id)
    {
      var foreignKeyDataContainer = DataContainer.CreateNew (id.ObjectID);
      return foreignKeyDataContainer;
    }

    public static DataContainer CreateExistingForeignKeyDataContainer (RelationEndPointID id, ObjectID initialForeignKeyValue)
    {
      var foreignKeyDataContainer = DataContainer.CreateForExisting (
          id.ObjectID, 
          null, 
          pd => pd.PropertyName == id.Definition.PropertyName ? initialForeignKeyValue : pd.DefaultValue);
      return foreignKeyDataContainer;
    }

    public static DataContainer CreateExistingDataContainer (RelationEndPointID id)
    {
      var foreignKeyDataContainer = DataContainer.CreateForExisting (
          id.ObjectID,
          null,
          pd => pd.DefaultValue);
      return foreignKeyDataContainer;
    }
  }
}