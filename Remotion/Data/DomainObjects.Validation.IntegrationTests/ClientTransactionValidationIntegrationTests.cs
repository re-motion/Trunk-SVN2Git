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
using FluentValidation;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain;

namespace Remotion.Data.DomainObjects.Validation.IntegrationTests
{
  [TestFixture]
  public class ClientTransactionValidationIntegrationTests
  {
    [Test]
    public void RootClientTransaction_ValidDomainObjects_NoValidationExceptionIsThrown ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var order = Order.NewObject();
        order.Number = "O2345";

        var product = Product.NewObject();
        product.Order = order;

        var customer = Customer.NewObject();
        customer.UserName = "test1";

        ClientTransaction.Current.Commit();
      }
    }

    [Test]
    [ExpectedException (typeof (ValidationException), ExpectedMessage = "Validation failed: \r\n "
                                                                        + "-- 'LocalizedNumber' must be between 3 and 8 characters. You entered 2 characters.\r\n "
                                                                        + "-- 'UserName' must not be empty.")]
    public void RootClientTransaction_InvalidDomainObjects_ValidationExceptionIsThrown ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var order = Order.NewObject();
        order.Number = "er";

        var product = Product.NewObject();
        product.Order = order;

        Customer.NewObject();

        ClientTransaction.Current.Commit();
      }
    }

    [Test]
    [ExpectedException (typeof (ValidationException), ExpectedMessage = "Validation failed: \r\n "
                                                                        + "-- 'LocalizedNumber' must be between 3 and 8 characters. You entered 2 characters.\r\n "
                                                                        + "-- 'UserName' must not be empty.")]
    public void SubClientTransaction_InvalidDomainObjects_ValidationExceptionIsThrown ()
    {
      using (ClientTransaction.CreateRootTransaction ().EnterDiscardingScope ())
      {
        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That (ClientTransaction.Current.ParentTransaction, Is.Not.Null);

          var order = Order.NewObject ();
          order.Number = "er";

          Customer.NewObject ();

          ClientTransaction.Current.Commit ();
        }

        throw new InvalidOperationException ("This exception should never be thrown.");
      }
    }
  }
}