using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Remotion.Security.Metadata;
using Remotion.Security.UnitTests.TestDomain;

namespace Remotion.Security.UnitTests.Core.Metadata
{

  [TestFixture]
  public class AssemblyReflectorTest
  {
    // types

    // static members

    // member fields

    private MockRepository _mocks;
    private IClassReflector _classReflectorMock;
    private IAbstractRoleReflector _abstractRoleReflectorMock;
    private IAccessTypeReflector _accessTypeReflectorMock;
    private AssemblyReflector _assemblyReflector;
    private MetadataCache _cache;

    // construction and disposing

    public AssemblyReflectorTest ()
    {
    }

    // methods and properties

    [SetUp]
    public void SetUp ()
    {
      _mocks = new MockRepository ();
      _accessTypeReflectorMock = _mocks.CreateMock<IAccessTypeReflector> ();
      _classReflectorMock = _mocks.CreateMock<IClassReflector> ();
      _abstractRoleReflectorMock = _mocks.CreateMock<IAbstractRoleReflector> ();
      _assemblyReflector = new AssemblyReflector (_accessTypeReflectorMock, _classReflectorMock, _abstractRoleReflectorMock);
      _cache = new MetadataCache ();
    }

    [Test]
    public void Initialize ()
    {
      Assert.AreSame (_classReflectorMock, _assemblyReflector.ClassReflector);
      Assert.AreSame (_accessTypeReflectorMock, _assemblyReflector.AccessTypeReflector);
      Assert.AreSame (_abstractRoleReflectorMock, _assemblyReflector.AbstractRoleReflector);
    }

    [Test]
    public void GetMetadata ()
    {
      Assembly securityAssembly = typeof (IAccessTypeReflector).Assembly;
      Assembly assembly = typeof (File).Assembly;

      Expect
          .Call (_accessTypeReflectorMock.GetAccessTypesFromAssembly(securityAssembly, _cache))
          .Return (new List<EnumValueInfo> (new EnumValueInfo[] {AccessTypes.Read, AccessTypes.Write}));
      Expect
        .Call (_accessTypeReflectorMock.GetAccessTypesFromAssembly (assembly, _cache))
        .Return (new List<EnumValueInfo> (new EnumValueInfo[] { AccessTypes.Journalize, AccessTypes.Archive }));
      Expect.Call (_abstractRoleReflectorMock.GetAbstractRoles (securityAssembly, _cache)).Return (new List<EnumValueInfo> ());
      Expect
          .Call (_abstractRoleReflectorMock.GetAbstractRoles (assembly, _cache))
          .Return (new List<EnumValueInfo> (new EnumValueInfo[] { AbstractRoles.Clerk, AbstractRoles.Secretary, AbstractRoles.Administrator }));
      Expect.Call (_classReflectorMock.GetMetadata (typeof (File), _cache)).Return (new SecurableClassInfo());
      Expect.Call (_classReflectorMock.GetMetadata (typeof (PaperFile), _cache)).Return (new SecurableClassInfo ());
      _mocks.ReplayAll ();

      _assemblyReflector.GetMetadata (assembly, _cache);

      _mocks.VerifyAll ();
    }
  }
}