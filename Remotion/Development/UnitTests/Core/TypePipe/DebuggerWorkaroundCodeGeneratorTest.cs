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
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.TypePipe;
using Remotion.Diagnostics;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Rhino.Mocks;

namespace Remotion.Development.UnitTests.Core.TypePipe
{
  [TestFixture]
  public class DebuggerWorkaroundCodeGeneratorTest
  {
    private IModuleBuilderFactory _moduleBuilderFactoryMock;
    private IDebuggerInterface _debuggerInterfaceMock;
    private int _maximumTypesPerAssembly;

    private DebuggerWorkaroundCodeGenerator _generator;

    [SetUp]
    public void SetUp ()
    {
      _moduleBuilderFactoryMock = MockRepository.GenerateStrictMock<IModuleBuilderFactory>();
      _debuggerInterfaceMock = MockRepository.GenerateStrictMock<IDebuggerInterface>();
      _maximumTypesPerAssembly = 2;

      _generator = new DebuggerWorkaroundCodeGenerator (_moduleBuilderFactoryMock, false, null, _debuggerInterfaceMock, _maximumTypesPerAssembly);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = "Method FlushCodeToDisk is not supported by DebuggerWorkaroundCodeGenerator.")]
    public void FlushCodeToDisk ()
    {
      _generator.FlushCodeToDisk (null);
    }

    [Test]
    public void DefineType ()
    {
      var moduleBuilderMock1 = MockRepository.GenerateStrictMock<IModuleBuilder>();
      var moduleBuilderMock2 = MockRepository.GenerateStrictMock<IModuleBuilder>();
      var moduleBuilderMock3 = MockRepository.GenerateStrictMock<IModuleBuilder>();
      _debuggerInterfaceMock.Expect (mock => mock.IsAttached).Return (true).Repeat.Times (5);
      _generator.SetAssemblyNamePattern ("assemblyName");
      Assert.That (_maximumTypesPerAssembly, Is.EqualTo (2));

      // Initialize for first type.
      _moduleBuilderFactoryMock.Expect (mock => mock.CreateModuleBuilder ("assemblyName", null, false, null)).Return (moduleBuilderMock1);
      ExpectAndDefineType (moduleBuilderMock1);
      ExpectAndDefineType (moduleBuilderMock1);

      // Start new assembly if threshold is exceeded.
      _moduleBuilderFactoryMock.Expect (mock => mock.CreateModuleBuilder ("assemblyName", null, false, null)).Return (moduleBuilderMock2);
      ExpectAndDefineType (moduleBuilderMock2);
      ExpectAndDefineType (moduleBuilderMock2);

      // Ensure that counter is reset.
      _moduleBuilderFactoryMock.Expect (mock => mock.CreateModuleBuilder ("assemblyName", null, false, null)).Return (moduleBuilderMock3);
      ExpectAndDefineType (moduleBuilderMock3);

      Assert.That (_generator.TypeCountForCurrentAssembly, Is.EqualTo (1));
      Assert.That (_generator.TotalTypeCount, Is.EqualTo (5));
      Assert.That (_generator.ResetCount, Is.EqualTo (2));

      _debuggerInterfaceMock.VerifyAllExpectations();
      _moduleBuilderFactoryMock.VerifyAllExpectations();
      moduleBuilderMock1.VerifyAllExpectations();
      moduleBuilderMock2.VerifyAllExpectations();
    }

    [Test]
    public void DefineType_WithoutDebugger ()
    {
      var moduleBuilderMock = MockRepository.GenerateStrictMock<IModuleBuilder> ();
      _debuggerInterfaceMock.Expect (mock => mock.IsAttached).Return (false).Repeat.Times (3);
      _generator.SetAssemblyNamePattern ("assemblyName");
      Assert.That (_maximumTypesPerAssembly, Is.EqualTo (2));

      // Initialize for first type.
      _moduleBuilderFactoryMock.Expect (mock => mock.CreateModuleBuilder ("assemblyName", null, false, null)).Return (moduleBuilderMock);
      ExpectAndDefineType (moduleBuilderMock);
      ExpectAndDefineType (moduleBuilderMock);
      ExpectAndDefineType (moduleBuilderMock); // No new assembly was started.

      Assert.That (_generator.TypeCountForCurrentAssembly, Is.EqualTo (3));
      Assert.That (_generator.TotalTypeCount, Is.EqualTo (3));
      Assert.That (_generator.ResetCount, Is.EqualTo (0));

      _debuggerInterfaceMock.VerifyAllExpectations();
      _moduleBuilderFactoryMock.VerifyAllExpectations();
      moduleBuilderMock.VerifyAllExpectations();
    }

    private void ExpectAndDefineType (IModuleBuilder moduleBuilderMock)
    {
      var name = "MyType";
      var attributes = (TypeAttributes) 7;
      var emittableOperandProviderMock = MockRepository.GenerateStrictMock<IEmittableOperandProvider>();
      var typeBuilderMock = MockRepository.GenerateStrictMock<ITypeBuilder>();
      moduleBuilderMock.Expect (mock => mock.DefineType (name, attributes)).Return (typeBuilderMock);

      var result = _generator.DefineType (name, attributes, emittableOperandProviderMock);

      Assert.That (result, Is.TypeOf<TypeBuilderDecorator>());
      var decorator = (TypeBuilderDecorator) result;
      Assert.That (decorator.DecoratedTypeBuilder, Is.SameAs (typeBuilderMock));
    }
  }
}