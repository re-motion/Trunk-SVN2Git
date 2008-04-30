using System;
using System.Security.Principal;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.DomainObjects.UnitTests.Security.TestDomain;
using Remotion.Development.UnitTesting;
using Remotion.Security;
using Remotion.Security.Configuration;
using Remotion.Security.Metadata;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.DomainObjects.UnitTests.Security.SecurityClientTransactionExtensionTests
{
  public delegate bool HasAccessDelegate (ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);
  public delegate bool HasStatelessAccessDelegate (Type type, ISecurityProvider securityProvider, IPrincipal user, params AccessType[] requiredAccessTypes);

  public class SecurityClientTransactionExtensionTestHelper
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IPrincipal _user;
    private ISecurityProvider _mockSecurityProvider;
    private IUserProvider _stubUserProvider;
    private IFunctionalSecurityStrategy _mockFunctionalSecurityStrategy;
    private IPermissionProvider _mockPermissionReflector;
    private ClientTransaction _transaction;

    // construction and disposing

    public SecurityClientTransactionExtensionTestHelper ()
    {
      _mocks = new MockRepository ();
      _mockSecurityProvider = _mocks.CreateMock<ISecurityProvider> ();
      _user = new GenericPrincipal (new GenericIdentity ("owner"), new string[0]);
      _stubUserProvider = _mocks.CreateMock<IUserProvider> ();
      SetupResult.For (_stubUserProvider.GetUser ()).Return (_user);
      _mockFunctionalSecurityStrategy = _mocks.CreateMock<IFunctionalSecurityStrategy> ();
      _mockPermissionReflector = _mocks.CreateMock<IPermissionProvider> ();
      _transaction = ClientTransaction.NewRootTransaction();

      SetupResult.For (_mockSecurityProvider.IsNull).Return (false);
    }

    // methods and properties

    public ClientTransaction Transaction
    {
      get { return _transaction; }
    }

    public SecurableObject CreateSecurableObject ()
    {
      return SecurableObject.NewObject (_transaction, CreateObjectSecurityStrategy ());
    }

    public NonSecurableObject CreateNonSecurableObject ()
    {
      return NonSecurableObject.NewObject (_transaction);
    }

    public IObjectSecurityStrategy CreateObjectSecurityStrategy ()
    {
      return _mocks.CreateMock<IObjectSecurityStrategy> ();
    }

    public void SetupSecurityConfiguration ()
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (SecurityConfiguration), "SetCurrent", new SecurityConfiguration ());
      SecurityConfiguration.Current.SecurityProvider = _mockSecurityProvider;
      SecurityConfiguration.Current.UserProvider = _stubUserProvider;
      SecurityConfiguration.Current.FunctionalSecurityStrategy = _mockFunctionalSecurityStrategy;
      SecurityConfiguration.Current.PermissionProvider = _mockPermissionReflector;
    }

    public void TearDownSecurityConfiguration ()
    {
      PrivateInvoke.InvokeNonPublicStaticMethod (typeof (SecurityConfiguration), "SetCurrent", new SecurityConfiguration ());
    }

    public void ReplayAll ()
    {
      _mocks.ReplayAll ();
    }

    public void VerifyAll ()
    {
      _mocks.VerifyAll ();
    }

    public IDisposable Ordered ()
    {
      return _mocks.Ordered ();
    }

    public void AddExtension (IClientTransactionExtension extension)
    {
      ArgumentUtility.CheckNotNullAndType<SecurityClientTransactionExtension> ("extension", extension);

      _transaction.Extensions.Add (typeof (SecurityClientTransactionExtension).FullName, extension);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, HasAccessDelegate doDelegate)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      Expect.Call (objectSecurityStrategy.HasAccess (_mockSecurityProvider, _user, AccessType.Get (accessTypeEnum))).Do (doDelegate);
    }

    public void ExpectObjectSecurityStrategyHasAccess (SecurableObject securableObject, Enum accessTypeEnum, bool returnValue)
    {
      IObjectSecurityStrategy objectSecurityStrategy = securableObject.GetSecurityStrategy ();
      Expect.Call (objectSecurityStrategy.HasAccess (_mockSecurityProvider, _user, AccessType.Get (accessTypeEnum))).Return (returnValue);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Type securableObjectType, Enum accessTypeEnum, HasStatelessAccessDelegate doDelegate)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (securableObjectType, _mockSecurityProvider, _user, AccessType.Get (accessTypeEnum)))
          .Do (doDelegate);
    }

    public void ExpectFunctionalSecurityStrategyHasAccess (Type securableObjectType, Enum accessTypeEnum, bool returnValue)
    {
      Expect
          .Call (_mockFunctionalSecurityStrategy.HasAccess (securableObjectType, _mockSecurityProvider, _user, AccessType.Get (accessTypeEnum)))
          .Return (returnValue);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyWritePermissions (string propertyName, params Enum[] returnedAccessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyWritePermissions (typeof (SecurableObject), propertyName)).Return (returnedAccessTypes);
    }

    public void ExpectPermissionReflectorGetRequiredPropertyReadPermissions (string propertyName, params Enum[] returnedAccessTypes)
    {
      Expect.Call (_mockPermissionReflector.GetRequiredPropertyReadPermissions (typeof (SecurableObject), propertyName)).Return (returnedAccessTypes);
    }

    public void ExpectSecurityProviderGetAccess (SecurityContext context, params Enum[] returnedAccessTypes)
    {
      Expect.Call (_mockSecurityProvider.GetAccess (context, _user)).Return (Array.ConvertAll<Enum, AccessType> (returnedAccessTypes, AccessType.Get));
    }
  }
}