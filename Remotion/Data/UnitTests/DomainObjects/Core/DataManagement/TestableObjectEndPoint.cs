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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  public class TestableObjectEndPoint : ObjectEndPoint
  {
    private ObjectID _originalOppositeObjectID;
    private ObjectID _oppositeObjectID;
    private bool _hasBeenTouched;

    public TestableObjectEndPoint (ClientTransaction clientTransaction, RelationEndPointID id, IRelationEndPointLazyLoader lazyLoader, IRelationEndPointProvider endPointProvider, ObjectID originalOppositeObjectID)
        : base (clientTransaction, id, lazyLoader, endPointProvider)
    {
      _originalOppositeObjectID = originalOppositeObjectID;
      _oppositeObjectID = originalOppositeObjectID;
      _hasBeenTouched = false;
    }

    public TestableObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
    }

    public override bool HasChanged
    {
      get { return _oppositeObjectID != _originalOppositeObjectID; }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override void Touch ()
    {
      _hasBeenTouched = true;
    }

    public override void Commit ()
    {
      throw new NotImplementedException();
    }

    public override void Rollback ()
    {
      throw new NotImplementedException();
    }

    public override ObjectID OppositeObjectID
    {
      get { return _oppositeObjectID; }
      protected set 
      { 
        _oppositeObjectID = value;
        Touch();
      }
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return _originalOppositeObjectID; }
    }
  }
}