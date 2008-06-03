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

using Remotion.Data.DomainObjects;

namespace Remotion.Data.DomainObjects.PerformanceTests.TestDomain
{
  public abstract class ClientBoundBaseClass : DomainObject
  {
    // types

    // static members and constants

    public static ClientBoundBaseClass GetObject (ObjectID id)
    {
      return DomainObject.GetObject<ClientBoundBaseClass> (id);
    }

    public static ClientBoundBaseClass GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      using (clientTransaction.EnterNonDiscardingScope())
      {
        return DomainObject.GetObject<ClientBoundBaseClass> (id);
      }
    }

    // member fields

    // construction and disposing

    protected ClientBoundBaseClass ()
    {
    }

    // methods and properties

    [DBBidirectionalRelation ("ClientBoundBaseClasses")]
    [Mandatory]
    public abstract Client Client { get; set;}
  }
}
