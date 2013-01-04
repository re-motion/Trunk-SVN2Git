// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using Remotion.BridgeImplementations;
using Rhino.Mocks;

namespace Remotion.UnitTests
{
  [TestFixture]
  public class AdapterRegistryTest
  {
    private AdapterRegistryImplementation _adapterRegistry;
    private MockRepository _mocks;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _adapterRegistry = new AdapterRegistryMock ();
    }

    [Test]
    public void GetInstance ()
    {
      Assert.That (AdapterRegistry.Instance, Is.Not.Null);
    }

    [Test]
    public void GetInstance_SameTwice ()
    {
      Assert.That (AdapterRegistry.Instance, Is.SameAs (AdapterRegistry.Instance));
    }

    [Test]
    public void SetAndGetProvider ()
    {
      IAdapter exptectedAdapter = _mocks.StrictMock<IAdapter> ();
      _mocks.ReplayAll ();

      _adapterRegistry.SetAdapter (typeof (IAdapter), exptectedAdapter);

      Assert.That (_adapterRegistry.GetAdapter<IAdapter> (), Is.SameAs (exptectedAdapter));
    }

    [Test]
    public void GetProviderNotSet ()
    {
      Assert.That (_adapterRegistry.GetAdapter<IAdapter> (), Is.Null);
    }

    [Test]
    public void SetProviderNull ()
    {
      IAdapter adapter = _mocks.StrictMock<IAdapter> ();
      _mocks.ReplayAll ();

      _adapterRegistry.SetAdapter (typeof (IAdapter), adapter);
      Assert.That (_adapterRegistry.GetAdapter<IAdapter> (), Is.Not.Null);

      _adapterRegistry.SetAdapter (typeof (IAdapter), null);
      Assert.That (_adapterRegistry.GetAdapter<IAdapter> (), Is.Null);
    }
  }
}
