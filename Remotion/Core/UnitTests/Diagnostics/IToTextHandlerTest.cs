// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.Logging;
using Remotion.Diagnostics.ToText;
using Remotion.UnitTests.Diagnostics.TestDomain;

namespace Remotion.UnitTests.Diagnostics
{
  [TestFixture]
  public class IToTextHandlerTest
  {
    private ISimpleLogger log = SimpleLogger.CreateForConsole (false);



    [Test]
    public void IToTextHandler ()
    {
      var toTextProvider = GetTextProvider();
      var testSimple = new TestSimpleIToTextHandler();
      string result = toTextProvider.ToTextString (testSimple);
      log.It (result);
      Assert.That (result, Is.EqualTo ("(daInt=2468,theName=\"Kal-El\")"));
    }

    //[Test]
    //public void RegisteredHandlerOverIToTextHandler ()
    //{
    //  var toTextProvider = GetTextProvider();
    //  toTextProvider.RegisterSpecificTypeHandler<TestSimple> ((x, ttb) => ttb.s ("TestSimple...").e (x.Int).comma.e (x.Name).s ("and out!"));
    //  var testSimple = new TestSimple();
    //  string result = toTextProvider.ToTextString (testSimple);
    //  log.It (result);
    //  Assert.That (result, Is.EqualTo ("TestSimple...2468,\"Kal-El\"and out!"));
    //}

    public static ToTextProvider GetTextProvider ()
    {
      var toTextProvider = new ToTextProvider();
      toTextProvider.Settings.UseAutomaticObjectToText = false;
      toTextProvider.Settings.UseAutomaticStringEnclosing = true;
      toTextProvider.Settings.UseAutomaticCharEnclosing = true;
      return toTextProvider;
    }
  }
}
