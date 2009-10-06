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
using System.Collections.Generic;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Definitions;
using Remotion.UnitTests.Mixins.SampleTypes;
using Rhino.Mocks;
using System.Reflection;
using Remotion.Mixins.Context;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class CodeGenerationCacheTest
  {
    private CodeGenerationCache _cache;
    private ConcreteTypeBuilder _typeBuilder;

    private MockRepository _mockRepository;
    private IModuleManager _moduleManagerMock;
    private ITypeGenerator _typeGeneratorMock;
    private IMixinTypeGenerator _mixinTypeGeneratorMock;

    private ClassContext _targetClassContext;
    private MixinDefinition _mixinDefinition;
    private INameProvider _nameProvider1;
    private INameProvider _nameProvider2;
    private ConcreteMixinType _concreteMixinTypeFake;

    [SetUp]
    public void SetUp()
    {
      _typeBuilder = new ConcreteTypeBuilder();

      _mockRepository = new MockRepository();
      _moduleManagerMock = _mockRepository.StrictMock<IModuleManager>();
      _typeBuilder.Scope = _moduleManagerMock;
      _typeGeneratorMock = _mockRepository.StrictMock<ITypeGenerator>();
      _mixinTypeGeneratorMock = _mockRepository.StrictMock<IMixinTypeGenerator>();

      _cache = new CodeGenerationCache (_typeBuilder);
      _targetClassContext = new ClassContext (typeof (BaseType1), typeof (BT1Mixin1));
      _mixinDefinition = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1)).Mixins[0];
      _nameProvider1 = MockRepository.GenerateStub<INameProvider>();
      _nameProvider2 = MockRepository.GenerateStub<INameProvider>();

      _concreteMixinTypeFake = new ConcreteMixinType (_mixinDefinition.GetConcreteMixinTypeIdentifier (), typeof (int), typeof (IServiceProvider), new Dictionary<MethodInfo, MethodInfo> ());
    }

    [Test]
    public void GetConcreteType_Uncached()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (
            Arg.Is (_cache), 
            Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)),
            Arg.Is (_nameProvider1),
            Arg.Is (_nameProvider2)))
          .Return (_typeGeneratorMock);
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (typeof (string));

      _mockRepository.ReplayAll();

      Type result = _cache.GetOrCreateConcreteType (_moduleManagerMock, _targetClassContext, _nameProvider1, _nameProvider2);
      Assert.That (result, Is.SameAs (typeof (string)));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetConcreteType_Cached()
    {
      _moduleManagerMock
          .Expect (mock => mock.CreateTypeGenerator (
              Arg.Is (_cache), 
              Arg<TargetClassDefinition>.Matches (tcd => tcd.ConfigurationContext.Equals (_targetClassContext)), 
              Arg.Is (_nameProvider1),
              Arg.Is (_nameProvider2)))
          .Return (_typeGeneratorMock)
          .Repeat.Once();
      _typeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (typeof (string)).Repeat.Once();

      _mockRepository.ReplayAll();

      Type result1 = _cache.GetOrCreateConcreteType (_moduleManagerMock, _targetClassContext, _nameProvider1, _nameProvider2);
      Type result2 = _cache.GetOrCreateConcreteType (_moduleManagerMock, _targetClassContext, _nameProvider1, _nameProvider2);
      Assert.That (result2, Is.SameAs (result1));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetConcreteMixinType_Uncached()
    {
      _moduleManagerMock.Expect (mock => mock.CreateMixinTypeGenerator (_mixinDefinition, _nameProvider1)).Return (
          _mixinTypeGeneratorMock);
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (_concreteMixinTypeFake);

      _mockRepository.ReplayAll();

      var result = _cache.GetOrCreateConcreteMixinType (_mixinDefinition, _nameProvider1);
      Assert.That (result, Is.SameAs (_concreteMixinTypeFake));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetConcreteMixinType_Cached()
    {
      _moduleManagerMock.Expect (mock => mock.CreateMixinTypeGenerator (_mixinDefinition, _nameProvider1)).Return (
          _mixinTypeGeneratorMock).Repeat.Once();
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (_concreteMixinTypeFake).Repeat.Once();

      _mockRepository.ReplayAll();

      var result1 = _cache.GetOrCreateConcreteMixinType (_mixinDefinition, _nameProvider1);
      var result2 = _cache.GetOrCreateConcreteMixinType (_mixinDefinition, _nameProvider1);
      Assert.That (result2, Is.SameAs (result1));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetConcreteMixinTypeFromCacheOnly_Null()
    {
      var result = _cache.GetConcreteMixinTypeFromCacheOnly (_mixinDefinition.GetConcreteMixinTypeIdentifier());
      Assert.That (result, Is.Null);
    }

    [Test]
    public void GetConcreteMixinTypeFromCacheOnly_NonNull()
    {
      _moduleManagerMock.Expect (mock => mock.CreateMixinTypeGenerator (_mixinDefinition, _nameProvider1)).Return (
          _mixinTypeGeneratorMock).Repeat.Once();
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (_concreteMixinTypeFake).Repeat.Once();

      _mockRepository.ReplayAll();

      var result1 = _cache.GetOrCreateConcreteMixinType (_mixinDefinition, _nameProvider1);
      var result2 = _cache.GetConcreteMixinTypeFromCacheOnly (_mixinDefinition.GetConcreteMixinTypeIdentifier ());
      Assert.That (result2, Is.Not.Null);
      Assert.That (result2, Is.SameAs (result1));
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

      Assert.That (_cache.GetOrCreateConcreteType (_moduleManagerMock, classContext1, _nameProvider1, _nameProvider2), 
          Is.SameAs (typeof (BaseType1)));
      Assert.That (_cache.GetOrCreateConcreteType (_moduleManagerMock, classContext2, _nameProvider1, _nameProvider2), 
          Is.SameAs (typeof (BaseType2)));
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

      var type1 = new ConcreteMixinType (identifier1, typeof (int), typeof (string), new Dictionary<MethodInfo, MethodInfo>());
      var type2 = new ConcreteMixinType (identifier2, typeof (int), typeof (string), new Dictionary<MethodInfo, MethodInfo> ());
      
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BT1Mixin1))).Return (type1);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BT1Mixin2))).Return (type2);
      
      _cache.ImportTypes (typesToImport, metadataImporterStub);

      Assert.That (_cache.GetConcreteMixinTypeFromCacheOnly (identifier1), Is.SameAs (type1));
      Assert.That (_cache.GetConcreteMixinTypeFromCacheOnly (identifier2), Is.SameAs (type2));
    }
  }
}
