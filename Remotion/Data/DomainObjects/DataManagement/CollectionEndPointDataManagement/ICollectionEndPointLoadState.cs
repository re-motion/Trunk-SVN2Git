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
using System.Collections.Generic;
using Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionEndPointDataManagement
{
  /// <summary>
  /// Represents the lazy-loading state of a <see cref="CollectionEndPoint"/> and implements accessor methods for that end-point.
  /// </summary>
  public interface ICollectionEndPointLoadState
  {
    void EnsureDataComplete ();

    IDomainObjectCollectionData GetCollectionData ();

    DomainObjectCollection GetCollectionWithOriginalData ();
    IEnumerable<IRelationEndPoint> GetOppositeRelationEndPoints (IDataManager dataManager);

    IDataManagementCommand CreateSetOppositeCollectionCommand (IAssociatableDomainObjectCollection newOppositeCollection);
    IDataManagementCommand CreateRemoveCommand (DomainObject removedRelatedObject);
    IDataManagementCommand CreateDeleteCommand ();
    IDataManagementCommand CreateInsertCommand (DomainObject insertedRelatedObject, int index);
    IDataManagementCommand CreateAddCommand (DomainObject addedRelatedObject);
    IDataManagementCommand CreateReplaceCommand (int index, DomainObject replacementObject);

    void SetValueFrom (ICollectionEndPoint sourceEndPoint);
    void CheckMandatory ();
  }
}