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
using System.Runtime.CompilerServices;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception.SampleTypes
{
  [DBTable]
  public class DOWithAutomaticProperties : DomainObject
  {
    public virtual int PropertyWithGetterAndSetter
    {
      [CompilerGenerated]
      get { throw new NotImplementedException (); }
      [CompilerGenerated]
      set { throw new NotImplementedException (); } 
    }

    public virtual string PropertyWithGetterOnly
    {
      [CompilerGenerated]
      get { throw new NotImplementedException (); }
    }

    public virtual DateTime PropertyWithSetterOnly
    {
      [CompilerGenerated]
      set { throw new NotImplementedException (); }
    }

    protected virtual int ProtectedProperty
    {
      [CompilerGenerated]
      get { throw new NotImplementedException (); }
      [CompilerGenerated]
      set { throw new NotImplementedException (); }
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }
  }
}
