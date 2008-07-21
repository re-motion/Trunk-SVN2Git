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
using NUnit.Framework;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.DomainObjects.UnitTests.DomainObjects.Queries
{
  [TestFixture]
  public class QueryParameterTest : StandardMappingTest
  {
    private QueryParameter _parameter;

    public override void SetUp ()
    {
      base.SetUp ();

      _parameter = new QueryParameter ("name", "value", QueryParameterType.Value);
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreEqual ("name", _parameter.Name);
      Assert.AreEqual ("value", _parameter.Value);
      Assert.AreEqual (QueryParameterType.Value, _parameter.ParameterType);
    }

    [Test]
    public void SetValue ()
    {
      _parameter.Value = "NewValue";
      Assert.AreEqual ("NewValue", _parameter.Value);
    }

    [Test]
    public void SetParameterType ()
    {
      _parameter.ParameterType = QueryParameterType.Text;
      Assert.AreEqual (QueryParameterType.Text, _parameter.ParameterType);
    }
  }
}
