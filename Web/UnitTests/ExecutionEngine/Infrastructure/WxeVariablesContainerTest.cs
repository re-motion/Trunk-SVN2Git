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
using System;
using System.Collections.Specialized;
using NUnit.Framework;
using Remotion.Web.UnitTests.ExecutionEngine.TestFunctions;

namespace Remotion.Web.UnitTests.ExecutionEngine.Infrastructure
{
  [TestFixture]
  public class WxeVariablesContainerTest
  {
    [Test]
    public void SerializeParameters ()
    {
      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters ("Hello World", null, 1);
      NameValueCollection parameters = function.VariablesContainer.SerializeParametersForQueryString();
      Assert.AreEqual (3, parameters.Count);
      Assert.AreEqual ("Hello World", parameters["StringValue"]);
      Assert.AreEqual ("", parameters["NaInt32Value"]);
      Assert.AreEqual ("1", parameters["IntValue"]);
    }

    [Test]
    public void SerializeParametersWithInt32BeingNull ()
    {
      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.Variables["StringValue"] = "Hello World";
      function.Variables["NaInt32Value"] = 1;
      function.Variables["Int32Value"] = null;

      NameValueCollection parameters = function.VariablesContainer.SerializeParametersForQueryString();

      Assert.AreEqual (2, parameters.Count);
      Assert.AreEqual ("Hello World", parameters["StringValue"]);
      Assert.AreEqual ("1", parameters["NaInt32Value"]);
    }

    [Test]
    public void InitializeParameters ()
    {
      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "Hello World");
      parameters.Add ("NaInt32Value", "");
      parameters.Add ("IntValue", "1");

      function.VariablesContainer.InitializeParameters (parameters);

      Assert.AreEqual ("Hello World", function.StringValue);
      Assert.AreEqual (null, function.NaInt32Value);
      Assert.AreEqual (1, function.IntValue);
    }

    [Test]
    public void InitializeParameters_WithStringBeingEmpty ()
    {
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "");
      parameters.Add ("NaInt32Value", "2");
      parameters.Add ("IntValue", "1");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.VariablesContainer.InitializeParameters (parameters);

      Assert.AreEqual ("", function.StringValue);
      Assert.AreEqual (2, function.NaInt32Value);
      Assert.AreEqual (1, function.IntValue);
    }

    [Test]
    public void InitializeParameters_WithNaInt32BeingEmpty ()
    {
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "Hello World");
      parameters.Add ("NaInt32Value", "");
      parameters.Add ("IntValue", "1");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.VariablesContainer.InitializeParameters (parameters);

      Assert.AreEqual ("Hello World", function.StringValue);
      Assert.AreEqual (null, function.NaInt32Value);
      Assert.AreEqual (1, function.IntValue);
    }

    [Test]
    [ExpectedException (typeof (ApplicationException))]
    public void InitializeParameters_WitInt32BeingEmpty ()
    {
      NameValueCollection parameters = new NameValueCollection();
      parameters.Add ("StringValue", "Hello World");
      parameters.Add ("NaInt32Value", "2");
      parameters.Add ("IntValue", "");

      TestFunctionWithSerializableParameters function = new TestFunctionWithSerializableParameters();
      function.VariablesContainer.InitializeParameters (parameters);
    }
  }
}
