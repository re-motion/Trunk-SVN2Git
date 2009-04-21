// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
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
