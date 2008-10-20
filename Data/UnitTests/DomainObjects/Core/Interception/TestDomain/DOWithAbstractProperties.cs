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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain
{
  [DBTable]
  [Instantiable]
  public abstract class DOWithAbstractProperties : DomainObject
  {
    public abstract int PropertyWithGetterAndSetter { get; set; }
    public abstract string PropertyWithGetterOnly { get; }
    public abstract DateTime PropertyWithSetterOnly { set; }
    protected abstract int ProtectedProperty { get; set; }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    public new string GetAndCheckCurrentPropertyName ()
    {
      return base.GetAndCheckCurrentPropertyName();
    }
  }
}
