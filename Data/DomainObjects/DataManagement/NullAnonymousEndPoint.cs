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
using Remotion.Data.DomainObjects.Mapping;

namespace Remotion.Data.DomainObjects.DataManagement
{
public class NullAnonymousEndPoint : AnonymousEndPoint
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public NullAnonymousEndPoint (RelationDefinition relationDefinition) : base (relationDefinition)
  {
  }

  // methods and properties

  public override ClientTransaction ClientTransaction
  {
    get { return null; }
  }

  public override DomainObject GetDomainObject ()
  {
    return null; 
  }

  public override DataContainer GetDataContainer ()
  {
    return null;
  }

  public override ObjectID ObjectID
  {
    get { return null; }
  }


  public override bool IsNull
  {
    get { return true; }
  }
}
}
