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
using Remotion.Data.DomainObjects.Persistence.Rdbms.Model;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class PropertyDefinitionTest : StandardMappingTest
  {
    private ReflectionBasedClassDefinition _classDefinition;

    public override void SetUp ()
    {
      base.SetUp();

      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (
          "Order", "OrderTable", TestDomainStorageProviderDefinition, typeof (Order), false);
    }

    [Test]
    public void GetToString ()
    {
      PropertyDefinition propertyDefinition = new TestablePropertyDefinition (_classDefinition, "ThePropertyName", null, StorageClass.None);

      Assert.That (propertyDefinition.ToString (), Is.EqualTo (typeof (TestablePropertyDefinition).FullName + ": ThePropertyName"));
    }

    [Test]
    public void SetStorageProperty ()
    {
      PropertyDefinition propertyDefinition = new TestablePropertyDefinition (_classDefinition, "ThePropertyName", null, StorageClass.Persistent);
      var columnDefinition = new SimpleColumnDefinition ("Test", typeof (string), "varchar", true, false);

      propertyDefinition.SetStorageProperty (columnDefinition);

      Assert.That (propertyDefinition.StoragePropertyDefinition, Is.SameAs (columnDefinition));
    }
  }
}