using System;
using System.Security.Principal;
using Remotion.Security;
using Remotion.Web.UnitTests.Security.Domain;
using Rhino.Mocks;
using Remotion.Web.ExecutionEngine;

namespace Remotion.Web.UnitTests.Security.UI.WebSecurityAdapterTests
{
  public class WebPermissionProviderTestHelper
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IPrincipal _user;
    private ISecurityProvider _mockSecurityProvider;
    private IUserProvider _mockUserProvider;
    private IObjectSecurityStrategy _mockObjectSecurityStrategy;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private IWxeSecurityAdapter _mockWxeSecurityAdapter;

    // construction and disposing

    public WebPermissionProviderTestHelper ()
    {
      _mocks = new MockRepository ();
      
      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
      _mockObjectSecurityStrategy = _mocks.CreateMock<IObjectSecurityStrategy> ();
      _mockFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();
      _mockWxeSecurityAdapter = _mocks.CreateMock<IWxeSecurityAdapter> ();

      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _mockUserProvider = _mocks.CreateMock<IUserProvider> ();
      SetupResult.For (_mockUserProvider.GetUser()).Return (_user);
    }

    // methods and properties

    public void ExpectHasAccess (Enum[] accessTypeEnums, bool returnValue)
    {
      AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (accessTypeEnums, AccessType.Get);
      Expect.Call (_mockObjectSecurityStrategy.HasAccess (_mockSecurityProvider, _user, accessTypes)).Return (returnValue);
    }

    public void ExpectHasStatelessAccessForSecurableObject (Enum[] accessTypeEnums, bool returnValue)
    {
        AccessType[] accessTypes = Array.ConvertAll<Enum, AccessType> (accessTypeEnums, AccessType.Get);
        Expect
            .Call (_mockFunctionalSecurityStrategy.HasAccess (typeof (SecurableObject), _mockSecurityProvider, _user, accessTypes))
            .Return (returnValue);
    }

    public void ExpectHasStatelessAccessForWxeFunction (Type functionType, bool returnValue)
    {
      Expect.Call (_mockWxeSecurityAdapter.HasStatelessAccess (functionType)).Return (returnValue);
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public ISecurityProvider SecurityProvider
    {
      get { return _mockSecurityProvider; }
    }

    public IUserProvider UserProvider
    {
      get { return _mockUserProvider; }
    }

    public IFunctionalSecurityStrategy FunctionalSecurityStrategy
    {
      get { return _mockFunctionalSecurityStrategy; }
    }

    public IWxeSecurityAdapter WxeSecurityAdapter
    {
      get { return _mockWxeSecurityAdapter; }
    }

    public SecurableObject CreateSecurableObject ()
    {
      return new SecurableObject (_mockObjectSecurityStrategy);
    }
  }
}
