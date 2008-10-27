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

namespace Remotion.UnitTests.Mixins.CodeGeneration.IntegrationTests.MixedTypeCodeGeneration.TestDomain
{
  [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
  public class TargetWithAttributes
  {
    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public virtual void Method ()
    {
    }

    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public virtual int Property
    {
      get { return 0; }
    }

    [MultiInherited, MultiNonInherited, NonMultiInherited, NonMultiNonInherited]
    public virtual event EventHandler Event;
  }
}