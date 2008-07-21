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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Interception.SampleTypes
{
  [DBTable]
  [Serializable]
  public class DOWithVirtualProperties : DomainObject
  {
    public virtual int PropertyWithGetterAndSetter
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }

    public virtual string PropertyWithGetterOnly
    {
      get { return CurrentProperty.GetValue<string> (); }
    }

    public virtual DateTime PropertyWithSetterOnly
    {
      set { CurrentProperty.SetValue (value); }
    }

    protected virtual int ProtectedProperty
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }

    public virtual DateTime PropertyThrowing
    {
      get { throw new Exception (); }
      set { throw new Exception (); }
    }

    [StorageClassNone]
    public virtual DateTime PropertyNotInMapping
    {
      get { return CurrentProperty.GetValue<DateTime>(); }
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    public new string GetAndCheckCurrentPropertyName()
    {
      return base.GetAndCheckCurrentPropertyName();
    }
  }
}
