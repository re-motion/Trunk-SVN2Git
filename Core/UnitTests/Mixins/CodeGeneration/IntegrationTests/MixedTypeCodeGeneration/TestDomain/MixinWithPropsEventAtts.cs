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
  [Replicatable (4)]
  public class MixinWithPropsEventAtts : IMixinWithPropsEventsAtts
  {
    [Replicatable ("bla")]
    public int Property
    {
      [Replicatable (5, Named2 = 1.0)]
      get;
      [Replicatable (5, Named2 = 2.0)]
      set;
    }

    [Replicatable ("blo")]
    public event EventHandler Event
    {
      [Replicatable (1)]
      add { }
      [Replicatable (2)]
      remove { }
    }
  }
}