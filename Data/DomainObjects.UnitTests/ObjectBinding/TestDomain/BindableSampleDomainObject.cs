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
using Remotion.Data.DomainObjects.ObjectBinding;

namespace Remotion.Data.DomainObjects.UnitTests.ObjectBinding.TestDomain
{
  [BindableDomainObject]
  [Instantiable]
  [Serializable]
  [DBTable]
  public abstract class BindableSampleDomainObject : DomainObject
  {
    public static BindableSampleDomainObject NewObject ()
    {
      return NewObject<BindableSampleDomainObject> ().With ();
    }

    public static BindableSampleDomainObject GetObject (ObjectID id)
    {
      return GetObject<BindableSampleDomainObject> (id);
    }

    public abstract string Name { get; set; }
    public abstract int Int32 { get; set; }
  }
}
