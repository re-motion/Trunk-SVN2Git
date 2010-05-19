// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
using Remotion.Scripting.UnitTests.Diagnostics.ToText;

namespace Remotion.Scripting.UnitTests.Diagnostics.ToTextUnitTests.TestDomain
{
  public class TestSimpleIToTextHandler : IToTextConvertible
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

    public void ToText (IToTextBuilder toTextBuilder)
    {
      toTextBuilder.sb ().e ("daInt", Int).e ("theName", Name).se ();
    }
  }
}
