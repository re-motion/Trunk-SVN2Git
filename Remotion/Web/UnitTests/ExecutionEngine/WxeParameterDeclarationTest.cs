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
using System.Globalization;
using NUnit.Framework;
using Remotion.Web.ExecutionEngine;
using Remotion.Web.ExecutionEngine.Infrastructure;

namespace Remotion.Web.UnitTests.ExecutionEngine
{
  [TestFixture]
  public class WxeParameterDeclarationTest
  {
    private static readonly WxeParameterDeclaration[] s_parameters = {
        new WxeParameterDeclaration ("p1", true, WxeParameterDirection.In, typeof (string)),
        new WxeParameterDeclaration ("p2", true, WxeParameterDirection.In, typeof (bool)),
        new WxeParameterDeclaration ("p3", true, WxeParameterDirection.In, typeof (DateTime)),
        new WxeParameterDeclaration ("p4", true, WxeParameterDirection.In, typeof (object))
    };

    [Test]
    public void TestParse1 ()
    {
      // "this \"special\", value", "true", "2004-03-25 12:00", var1
      string args = @"""this \""special\"", value"", ""true"", ""2004-03-25 12:00"", var1";
      object[] result = WxeVariablesContainer.ParseActualParameters (s_parameters, args, CultureInfo.InvariantCulture);
      Assert.AreEqual (4, result.Length);
      Assert.AreEqual ("this \"special\", value", result[0]);
      Assert.AreEqual (true, result[1]);
      Assert.AreEqual (new DateTime (2004, 3, 25, 12, 0, 0), result[2]);
      Assert.AreEqual (new WxeVariableReference ("var1"), result[3]);
    }

    [Test]
    public void TestParse2 ()
    {
      // "value", true, 2004-03-25 12:00, var1
      string args = @"""value"", true, 2004-03-25 12:00, var1";
      object[] result = WxeVariablesContainer.ParseActualParameters (s_parameters, args, CultureInfo.InvariantCulture);
      Assert.AreEqual (4, result.Length);
      Assert.AreEqual ("value", result[0]);
      Assert.AreEqual (true, result[1]);
      Assert.AreEqual (new DateTime (2004, 3, 25, 12, 0, 0), result[2]);
      Assert.AreEqual (new WxeVariableReference ("var1"), result[3]);
    }

    [Test]
    [ExpectedException (typeof (ApplicationException))]
    public void TestParseEx1 ()
    {
      WxeVariablesContainer.ParseActualParameters (s_parameters, "a, b\"b, c", CultureInfo.InvariantCulture);
    }

    [Test]
    [ExpectedException (typeof (ApplicationException))]
    public void TestParseEx2 ()
    {
      WxeVariablesContainer.ParseActualParameters (s_parameters, "a, \"xyz\"", CultureInfo.InvariantCulture);
    }

    public void g ()
    {
      WxeParameterDeclaration parameter =
          new WxeParameterDeclaration ("param", true, WxeParameterDirection.In, typeof (string));
    }
  }
}
