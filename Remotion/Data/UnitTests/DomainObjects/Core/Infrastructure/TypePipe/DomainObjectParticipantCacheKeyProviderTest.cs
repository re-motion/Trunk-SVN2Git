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
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Development.UnitTesting.Reflection;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.TypePipe
{
  [TestFixture]
  public class DomainObjectParticipantCacheKeyProviderTest
  {
    private ITypeDefinitionProvider _typeDefinitionProviderMock;

    private DomainObjectParticipantCacheKeyProvider _provider;

    [SetUp]
    public void SetUp ()
    {
      _typeDefinitionProviderMock = MockRepository.GenerateStrictMock<ITypeDefinitionProvider> ();

      _provider = new DomainObjectParticipantCacheKeyProvider (_typeDefinitionProviderMock);
    }

    [Test]
    public void GetCacheKey ()
    {
      var requestedType = ReflectionObjectMother.GetSomeType();
      var fakePublicDomainType = ReflectionObjectMother.GetSomeOtherType();
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _typeDefinitionProviderMock.Expect (mock => mock.GetPublicDomainObjectType (requestedType)).Return (fakePublicDomainType);
      _typeDefinitionProviderMock.Expect (mock => mock.GetTypeDefinition (fakePublicDomainType)).Return (fakeClassDefinition);

      var result = _provider.GetCacheKey (requestedType);

      _typeDefinitionProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeClassDefinition));
    }

    [Test]
    public void RebuildCacheKey ()
    {
      var generatedType = ReflectionObjectMother.GetSomeType();
      var fakePublicDomainType = ReflectionObjectMother.GetSomeOtherType();
      var fakeClassDefinition = ClassDefinitionObjectMother.CreateClassDefinition();
      _typeDefinitionProviderMock.Expect (mock => mock.GetPublicDomainObjectType (generatedType)).Return (fakePublicDomainType);
      _typeDefinitionProviderMock.Expect (mock => mock.GetTypeDefinition (fakePublicDomainType)).Return (fakeClassDefinition);

      var result = _provider.RebuildCacheKey (generatedType);

      _typeDefinitionProviderMock.VerifyAllExpectations();
      Assert.That (result, Is.SameAs (fakeClassDefinition));
    }
  }
}