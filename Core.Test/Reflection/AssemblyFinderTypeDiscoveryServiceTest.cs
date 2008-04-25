using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class AssemblyFinderTypeDiscoveryServiceTest
  {
    private MockRepository _mockRepository;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();
    }

    [Test]
    public void GetTypes_UsesAssemblyFinder ()
    {
      AssemblyFinder finderMock = _mockRepository.CreateMock<AssemblyFinder> (ApplicationAssemblyFinderFilter.Instance,
          new Assembly[] { typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly });

      AssemblyFinderTypeDiscoveryService service = new AssemblyFinderTypeDiscoveryService (finderMock);

      Expect.Call (finderMock.FindAssemblies()).Return (new Assembly[0]);

      _mockRepository.ReplayAll();
      service.GetTypes (typeof (object), true);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetTypes_ReturnsTypesFromFoundAssemblies ()
    {
      AssemblyFinder finderMock = _mockRepository.CreateMock<AssemblyFinder> (ApplicationAssemblyFinderFilter.Instance,
          new Assembly[] { typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly });

      AssemblyFinderTypeDiscoveryService service = new AssemblyFinderTypeDiscoveryService (finderMock);

      Assembly assembly1 = typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly;
      Assembly assembly2 = typeof (AssemblyFinder).Assembly;
      Expect.Call (finderMock.FindAssemblies ()).Return (new Assembly[] { assembly1, assembly2 });

      List<Type> allTypes = new List<Type>();
      allTypes.AddRange (assembly1.GetTypes ());
      allTypes.AddRange (assembly2.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), true);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithGlobalTypes ()
    {
      AssemblyFinder finderMock = _mockRepository.CreateMock<AssemblyFinder> (ApplicationAssemblyFinderFilter.Instance,
          new Assembly[] { typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly });

      AssemblyFinderTypeDiscoveryService service = new AssemblyFinderTypeDiscoveryService (finderMock);

      Assembly assembly1 = typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly;
      Assembly assembly2 = typeof (object).Assembly;
      Expect.Call (finderMock.FindAssemblies ()).Return (new Assembly[] { assembly1, assembly2 });

      List<Type> allTypes = new List<Type> ();
      allTypes.AddRange (assembly1.GetTypes ());
      allTypes.AddRange (assembly2.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), false);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithoutGlobalTypes ()
    {
      AssemblyFinder finderMock = _mockRepository.CreateMock<AssemblyFinder> (ApplicationAssemblyFinderFilter.Instance,
          new Assembly[] { typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly });

      AssemblyFinderTypeDiscoveryService service = new AssemblyFinderTypeDiscoveryService (finderMock);

      Assembly assembly1 = typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly;
      Assembly assembly2 = typeof (object).Assembly;
      Expect.Call (finderMock.FindAssemblies ()).Return (new Assembly[] { assembly1, assembly2 });

      List<Type> allTypes = new List<Type> ();
      allTypes.AddRange (assembly1.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), true);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithoutSpecificBase ()
    {
      AssemblyFinder finderMock = _mockRepository.CreateMock<AssemblyFinder> (ApplicationAssemblyFinderFilter.Instance,
          new Assembly[] { typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly });

      AssemblyFinderTypeDiscoveryService service = new AssemblyFinderTypeDiscoveryService (finderMock);

      Assembly assembly1 = typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly;
      Expect.Call (finderMock.FindAssemblies ()).Return (new Assembly[] { assembly1 });

      List<Type> allTypes = new List<Type> ();
      allTypes.AddRange (assembly1.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (null, true);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }

    public class Base { }
    public class Derived1 : Base { }
    public class Derived2 : Base { }

    [Test]
    public void GetTypes_WithSpecificBase ()
    {
      AssemblyFinder finderMock = _mockRepository.CreateMock<AssemblyFinder> (ApplicationAssemblyFinderFilter.Instance,
          new Assembly[] { typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly });

      AssemblyFinderTypeDiscoveryService service = new AssemblyFinderTypeDiscoveryService (finderMock);

      Assembly assembly1 = typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly;
      Expect.Call (finderMock.FindAssemblies ()).Return (new Assembly[] { assembly1 });

      List<Type> allTypes = new List<Type> ();
      allTypes.AddRange (assembly1.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (Base), true);
      Assert.That (types, Is.Not.EquivalentTo (allTypes));
      Assert.That (types, Is.EquivalentTo (new Type[] {typeof (Base), typeof (Derived1), typeof (Derived2)}));
      _mockRepository.VerifyAll ();
    }
  }
}
