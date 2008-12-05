// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
using Remotion.Utilities;

namespace Remotion.Data.UnitTests.DomainObjects.Core.EventReceiver
{
  [Serializable]
  public class CollectionChangeState : ChangeState
  {
    // types

    // static members and constants

    // member fields

    private DomainObject _domainObject;

    // construction and disposing

    public CollectionChangeState (object sender, DomainObject domainObject)
      : this (sender, domainObject, null)
    {
    }

    public CollectionChangeState (object sender, DomainObject domainObject, string message)
      : base (sender, message)
    {
      ArgumentUtility.CheckNotNull ("domainObject", domainObject);

      _domainObject = domainObject;
    }

    // methods and properties

    public DomainObject DomainObject
    {
      get { return _domainObject; }
    }

    public override void Check (ChangeState expectedState)
    {
      base.Check (expectedState);

      CollectionChangeState collectionChangeState = (CollectionChangeState) expectedState;

      if (!ReferenceEquals (_domainObject, collectionChangeState.DomainObject))
      {
        throw CreateApplicationException (
            "Affected actual DomainObject '{0}' and expected DomainObject '{1}' do not match.",
            _domainObject.ID,
            collectionChangeState.DomainObject.ID);
      }
    }
  }
}
