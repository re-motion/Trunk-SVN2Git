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
using Remotion.Data.DomainObjects.DataManagement.VirtualEndPoints;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.VirtualEndPoints
{
  public class TestableIncompleteVirtualEndPointLoadState : IncompleteVirtualEndPointLoadStateBase<IVirtualEndPoint<object>, object, IVirtualEndPointDataKeeper>
  {
    private IRealObjectEndPoint[] _stubbedOriginalOppositeEndPoints;

    public TestableIncompleteVirtualEndPointLoadState (IVirtualEndPointDataKeeper dataKeeper, IRelationEndPointLazyLoader lazyLoader, IVirtualEndPointDataKeeperFactory<IVirtualEndPointDataKeeper> dataKeeperFactory)
        : base(dataKeeper, lazyLoader, dataKeeperFactory)
    {
    }

    public TestableIncompleteVirtualEndPointLoadState (FlattenedDeserializationInfo info)
        : base(info)
    {
    }

    public void StubOriginalOppositeEndPoints (IRealObjectEndPoint[] originalOppositeEndPoints)
    {
      _stubbedOriginalOppositeEndPoints = originalOppositeEndPoints;
    }
    
    protected override IEnumerable<IRealObjectEndPoint> GetOriginalOppositeEndPoints ()
    {
      return _stubbedOriginalOppositeEndPoints;
    }
  }
}