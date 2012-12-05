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
using NUnit.Framework;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.Mixins.UnitTests.Core.TestDomain;
using Remotion.Mixins.Utilities;
using Rhino.Mocks;
using System.Reflection;
using Remotion.Mixins.Context;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class CodeGenerationCacheTest
  {
    private MockRepository _mockRepository;
    private IModuleManager _moduleManagerMock;
    private ITypeGenerator _typeGeneratorMock;
    private IMixinTypeGenerator _mixinTypeGeneratorMock;

    private IConcreteMixedTypeNameProvider _mixedTypeNameProvider;
    private IConcreteMixinTypeNameProvider _mixinTypeNameProvider;
    private IConcreteMixinTypeProvider _concreteMixinTypeProvider;

    private CodeGenerationCache _cache;

    private ClassContext _targetClassContext;
    private ConcreteMixinTypeIdentifier _concreteMixinTypeIdentifier;
    private ConcreteMixinType _concreteMixinTypeFake;

    [SetUp]
    public void SetUp()
    {
      _mockRepository = new MockRepository();
      _moduleManagerMock = _mockRepository.StrictMock<IModuleManager>();
      _typeGeneratorMock = _mockRepository.StrictMock<ITypeGenerator>();
      _mixinTypeGeneratorMock = _mockRepository.StrictMock<IMixinTypeGenerator>();

      _mixedTypeNameProvider = MockRepository.GenerateStub<IConcreteMixedTypeNameProvider> ();
      _mixinTypeNameProvider = MockRepository.GenerateStub<IConcreteMixinTypeNameProvider> ();

      _concreteMixinTypeProvider = MockRepository.GenerateStub<IConcreteMixinTypeProvider> ();

      _cache = new CodeGenerationCache (_moduleManagerMock, _mixedTypeNameProvider, _mixinTypeNameProvider, _concreteMixinTypeProvider);

      _targetClassContext = ClassContextObjectMother.Create (typeof (BaseType1), typeof (BT1Mixin1));

      var mixinDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1)).Mixins[0];
      _concreteMixinTypeIdentifier = mixinDefinition.GetConcreteMixinTypeIdentifier ();
      _concreteMixinTypeFake = CreateConcreteMixinTypeFake (_concreteMixinTypeIdentifier);
    }

    private ConcreteMixinType CreateConcreteMixinTypeFake (ConcreteMixinTypeIdentifier concreteMixinTypeIdentifier)
    {
      return new ConcreteMixinType (
          concreteMixinTypeIdentifier, 
          typeof (int), 
          typeof (IServiceProvider), 
          new Dictionary<MethodInfo, MethodInfo> (), 
          new Dictionary<MethodInfo, MethodInfo>());
    }

    [Test]
    public void GetConcreteType_Uncached()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)),
            Arg.Is (_mixedTypeNameProvider), Arg.Is (_concreteMixinTypeProvider)))
          .Return (_typeGeneratorMock);
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (typeof (string));

      _mockRepository.ReplayAll();

      Type result = _cache.GetOrCreateConcreteType (_targetClassContext);
      Assert.That (result, Is.SameAs (typeof (string)));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetConcreteType_Cached()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)), 
              Arg.Is (_mixedTypeNameProvider), Arg.Is (_concreteMixinTypeProvider)))
          .Return (_typeGeneratorMock)
          .Repeat.Once();
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (typeof (string)).Repeat.Once();

      _mockRepository.ReplayAll();

      Type result1 = _cache.GetOrCreateConcreteType (_targetClassContext);
      Type result2 = _cache.GetOrCreateConcreteType (_targetClassContext);
      Assert.That (result2, Is.SameAs (result1));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetOrCreateConstructorLookupInfo_Uncached ()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)),
            Arg.Is (_mixedTypeNameProvider), Arg.Is (_concreteMixinTypeProvider)))
          .Return (_typeGeneratorMock);
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType ()).Return (typeof (string));

      _mockRepository.ReplayAll ();

      var result = _cache.GetOrCreateConstructorLookupInfo (_targetClassContext, true);
      Assert.That (result, Is.TypeOf (typeof (MixedTypeConstructorLookupInfo)));
      Assert.That (result.DefiningType, Is.SameAs (typeof (string)));
      Assert.That (((MixedTypeConstructorLookupInfo) result).TargetType, Is.SameAs (_targetClassContext.Type));
      Assert.That (((MixedTypeConstructorLookupInfo) result).AllowNonPublic, Is.True);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOrCreateConstructorLookupInfo_Cached ()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)),
            Arg.Is (_mixedTypeNameProvider), Arg.Is (_concreteMixinTypeProvider)))
          .Return (_typeGeneratorMock).Repeat.Once ();
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType ()).Return (typeof (string)).Repeat.Once();

      _mockRepository.ReplayAll ();

      var result1 = _cache.GetOrCreateConstructorLookupInfo (_targetClassContext, true);
      var result2 = _cache.GetOrCreateConstructorLookupInfo (_targetClassContext, true);

      Assert.That (result2, Is.SameAs (result1));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetOrCreateConstructorLookupInfo_Cached_KeyContainsAllowNonPublicFlag ()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)),
            Arg.Is (_mixedTypeNameProvider), Arg.Is (_concreteMixinTypeProvider)))
          .Return (_typeGeneratorMock).Repeat.Once ();
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType ()).Return (typeof (string)).Repeat.Once ();

      _mockRepository.ReplayAll ();

      var result1 = _cache.GetOrCreateConstructorLookupInfo (_targetClassContext, true);
      var result2 = _cache.GetOrCreateConstructorLookupInfo (_targetClassContext, false);

      Assert.That (result2, Is.Not.SameAs (result1));
      Assert.That (((MixedTypeConstructorLookupInfo) result1).AllowNonPublic, Is.True);
      Assert.That (((MixedTypeConstructorLookupInfo) result2).AllowNonPublic, Is.False);

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void GetConcreteMixinType_Uncached()
    {
      _moduleManagerMock.Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinTypeNameProvider)).Return (
          _mixinTypeGeneratorMock);
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (_concreteMixinTypeFake);

      _mockRepository.ReplayAll();

      var result = _cache.GetOrCreateConcreteMixinType (_concreteMixinTypeIdentifier);
      Assert.That (result, Is.SameAs (_concreteMixinTypeFake));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetConcreteMixinType_Cached()
    {
      _moduleManagerMock.Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinTypeNameProvider)).Return (
          _mixinTypeGeneratorMock).Repeat.Once();
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (_concreteMixinTypeFake).Repeat.Once();

      _mockRepository.ReplayAll();

      var result1 = _cache.GetOrCreateConcreteMixinType (_concreteMixinTypeIdentifier);
      var result2 = _cache.GetOrCreateConcreteMixinType (_concreteMixinTypeIdentifier);
      Assert.That (result2, Is.SameAs (result1));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void Clear_ClearsTypes ()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)),
              Arg.Is (_mixedTypeNameProvider), Arg.Is (_concreteMixinTypeProvider)))
          .Return (_typeGeneratorMock)
          .Repeat.Twice();
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType ()).Return (typeof (string)).Repeat.Once ();
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType ()).Return (typeof (object)).Repeat.Once ();

      _mockRepository.ReplayAll ();

      Type result1 = _cache.GetOrCreateConcreteType (_targetClassContext);
      _cache.Clear ();
      Type result2 = _cache.GetOrCreateConcreteType (_targetClassContext);
      Assert.That (result2, Is.Not.SameAs (result1));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Clear_ClearsConstructorLookupInfos ()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)),
            Arg.Is (_mixedTypeNameProvider), Arg.Is (_concreteMixinTypeProvider)))
          .Return (_typeGeneratorMock)
          .Repeat.Twice();
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType ()).Return (typeof (string)).Repeat.Twice ();

      _mockRepository.ReplayAll ();

      var result1 = _cache.GetOrCreateConstructorLookupInfo (_targetClassContext, true);

      _cache.Clear();

      var result2 = _cache.GetOrCreateConstructorLookupInfo (_targetClassContext, true);

      Assert.That (result2, Is.Not.SameAs (result1));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void Clear_ClearsMixinTypes ()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateMixinTypeGenerator (_concreteMixinTypeIdentifier, _mixinTypeNameProvider))
          .Return (_mixinTypeGeneratorMock).Repeat.Twice();
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType ()).Return (_concreteMixinTypeFake).Repeat.Once ();
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType ()).Return (CreateConcreteMixinTypeFake (_concreteMixinTypeIdentifier)).Repeat.Once ();

      _mockRepository.ReplayAll ();

      var result1 = _cache.GetOrCreateConcreteMixinType (_concreteMixinTypeIdentifier);

      _cache.Clear();

      var result2 = _cache.GetOrCreateConcreteMixinType (_concreteMixinTypeIdentifier);
      Assert.That (result2, Is.Not.SameAs (result1));

      _mockRepository.VerifyAll ();
    }

    [Test]
    public void ImportTypes_MixedTypes()
    {
      var targetClassDefinition1 = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1));
      var targetClassDefinition2 = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType2), typeof (BT1Mixin1));

      var classContext1 = targetClassDefinition1.ConfigurationContext;
      var classContext2 = targetClassDefinition2.ConfigurationContext;

      var typesToImport = new[] { typeof (BaseType1), typeof (BaseType2) };
      var metadataImporterStub = MockRepository.GenerateStub<IConcreteTypeMetadataImporter> ();
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BaseType1))).Return (classContext1);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BaseType2))).Return (classContext2);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BaseType1))).Return (null);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BaseType2))).Return (null);

      _cache.ImportTypes (typesToImport, metadataImporterStub);

      Assert.That (_cache.GetOrCreateConcreteType (classContext1), Is.SameAs (typeof (BaseType1)));
      Assert.That (_cache.GetOrCreateConcreteType (classContext2), Is.SameAs (typeof (BaseType2)));
    }

    [Test]
    public void ImportTypes_MixinTypes ()
    {
      var identifier1 = new ConcreteMixinTypeIdentifier (typeof (BT1Mixin1), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      var identifier2 = new ConcreteMixinTypeIdentifier (typeof (BT1Mixin2), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      var typesToImport = new[] { typeof (BT1Mixin1), typeof (BT1Mixin2) };
      var metadataImporterStub = MockRepository.GenerateStub<IConcreteTypeMetadataImporter> ();

      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BT1Mixin1))).Return (null);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BT1Mixin2))).Return (null);

      var type1 = new ConcreteMixinType (
          identifier1, 
          typeof (int), 
          typeof (string), 
          new Dictionary<MethodInfo, MethodInfo> (), 
          new Dictionary<MethodInfo, MethodInfo> ());
      var type2 = new ConcreteMixinType (
          identifier2, 
          typeof (int), 
          typeof (string), 
          new Dictionary<MethodInfo, MethodInfo> (), 
          new Dictionary<MethodInfo, MethodInfo> ());
      
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BT1Mixin1))).Return (type1);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BT1Mixin2))).Return (type2);
      
      _cache.ImportTypes (typesToImport, metadataImporterStub);

      Assert.That (_cache.GetOrCreateConcreteMixinType (identifier1), Is.SameAs (type1));
      Assert.That (_cache.GetOrCreateConcreteMixinType (identifier2), Is.SameAs (type2));
    }
  }
}
