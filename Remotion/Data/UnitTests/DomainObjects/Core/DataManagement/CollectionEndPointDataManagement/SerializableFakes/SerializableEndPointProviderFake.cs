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
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Collections;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.CollectionEndPointDataManagement.SerializableFakes
{
  [Serializable]
  public class SerializableEndPointProviderFake : IRelationEndPointProvider
  {
    [NonSerialized]
    private readonly Dictionary<RelationEndPointID, IRelationEndPoint> _endPoints;

    public SerializableEndPointProviderFake ( params IRelationEndPoint[] endPoints)
    {
      _endPoints = endPoints.ToDictionary(ep => ep.ID);
    }

    public IRelationEndPoint GetRelationEndPointWithLazyLoad (RelationEndPointID endPointID)
    {
      return _endPoints[endPointID];
    }

    public IRelationEndPoint GetRelationEndPointWithoutLoading (RelationEndPointID endPointID)
    {
      return _endPoints.GetValueOrDefault (endPointID);
    }

    public IRelationEndPoint GetOppositeEndPoint (IRealObjectEndPoint objectEndPoint)
    {
      throw new NotImplementedException();
    }
  }
}