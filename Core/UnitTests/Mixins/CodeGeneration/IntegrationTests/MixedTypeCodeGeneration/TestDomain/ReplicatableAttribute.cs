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
  public class ReplicatableAttribute : Attribute
  {
    private readonly int _i;
    private readonly string _s;

    public ReplicatableAttribute (int i)
    {
      _i = i;
    }

    public ReplicatableAttribute (string s)
    {
      _s = s;
    }

    public int I
    {
      get { return _i; }
    }

    public string S
    {
      get { return _s; }
    }

    public double Named2 { get; set; }
  }
}