/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Runtime.InteropServices;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Mixins;
using Remotion.Mixins.CodeGeneration;
using Remotion.Mixins.Context;
using Remotion.Mixins.Definitions;
using Rhino.Mocks;
using System.Linq;

namespace Remotion.UnitTests.Mixins.CodeGeneration
{
  [TestFixture]
  public class AttributeBasedMetadataImporterTest
  {
    private ITargetClassDefinitionCache _targetClassDefinitionCacheStub;
    private TargetClassDefinition _targetClassDefinition1;
    private TargetClassDefinition _targetClassDefinition2;

    [SetUp]
    public void SetUp()
    {
      _targetClassDefinition1 = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object), typeof (int));
      _targetClassDefinition2 = DefinitionObjectMother.CreateTargetClassDefinition (typeof (object), typeof (int));

      _targetClassDefinitionCacheStub = MockRepository.GenerateStub<ITargetClassDefinitionCache> ();
      _targetClassDefinitionCacheStub.Stub (stub => stub.GetTargetClassDefinition (new ClassContext (typeof (object)))).Return (_targetClassDefinition1);
      _targetClassDefinitionCacheStub.Stub (stub => stub.GetTargetClassDefinition (new ClassContext (typeof (string)))).Return (_targetClassDefinition2);
    }

    [Test]
    public void GetMetadataForMixedType_Wrapper()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedResult = new TargetClassDefinition[0];
      importerMock.Expect (mock => mock.GetMetadataForMixedType ((_Type) typeof (object), _targetClassDefinitionCacheStub)).Return (expectedResult);

      importerMock.Replay ();
      var result = importerMock.GetMetadataForMixedType (typeof (object), _targetClassDefinitionCacheStub);

      Assert.That (result, Is.SameAs (expectedResult));
      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType_Wrapper ()
    {
      var importerMock = new MockRepository ().PartialMock<AttributeBasedMetadataImporter> ();
      var expectedResult = new MixinDefinition[0];
      importerMock.Expect (mock => mock.GetMetadataForMixinType ((_Type) typeof (object), TargetClassDefinitionCache.Current)).Return (expectedResult);

      importerMock.Replay ();
      var result = importerMock.GetMetadataForMixinType (typeof (object), TargetClassDefinitionCache.Current);

      Assert.That (result, Is.SameAs (expectedResult));
      importerMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixedType ()
    {
      var importer = new AttributeBasedMetadataImporter ();

      var typeMock = MockRepository.GenerateMock<_Type> ();
      var attribute1 = new ConcreteMixedTypeAttribute (typeof (object), new MixinKind[0], new Type[0], new Type[0], new Type[0]);
      var attribute2 = new ConcreteMixedTypeAttribute (typeof (string), new MixinKind[0], new Type[0], new Type[0], new Type[0]);

      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixedTypeAttribute), false)).Return (new[] { attribute1, attribute2});
      typeMock.Replay ();

      var results = importer.GetMetadataForMixedType (typeMock, _targetClassDefinitionCacheStub);
      Assert.That (results.ToArray(), Is.EqualTo (new[] { _targetClassDefinition1, _targetClassDefinition2 }));

      typeMock.VerifyAllExpectations ();
    }

    [Test]
    public void GetMetadataForMixinType ()
    {
      var importer = new AttributeBasedMetadataImporter ();

      var typeMock = MockRepository.GenerateMock<_Type> ();
      var attribute1 = new ConcreteMixinTypeAttribute (0, typeof (object), new MixinKind[0], new Type[0], new Type[0], new Type[0]);
      var attribute2 = new ConcreteMixinTypeAttribute (0, typeof (string), new MixinKind[0], new Type[0], new Type[0], new Type[0]);

      typeMock.Expect (mock => mock.GetCustomAttributes (typeof (ConcreteMixinTypeAttribute), false)).Return (new[] { attribute1, attribute2 });
      typeMock.Replay ();

      var results = importer.GetMetadataForMixinType (typeMock, _targetClassDefinitionCacheStub);
      Assert.That (results.ToArray (), Is.EqualTo (new[] { _targetClassDefinition1.Mixins[0], _targetClassDefinition2.Mixins[0] }));

      typeMock.VerifyAllExpectations ();
    }
  }
}