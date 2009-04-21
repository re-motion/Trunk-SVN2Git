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
      Assert.IsNotNull (AdapterRegistry.Instance);
    }

    [Test]
    public void SetAndGetProvider ()
    {
      IAdapter exptectedAdapter = _mocks.StrictMock<IAdapter> ();
      _mocks.ReplayAll ();

      _adapterRegistry.SetAdapter (typeof (IAdapter), exptectedAdapter);

      Assert.AreSame (exptectedAdapter, _adapterRegistry.GetAdapter<IAdapter> ());
    }

    [Test]
    public void GetProviderNotSet ()
    {
      Assert.IsNull (_adapterRegistry.GetAdapter<IAdapter> ());
    }

    [Test]
    public void SetProviderNull ()
    {
      IAdapter adapter = _mocks.StrictMock<IAdapter> ();
      _mocks.ReplayAll ();

      _adapterRegistry.SetAdapter (typeof (IAdapter), adapter);
      Assert.IsNotNull (_adapterRegistry.GetAdapter<IAdapter> ());

      _adapterRegistry.SetAdapter (typeof (IAdapter), null);
      Assert.IsNull (_adapterRegistry.GetAdapter<IAdapter> ());
    }
  }
}
