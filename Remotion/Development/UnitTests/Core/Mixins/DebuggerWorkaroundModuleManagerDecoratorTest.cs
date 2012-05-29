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
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.Mixins;
using Remotion.Diagnostics;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.Mixins
{
  [TestFixture]
  public class DebuggerWorkaroundModuleManagerDecoratorTest
  {
    private IModuleManager _innerModuleManagerMock;
    private IDebuggerInterface _debuggerInterfaceStub;
    private DebuggerWorkaroundModuleManagerDecorator _decorator;

    private ITypeGenerator _innerTypeGeneratorStub;
    private IMixinTypeGenerator _innerMixinTypeGeneratorStub;

    private TargetClassDefinition _configuration;
    private IConcreteMixedTypeNameProvider _nameProvider;
    private IConcreteMixinTypeNameProvider _mixinNameProvider;
    private IConcreteMixinTypeProvider _concreteMixinTypeProvider;
    private ConcreteMixinTypeIdentifier _concreteMixinTypeIdentifier;

    [SetUp]
    public void SetUp ()
    {
      _innerModuleManagerMock = MockRepository.GenerateStrictMock<IModuleManager>();
      _debuggerInterfaceStub = MockRepository.GenerateStub<IDebuggerInterface>();
      _decorator = new DebuggerWorkaroundModuleManagerDecorator (3, _debuggerInterfaceStub, _innerModuleManagerMock);

      _innerTypeGeneratorStub = MockRepository.GenerateStub<ITypeGenerator> ();
      _innerMixinTypeGeneratorStub = MockRepository.GenerateStub<IMixinTypeGenerator> ();

      _configuration = new TargetClassDefinition (new ClassContext (typeof (object), Enumerable.Empty<MixinContext>(), Enumerable.Empty<Type>()));
      _nameProvider = MockRepository.GenerateStub<IConcreteMixedTypeNameProvider> ();
      _mixinNameProvider = MockRepository.GenerateStub<IConcreteMixinTypeNameProvider> ();
      _concreteMixinTypeProvider = MockRepository.GenerateStub<IConcreteMixinTypeProvider> ();
      _concreteMixinTypeIdentifier = new ConcreteMixinTypeIdentifier (typeof (object), new HashSet<MethodInfo>(), new HashSet<MethodInfo>());
    }

    [Test]
    public void TypeGenerated ()
    {
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (0));

      _decorator.TypeGenerated (TimeSpan.Zero);

      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (1));
    }

    [Test]
    public void Reset ()
    {
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (1));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      _innerModuleManagerMock.Expect (mock => mock.Reset ());
      _innerModuleManagerMock.Replay ();

      _decorator.Reset();

      _innerModuleManagerMock.VerifyAllExpectations ();
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (0));
      Assert.That (_decorator.ResetCount, Is.EqualTo (1));
    }

    [Test]
    public void CreateTypeGenerator_ReturnsDecorator ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false);

      _innerModuleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider))
          .Return (_innerTypeGeneratorStub);
      _innerModuleManagerMock.Replay();

      var result = _decorator.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider);

      _innerModuleManagerMock.VerifyAllExpectations();
      Assert.That (result, Is.TypeOf<DebuggerWorkaroundTypeGeneratorDecorator> ());
      Assert.That (PrivateInvoke.GetNonPublicField(result, "_innerTypeGenerator"), Is.SameAs (_innerTypeGeneratorStub));
    }

    [Test]
    public void CreateTypeGenerator_ThresholdReached_WithDebuggerAttached ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (true);

      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (3));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      using (_innerModuleManagerMock.GetMockRepository ().Ordered ())
      {
        _innerModuleManagerMock.Expect (mock => mock.Reset());
        _innerModuleManagerMock
            .Expect (mock => mock.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider))
            .Return (_innerTypeGeneratorStub)
            .WhenCalled (mi => Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (0)));
      }
      _innerModuleManagerMock.Replay ();

      _decorator.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider);

      _innerModuleManagerMock.VerifyAllExpectations ();
      Assert.That (_decorator.ResetCount, Is.EqualTo (1));
    }

    [Test]
    public void CreateTypeGenerator_ThresholdNotReached_WithDebuggerAttached ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (true);

      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (2));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      _innerModuleManagerMock
            .Expect (mock => mock.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider))
            .Return (_innerTypeGeneratorStub);
      _innerModuleManagerMock.Replay();

      _decorator.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider);

      _innerModuleManagerMock.AssertWasNotCalled (mock => mock.Reset());
      _innerModuleManagerMock.VerifyAllExpectations ();

      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (2));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));
    }

    [Test]
    public void CreateTypeGenerator_ThresholdReached_WithDebuggerNotAttached ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false);

      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (3));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      _innerModuleManagerMock
            .Expect (mock => mock.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider))
            .Return (_innerTypeGeneratorStub)
            .WhenCalled (mi =>_decorator.TypeGenerated (TimeSpan.Zero));
      _innerModuleManagerMock.Replay ();

      _decorator.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider);

      _innerModuleManagerMock.AssertWasNotCalled (mock => mock.Reset ());
      _innerModuleManagerMock.VerifyAllExpectations ();

      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (4));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));
    }

    [Test]
    public void CreateTypeGenerator_ThresholdReached_DebuggerAttachedLater ()
    {
      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (3));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      _innerModuleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider))
          .Return (_innerTypeGeneratorStub)
          .WhenCalled (mi => _decorator.TypeGenerated (TimeSpan.Zero));
      _innerModuleManagerMock.Replay ();

      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false).Repeat.Once();
      _decorator.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider);

      _innerModuleManagerMock.AssertWasNotCalled (mock => mock.Reset ());
      _innerModuleManagerMock.VerifyAllExpectations ();
      _innerModuleManagerMock.BackToRecord();

      using (_innerModuleManagerMock.GetMockRepository().Ordered ())
      {
        _innerModuleManagerMock
            .Expect (mock => mock.Reset ())
            .WhenCalled (mi => Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (4)));
        _innerModuleManagerMock
            .Expect (mock => mock.CreateTypeGenerator (_configuration, _nameProvider, _concreteMixinTypeProvider))
            .Return (_innerTypeGeneratorStub)
            .WhenCalled (mi => Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (0)));
      }
      _innerModuleManagerMock.Replay();
    }

    [Test]
    public void CreateMixinTypeGenerator_ReturnsDecorator ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false);

      _innerModuleManagerMock
          .Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider))
          .Return (_innerMixinTypeGeneratorStub);
      _innerModuleManagerMock.Replay ();

      var result = _decorator.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider);

      _innerModuleManagerMock.VerifyAllExpectations ();
      Assert.That (result, Is.TypeOf<DebuggerWorkaroundMixinTypeGeneratorDecorator> ());
      Assert.That (PrivateInvoke.GetNonPublicField (result, "_innerMixinTypeGenerator"), Is.SameAs (_innerMixinTypeGeneratorStub));
    }

    [Test]
    public void CreateMixinTypeGenerator_ThresholdReached_WithDebuggerAttached ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (true);

      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (3));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      using (_innerModuleManagerMock.GetMockRepository ().Ordered ())
      {
        _innerModuleManagerMock.Expect (mock => mock.Reset ());
        _innerModuleManagerMock
            .Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider))
            .Return (_innerMixinTypeGeneratorStub)
            .WhenCalled (mi => Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (0)));
      }
      _innerModuleManagerMock.Replay ();

      _decorator.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider);

      _innerModuleManagerMock.VerifyAllExpectations ();
      Assert.That (_decorator.ResetCount, Is.EqualTo (1));
    }

    [Test]
    public void CreateMixinTypeGenerator_ThresholdNotReached_WithDebuggerAttached ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (true);

      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (2));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      _innerModuleManagerMock
            .Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider))
            .Return (_innerMixinTypeGeneratorStub);
      _innerModuleManagerMock.Replay ();

      _decorator.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider);

      _innerModuleManagerMock.AssertWasNotCalled (mock => mock.Reset ());
      _innerModuleManagerMock.VerifyAllExpectations ();

      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (2));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));
    }

    [Test]
    public void CreateMixinTypeGenerator_ThresholdReached_WithDebuggerNotAttached ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false);

      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (3));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      _innerModuleManagerMock
            .Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider))
            .Return (_innerMixinTypeGeneratorStub)
            .WhenCalled (mi => _decorator.TypeGenerated (TimeSpan.Zero));
      _innerModuleManagerMock.Replay ();

      _decorator.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider);

      _innerModuleManagerMock.AssertWasNotCalled (mock => mock.Reset ());
      _innerModuleManagerMock.VerifyAllExpectations ();

      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (4));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));
    }

    [Test]
    public void CreateMixinTypeGenerator_ThresholdReached_DebuggerAttachedLater ()
    {
      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      _decorator.TypeGenerated (TimeSpan.Zero);
      Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (3));
      Assert.That (_decorator.ResetCount, Is.EqualTo (0));

      _innerModuleManagerMock
          .Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider))
          .Return (_innerMixinTypeGeneratorStub)
          .WhenCalled (mi => _decorator.TypeGenerated (TimeSpan.Zero));
      _innerModuleManagerMock.Replay ();

      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false).Repeat.Once ();
      _decorator.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider);

      _innerModuleManagerMock.AssertWasNotCalled (mock => mock.Reset ());
      _innerModuleManagerMock.VerifyAllExpectations ();
      _innerModuleManagerMock.BackToRecord ();

      using (_innerModuleManagerMock.GetMockRepository ().Ordered ())
      {
        _innerModuleManagerMock
            .Expect (mock => mock.Reset ())
            .WhenCalled (mi => Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (4)));
        _innerModuleManagerMock
            .Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinNameProvider))
            .Return (_innerMixinTypeGeneratorStub)
            .WhenCalled (mi => Assert.That (_decorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (0)));
      }
      _innerModuleManagerMock.Replay ();
    }

  }
}