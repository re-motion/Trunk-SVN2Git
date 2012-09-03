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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Infrastructure.Enlistment;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.UnitTests.DomainObjects.Core.DataManagement.SerializableFakes;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure.ObjectLifetime
{
  [TestFixture]
  public class ObjectInitializationContextProviderTest : StandardMappingTest
  {
    private ObjectInitializationContextProvider _provider;
    private IEnlistedDomainObjectManager _enlistedDomainObjectManagerStub;
    private IDataManager _dataManagerStub;

    public override void SetUp ()
    {
      base.SetUp ();

      _enlistedDomainObjectManagerStub = MockRepository.GenerateStub<IEnlistedDomainObjectManager>();
      _dataManagerStub = MockRepository.GenerateStub<IDataManager>();

      _provider = new ObjectInitializationContextProvider (_enlistedDomainObjectManagerStub, _dataManagerStub);
    }
    
    [Test]
    public void CreateContext ()
    {
      var objectID = DomainObjectIDs.Order1;
      var bindingTransaction = ClientTransaction.CreateRootTransaction();

      var result = _provider.CreateContext (objectID, bindingTransaction);

      Assert.That (result, Is.TypeOf<ObjectInitializationContext> ());
      Assert.That (result.ObjectID, Is.EqualTo (objectID));
      Assert.That (result.BindingTransaction, Is.SameAs (bindingTransaction));
      Assert.That (((ObjectInitializationContext) result).EnlistedDomainObjectManager, Is.SameAs (_enlistedDomainObjectManagerStub));
      Assert.That (((ObjectInitializationContext) result).DataManager, Is.SameAs (_dataManagerStub));
    }

    [Test]
    public void CreateContext_NullBindingTransaction ()
    {
      var objectID = DomainObjectIDs.Order1;

      var result = _provider.CreateContext (objectID, null);

      Assert.That (result.BindingTransaction, Is.Null);
    }

    [Test]
    public void Serializable ()
    {
      var provider = new ObjectInitializationContextProvider (new SerializableEnlistedDomainObjectManagerFake(), new SerializableDataManagerFake());

      var deserializedInstance = Serializer.SerializeAndDeserialize (provider);

      Assert.That (deserializedInstance.EnlistedDomainObjectManager, Is.Not.Null);
      Assert.That (deserializedInstance.DataManager, Is.Not.Null);
    }
  }
}