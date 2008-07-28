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

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public abstract class ClassFromOtherBusinessObjectImplementation : IBusinessObject
  {
    public abstract object GetProperty (IBusinessObjectProperty property);

    public abstract void SetProperty (IBusinessObjectProperty property, object value);

    public abstract string GetPropertyString (IBusinessObjectProperty property, string format);

    public abstract string DisplayName { get; }

    public abstract string DisplayNameSafe { get; }

    public abstract IBusinessObjectClass BusinessObjectClass { get; }
  }
}
