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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectPersistence;
using Remotion.Data.DomainObjects.Security;
using Remotion.Data.UnitTests.DomainObjects.Core;
using Remotion.Data.UnitTests.DomainObjects.Security.SecurityClientTransactionExtensionTests;
using Remotion.Security.Configuration;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Security
{
  [TestFixture]
  public class SecurityClientTransactionFactoryTest
  {
    private TestHelper _testHelper;

    [SetUp]
    public void SetUp ()
    {
      _testHelper = new TestHelper ();
    }

    [Test]
    public void CreateRootTransaction_WithEnabledSecurity ()
    {
      ITransactionFactory factory = new SecurityClientTransactionFactory();

      _testHelper.SetupSecurityConfiguration();
      SecurityConfiguration.Current.SecurityProvider.BackToRecord();
      SecurityConfiguration.Current.SecurityProvider.Stub (stub => stub.IsNull).Return (false).Repeat.Any();
      SecurityConfiguration.Current.SecurityProvider.Replay();
      ITransaction transaction;
      try
      {
        Assert.That (SecurityConfiguration.Current.SecurityProvider.IsNull, Is.False);
        transaction = factory.CreateRootTransaction ();
      }
      finally
      {
        _testHelper.TearDownSecurityConfiguration();
      }

      var clientTransaction = transaction.To<ClientTransaction>();
      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy (clientTransaction);
      Assert.That (persistenceStrategy, Is.InstanceOf (typeof (RootPersistenceStrategy)));
      Assert.That (
          clientTransaction.Extensions, 
          Has.Some.InstanceOf (typeof (SecurityClientTransactionExtension))
              .With.Property ("Key").EqualTo (typeof (SecurityClientTransactionExtension).FullName));
    }

    [Test]
    public void CreateRootTransaction_WithDisabledSecurity ()
    {
      ITransactionFactory factory = new SecurityClientTransactionFactory ();

      Assert.That (SecurityConfiguration.Current.SecurityProvider.IsNull, Is.True);
      ITransaction transaction = factory.CreateRootTransaction ();

      var clientTransaction = transaction.To<ClientTransaction> ();
      var persistenceStrategy = ClientTransactionTestHelper.GetPersistenceStrategy (clientTransaction);
      Assert.That (persistenceStrategy, Is.InstanceOf (typeof (RootPersistenceStrategy)));
      Assert.That (clientTransaction.Extensions, Has.No.InstanceOf<SecurityClientTransactionExtension> ());
    }
  }
}
