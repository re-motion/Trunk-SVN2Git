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
using System.Web.Script.Services;
using System.Web.Services;
using NUnit.Framework;
using Remotion.Web.Utilities;

namespace Remotion.Web.UnitTests.Core.Utilities
{
  [TestFixture]
  public class WebServiceUtilityTest
  {
    [ScriptService]
    private class TestScriptService : WebService
    {
      [ScriptMethod]
      public void ScriptMethod ()
      {
      }

      [ScriptMethod (ResponseFormat = ResponseFormat.Json)]
      public void JsonMethod ()
      {
      }

      public void MethodWithoutScriptMethodAttribute ()
      {
      }

      [ScriptMethod (ResponseFormat = ResponseFormat.Xml)]
      public void MethodWithResponeFormatNotJson ()
      {
      }
    }

    private class TestWebService : WebService
    {
      public void Method ()
      {
      }
    }

    [Test]
    public void CheckJsonServiceMethod_Valid ()
    {
      Assert.That (() => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "JsonMethod"), Throws.Nothing);
    }

    [Test]
    public void CheckJsonService_ResponseFormatNotJson ()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "MethodWithResponeFormatNotJson"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'MethodWithResponeFormatNotJson' on web service type "
                  + "'Remotion.Web.UnitTests.Core.Utilities.WebServiceUtilityTest+TestScriptService'"
                  + " does not have the ResponseFormat property of the ScriptMethodAttribute set to Json."));
    }

    [Test]
    public void CheckJsonService_MissingScriptServiceAttribute ()
    {
      Assert.That (
          () => WebServiceUtility.CheckJsonService (typeof (TestWebService), "Method"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web service type 'Remotion.Web.UnitTests.Core.Utilities.WebServiceUtilityTest+TestWebService'"
                  + " does not have the 'System.Web.Script.Services.ScriptServiceAttribute' applied."));
    }

    [Test]
    public void CheckScriptServiceMethod_Valid ()
    {
      Assert.That (() => WebServiceUtility.CheckJsonService (typeof (TestScriptService), "ScriptMethod"), Throws.Nothing);
    }

    [Test]
    public void CheckScriptService_MissingScriptMethodAttribute ()
    {
      Assert.That (
          () => WebServiceUtility.CheckScriptService (typeof (TestScriptService), "MethodWithoutScriptMethodAttribute"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web method 'MethodWithoutScriptMethodAttribute' on web service type "
                  + "'Remotion.Web.UnitTests.Core.Utilities.WebServiceUtilityTest+TestScriptService'"
                  + " does not have the 'System.Web.Script.Services.ScriptMethodAttribute' applied."));
    }

    [Test]
    public void CheckScriptService_MissingScriptServiceAttribute ()
    {
      Assert.That (
          () => WebServiceUtility.CheckScriptService (typeof (TestWebService), "Method"),
          Throws.ArgumentException
              .And.Message.EqualTo (
                  "Web service type 'Remotion.Web.UnitTests.Core.Utilities.WebServiceUtilityTest+TestWebService'"
                  + " does not have the 'System.Web.Script.Services.ScriptServiceAttribute' applied."));
    }
  }
}