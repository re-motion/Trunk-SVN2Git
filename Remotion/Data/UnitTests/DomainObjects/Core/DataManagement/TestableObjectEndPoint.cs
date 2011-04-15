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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.DataManagement.Commands;
using Remotion.Data.DomainObjects.DataManagement.Commands.EndPointModifications;
using Remotion.Data.DomainObjects.Infrastructure.Serialization;

namespace Remotion.Data.UnitTests.DomainObjects.Core.DataManagement
{
  public class TestableObjectEndPoint : ObjectEndPoint
  {
    private readonly DomainObject _originalOppositeObject;
    private DomainObject _oppositeObject;
    private bool _hasBeenTouched;
    private bool _isSetOppositeObjectFromExpected;

    public TestableObjectEndPoint (ClientTransaction clientTransaction, RelationEndPointID id, IRelationEndPointLazyLoader lazyLoader, IRelationEndPointProvider endPointProvider, DomainObject oppositeObject)
        : base (clientTransaction, id, lazyLoader, endPointProvider)
    {
      _originalOppositeObject = oppositeObject;
      _oppositeObject = oppositeObject;
      _hasBeenTouched = false;
    }

    public TestableObjectEndPoint (FlattenedDeserializationInfo info)
        : base (info)
    {
    }

    public override bool HasChanged
    {
      get { return _oppositeObject != _originalOppositeObject; }
    }

    public override bool HasBeenTouched
    {
      get { return _hasBeenTouched; }
    }

    public override bool IsDataComplete
    {
      get { throw new NotImplementedException(); }
    }

    public override bool IsSynchronized
    {
      get { throw new NotImplementedException(); }
    }

    public override void EnsureDataComplete ()
    {
      throw new NotImplementedException ();
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
      get { return DomainObject.GetIDOrNull (_oppositeObject); }
    }

    public void SetOppositeObject (DomainObject domainObject)
    {
      _oppositeObject = domainObject;
    }

    public override ObjectID OriginalOppositeObjectID
    {
      get { return DomainObject.GetIDOrNull (_originalOppositeObject); }
    }

    public override DomainObject GetOppositeObject (bool includeDeleted)
    {
      return _oppositeObject;
    }

    public override DomainObject GetOriginalOppositeObject ()
    {
      return _originalOppositeObject;
    }

    public void ExpectSetOppositeObjectFrom()
    {
      _isSetOppositeObjectFromExpected = true;
    }

    public void ExpectNotSetOppositeObjectFrom ()
    {
      _isSetOppositeObjectFromExpected = false;
    }

    public override IDataManagementCommand CreateSetCommand (DomainObject newRelatedObject)
    {
      return new TestSetCommand (this, newRelatedObject, id => { throw new NotImplementedException (); });
    }

    protected override void SetOppositeObjectDataFromSubTransaction (IObjectEndPoint sourceObjectEndPoint)
    {
      Assert.That (_isSetOppositeObjectFromExpected, Is.True);
      _oppositeObject = sourceObjectEndPoint.GetOppositeObject (true);
    }

    public class TestSetCommand : ObjectEndPointSetCommand
    {
      public TestSetCommand (IObjectEndPoint modifiedEndPoint, DomainObject newRelatedObject, Action<DomainObject> oppositeObjectSetter)
        : base (modifiedEndPoint, newRelatedObject, oppositeObjectSetter)
      {
      }

      public override ExpandedCommand ExpandToAllRelatedObjects ()
      {
        throw new NotImplementedException ();
      }
    }
  }
}