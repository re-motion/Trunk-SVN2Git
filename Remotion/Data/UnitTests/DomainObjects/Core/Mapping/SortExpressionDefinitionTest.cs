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
  public class SortExpressionDefinitionTest : StandardMappingTest
  {
    private ClassDefinition _orderItemClassDefinition;
    private PropertyDefinition _productPropertyDefinition;
    private PropertyDefinition _positionPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _orderItemClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      _productPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Product");
      _positionPropertyDefinition = _orderItemClassDefinition.GetMandatoryPropertyDefinition (typeof (OrderItem).FullName + ".Position");
    }

    [Test]
    public new void ToString ()
    {
      var sortExpressionDefinition =
          new SortExpressionDefinition (
              new[]
              {
                  new SortExpressionDefinition.SortedProperty (_productPropertyDefinition, SortExpressionDefinition.SortOrder.Ascending),
                  new SortExpressionDefinition.SortedProperty (_positionPropertyDefinition, SortExpressionDefinition.SortOrder.Descending),
              });

      var result = sortExpressionDefinition.ToString();

      var expected =
          "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product ASC, "
          + "Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Position DESC";
      Assert.That (result, Is.EqualTo (expected));
    }
  }
}