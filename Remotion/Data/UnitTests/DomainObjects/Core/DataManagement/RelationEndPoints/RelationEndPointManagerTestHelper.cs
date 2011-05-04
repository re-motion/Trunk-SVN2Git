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
using Remotion.Data.DomainObjects.DataManagement.RelationEndPoints;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.RelationEndPoints
{
  public static class RelationEndPointManagerTestHelper
  {
    public static void AddEndPoint (RelationEndPointManager endPointManager, IRelationEndPoint endPoint)
    {
      var map = GetMap (endPointManager);
      map.AddEndPoint (endPoint);
    }

    public static void ReplaceEndPoint (RelationEndPointManager endPointManager, IRelationEndPoint endPoint)
    {
      endPointManager.RemoveEndPoint (endPoint.ID);
      AddEndPoint (endPointManager, endPoint);
    }

    public static void RemoveEndPoint (RelationEndPointManager manager, RelationEndPointID relationEndPointID)
    {
      manager.RemoveEndPoint (relationEndPointID);
    }

    public static IRelationEndPointRegistrationAgent GetRegistrationAgent (RelationEndPointManager manager)
    {
      return (IRelationEndPointRegistrationAgent) PrivateInvoke.GetNonPublicField (manager, "_registrationAgent");
    }

    public static RelationEndPointMap GetMap (RelationEndPointManager manager)
    {
      return (RelationEndPointMap) PrivateInvoke.GetNonPublicField (manager, "_relationEndPoints");
    }
  }
}