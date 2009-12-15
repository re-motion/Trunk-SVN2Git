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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement
{
  /// <summary>
  /// Provides extension methods for <see cref="IObjectEndPoint"/>.
  /// </summary>
  public static class ObjectEndPointExtensions
  {
    public static DomainObject GetOppositeObject (this IObjectEndPoint endPoint, bool includeDeleted)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      var oppositeObjectID = endPoint.OppositeObjectID;
      var clientTransaction = endPoint.ClientTransaction;

      if (oppositeObjectID == null)
        return null;
      else if (includeDeleted && clientTransaction.DataManager.IsDiscarded (oppositeObjectID))
        return clientTransaction.DataManager.GetDiscardedDataContainer (oppositeObjectID).DomainObject;
      else
        return clientTransaction.GetObject (oppositeObjectID, includeDeleted);
    }

    public static DomainObject GetOriginalOppositeObject (this IObjectEndPoint endPoint)
    {
      ArgumentUtility.CheckNotNull ("endPoint", endPoint);

      var originalOppositeObjectID = endPoint.OriginalOppositeObjectID;
      if (originalOppositeObjectID == null)
        return null;

      return endPoint.ClientTransaction.GetObject (originalOppositeObjectID, true);
    }
  }
}