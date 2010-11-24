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
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Integration;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  [TestFixture]
  public class PropertyNotFoundRelationEndPointDefinitionTest
  {
    private PropertyNotFoundRelationEndPointDefinition _propertyNotFoundEndPointDefinition;
    private ReflectionBasedClassDefinition _classDefinition;

    [SetUp]
    public void SetUp ()
    {
      _classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (typeof (ClassDerivedFromSimpleDomainObject));
      _propertyNotFoundEndPointDefinition = new PropertyNotFoundRelationEndPointDefinition (_classDefinition, "TestProperty");
    }

    [Test]
    public void Initialization ()
    {
      Assert.That (_propertyNotFoundEndPointDefinition.ClassDefinition, Is.SameAs (_classDefinition));
      Assert.That (_propertyNotFoundEndPointDefinition.PropertyName, Is.EqualTo ("TestProperty"));
      Assert.That (_propertyNotFoundEndPointDefinition.IsVirtual, Is.False);
      Assert.That (_propertyNotFoundEndPointDefinition.IsAnonymous, Is.False);
      Assert.That (_propertyNotFoundEndPointDefinition.IsPropertyInfoResolved, Is.False);
    }

    [Test]
    public void SetRelationDefinition ()
    {
      var endPoint = new AnonymousRelationEndPointDefinition (_classDefinition);
      var relationDefinition = new RelationDefinition ("Test", endPoint, endPoint);

      _propertyNotFoundEndPointDefinition.SetRelationDefinition (relationDefinition);

      Assert.That (_propertyNotFoundEndPointDefinition.RelationDefinition, Is.SameAs (relationDefinition));
    }

  }
}