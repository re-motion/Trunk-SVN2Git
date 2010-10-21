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
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class SortExpressionParserTest : StandardMappingTest
  {
    private SortExpressionParser _parser;
    private ClassDefinition _orderItemClassDefinition;
    private PropertyDefinition _productPropertyDefinition;
    private PropertyDefinition _positionPropertyDefinition;
    private PropertyDefinition _orderPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      _productPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Product");
      _positionPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Position");
      _orderPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Order");

      _parser = new SortExpressionParser (_orderItemClassDefinition, MappingConfiguration.Current.NameResolver);
    }

    [Test]
    public void Parse_Empty ()
    {
      var sortExpression = "";

      var result = _parser.Parse (sortExpression);

      Assert.That (result.SortedProperties, Is.Empty);
    }

    [Test]
    public void Parse_WithFullIdentifier ()
    {
      var sortExpression = "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product";

      var result = _parser.Parse (sortExpression);

      var expected = new[] { CreateSortedPropertyAscending (_productPropertyDefinition) };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    public void Parse_WithShortPropertyName ()
    {
      var sortExpression = "Product";

      var result = _parser.Parse (sortExpression);

      var expected = new[] { CreateSortedPropertyAscending (_productPropertyDefinition) };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    public void Parse_WithRealRelationEndPoint ()
    {
      var sortExpression = "Order";

      var result = _parser.Parse (sortExpression);

      var expected = new[] { CreateSortedPropertyAscending (_orderPropertyDefinition) };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "SortExpression 'OrderTicket' cannot be parsed: The property 'Remotion.Data.UnitTests.DomainObjects.TestDomain.Order.OrderTicket' is a "
        + "virtual relation end point. SortExpressions can only contain relation end points if the object to be sorted contains the foreign key.")]
    public void Parse_WithVirtualRelationEndPoint ()
    {
      var orderClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
      var parser = new SortExpressionParser (orderClassDefinition, MappingConfiguration.Current.NameResolver);

      var sortExpression = "OrderTicket";

      parser.Parse (sortExpression);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage =
        "SortExpression 'UnknownProduct' cannot be parsed: 'UnknownProduct' is not a valid mapped property name. Expected a full property identifier "
        + "or a property name that can be resolved by performing Reflection on the 'OrderItem' class.")]
    public void Parse_WithUnknownPropertyName ()
    {
      var sortExpression = "UnknownProduct";

      _parser.Parse (sortExpression);
    }

    [Test]
    public void Parse_WithOrderSpecification_Ascending ()
    {
      var sortExpression = "Product asc";

      var result = _parser.Parse (sortExpression);

      var expected = new[] { CreateSortedPropertyAscending (_productPropertyDefinition) };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    public void Parse_WithOrderSpecification_Descending ()
    {
      var sortExpression = "Product desc";

      var result = _parser.Parse (sortExpression);

      var expected = new[] { CreateSortedPropertyDescending (_productPropertyDefinition) };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    public void Parse_WithOrderSpecification_CaseIsIgnored ()
    {
      var sortExpression = "Product dEsC";

      var result = _parser.Parse (sortExpression);

      var expected = new[] { CreateSortedPropertyDescending (_productPropertyDefinition) };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    public void Parse_WithOrderSpecification_MultipleSpaces ()
    {
      var sortExpression = "Product  desc";

      var result = _parser.Parse (sortExpression);

      var expected = new[] { CreateSortedPropertyDescending (_productPropertyDefinition) };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "SortExpression 'Product unknown' cannot be parsed: 'unknown' is not a valid sort order. Expected 'asc' or 'desc'.")]
    public void Parse_WithOrderSpecification_Unknown ()
    {
      var sortExpression = "Product unknown";

      _parser.Parse (sortExpression);
    }

    [Test]
    [ExpectedException (typeof (MappingException), ExpectedMessage = 
        "SortExpression 'Product asc asc' cannot be parsed: Expected one or two parts (a property name and an optional identifier), found 3 parts "
        + "instead.")]
    public void Parse_WithTooManyWords ()
    {
      var sortExpression = "Product asc asc";

      _parser.Parse (sortExpression);
    }

    [Test]
    public void Parse_Many ()
    {
      var sortExpression = "Product asc,Position,Order desc";

      var result = _parser.Parse (sortExpression);

      var expected = new[]
                     {
                         CreateSortedPropertyAscending (_productPropertyDefinition), 
                         CreateSortedPropertyAscending (_positionPropertyDefinition),
                         CreateSortedPropertyDescending (_orderPropertyDefinition)
                     };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    public void Parse_Many_Space ()
    {
      var sortExpression = "Product asc, Position, Order desc";

      var result = _parser.Parse (sortExpression);

      var expected = new[]
                     {
                         CreateSortedPropertyAscending (_productPropertyDefinition), 
                         CreateSortedPropertyAscending (_positionPropertyDefinition),
                         CreateSortedPropertyDescending (_orderPropertyDefinition)
                     };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    public void Parse_Many_TrailingComma ()
    {
      var sortExpression = "Product asc,Position,Order desc,";

      var result = _parser.Parse (sortExpression);

      var expected = new[]
                     {
                         CreateSortedPropertyAscending (_productPropertyDefinition), 
                         CreateSortedPropertyAscending (_positionPropertyDefinition),
                         CreateSortedPropertyDescending (_orderPropertyDefinition)
                     };
      Assert.That (result.SortedProperties, Is.EqualTo (expected));
    }

    [Test]
    public void Parse_RoundTrip ()
    {
      var sortExpression = "Product asc, Position, Order desc";

      var result1 = _parser.Parse (sortExpression);
      var result2 = _parser.Parse (result1.ToString ());

      var expected = new[]
                     {
                         CreateSortedPropertyAscending (_productPropertyDefinition), 
                         CreateSortedPropertyAscending (_positionPropertyDefinition),
                         CreateSortedPropertyDescending (_orderPropertyDefinition)
                     };
      Assert.That (result2.SortedProperties, Is.EqualTo (expected));
    }

    private SortExpressionDefinition.SortedProperty CreateSortedPropertyAscending (PropertyDefinition productPropertyDefinition)
    {
      return new SortExpressionDefinition.SortedProperty (productPropertyDefinition, SortExpressionDefinition.SortOrder.Ascending);
    }

    private SortExpressionDefinition.SortedProperty CreateSortedPropertyDescending (PropertyDefinition productPropertyDefinition)
    {
      return new SortExpressionDefinition.SortedProperty (productPropertyDefinition, SortExpressionDefinition.SortOrder.Descending);
    }
  }
}