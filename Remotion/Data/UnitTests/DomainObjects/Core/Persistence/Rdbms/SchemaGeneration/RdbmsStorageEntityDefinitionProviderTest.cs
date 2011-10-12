// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Persistence.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SchemaGeneration;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SchemaGeneration
{
  [TestFixture]
  public class RdbmsStorageEntityDefinitionProviderTest
  {
    private UnitTestStorageProviderStubDefinition _storageProviderDefinition;
    private RdbmsStorageEntityDefinitionProvider _entityDefinitionProvider;
    private ClassDefinition _classDefinition1;
    private ClassDefinition _classDefinition2;
    private ClassDefinition _classDefinition3;

    [SetUp]
    public void SetUp ()
    {
      _storageProviderDefinition = new UnitTestStorageProviderStubDefinition ("SPID");
      _entityDefinitionProvider = new RdbmsStorageEntityDefinitionProvider();
      _classDefinition1 = ClassDefinitionObjectMother.CreateClassDefinition ("Order", "Order", _storageProviderDefinition, typeof (Order), false);
      _classDefinition2 = ClassDefinitionObjectMother.CreateClassDefinition (
          "OrderItem", "OrderItem", _storageProviderDefinition, typeof (OrderItem), false);
      _classDefinition3 = ClassDefinitionObjectMother.CreateClassDefinition ("Customer", "Customer", _storageProviderDefinition, typeof (Customer), false);
    }

    [Test]
    public void GetEntityDefinitions_NoClassDefinition ()
    {
      Assert.That (_entityDefinitionProvider.GetEntityDefinitions (new ClassDefinition[0]), Is.Empty);
    }

    [Test]
    public void GetEntityDefinitions_OneClassDefinition ()
    {
      var result = _entityDefinitionProvider.GetEntityDefinitions (new[] { _classDefinition1 }).ToList();

      Assert.That (result.Count, Is.EqualTo (1));
      Assert.That (result[0], Is.SameAs (_classDefinition1.StorageEntityDefinition));
    }

    [Test]
    public void GetEntityDefinitions_SeveralClassDefinitions ()
    {
      var result = _entityDefinitionProvider.GetEntityDefinitions (new[] { _classDefinition1, _classDefinition2, _classDefinition3 }).ToList();

      Assert.That (
          result,
          Is.EquivalentTo (
              new[]
              { _classDefinition1.StorageEntityDefinition, _classDefinition2.StorageEntityDefinition, _classDefinition3.StorageEntityDefinition }));
    }

    [Test]
    public void GetEntityDefinitions_OnClassWithNoIEntityDefinition ()
    {
      var storageEntityDefinitionStub = MockRepository.GenerateStub<IStorageEntityDefinition> ();
      storageEntityDefinitionStub.Stub (stub => stub.StorageProviderDefinition).Return (_storageProviderDefinition);
      _classDefinition1.SetStorageEntity (storageEntityDefinitionStub);

      var result = _entityDefinitionProvider.GetEntityDefinitions (new[] { _classDefinition1, _classDefinition2, _classDefinition3 });

      Assert.That (
          result,
          Is.EquivalentTo (
              new[] { _classDefinition2.StorageEntityDefinition, _classDefinition3.StorageEntityDefinition }));
    }
  }
}