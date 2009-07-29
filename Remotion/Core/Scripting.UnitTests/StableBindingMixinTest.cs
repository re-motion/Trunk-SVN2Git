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
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Scripting.UnitTests.TestDomain;

namespace Remotion.Scripting.UnitTests
{
  [TestFixture]
  public class StableBindingMixinTest
  {
    [Test]
    public void MixinTest_IsAmbigous ()
    {
      // Create Script Context which filters IAmbigous1
      // Create class MixinTest which explicitely implements IAmbigous1 & 2
      // Check that MixinTest is ambigous in script
      // Create MixinTestChild derived from MixinTest which is mixed w StableBindingMixin
      // Check that MixinTestChild is not ambigous in script
    }  
  }

  public class MixinTest : IAmbigous1, IAmbigous2
  {
    string IAmbigous1.StringTimes (string text, int number)
    {
      return "IAmbigous1.StringTimes" + StringTimes (text, number);
    }
    
    string IAmbigous2.StringTimes (string text, int number)
    {
      return "IAmbigous2.StringTimes" + StringTimes (text, number);
    }

    private string StringTimes (string text, int number)
    {
      return text.ToSequence (number).Aggregate ((sa, s) => sa + s);
    }
  }
}