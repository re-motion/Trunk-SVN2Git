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
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.DomainObjects.Persistence.Rdbms.SqlServer;
using Remotion.Data.DomainObjects.Tracing;
using Remotion.Mixins;
using Remotion.SecurityManager.Persistence;
using Rhino.Mocks;

namespace Remotion.SecurityManager.UnitTests.Persistence
{
  [TestFixture]
  public class SecurityManagerSqlStorageObjectFactoryTest
  {
    private RdbmsProviderDefinition _rdbmsProviderDefinition;
    private SecurityManagerSqlStorageObjectFactory _securityManagerSqlStorageObjectFactory;
    private IPersistenceListener _persistenceListenerStub;

    [SetUp]
    public void SetUp ()
    {
      _rdbmsProviderDefinition = new RdbmsProviderDefinition ("TestDomain", new SecurityManagerSqlStorageObjectFactory(), "ConnectionString");
      _securityManagerSqlStorageObjectFactory = new SecurityManagerSqlStorageObjectFactory ();
      _persistenceListenerStub = MockRepository.GenerateStub<IPersistenceListener>();
    }

    [Test]
    public void CreateStorageProvider ()
    {
      var result = _securityManagerSqlStorageObjectFactory.CreateStorageProvider (_persistenceListenerStub, _rdbmsProviderDefinition);

      Assert.That (result, Is.TypeOf (typeof (SecurityManagerSqlProvider)));
      Assert.That (result.PersistenceListener, Is.SameAs (_persistenceListenerStub));
      Assert.That (result.StorageProviderDefinition, Is.SameAs (_rdbmsProviderDefinition));
    }

    [Test]
    public void CreateStorageProviderWithMixin ()
    {
      using (
          MixinConfiguration.BuildFromActive().ForClass (typeof (SqlProvider)).Clear().AddMixins (typeof (SecurityManagerSqlProviderTestMixin)).
              EnterScope())
      {
        var result = _securityManagerSqlStorageObjectFactory.CreateStorageProvider (_persistenceListenerStub, _rdbmsProviderDefinition);

        Assert.That (Mixin.Get<SecurityManagerSqlProviderTestMixin> (result), Is.Not.Null);
      }
    }
  }
}