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