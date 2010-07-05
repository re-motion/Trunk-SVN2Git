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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.Linq.UnitTests.Linq.Core.Parsing.ExpressionTreeVisitorTests;

namespace Remotion.Data.Linq.UnitTests.Linq.Core.Utilities
{
  [TestFixture]
  public class RegistryBaseTest
  {
    private TestRegistry _registry;

    [SetUp]
    public void SetUp ()
    {
      _registry = new TestRegistry();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_registry.GetItem (typeof (TestRegistryImplementation)), Is.Null);
    }

    [Test]
    public void CreateDefault_GetItem ()
    {
      _registry = TestRegistry.CreateDefault();
      Assert.That (_registry.GetItem (typeof (TestRegistryImplementation)), Is.TypeOf (typeof (TestRegistryImplementation)));
    }

    [Test]
    public void CreateDefault_GetItemExact ()
    {
      _registry = TestRegistry.CreateDefault ();
      Assert.That (_registry.GetItemExact (typeof (TestRegistryImplementation)), Is.TypeOf (typeof (TestRegistryImplementation)));
    }

    [Test]
    public void CreateDefault_GetUnRegisteredType ()
    {
      Assert.That (_registry.GetItem (typeof (TestRegistry)), Is.Null);
    }

    [Test]
    public void RegisterTypeManually ()
    {
      _registry.Register (typeof (TestRegistryImplementation), new TestRegistryImplementation());
      Assert.That (_registry.GetItem (typeof (TestRegistryImplementation)), Is.Not.Null);
    }

    [Test]
    public void RegisterTypesManually ()
    {
      _registry.Register (new [] { typeof (TestRegistryImplementation) }, new TestRegistryImplementation ());
      Assert.That (_registry.GetItem (typeof (TestRegistryImplementation)), Is.Not.Null);
    }

    [Test]
    public void IsRegistered ()
    {
      _registry.Register (new[] { typeof (TestRegistryImplementation) }, new TestRegistryImplementation ());

      Assert.That (_registry.IsRegistered (typeof (TestRegistryImplementation)), Is.True);
      Assert.That (_registry.IsRegistered (typeof (TypeForNewExpression)), Is.False);
    }
    
  }
}
