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
using Remotion.Collections;
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
      var concreteMixinType = new ConcreteMixinType (typeof (int));

      _moduleManagerMock.Expect (mock => mock.CreateMixinTypeGenerator (_typeGeneratorMock, _mixinDefinition, _nameProvider1)).Return (
          _mixinTypeGeneratorMock);
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (concreteMixinType);

      _mockRepository.ReplayAll();

      var result = _cache.GetOrCreateConcreteMixinType (_typeGeneratorMock, _mixinDefinition, _nameProvider1);
      Assert.That (result, Is.SameAs (concreteMixinType));

      _mockRepository.VerifyAll();
    }

    [Test]
    public void GetConcreteMixinType_Cached()
    {
      var concreteMixinType = new ConcreteMixinType (typeof (int));

      _moduleManagerMock.Expect (mock => mock.CreateMixinTypeGenerator (_typeGeneratorMock, _mixinDefinition, _nameProvider1)).Return (
          _mixinTypeGeneratorMock).Repeat.Once();
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (concreteMixinType).Repeat.Once();

      _mockRepository.ReplayAll();

      var result1 = _cache.GetOrCreateConcreteMixinType (_typeGeneratorMock, _mixinDefinition, _nameProvider1);
      var result2 = _cache.GetOrCreateConcreteMixinType (_typeGeneratorMock, _mixinDefinition, _nameProvider1);
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
      var concreteMixinType = new ConcreteMixinType (typeof (int));
      _moduleManagerMock.Expect (mock => mock.CreateMixinTypeGenerator (_typeGeneratorMock, _mixinDefinition, _nameProvider1)).Return (
          _mixinTypeGeneratorMock).Repeat.Once();
      _mixinTypeGeneratorMock.Expect (mock => mock.GetBuiltType()).Return (concreteMixinType).Repeat.Once();

      _mockRepository.ReplayAll();

      var result1 = _cache.GetOrCreateConcreteMixinType (_typeGeneratorMock, _mixinDefinition, _nameProvider1);
      var result2 = _cache.GetConcreteMixinTypeFromCacheOnly (_mixinDefinition.GetConcreteMixinTypeIdentifier ());
      Assert.That (result2, Is.Not.Null);
      Assert.That (result2, Is.SameAs (result1));
    }

    [Test]
    public void ImportTypes_MixedTypes()
    {
      var targetClassDefinition1 = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1));
      var targetClassDefinition2 = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType1), typeof (BT1Mixin1));
      var targetClassDefinition3 = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType2), typeof (BT1Mixin1));
      var targetClassDefinition4 = DefinitionObjectMother.CreateTargetClassDefinition (typeof (BaseType2), typeof (BT1Mixin1));

      var typesToImport = new[] { typeof (BaseType1), typeof (BaseType2) };
      var metadataImporterStub = MockRepository.GenerateStub<IConcreteTypeMetadataImporter> ();
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BaseType1), TargetClassDefinitionCache.Current)).Return (new[] { targetClassDefinition1, targetClassDefinition2 });
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BaseType2), TargetClassDefinitionCache.Current)).Return (new[] { targetClassDefinition3, targetClassDefinition4 });
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BaseType1))).Return (null);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BaseType2))).Return (null);

      _cache.ImportTypes (typesToImport, metadataImporterStub);

      Assert.That (_cache.GetOrCreateConcreteType (_moduleManagerMock, targetClassDefinition1.ConfigurationContext, _nameProvider1, _nameProvider2), 
          Is.SameAs (typeof (BaseType1)));
      Assert.That (_cache.GetOrCreateConcreteType (_moduleManagerMock, targetClassDefinition2.ConfigurationContext, _nameProvider1, _nameProvider2), 
          Is.SameAs (typeof (BaseType1)));
      Assert.That (_cache.GetOrCreateConcreteType (_moduleManagerMock, targetClassDefinition3.ConfigurationContext, _nameProvider1, _nameProvider2), 
          Is.SameAs (typeof (BaseType2)));
      Assert.That (_cache.GetOrCreateConcreteType (_moduleManagerMock, targetClassDefinition4.ConfigurationContext, _nameProvider1, _nameProvider2), 
          Is.SameAs (typeof (BaseType2)));
    }

    [Test]
    public void ImportTypes_MixinTypes ()
    {
      var identifier1 = new ConcreteMixinTypeIdentifier (typeof (BT1Mixin1), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());
      var identifier2 = new ConcreteMixinTypeIdentifier (typeof (BT1Mixin2), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      var typesToImport = new[] { typeof (BT1Mixin1), typeof (BT1Mixin2) };
      var metadataImporterStub = MockRepository.GenerateStub<IConcreteTypeMetadataImporter> ();
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BT1Mixin1), TargetClassDefinitionCache.Current)).Return (new TargetClassDefinition[0]);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BT1Mixin2), TargetClassDefinitionCache.Current)).Return (new TargetClassDefinition[0]);
     
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BT1Mixin1))).Return (identifier1);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BT1Mixin2))).Return (identifier2);
      
      metadataImporterStub.Stub (stub => stub.GetMethodWrappersForMixinType (typeof (BT1Mixin1))).Return (new Tuple<MethodInfo, MethodInfo>[0]);
      metadataImporterStub.Stub (stub => stub.GetMethodWrappersForMixinType (typeof (BT1Mixin2))).Return (new Tuple<MethodInfo, MethodInfo>[0]);

      _cache.ImportTypes (typesToImport, metadataImporterStub);

      Assert.That (_cache.GetConcreteMixinTypeFromCacheOnly (identifier1).GeneratedType, Is.SameAs (typeof (BT1Mixin1)));
      Assert.That (_cache.GetConcreteMixinTypeFromCacheOnly (identifier2).GeneratedType, Is.SameAs (typeof (BT1Mixin2)));
    }

    [Test]
    public void Import_AddsPublicMethodWrappers()
    {
      var method1 = typeof (DateTime).GetMethod ("get_Now");
      var method2 = typeof (DateTime).GetMethod ("get_Day");
      var wrapper1 = typeof (DateTime).GetMethod ("get_Month");
      var wrapper2 = typeof (DateTime).GetMethod ("get_Year");

      var identifier = new ConcreteMixinTypeIdentifier (typeof (BT1Mixin1), new HashSet<MethodInfo> (), new HashSet<MethodInfo> ());

      var metadataImporterStub = MockRepository.GenerateStub<IConcreteTypeMetadataImporter> ();
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixedType (typeof (BT1Mixin1), TargetClassDefinitionCache.Current)).Return (new TargetClassDefinition[0]);
      metadataImporterStub.Stub (stub => stub.GetMetadataForMixinType (typeof (BT1Mixin1))).Return (identifier);

      metadataImporterStub.Stub (stub => stub.GetMethodWrappersForMixinType (typeof (BT1Mixin1)))
          .Return (new[] { Tuple.NewTuple (method1, wrapper1), Tuple.NewTuple (method2, wrapper2) });

      _cache.ImportTypes (new[] {typeof (BT1Mixin1)}, metadataImporterStub);

      ConcreteMixinType importedType = _cache.GetConcreteMixinTypeFromCacheOnly (identifier);
      Assert.That (importedType.GetMethodWrapper (method1), Is.EqualTo (wrapper1));
      Assert.That (importedType.GetMethodWrapper (method2), Is.EqualTo (wrapper2));
    }
  }
}
