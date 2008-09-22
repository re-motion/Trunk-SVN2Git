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

namespace Remotion.UnitTests.Diagnostics.TestDomain
{
  internal class TestSimpleToTextBuilderXmlTest
  {
    public TestSimpleToTextBuilderXmlTest ()
    {
      Name = "ABC abc";
      Int = 54321;
    }

    public TestSimpleToTextBuilderXmlTest (string name, int i)
    {
      Name = name;
      Int = i;
    }

    public string Name { get; set; }
    public int Int { get; set; }
    public TestSimpleToTextBuilderXmlTestOwned Talk { get; set; }

    //public override string ToString ()
    //{
    //  return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
    //}
  }




}