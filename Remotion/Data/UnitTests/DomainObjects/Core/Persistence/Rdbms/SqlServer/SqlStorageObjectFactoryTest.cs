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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Persistence;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.SchemaGeneration;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer.Sql2005;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance;
using Remotion.Mixins;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms.SqlServer
{
  [TestFixture]
  public class SqlStorageObjectFactoryTest
  {
    private SqlStorageObjectFactory _sqlProviderFactory;
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private IPersistenceListener _persistenceListenerStub;
    private StorageProviderDefinitionFinder _storageProviderDefinitionFinder;

    [SetUp]
    public void SetUp ()
    {
      _rdbmsProviderDefinition = new RdbmsProviderDefinition ("TestDomain",new SqlStorageObjectFactory(), "ConnectionString");
      _sqlProviderFactory = new SqlStorageObjectFactory ();
      _persistenceListenerStub = MockRepository.GenerateStub<IPersistenceListener> ();
      _storageProviderDefinitionFinder = new StorageProviderDefinitionFinder (DomainObjectsConfiguration.Current.Storage);
    }

    [Test]
    public void StorageProviderType ()
    {
      Assert.That (_sqlProviderFactory.StorageProviderType, Is.SameAs(typeof (SqlProvider)));
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _sqlProviderFactory.CreateStorageProvider (_persistenceListenerStub, _rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SqlProvider)));
      Assert.That (result.PersistenceListener, Is.SameAs (_persistenceListenerStub));
      Assert.That (result.Definition, Is.SameAs (_rdbmsProviderDefinition));
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (SqlProvider)).Clear ().AddMixins (typeof (SqlProviderTestMixin)).EnterScope ())
      {
        var result = _sqlProviderFactory.CreateStorageProvider (_persistenceListenerStub, _rdbmsProviderDefinition);

        Assert.That (Mixin.Get<SqlProviderTestMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void GetTypeConversionProvider ()
    {
      var result = _sqlProviderFactory.CreateTypeConversionProvider();

      Assert.That (result, Is.TypeOf (typeof (TypeConversionProvider)));
    }

    [Test]
    public void GetTypeProvider ()
    {
      var result = _sqlProviderFactory.CreateTypeProvider();

      Assert.That (result, Is.TypeOf (typeof (TypeProvider)));
    }

    [Test]
    public void GetPersistenceModelLoader ()
    {
      var result = _sqlProviderFactory.CreatePersistenceModelLoader(_storageProviderDefinitionFinder, _rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (RdbmsPersistenceModelLoader)));
      Assert.That (((RdbmsPersistenceModelLoader) result).ColumnDefinitionFactory, Is.TypeOf (typeof (ColumnDefinitionFactory)));
      Assert.That (((RdbmsPersistenceModelLoader) result).StorageProviderID, Is.EqualTo ("TestDomain"));
    }

    [Test]
    public void CreateSchemaFileBuilder ()
    {
      var result = _sqlProviderFactory.CreateSchemaFileBuilder(_rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (FileBuilder)));
      Assert.That (result.RdbmsProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));
    }
  }
}