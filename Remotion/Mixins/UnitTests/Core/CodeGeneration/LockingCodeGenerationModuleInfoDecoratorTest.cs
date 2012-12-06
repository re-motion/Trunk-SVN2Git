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
using Remotion.Development.RhinoMocks.UnitTesting.Threading;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Mixins.CodeGeneration;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class LockingCodeGenerationModuleInfoDecoratorTest
  {
    private ICodeGenerationModuleInfo _innerModuleManagerMock;
    private object _lockObject;

    private LockingCodeGenerationModuleInfoDecorator _decorator;
    private LockingDecoratorTestHelper<ICodeGenerationModuleInfo> _lockingDecoratorTestHelper;

    [SetUp]
    public void SetUp ()
    {
      _innerModuleManagerMock = MockRepository.GenerateStrictMock<ICodeGenerationModuleInfo> ();
      _lockObject = new object();

      _decorator = new LockingCodeGenerationModuleInfoDecorator (_innerModuleManagerMock, _lockObject);
      _lockingDecoratorTestHelper = new LockingDecoratorTestHelper<ICodeGenerationModuleInfo> (_decorator, _lockObject, _innerModuleManagerMock);
    }

    [Test]
    public void LockingMembers ()
    {
      var boolean = BooleanObjectMother.GetRandomBoolean();

      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.SignedAssemblyName, "x");
      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.SignedAssemblyName = "x");
      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.UnsignedAssemblyName, "x");
      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.UnsignedAssemblyName = "x");

      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.SignedModulePath, "x");
      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.SignedModulePath = "x");
      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.UnsignedModulePath, "x");
      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.UnsignedModulePath = "x");

      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.HasAssemblies, boolean);
      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.HasSignedAssembly, boolean);
      _lockingDecoratorTestHelper.ExpectSynchronizedDelegation (m => m.HasUnsignedAssembly, boolean);
    }
  }
}