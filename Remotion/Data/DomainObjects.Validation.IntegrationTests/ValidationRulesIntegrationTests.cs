﻿// This file is part of the re-motion Core Framework (www.re-motion.org)
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
using System.Linq;
using NUnit.Framework;
using Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain;
using Remotion.Validation;
using Remotion.Validation.Implementation;
using Remotion.Validation.Results;

namespace Remotion.Data.DomainObjects.Validation.IntegrationTests
{
  [TestFixture]
  public class ValidationRulesIntegrationTests : IntegrationTestBase
  {
    public override void SetUp ()
    {
      base.SetUp ();

      ShowLogOutput = false;
    }

    [Test]
    public void BuildValidator_MandatoryReStoreAttributeIsAppliedOnDomainObject ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var orderItem1 = OrderItem.NewObject();
        orderItem1.Order = null;
        orderItem1.ProductReference = ProductReference.NewObject();

        var orderItem2 = OrderItem.NewObject();
        orderItem2.Order = Order.NewObject();
        orderItem2.ProductReference = ProductReference.NewObject();

        var validator = ValidationBuilder.BuildValidator<OrderItem> ();

        var result1 = validator.Validate (orderItem1);
        Assert.That (result1.IsValid, Is.False);
        Assert.That (result1.Errors.Count, Is.EqualTo (1));
        Assert.That (result1.Errors, Is.All.InstanceOf<PropertyValidationFailure>());
        Assert.That (result1.Errors.OfType<PropertyValidationFailure>().First().ValidatedProperty.Name, Is.EqualTo ("Order"));
        Assert.That (result1.Errors.First().ErrorMessage, Is.EqualTo ("The value must not be null."));

        var result2 = validator.Validate (orderItem2);
        Assert.That (result2.IsValid, Is.True);
      }
    }

    [Test]
    public void BuildValidator_MandatoryReStoreAttributeAppliedOnDomainObjectMixin ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var customer1 = Customer.NewObject();
        var customer2 = Customer.NewObject ();
        ((ICustomerIntroduced) customer2).Address = Address.NewObject();

        var validator = ValidationBuilder.BuildValidator<Customer>();

        var result1 = validator.Validate (customer1);
        Assert.That (result1.IsValid, Is.False);
        Assert.That (result1.Errors.Count, Is.EqualTo (1));
        Assert.That (result1.Errors, Is.All.InstanceOf<PropertyValidationFailure>());
        Assert.That (result1.Errors.OfType<PropertyValidationFailure>().First().ValidatedProperty.Name, Is.EqualTo ("Address"));
        Assert.That (result1.Errors.First().ErrorMessage, Is.EqualTo ("The value must not be null."));

        var result2 = validator.Validate (customer2);
        Assert.That (result2.IsValid, Is.True);
      }
    }

    [Test]
    public void BuildValidator_StringPropertyReStoreAttributeIsReplaced_MaxLengthMetaValidationRuleFails ()
    {
      Assert.That (
          () => ValidationBuilder.BuildValidator<InvalidOrder> (),
          Throws.TypeOf<ValidationConfigurationException> ().And.Message.EqualTo (
              "'RemotionMaxLengthPropertyMetaValidationRule' failed for property 'Remotion.Data.DomainObjects.Validation.IntegrationTests.Testdomain.InvalidOrder.Number': "
              + "Max-length validation rule value '15' exceeds meta validation rule max-length value of '10'."));
    }

    [Test]
    public void BuildValidator_NotLoadedCollectionNotValidated_AndDataNotLoaded ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var order1 = Order.NewObject();
        order1.Number = "001";

        var order2 = Order.NewObject();
        order2.Number = "002";

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          Assert.That (order1.OrderItems, Is.Empty);
          Assert.That (order1.OrderItems.IsDataComplete, Is.True);
          Assert.That (order2.OrderItems.IsDataComplete, Is.False);

          var validator = ValidationBuilder.BuildValidator<Order>();

          var result1 = validator.Validate (order1);
          Assert.That (result1.IsValid, Is.False);
          Assert.That (result1.Errors.Count, Is.EqualTo (1));
          Assert.That (result1.Errors, Is.All.InstanceOf<PropertyValidationFailure>());
          Assert.That (result1.Errors.OfType<PropertyValidationFailure>().First().ValidatedProperty.Name, Is.EqualTo ("OrderItems"));
          Assert.That (result1.Errors.First().ErrorMessage, Is.EqualTo ("The value must not be empty."));
          Assert.That (order1.OrderItems.IsDataComplete, Is.True);

          var result2 = validator.Validate (order2);
          Assert.That (result2.IsValid, Is.True);
          Assert.That (order2.OrderItems.IsDataComplete, Is.False);
        }
      }
    }

    [Test]
    public void BuildValidator_NotLoadedReferenceNotValidated_AndDataNotLoaded ()
    {
      using (ClientTransaction.CreateRootTransaction().EnterDiscardingScope())
      {
        var product1 = Product.NewObject();
        var productReference1 = ProductReference.NewObject();
        productReference1.Product = product1;

        var product2 = Product.NewObject();
        var productReference2 = ProductReference.NewObject();
        productReference2.Product = product2;

        using (ClientTransaction.Current.CreateSubTransaction().EnterDiscardingScope())
        {
          productReference1.EnsureDataAvailable();
          Assert.That (productReference1.OrderItem, Is.Null);
          Assert.That (product1.State, Is.EqualTo (StateType.NotLoadedYet));

          productReference2.EnsureDataAvailable();
          Assert.That (product2.State, Is.EqualTo (StateType.NotLoadedYet));

          var validator = ValidationBuilder.BuildValidator<ProductReference>();

          var result1 = validator.Validate (productReference1);
          Assert.That (result1.IsValid, Is.False);
          Assert.That (result1.Errors.Count, Is.EqualTo (1));
          Assert.That (result1.Errors, Is.All.InstanceOf<PropertyValidationFailure>());
          Assert.That (result1.Errors.OfType<PropertyValidationFailure>().First().ValidatedProperty.Name, Is.EqualTo ("OrderItem"));
          Assert.That (result1.Errors.First().ErrorMessage, Is.EqualTo ("The value must not be null."));
          Assert.That (product1.State, Is.EqualTo (StateType.NotLoadedYet));

          var result2 = validator.Validate (productReference2);
          Assert.That (result2.IsValid, Is.True);
          Assert.That (product2.State, Is.EqualTo (StateType.NotLoadedYet));
        }
      }
    }
  }
}