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
using Remotion.Data.DomainObjects.DomainImplementation;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Infrastructure
{
  [TestFixture]
  public class HiearchyInvalidationClientTransactionListenerTest : StandardMappingTest
  {
    private ClientTransaction _ancestor1;
    private ClientTransaction _ancestor2;
    private ClientTransaction _subTransaction;
    
    private HiearchyInvalidationClientTransactionListener _listener;

    public override void SetUp ()
    {
      base.SetUp();

      _ancestor1 = ClientTransaction.CreateRootTransaction ();
      _ancestor2 = _ancestor1.CreateSubTransaction();
      _subTransaction = _ancestor2.CreateSubTransaction ();

      _listener = new HiearchyInvalidationClientTransactionListener ();
    }

    [Test]
    public void DataContainerMapRegistering_MarksNewObjectsInvalid ()
    {
      var dataContainer = DataContainer.CreateNew (DomainObjectIDs.Order1);
      var domainObject = LifetimeService.GetObjectReference (_subTransaction, dataContainer.ID);
      dataContainer.SetDomainObject (domainObject);

      _listener.DataContainerMapRegistering (_subTransaction, dataContainer);

      Assert.That (_ancestor1.IsInvalid (DomainObjectIDs.Order1), Is.True);
      Assert.That (ClientTransactionTestHelper.CallGetInvalidObjectReference (_ancestor1, DomainObjectIDs.Order1), Is.SameAs (domainObject));
      
      Assert.That (_ancestor2.IsInvalid (DomainObjectIDs.Order1), Is.True);
      Assert.That (ClientTransactionTestHelper.CallGetInvalidObjectReference (_ancestor2, DomainObjectIDs.Order1), Is.SameAs (domainObject));

      Assert.That (_subTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void DataContainerMapRegistering_IgnoresNonNewObjects ()
    {
      var dataContainer = DataContainer.CreateForExisting (DomainObjectIDs.Order1, null, pd => pd.DefaultValue);
      var domainObject = LifetimeService.GetObjectReference (_subTransaction, dataContainer.ID);
      dataContainer.SetDomainObject (domainObject);

      _listener.DataContainerMapRegistering (_subTransaction, dataContainer);

      Assert.That (_ancestor1.IsInvalid (DomainObjectIDs.Order1), Is.False);
      Assert.That (_ancestor2.IsInvalid (DomainObjectIDs.Order1), Is.False);
      Assert.That (_subTransaction.IsInvalid (DomainObjectIDs.Order1), Is.False);
    }

    [Test]
    public void Serialization ()
    {
      Serializer.SerializeAndDeserialize (_listener);
    }
  }
}