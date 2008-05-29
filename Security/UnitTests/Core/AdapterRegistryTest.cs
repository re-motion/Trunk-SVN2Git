using System;
using NUnit.Framework;
using Remotion.Security.BridgeImplementations;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core
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