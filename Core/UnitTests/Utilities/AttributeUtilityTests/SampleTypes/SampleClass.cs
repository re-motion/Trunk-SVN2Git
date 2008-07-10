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

namespace Remotion.UnitTests.Utilities.AttributeUtilityTests.SampleTypes
{
  [Inherited]
  public class SampleClass
  {
    [Inherited]
    public virtual string PropertyWithSingleAttribute
    {
      get { return null; }
    }

    [Inherited]
    protected virtual string ProtectedPropertyWithAttribute
    {
      get { return null; }
    }

    [Multiple]
    public virtual string PropertyWithMultipleAttribute
    {
      get { return null; }
    }
  }
}