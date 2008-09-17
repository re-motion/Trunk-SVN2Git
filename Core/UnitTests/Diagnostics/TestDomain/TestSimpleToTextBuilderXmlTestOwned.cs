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
  public class TestSimpleToTextBuilderXmlTestOwned
  {
    public TestSimpleToTextBuilderXmlTestOwned (string name)
    {
      Name = name;
    }

    public string Name { get; set; }
    public string Short { get; set; }
    public string Description { get; set; }
    public System.Collections.Generic.List<String> Participants { get; set; }

    //public override string ToString ()
    //{
    //  return String.Format ("((TestSimple) Name:{0},Int:{1})", Name, Int);
    //}
  }
}