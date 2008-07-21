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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.EventReceiver
{
  [Serializable]
  public class RelationChangeState : ChangeState
  {
    // types

    // static members and constants

    // member fields

    private string _propertyName;
    private DomainObject _oldDomainObject;
    private DomainObject _newDomainObject;

    // construction and disposing

    public RelationChangeState (
        object sender,
        string propertyName,
        DomainObject oldDomainObject,
        DomainObject newDomainObject)
      : this (sender, propertyName, oldDomainObject, newDomainObject, null)
    {
    }

    public RelationChangeState (
        object sender,
        string propertyName,
        DomainObject oldDomainObject,
        DomainObject newDomainObject,
        string message)
      : base (sender, message)
    {
      ArgumentUtility.CheckNotNull ("propertyName", propertyName);

      _propertyName = propertyName;
      _oldDomainObject = oldDomainObject;
      _newDomainObject = newDomainObject;
    }

    // methods and properties

    public string PropertyName
    {
      get { return _propertyName; }
    }

    public DomainObject OldDomainObject
    {
      get { return _oldDomainObject; }
    }

    public DomainObject NewDomainObject
    {
      get { return _newDomainObject; }
    }

    public override void Check (ChangeState expectedState)
    {
      base.Check (expectedState);

      RelationChangeState relationChangeState = (RelationChangeState) expectedState;

      if (_propertyName != relationChangeState.PropertyName)
      {
        throw CreateApplicationException (
            "Actual PropertyName '{0}' and expected PropertyName '{1}' do not match.",
            _propertyName,
            relationChangeState.PropertyName);
      }

      if (!ReferenceEquals (_oldDomainObject, relationChangeState.OldDomainObject))
      {
        throw CreateApplicationException (
            "Actual old related DomainObject '{0}' and expected old related DomainObject '{1}' do not match.",
            GetObjectIDAsText (_oldDomainObject),
            GetObjectIDAsText (relationChangeState.OldDomainObject));
      }

      if (!ReferenceEquals (_newDomainObject, relationChangeState.NewDomainObject))
      {
        throw CreateApplicationException (
            "Actual new related DomainObject '{0}' and expected new related DomainObject '{1}' do not match.",
            GetObjectIDAsText (_newDomainObject),
            GetObjectIDAsText (relationChangeState.NewDomainObject));
      }
    }

    private string GetObjectIDAsText (DomainObject domainObject)
    {
      if (domainObject != null)
        return domainObject.ID.ToString ();
      else
        return "null";
    }
  }
}
