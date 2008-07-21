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

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.EventReceiver
{
  [Serializable]
  public class ObjectDeletionState : ChangeState
  {
    // types

    // static members and constants

    // member fields

    // construction and disposing

    public ObjectDeletionState (object sender)
      : this (sender, null)
    {
    }

    public ObjectDeletionState (object sender, string message)
      : base (sender, message)
    {
    }

    // methods and properties
  }
}
