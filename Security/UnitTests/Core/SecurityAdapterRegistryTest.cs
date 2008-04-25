using System;
using NUnit.Framework;
using Rhino.Mocks;

namespace Remotion.Security.UnitTests.Core
{
  [TestFixture]
  public class SecurityAdapterRegistryTest
  {
    private SecurityAdapterRegistry _securityAdapterRegistry;
    private MockRepository _mocks;

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _securityAdapterRegistry = new SecurityAdapterRegistryMock ();
    }

    [Test]
    public void GetInstance ()
    {
      Assert.IsNotNull (SecurityAdapterRegistry.Instance);
    }

    [Test]
    public void SetAndGetProvider ()
    {
      ISecurityAdapter exptectedAdapter = _mocks.CreateMock<ISecurityAdapter> ();
      _mocks.ReplayAll ();
      
      _securityAdapterRegistry.SetAdapter (typeof (ISecurityAdapter), exptectedAdapter);

      Assert.AreSame (exptectedAdapter, _securityAdapterRegistry.GetAdapter<ISecurityAdapter> ());
    }

    [Test]
    public void GetProviderNotSet ()
    {
      Assert.IsNull (_securityAdapterRegistry.GetAdapter<ISecurityAdapter> ());
    }

    [Test]
    public void SetProviderNull ()
    {
      ISecurityAdapter adapter = _mocks.CreateMock<ISecurityAdapter> ();
      _mocks.ReplayAll ();

      _securityAdapterRegistry.SetAdapter(typeof (ISecurityAdapter), adapter);
      Assert.IsNotNull (_securityAdapterRegistry.GetAdapter<ISecurityAdapter> ());

      _securityAdapterRegistry.SetAdapter (typeof (ISecurityAdapter), null);
      Assert.IsNull (_securityAdapterRegistry.GetAdapter<ISecurityAdapter> ());
    }
  }
}