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
using NUnit.Framework;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Queries
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
