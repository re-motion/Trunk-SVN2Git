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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Development.UnitTesting;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class ClientTransactionFactoryTest
  {
    [Test]
    public void CreateRootTransaction ()
    {
      ITransactionFactory transactionFactory = new ClientTransactionFactory();

      ITransaction transaction = transactionFactory.CreateRootTransaction();
      Assert.That (transaction, Is.InstanceOfType (typeof (ClientTransactionWrapper)));
      Assert.That (transaction.To<ClientTransaction>(), Is.InstanceOfType (typeof (RootClientTransaction)));
    }

    [Test]
    public void CreateRootTransaction_WithExtension ()
    {
      ITransactionFactory factory = MockRepository.GenerateMock<ClientTransactionFactory>();

      var extensionStub = MockRepository.GenerateStub<IClientTransactionExtension>();
      factory.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnTransactionCreated", Arg<ClientTransaction>.Is.NotNull)).Do (
          invocation => ((ClientTransaction) invocation.Arguments[0]).Extensions.Add ("extension", extensionStub));

      ITransaction transaction = factory.CreateRootTransaction();

      var clientTransaction = transaction.To<ClientTransaction>();
      Assert.That (clientTransaction, Is.InstanceOfType (typeof (RootClientTransaction)));
      Assert.That (clientTransaction.Extensions.Count, Is.EqualTo (1));
      Assert.That (clientTransaction.Extensions[0], Is.SameAs (extensionStub));
    }

    [Test]
    public void CreateChildTransaction_WithExtension ()
    {
      ITransactionFactory factory = MockRepository.GenerateMock<ClientTransactionFactory>();

      var extensionStub = MockRepository.GenerateStub<IClientTransactionExtension>();
      factory.Expect (mock => PrivateInvoke.InvokeNonPublicMethod (mock, "OnTransactionCreated", Arg<ClientTransaction>.Is.NotNull)).Do (
          invocation => ((ClientTransaction) invocation.Arguments[0]).Extensions.Add ("extension", extensionStub));

      ITransaction rootTransaction = factory.CreateRootTransaction();
      ITransaction childTransaction = rootTransaction.CreateChild();

      var clientTransaction = childTransaction.To<ClientTransaction>();
      Assert.That (rootTransaction.To<ClientTransaction>().Extensions[0], Is.SameAs (extensionStub));
      Assert.That (clientTransaction.Extensions, Is.EqualTo (rootTransaction.To<ClientTransaction>().Extensions));
    }
  }
}