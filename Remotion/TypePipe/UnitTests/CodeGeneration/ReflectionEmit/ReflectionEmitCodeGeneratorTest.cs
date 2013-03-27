// Copyright (c) rubicon IT GmbH, www.rubicon.eu
//
// See the NOTICE file distributed with this work for additional information
// regarding copyright ownership.  rubicon licenses this file to you under 
// the Apache License, Version 2.0 (the "License"); you may not use this 
// file except in compliance with the License.  You may obtain a copy of the 
// License at
//
//   http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, WITHOUT 
// WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.  See the 
// License for the specific language governing permissions and limitations
// under the License.
// 
using System;
using System.Reflection;
using NUnit.Framework;
using Remotion.Development.UnitTesting;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit;
using Remotion.TypePipe.CodeGeneration.ReflectionEmit.Abstractions;
using Remotion.TypePipe.Configuration;
using Rhino.Mocks;

namespace Remotion.TypePipe.UnitTests.CodeGeneration.ReflectionEmit
{
  [TestFixture]
  public class ReflectionEmitCodeGeneratorTest
  {
    private const string c_assemblyNamePattern = @"TypePipe_GeneratedAssembly_\d+";

    private IModuleBuilderFactory _moduleBuilderFactoryMock;
    private ITypePipeConfigurationProvider _configurationProviderMock;

    private ReflectionEmitCodeGenerator _generator;

    private IModuleBuilder _moduleBuilderMock;
    private IEmittableOperandProvider _emittableOperandProviderMock;

    [SetUp]
    public void SetUp ()
    {
      _moduleBuilderFactoryMock = MockRepository.GenerateStrictMock<IModuleBuilderFactory>();
      _configurationProviderMock = MockRepository.GenerateStrictMock<ITypePipeConfigurationProvider>();

      _generator = new ReflectionEmitCodeGenerator (_moduleBuilderFactoryMock, _configurationProviderMock);

      _moduleBuilderMock = MockRepository.GenerateStrictMock<IModuleBuilder>();
      _emittableOperandProviderMock = MockRepository.GenerateStrictMock<IEmittableOperandProvider>();
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_generator.AssemblyDirectory, Is.Null);
      Assert.That (_generator.AssemblyName, Is.StringMatching (c_assemblyNamePattern));

      var debugInfoGenerator = _generator.DebugInfoGenerator;
      Assert.That (debugInfoGenerator.GetType().FullName, Is.EqualTo ("System.Runtime.CompilerServices.SymbolDocumentGenerator"));
      Assert.That (_generator.DebugInfoGenerator, Is.SameAs (debugInfoGenerator));
    }

    [Test]
    public void AssemblyName_Unique ()
    {
      var generator = new ReflectionEmitCodeGenerator (_moduleBuilderFactoryMock, _configurationProviderMock);
      Assert.That (generator.AssemblyName, Is.Not.EqualTo (_generator.AssemblyName));
    }

    [Test]
    public void SetAssemblyDirectory ()
    {
      _generator.SetAssemblyDirectory ("Abc");
      Assert.That (_generator.AssemblyDirectory, Is.EqualTo ("Abc"));

      _generator.SetAssemblyDirectory (null);
      Assert.That (_generator.AssemblyDirectory, Is.Null);
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot set assembly directory after a type has been defined (use FlushCodeToDisk() to start a new assembly).")]
    public void SetAssemblyDirectory_ThrowsForExistingCurrentModuleBuilder ()
    {
      DefineSomeType();
      _generator.SetAssemblyDirectory ("Abc");
    }

    [Test]
    public void SetAssemblyName ()
    {
      _generator.SetAssemblyName ("Def");
      Assert.That (_generator.AssemblyName, Is.EqualTo ("Def"));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException),
        ExpectedMessage = "Cannot set assembly name after a type has been defined (use FlushCodeToDisk() to start a new assembly).")]
    public void SetAssemblyName_ThrowsForExistingCurrentModuleBuilder ()
    {
      DefineSomeType();
      _generator.SetAssemblyName ("Abc");
    }

    [Test]
    public void FlushCodeToDisk ()
    {
      DefineSomeType();
      var fakeAssemblyPath = "fake path";
      _moduleBuilderMock.Expect (mock => mock.SaveToDisk()).Return (fakeAssemblyPath);
      var previousAssemblyName = _generator.AssemblyName;

      var result = _generator.FlushCodeToDisk();

      _moduleBuilderMock.VerifyAllExpectations();
      Assert.That (result, Is.EqualTo (fakeAssemblyPath));
      Assert.That (_generator.AssemblyName, Is.Not.SameAs (previousAssemblyName).And.StringMatching (c_assemblyNamePattern));
    }

    [Test]
    public void FlushCodeToDisk_NoTypeDefined ()
    {
      Assert.That (_generator.FlushCodeToDisk(), Is.Null);
    }

    [Test]
    public void CreateEmittableOperandProvider ()
    {
      _configurationProviderMock.Expect (mock => mock.ForceStrongNaming).Return (false);

      var result = _generator.CreateEmittableOperandProvider ();

      Assert.That (result, Is.TypeOf<EmittableOperandProvider> ());
    }

    [Test]
    public void CreateEmittableOperandProvider_StrongNaming ()
    {
      _configurationProviderMock.Expect (mock => mock.ForceStrongNaming).Return (true);

      var result = _generator.CreateEmittableOperandProvider ();

      Assert.That (result, Is.TypeOf<StrongNameCheckingEmittableOperandProviderDecorator> ());
      var strongNamingDecorator = (StrongNameCheckingEmittableOperandProviderDecorator) result;
      Assert.That (strongNamingDecorator.InnerEmittableOperandProvider, Is.TypeOf<EmittableOperandProvider> ());
    }

    [Test]
    public void DefineType ()
    {
      var name = "DomainType";
      var attributes = (TypeAttributes) 7;
      var forceStrongNaming = BooleanObjectMother.GetRandomBoolean();
      var keyFilePath = "key file path";

      _configurationProviderMock.Expect (mock => mock.ForceStrongNaming).Return (forceStrongNaming);
      _configurationProviderMock.Expect (mock => mock.KeyFilePath).Return (keyFilePath);
      _moduleBuilderFactoryMock
          .Expect (mock => mock.CreateModuleBuilder (_generator.AssemblyName, null, forceStrongNaming, keyFilePath))
          .Return (_moduleBuilderMock);

      var fakeTypeBuilder1 = MockRepository.GenerateStub<ITypeBuilder>();
      var fakeTypeBuilder2 = MockRepository.GenerateStub<ITypeBuilder>();
      _moduleBuilderMock.Expect (mock => mock.DefineType (name, attributes)).Return (fakeTypeBuilder1);
      _moduleBuilderMock.Expect (mock => mock.DefineType ("OtherType", 0)).Return (fakeTypeBuilder2);

      var result1 = _generator.DefineType (name, attributes, _emittableOperandProviderMock);
      var result2 = _generator.DefineType ("OtherType", 0, _emittableOperandProviderMock);

      _moduleBuilderFactoryMock.VerifyAllExpectations();
      _moduleBuilderMock.VerifyAllExpectations();
      _configurationProviderMock.VerifyAllExpectations();

      Assert.That (result1.As<TypeBuilderDecorator>().DecoratedTypeBuilder, Is.SameAs (fakeTypeBuilder1));
      Assert.That (result2.As<TypeBuilderDecorator>().DecoratedTypeBuilder, Is.SameAs (fakeTypeBuilder2));
      Assert.That (PrivateInvoke.GetNonPublicField (result1, "EmittableOperandProvider"), Is.SameAs (_emittableOperandProviderMock));
      Assert.That (PrivateInvoke.GetNonPublicField (result2, "EmittableOperandProvider"), Is.SameAs (_emittableOperandProviderMock));
    }

    private void DefineSomeType ()
    {
      _configurationProviderMock.Stub (stub => stub.ForceStrongNaming).Return (false);
      _configurationProviderMock.Stub (stub => stub.KeyFilePath).Return (null);
      _moduleBuilderFactoryMock
          .Stub (stub => stub.CreateModuleBuilder (_generator.AssemblyName, _generator.AssemblyDirectory, false, null))
          .Return (_moduleBuilderMock);
      _moduleBuilderMock.Stub (stub => stub.DefineType (null, 0)).IgnoreArguments();

      _generator.DefineType ("SomeType", 0, _emittableOperandProviderMock);
    }
  }
}