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
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.Mixins;
using Remotion.Diagnostics;
using Remotion.Mixins.CodeGeneration;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.Mixins
{
  [TestFixture]
  public class DebuggerWorkaroundMixinTypeGeneratorDecoratorTest
  {
    private IMixinTypeGenerator _innerMixinTypeGeneratorMock;
    private DebuggerWorkaroundModuleManagerDecorator _moduleManager;
    private DebuggerWorkaroundMixinTypeGeneratorDecorator _decorator;

    [SetUp]
    public void SetUp ()
    {
      _innerMixinTypeGeneratorMock = MockRepository.GenerateStrictMock<IMixinTypeGenerator>();
      
      var innerModuleManager = MockRepository.GenerateStub<IModuleManager>();
      var debuggerInterface = MockRepository.GenerateStub<IDebuggerInterface>();
      _moduleManager = new DebuggerWorkaroundModuleManagerDecorator (5, debuggerInterface, innerModuleManager);
      
      _decorator = new DebuggerWorkaroundMixinTypeGeneratorDecorator (_innerMixinTypeGeneratorMock, _moduleManager);
    }

    [Test]
    public void GetBuiltType ()
    {
      Assert.That (_moduleManager.GeneratedTypeCountForCurrentScope, Is.EqualTo (0));

      var fakeBuiltType =
          new ConcreteMixinType (
              new ConcreteMixinTypeIdentifier (typeof (string), new HashSet<MethodInfo>(), new HashSet<MethodInfo>()),
              typeof (string),
              typeof (string),
              new Dictionary<MethodInfo, MethodInfo>(),
              new Dictionary<MethodInfo, MethodInfo>());
      _innerMixinTypeGeneratorMock
          .Expect (mock => mock.GetBuiltType())
          .Return (fakeBuiltType)
          .WhenCalled (mi => Assert.That (_moduleManager.GeneratedTypeCountForCurrentScope, Is.EqualTo (0)));
      _innerMixinTypeGeneratorMock.Replay();

      var result = _decorator.GetBuiltType();

      _innerMixinTypeGeneratorMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeBuiltType));
      Assert.That (_moduleManager.GeneratedTypeCountForCurrentScope, Is.EqualTo (1));
    }
  }
}