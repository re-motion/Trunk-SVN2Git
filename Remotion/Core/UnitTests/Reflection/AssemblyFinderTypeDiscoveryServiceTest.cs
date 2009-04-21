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
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Reflection;
using Rhino.Mocks;
using System.Runtime.InteropServices;

namespace Remotion.UnitTests.Reflection
{
  [TestFixture]
  public class AssemblyFinderTypeDiscoveryServiceTest
  {
    private MockRepository _mockRepository;
    private AssemblyFinder _finderMock;

    private readonly Assembly _testAssembly = typeof (AssemblyFinderTypeDiscoveryServiceTest).Assembly;
    private readonly Assembly _coreAssembly = typeof (AssemblyFinder).Assembly;
    private readonly Assembly _mscorlibAssembly = typeof (object).Assembly;

    [SetUp]
    public void SetUp ()
    {
      _mockRepository = new MockRepository ();

      _finderMock = _mockRepository.StrictMock<AssemblyFinder> (ApplicationAssemblyFinderFilter.Instance, 
          new[] { _testAssembly });
    }

    [Test]
    public void GetTypes_UsesAssemblyFinder ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindMockableAssemblies ()).Return (new Assembly[0]);

      _mockRepository.ReplayAll();
      service.GetTypes (typeof (object), true);
      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetTypes_ReturnsTypesFromFoundAssemblies ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindMockableAssemblies ()).Return (new[] { _testAssembly, _coreAssembly });

      var allTypes = new List<Type>();
      allTypes.AddRange (_testAssembly.GetTypes ());
      allTypes.AddRange (_coreAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), true);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithGlobalTypes ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindMockableAssemblies ()).Return (new[] { _testAssembly, _mscorlibAssembly });

      var allTypes = new List<Type> ();
      allTypes.AddRange (_testAssembly.GetTypes ());
      allTypes.AddRange (_mscorlibAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), false);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithoutGlobalTypes ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindMockableAssemblies ()).Return (new[] { _testAssembly, _mscorlibAssembly });

      var allTypes = new List<Type> ();
      allTypes.AddRange (_testAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (object), true);
      Assert.That (types, Is.EquivalentTo (allTypes));
      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetTypes_WithoutSpecificBase ()
    {
      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);

      _finderMock.Expect (mock => mock.FindMockableAssemblies ()).Return (new[] { _testAssembly });

      var allTypes = new List<Type> ();
      allTypes.AddRange (_testAssembly.GetTypes ());

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
      var finderMock = _mockRepository.StrictMock<AssemblyFinder> (ApplicationAssemblyFinderFilter.Instance,
          new[] { _testAssembly });

      var service = new AssemblyFinderTypeDiscoveryService (finderMock);

      finderMock.Expect (mock => mock.FindMockableAssemblies ()).Return (new[] { _testAssembly });

      var allTypes = new List<Type> ();
      allTypes.AddRange (_testAssembly.GetTypes ());

      _mockRepository.ReplayAll ();
      ICollection types = service.GetTypes (typeof (Base), true);
      Assert.That (types, Is.Not.EquivalentTo (allTypes));
      Assert.That (types, Is.EquivalentTo (new[] {typeof (Base), typeof (Derived1), typeof (Derived2)}));
      _mockRepository.VerifyAll ();
    }

    [Test]
    [ExpectedException (typeof (TypeLoadException), ExpectedMessage = "The types from assembly 'abc' could not be loaded.\r\nTest 1\r\nTest 2")]
    public void GetTypes_AssemblyThrowsReflectionTypeLoadException ()
    {
      var ex1 = new Exception ("Test 1");
      var ex2 = new Exception ("Test 2");
      var assemblyMock = _mockRepository.DynamicMock<_Assembly>();
      assemblyMock.Stub (mock => mock.GetName()).Return (new AssemblyName ("abc"));
      assemblyMock.Expect (mock => mock.GetTypes ()).Throw (new ReflectionTypeLoadException (new[] { typeof (object), null, null }, new[] { ex1, ex2 }));
      assemblyMock.Replay ();

      _finderMock.Expect (mock => mock.FindMockableAssemblies()).Return (new[] { assemblyMock });
      _finderMock.Replay ();

      var service = new AssemblyFinderTypeDiscoveryService (_finderMock);
      service.GetTypes (typeof (object), false);
    }
  }
}
