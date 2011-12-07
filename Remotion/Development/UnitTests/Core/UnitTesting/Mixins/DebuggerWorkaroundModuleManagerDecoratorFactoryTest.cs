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
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.Mixins;
using Remotion.Diagnostics;
using Remotion.Mixins.CodeGeneration;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Mixins
{
  [TestFixture]
  public class DebuggerWorkaroundModuleManagerDecoratorFactoryTest
  {
    private IDebuggerInterface _debuggerInterfaceStub;
    private IModuleManagerFactory _decoratedFactoryMock;

    private DebuggerWorkaroundModuleManagerDecoratorFactory _factory;

    [SetUp]
    public void SetUp ()
    {
      _debuggerInterfaceStub = MockRepository.GenerateStub<IDebuggerInterface> ();
      _decoratedFactoryMock = MockRepository.GenerateStub<IModuleManagerFactory> ();

      _factory = new DebuggerWorkaroundModuleManagerDecoratorFactory (10, _debuggerInterfaceStub, _decoratedFactoryMock);
    }

    [Test]
    public void Create ()
    {
      var moduleManagerStub = MockRepository.GenerateStub<IModuleManager>();
      _decoratedFactoryMock.Expect (mock => mock.Create()).Return (moduleManagerStub);
      _decoratedFactoryMock.Replay ();

      var result = _factory.Create();

      _decoratedFactoryMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<DebuggerWorkaroundModuleManagerDecorator> ());
      Assert.That (((DebuggerWorkaroundModuleManagerDecorator) result).MaximumTypesPerAssembly, Is.EqualTo (10));
      Assert.That (PrivateInvoke.GetNonPublicField (result, "_innerModuleManager"), Is.SameAs (moduleManagerStub));
    }
  }
}