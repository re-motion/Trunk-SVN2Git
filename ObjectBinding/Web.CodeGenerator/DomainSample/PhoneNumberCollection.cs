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
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.NullableValueTypes;

namespace DomainSample
{
public class PhoneNumberCollection : DomainObjectCollection
{
  // types

  // static members and constants

  // member fields

  // construction and disposing

  public PhoneNumberCollection () : base (typeof (PhoneNumber))
  {
  }

  // methods and properties

  public new PhoneNumber this[int index]
  {
    get { return (PhoneNumber) base[index]; }
    set { base[index] = value; }
  }

  public new PhoneNumber this[ObjectID id]
  {
    get { return (PhoneNumber) base[id]; }
  }

}
}
