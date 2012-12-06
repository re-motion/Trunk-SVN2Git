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
using Remotion.Development.RhinoMocks.UnitTesting.Threading;
using Remotion.Development.UnitTesting.ObjectMothers;
using Remotion.Mixins.CodeGeneration;
using Remotion.Reflection;
using Rhino.Mocks;

namespace Remotion.Mixins.UnitTests.Core.CodeGeneration
{
  [TestFixture]
  public class LockingCodeGenerationCacheDecoratorTest
  {
    private ICodeGenerationCache _innerCacheMock;
    private object _lockObject;

    private LockingCodeGenerationCacheDecorator _decorator;
    private LockingDecoratorTestHelper<ICodeGenerationCache> _testHelper;

    [SetUp]
    public void SetUp ()
    {
      _innerCacheMock = MockRepository.GenerateStrictMock<ICodeGenerationCache>();
      _lockObject = new object();

      _decorator = new LockingCodeGenerationCacheDecorator (_innerCacheMock, _lockObject);
      _testHelper = new LockingDecoratorTestHelper<ICodeGenerationCache> (_decorator, _lockObject, _innerCacheMock);
    }

    [Test]
    public void AllMembers_AreGuardedByLock ()
    {
      var classContext = ClassContextObjectMother.Create();
      var type = typeof (object);
      var boolean = BooleanObjectMother.GetRandomBoolean ();
      var lookupInfo = MockRepository.GenerateStub<IConstructorLookupInfo> ();
      var concreteMixinTypeIdentifier = ConcreteMixinTypeIdentifierObjectMother.Create();
      var concreteMixinType = ConcreteMixinTypeObjectMother.Create();
      var types = new[] { typeof (int) };
      var metadataImporter = MockRepository.GenerateStub<IConcreteTypeMetadataImporter>();

      _testHelper.ExpectSynchronizedDelegation (c => c.Clear ());
      _testHelper.ExpectSynchronizedDelegation (c => c.GetOrCreateConcreteType (classContext), type);
      _testHelper.ExpectSynchronizedDelegation (c => c.GetOrCreateConstructorLookupInfo (classContext, boolean), lookupInfo);
      _testHelper.ExpectSynchronizedDelegation (c => c.GetOrCreateConcreteMixinType (concreteMixinTypeIdentifier), concreteMixinType);
      _testHelper.ExpectSynchronizedDelegation (c => c.ImportTypes (types, metadataImporter));
    }
  }
}