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
  public class ChangeState
  {
    // types

    // static members and constants

    // member fields

    private object _sender;
    private string _message;

    // construction and disposing

    public ChangeState (object sender)
      : this (sender, null)
    {
    }

    public ChangeState (object sender, string message)
    {
      ArgumentUtility.CheckNotNull ("sender", sender);

      _sender = sender;
      _message = message;
    }

    // methods and properties

    public object Sender
    {
      get { return _sender; }
    }

    public string Message
    {
      get { return _message; }
    }

    public virtual void Check (ChangeState expectedState)
    {
      ArgumentUtility.CheckNotNull ("expectedState", expectedState);

      if (this.GetType () != expectedState.GetType ())
      {
        throw CreateApplicationException (
            "Type of actual state '{0}' does not match type of expected state '{1}'.",
            this.GetType (),
            expectedState.GetType ());
      }

      if (!ReferenceEquals (_sender, expectedState.Sender))
      {
        throw CreateApplicationException (
            "Actual sender '{0}' does not match expected sender '{1}'.", _sender, expectedState.Sender);
      }
    }

    protected ApplicationException CreateApplicationException (string message, params object[] args)
    {
      return new ApplicationException (string.Format (message, args));
    }
  }
}
