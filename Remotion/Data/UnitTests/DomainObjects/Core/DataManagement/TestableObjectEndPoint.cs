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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  public class TestableObjectEndPoint : ObjectEndPoint
  {
    private readonly ObjectID _originalOppositeObjectID;
    private ObjectID _oppositeObjectID;
    private bool _hasBeenTouched;
    private bool _isSetOppositeObjectIDValueFromExpected;

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

    public override bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    public override void Synchronize ()
    {
      throw new NotImplementedException();
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

    public override IDataManagementCommand CreateDeleteCommand ()
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

    public void SetOppositeObjectID (ObjectID objectID)
    {
      OppositeObjectID = objectID;
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return _originalOppositeObjectID; }
    }

    public void ExpectSetOppositeObjectIDValueFrom()
    {
      _isSetOppositeObjectIDValueFromExpected = true;
    }

    public void ExpectNotSetOppositeObjectIDValueFrom ()
    {
      _isSetOppositeObjectIDValueFromExpected = false;
    }

    public override IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      return new TestSetCommand (this, newRelatedObject, id => { throw new NotImplementedException (); });
    }

    protected override void SetOppositeObjectIDValueFrom (IObjectEndPoint sourceObjectEndPoint)
    {
      Assert.That (_isSetOppositeObjectIDValueFromExpected, Is.True);
      _oppositeObjectID = sourceObjectEndPoint.OppositeObjectID;
    }

    public class TestSetCommand : ObjectEndPointSetCommand
    {
      public TestSetCommand (IObjectEndPoint modifiedEndPoint, DomainObject newRelatedObject, Action<ObjectID> oppositeObjectIDSetter)
        : base (modifiedEndPoint, newRelatedObject, oppositeObjectIDSetter)
      {
      }

      public override ExpandedCommand ExpandToAllRelatedObjects ()
      {
        throw new NotImplementedException ();
      }
    }
  }
}