/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.UnitTests.Core.EventReceiver
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
