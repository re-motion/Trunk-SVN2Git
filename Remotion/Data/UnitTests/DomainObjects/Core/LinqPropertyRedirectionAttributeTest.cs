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
using System.Linq.Expressions;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.Core.Linq;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core
{
  [TestFixture]
  public class LinqPropertyRedirectionAttributeTest
  {
    [Test]
    public void Initialization ()
    {
      var attribute = new LinqPropertyRedirectionAttribute (typeof (Order), "OrderNumber");
      
      var expected = Is.EqualTo (typeof (Order).GetProperty ("OrderNumber"));
      Assert.That (attribute.GetMappedProperty(), expected);
    }

    [Test]
    public void Initialization_NonPublicProperty ()
    {
      var attribute = new LinqPropertyRedirectionAttribute (typeof (ClassWithNonPublicProperties), "PrivateGetSet");
      
      var expected = typeof (ClassWithNonPublicProperties).GetProperty ("PrivateGetSet", BindingFlags.NonPublic | BindingFlags.Instance);
      Assert.That (attribute.GetMappedProperty(), Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "The member redirects LINQ queries to 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Hugo', which does not exist.")]
    public void Initialization_NonExistingProperty ()
    {
      new LinqPropertyRedirectionAttribute (typeof (Order), "Hugo").GetMappedProperty ();
    }

    [Test]
    public void GetTransformer ()
    {
      var attribute = new LinqPropertyRedirectionAttribute (typeof (Order), "OrderNumber");

      var transformer = attribute.GetTransformer ();

      Assert.That (transformer, Is.TypeOf (typeof (LinqPropertyRedirectionAttribute.MethodCallTransformer)));
      Assert.That (((LinqPropertyRedirectionAttribute.MethodCallTransformer) transformer).MappedProperty, 
          Is.SameAs (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    [ExpectedException (typeof (InvalidOperationException), ExpectedMessage =
        "The member redirects LINQ queries to 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.Hugo', which does not exist.")]
    public void GetTransformer_NonExistingProperty ()
    {
      var attribute = new LinqPropertyRedirectionAttribute (typeof (Order), "Hugo");
      attribute.GetTransformer ();
    }

    [Test]
    public void MethodCallTransformer_Transform ()
    {
      var instance = Expression.Constant (null, typeof (Order));
      var call = Expression.Call (instance, typeof (Order).GetProperty ("RedirectedOrderNumber").GetGetMethod());

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer (typeof (Order).GetProperty ("OrderNumber"));
      var result = transformer.Transform (call);

      var expected = Expression.MakeMemberAccess (instance, typeof (Order).GetProperty ("OrderNumber"));
      ExpressionTreeComparer.CheckAreEqualTrees (expected, result);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage = 
        "The method call 'null.get_Position()' cannot be redirected to the property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderNumber'. Property 'Int32 OrderNumber' is not defined for type "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem'")]
    public void MethodCallTransformer_Transform_InvalidType ()
    {
      var instance = Expression.Constant (null, typeof (OrderItem));
      var call = Expression.Call (instance, typeof (OrderItem).GetProperty ("Position").GetGetMethod ());

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer (typeof (Order).GetProperty ("OrderNumber"));
      transformer.Transform (call);
    }

    [Test]
    [ExpectedException (typeof (NotSupportedException), ExpectedMessage =
        "The method call 'null.get_OrderNumber()' cannot be redirected to the property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.DeliveryDate'. The property has an incompatible return type.")]
    public void MethodCallTransformer_Transform_RedirectionToPropertyWithOtherReturnType ()
    {
      var instance = Expression.Constant (null, typeof (Order));
      var call = Expression.Call (instance, typeof (Order).GetProperty ("OrderNumber").GetGetMethod ());

      var transformer = new LinqPropertyRedirectionAttribute.MethodCallTransformer (typeof (Order).GetProperty ("DeliveryDate"));
      transformer.Transform (call);
    }

    [Test]
    public void GetTargetProperty_NonRedirected ()
    {
      var property = typeof (Order).GetProperty ("OrderNumber");

      var redirected = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      Assert.That (redirected, Is.SameAs (property));
    }

    [Test]
    public void GetTargetProperty_SimpleRedirected ()
    {
      var property = typeof (Order).GetProperty ("RedirectedOrderNumber");

      var redirected = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      Assert.That (redirected, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    public void GetTargetProperty_TwiceRedirected ()
    {
      var property = typeof (Order).GetProperty ("RedirectedRedirectedOrderNumber");

      var redirected = LinqPropertyRedirectionAttribute.GetTargetProperty (property);

      Assert.That (redirected, Is.EqualTo (typeof (Order).GetProperty ("OrderNumber")));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.SelfRedirected': The member redirects LINQ queries "
        + "to itself.")]
    public void GetTargetProperty_InfiniteRedirection ()
    {
      var property = typeof (ClassWithInvalidRedirectedProperties).GetProperty ("SelfRedirected");

      LinqPropertyRedirectionAttribute.GetTargetProperty (property);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.RedirectedToNonexistent': The member redirects LINQ "
        + "queries to 'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.Nonexistent', which does not exist.")]
    public void GetTargetProperty_RedirectionToNonExistentProperty ()
    {
      var property = typeof (ClassWithInvalidRedirectedProperties).GetProperty ("RedirectedToNonexistent");

      LinqPropertyRedirectionAttribute.GetTargetProperty (property);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.RedirectedToPropertyWithOtherType': The member "
        + "redirects LINQ queries to the property "
        + "'Remotion.Data.UnitTests.DomainObjects.TestDomain.ClassWithInvalidRedirectedProperties.PropertyWithOtherType', which has a different "
        + "return type.")]
    public void GetTargetProperty_RedirectionToPropertyWithOtherType ()
    {
      var property = typeof (ClassWithInvalidRedirectedProperties).GetProperty ("RedirectedToPropertyWithOtherType");

      LinqPropertyRedirectionAttribute.GetTargetProperty (property);
    }
  }
}