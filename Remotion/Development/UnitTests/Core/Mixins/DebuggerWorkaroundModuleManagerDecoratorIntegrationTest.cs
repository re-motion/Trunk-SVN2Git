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
using NUnit.Framework;
using Remotion.Context;
using Remotion.Development.UnitTesting;
using Remotion.Development.Mixins;
using Remotion.Development.UnitTests.Core.Mixins.TestDomain;
using Remotion.Diagnostics;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.CodeGeneration.DynamicProxy;
using Remotion.ServiceLocation;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.Mixins
{
  [TestFixture]
  public class DebuggerWorkaroundModuleManagerDecoratorIntegrationTest
  {
    private IDebuggerInterface _debuggerInterfaceStub;
    private ServiceLocatorScope _serviceLocatorScope;

    [SetUp]
    public void SetUp ()
    {
      // Cause type generation for SafeContext so that this class (mixed in this project) doesn't influence our tests below
      Dev.Null = SafeContext.Instance;

      _debuggerInterfaceStub = MockRepository.GenerateStub<IDebuggerInterface>();

      var temporaryServiceLocator = new DefaultServiceLocator();
      temporaryServiceLocator.Register (
          typeof (IModuleManager),
          () => new DebuggerWorkaroundModuleManagerDecorator (3, _debuggerInterfaceStub, new ModuleManager()));
      _serviceLocatorScope = new ServiceLocatorScope (temporaryServiceLocator);
      ConcreteTypeBuilder.SetCurrent (null);
    }

    [TearDown]
    public void TearDown ()
    {
      if (_serviceLocatorScope != null)
        _serviceLocatorScope.Dispose();
      ConcreteTypeBuilder.SetCurrent (null);
    }

    [Test]
    public void ScopeInitialization ()
    {
      var moduleInfo = ((ConcreteTypeBuilder) ConcreteTypeBuilder.Current).ModuleInfo;
      Assert.That (moduleInfo, Is.TypeOf<LockingCodeGenerationModuleInfoDecorator> ());
      Assert.That (((LockingCodeGenerationModuleInfoDecorator) moduleInfo).InnerCodeGenerationModuleInfo, Is.TypeOf<DebuggerWorkaroundModuleManagerDecorator>());
    }

    [Test]
    public void GenerateTypes_WithoutDebugger ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false);

      var type1 = TypeFactory.GetConcreteType (typeof (MixedClass1));
      var type2 = TypeFactory.GetConcreteType (typeof (MixedClass2));
      var type3 = TypeFactory.GetConcreteType (typeof (MixedClass3));
      var type4 = TypeFactory.GetConcreteType (typeof (MixedClass4));
      var type1Again = TypeFactory.GetConcreteType (typeof (MixedClass1));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (4));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (0));

      Assert.That (type1.Assembly, Is.SameAs (type2.Assembly));
      Assert.That (type1.Assembly, Is.SameAs (type3.Assembly));
      Assert.That (type1.Assembly, Is.SameAs (type4.Assembly));
      Assert.That (type1, Is.SameAs (type1Again));
    }

    [Test]
    public void GenerateTypes_WithoutDebugger_WithConcreteMixinType ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false);

      var type1 = TypeFactory.GetConcreteType (typeof (MixedClassWithMixinNeedingConcreteType));
      var type2 = TypeFactory.GetConcreteType (typeof (MixedClass2));
      var type3 = TypeFactory.GetConcreteType (typeof (MixedClass3));
      var type4 = TypeFactory.GetConcreteType (typeof (MixedClass4));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (5));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (0));
      
      Assert.That (type1.Assembly, Is.SameAs (type2.Assembly));
      Assert.That (type1.Assembly, Is.SameAs (type3.Assembly));
      Assert.That (type1.Assembly, Is.SameAs (type4.Assembly));
    }

    [Test]
    public void GenerateTypes_WithDebugger ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (true);

      var type1 = TypeFactory.GetConcreteType (typeof (MixedClass1));
      var type2 = TypeFactory.GetConcreteType (typeof (MixedClass2));
      var type3 = TypeFactory.GetConcreteType (typeof (MixedClass3));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (3));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (0));

      var type4 = TypeFactory.GetConcreteType (typeof (MixedClass4));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (1));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (1));

      var type1Again = TypeFactory.GetConcreteType (typeof (MixedClass1));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (1));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (1));

      Assert.That (type1.Assembly, Is.SameAs (type2.Assembly));
      Assert.That (type1.Assembly, Is.SameAs (type3.Assembly));
      Assert.That (type1.Assembly, Is.Not.SameAs (type4.Assembly));
      Assert.That (type1, Is.SameAs (type1Again));
    }

    [Test]
    public void GenerateTypes_WithDebugger_WithConcreteMixinType ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (true);

      var type1 = TypeFactory.GetConcreteType (typeof (MixedClassWithMixinNeedingConcreteType));
      var type2 = TypeFactory.GetConcreteType (typeof (MixedClass2));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (3));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (0));
      
      var type3 = TypeFactory.GetConcreteType (typeof (MixedClass3));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (1));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (1));

      var type4 = TypeFactory.GetConcreteType (typeof (MixedClass4));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (2));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (1));

      Assert.That (type1.Assembly, Is.SameAs (type2.Assembly));
      Assert.That (type1.Assembly, Is.Not.SameAs (type3.Assembly));
      Assert.That (type3.Assembly, Is.SameAs (type4.Assembly));
    }

    [Test]
    public void GenerateTypes_WithDebugger_AttachedLater ()
    {
      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (false).Repeat.Times (4);

      var type1 = TypeFactory.GetConcreteType (typeof (MixedClass1));
      var type2 = TypeFactory.GetConcreteType (typeof (MixedClass2));
      var type3 = TypeFactory.GetConcreteType (typeof (MixedClass3));
      var type4 = TypeFactory.GetConcreteType (typeof (MixedClass4));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (4));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (0));

      _debuggerInterfaceStub.Stub (stub => stub.IsAttached).Return (true);

      var type5 = TypeFactory.GetConcreteType (typeof (MixedClass5));

      Assert.That (CurrentModuleManagerDecorator.GeneratedTypeCountForCurrentScope, Is.EqualTo (1));
      Assert.That (CurrentModuleManagerDecorator.ResetCount, Is.EqualTo (1));
      
      Assert.That (type1.Assembly, Is.SameAs (type2.Assembly));
      Assert.That (type1.Assembly, Is.SameAs (type3.Assembly));
      Assert.That (type1.Assembly, Is.SameAs (type4.Assembly));
      Assert.That (type1.Assembly, Is.Not.SameAs (type5.Assembly));
    }

    private DebuggerWorkaroundModuleManagerDecorator CurrentModuleManagerDecorator
    {
      get
      {
        var concreteTypeBuilder = (ConcreteTypeBuilder) ConcreteTypeBuilder.Current;
        return (DebuggerWorkaroundModuleManagerDecorator) ((LockingCodeGenerationModuleInfoDecorator) concreteTypeBuilder.ModuleInfo).InnerCodeGenerationModuleInfo;
      }
    }
  }
}