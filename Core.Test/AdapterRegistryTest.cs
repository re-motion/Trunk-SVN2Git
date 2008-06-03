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
      IAdapter exptectedAdapter = _mocks.CreateMock<IAdapter> ();
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
      IAdapter adapter = _mocks.CreateMock<IAdapter> ();
      _mocks.ReplayAll ();

      _adapterRegistry.SetAdapter (typeof (IAdapter), adapter);
      Assert.IsNotNull (_adapterRegistry.GetAdapter<IAdapter> ());

      _adapterRegistry.SetAdapter (typeof (IAdapter), null);
      Assert.IsNull (_adapterRegistry.GetAdapter<IAdapter> ());
    }
  }
}
