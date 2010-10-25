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
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.DomainObjects.Mapping.SortExpressions;
using Remotion.Data.DomainObjects.Persistence.Rdbms;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Persistence.Rdbms
{
  [TestFixture]
  public class SortExpressionSqlGeneratorTest : SqlProviderBaseTest
  {
    private SortExpressionSqlGenerator _generator;
    private ClassDefinition _orderClassDefinition;
    
    private PropertyDefinition _orderNumberPropertyDefinition;
    private PropertyDefinition _deliveryDatePropertyDefinition;
    private PropertyDefinition _customerPropertyDefinition;

    public override void SetUp ()
    {
      base.SetUp ();

      _generator = new SortExpressionSqlGenerator (Provider);
      _orderClassDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (Order));
    
      _orderNumberPropertyDefinition = _orderClassDefinition.GetMandatoryPropertyDefinition (typeof (Order).FullName + ".OrderNumber");
      _deliveryDatePropertyDefinition = _orderClassDefinition.GetMandatoryPropertyDefinition (typeof (Order).FullName + ".DeliveryDate");
      _customerPropertyDefinition = _orderClassDefinition.GetMandatoryPropertyDefinition (typeof (Order).FullName + ".Customer");
    }

    [Test]
    public void GenerateOrderByExpressionString_One_Ascending ()
    {
      var sortExpression = new SortExpressionDefinition (
          new[]
          {
              SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_orderNumberPropertyDefinition)
          });

      var result = _generator.GenerateOrderByExpressionString (sortExpression);

      Assert.That (result, Is.EqualTo ("[OrderNo] ASC"));
    }

    [Test]
    public void GenerateOrderByExpressionString_One_Descending ()
    {
      var sortExpression = new SortExpressionDefinition (
          new[]
          {
              SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending (_orderNumberPropertyDefinition)
          });

      var result = _generator.GenerateOrderByExpressionString (sortExpression);

      Assert.That (result, Is.EqualTo ("[OrderNo] DESC"));
    }

    [Test]
    public void GenerateOrderByExpressionString_Many ()
    {
      var sortExpression = new SortExpressionDefinition (
          new[]
          {
              SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_orderNumberPropertyDefinition),
              SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending (_deliveryDatePropertyDefinition),
              SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_customerPropertyDefinition),
          });

      var result = _generator.GenerateOrderByExpressionString (sortExpression);

      Assert.That (result, Is.EqualTo ("[OrderNo] ASC, [DeliveryDate] DESC, [CustomerID] ASC"));
    }

    [Test]
    public void GenerateOrderByClauseString ()
    {
      var sortExpression = new SortExpressionDefinition (
          new[]
          {
              SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_orderNumberPropertyDefinition),
              SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending (_deliveryDatePropertyDefinition),
              SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_customerPropertyDefinition),
          });

      var result = _generator.GenerateOrderByClauseString (sortExpression);

      Assert.That (result, Is.EqualTo ("ORDER BY [OrderNo] ASC, [DeliveryDate] DESC, [CustomerID] ASC"));
    }

    [Test]
    public void GenerateColumnListString_One ()
    {
      var sortExpression = new SortExpressionDefinition (
          new[]
          {
              SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_orderNumberPropertyDefinition)
          });

      var result = _generator.GenerateColumnListString (sortExpression);

      Assert.That (result, Is.EqualTo ("[OrderNo]"));
    }

    [Test]
    public void GenerateColumnListString_Many ()
    {
      var sortExpression = new SortExpressionDefinition (
          new[]
          {
              SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_orderNumberPropertyDefinition),
              SortExpressionDefinitionObjectMother.CreateSortedPropertyDescending (_deliveryDatePropertyDefinition),
              SortExpressionDefinitionObjectMother.CreateSortedPropertyAscending (_customerPropertyDefinition),
          });

      var result = _generator.GenerateColumnListString (sortExpression);

      Assert.That (result, Is.EqualTo ("[OrderNo], [DeliveryDate], [CustomerID]"));
    }

  }
}