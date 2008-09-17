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
using Remotion.Diagnostics.ToText;

namespace Remotion.UnitTests.Diagnostics.TestDomain
{
  public class TestSimpleIToTextHandler : IToText
  {
    public TestSimpleIToTextHandler ()
    {
      Name = "Kal-El";
      Int = 2468;
    }

    public string Name { get; set; }
    public int Int { get; set; }

    public override string ToString ()
    {
      return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
    }

    public void ToText (IToTextBuilderBase toTextBuilder)
    {
      toTextBuilder.sb ().e ("daInt", Int).e ("theName", Name).se ();
    }
  }
}