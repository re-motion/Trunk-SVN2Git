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
using Remotion.Development.UnitTesting.Mixins;
using Remotion.Diagnostics;
using Remotion.Mixins.CodeGeneration;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.UnitTesting.Mixins
{
  [TestFixture]
  public class DebuggerWorkaroundTypeGeneratorDecoratorTest
  {
    private ITypeGenerator _innerTypeGeneratorMock;
    private DebuggerWorkaroundModuleManagerDecorator _moduleManager;
    private DebuggerWorkaroundTypeGeneratorDecorator _decorator;

    [SetUp]
    public void SetUp ()
    {
      _innerTypeGeneratorMock = MockRepository.GenerateStrictMock<ITypeGenerator>();
      
      var innerModuleManager = MockRepository.GenerateStub<IModuleManager>();
      var debuggerInterface = MockRepository.GenerateStub<IDebuggerInterface>();
      _moduleManager = new DebuggerWorkaroundModuleManagerDecorator (5, innerModuleManager, debuggerInterface);
      
      _decorator = new DebuggerWorkaroundTypeGeneratorDecorator (_innerTypeGeneratorMock, _moduleManager);
    }

    [Test]
    public void GetBuiltType ()
    {
      Assert.That (_moduleManager.GeneratedTypeCountForCurrentScope, Is.EqualTo (0));

      var fakeBuiltType = typeof (string);
      _innerTypeGeneratorMock
          .Expect (mock => mock.GetBuiltType())
          .Return (fakeBuiltType)
          .WhenCalled (mi => Assert.That (_moduleManager.GeneratedTypeCountForCurrentScope, Is.EqualTo (0)));
      _innerTypeGeneratorMock.Replay();

      var result = _decorator.GetBuiltType();

      _innerTypeGeneratorMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeBuiltType));
      Assert.That (_moduleManager.GeneratedTypeCountForCurrentScope, Is.EqualTo (1));
    }
  }
}