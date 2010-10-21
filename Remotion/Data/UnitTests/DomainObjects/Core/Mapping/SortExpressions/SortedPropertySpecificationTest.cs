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
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.SortExpressions
{
  [TestFixture]
  public class SortedPropertySpecificationTest : StandardMappingTest
  {
    [Test]
    public new void ToString ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      var propertyDefinition = classDefinition.GetPropertyDefinition (typeof (OrderItem).FullName + ".Product");

      var specificationAsc = new SortedPropertySpecification (propertyDefinition, SortOrder.Ascending);
      var specificationDesc = new SortedPropertySpecification (propertyDefinition, SortOrder.Descending);

      Assert.That (specificationAsc.ToString (), Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product ASC"));
      Assert.That (specificationDesc.ToString (), Is.EqualTo ("Remotion.Data.UnitTests.DomainObjects.TestDomain.OrderItem.Product DESC"));
    }

    [Test]
    public void Equals ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      var propertyDefinition = classDefinition.GetPropertyDefinition (typeof (OrderItem).FullName + ".Product");
      var propertyDefinition2 = classDefinition.GetPropertyDefinition (typeof (OrderItem).FullName + ".Position");

      var specification1 = new SortedPropertySpecification (propertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification (propertyDefinition, SortOrder.Ascending);
      var specification3 = new SortedPropertySpecification (propertyDefinition, SortOrder.Descending);
      var specification4 = new SortedPropertySpecification (propertyDefinition2, SortOrder.Ascending);

      Assert.That (specification1, Is.EqualTo (specification2));
      Assert.That (specification1, Is.Not.EqualTo (specification3));
      Assert.That (specification1, Is.Not.EqualTo (specification4));
    }

    [Test]
    public new void GetHashCode ()
    {
      var classDefinition = MappingConfiguration.Current.ClassDefinitions.GetMandatory (typeof (OrderItem));
      var propertyDefinition = classDefinition.GetPropertyDefinition (typeof (OrderItem).FullName + ".Product");

      var specification1 = new SortedPropertySpecification (propertyDefinition, SortOrder.Ascending);
      var specification2 = new SortedPropertySpecification (propertyDefinition, SortOrder.Ascending);

      Assert.That (specification1.GetHashCode(), Is.EqualTo (specification2.GetHashCode()));
    }
  }
}