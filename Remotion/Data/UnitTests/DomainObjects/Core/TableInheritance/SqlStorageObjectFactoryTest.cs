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
using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Persistence.Configuration;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Mixins;
using Remotion.Utilities;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  [TestFixture]
  public class SqlStorageObjectFactoryTest
  {
    private SqlStorageObjectFactory _sqlProviderFactory;
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private IPersistenceListener _persistenceListenerStub;
  
    [SetUp]
    public void SetUp ()
    {
      _rdbmsProviderDefinition = new RdbmsProviderDefinition ("TestDomain", typeof(SqlStorageObjectFactory), "ConnectionString");
      _sqlProviderFactory = new SqlStorageObjectFactory (_rdbmsProviderDefinition);
      _persistenceListenerStub = MockRepository.GenerateStub<IPersistenceListener> ();
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _sqlProviderFactory.CreateStorageProvider (_persistenceListenerStub);

      Assert.That (result, Is.TypeOf (typeof (SqlProvider)));
      Assert.That (result.PersistenceListener, Is.SameAs (_persistenceListenerStub));
      Assert.That (result.Definition, Is.SameAs (_rdbmsProviderDefinition));
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (MixinConfiguration.BuildFromActive ().ForClass (typeof (SqlProvider)).Clear ().AddMixins (typeof (StorageObjectFactoryMixin)).EnterScope ())
      {
        var result = _sqlProviderFactory.CreateStorageProvider (_persistenceListenerStub);

        Assert.That (Mixin.Get<StorageObjectFactoryMixin> (result), Is.Not.Null);
      }
    }

    [Test]
    public void GetTypeConversionProvider ()
    {
      var result = _sqlProviderFactory.GetTypeConversionProvider();

      Assert.That (result, Is.TypeOf (typeof (TypeConversionProvider)));
    }

    [Test]
    public void GetTypeProvider ()
    {
      var result = _sqlProviderFactory.GetTypeProvider();

      Assert.That (result, Is.TypeOf (typeof (TypeProvider)));
    }

    [Test]
    public void GetPersistenceModelLoader ()
    {
      var result = _sqlProviderFactory.GetPersistenceModelLoader();

      Assert.That (result, Is.TypeOf (typeof (PersistenceModelLoader)));
    }
  }
}