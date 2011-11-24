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
using NUnit.Framework;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MappingReflectionIntegrationTests.RelationsAboveInheritanceRoot
{
  [TestFixture]
  public class When_TwoClasses_InheritARelation_FromAboveInheritanceRoot : MappingReflectionIntegrationTestBase
  {
    [Test]
    public void TheDerivedClasses_ShouldGetSeparateRelationDefinitions_WithDifferentIDs ()
    {
      var derivedClass1 = ClassDefinitions[typeof (DerivedInheritanceRootClass1)];
      var derivedClass2 = ClassDefinitions[typeof (DerivedInheritanceRootClass2)];

      var endPointInDerivedClass1 = GetRelationEndPointDefinition (derivedClass1, typeof (AboveInheritanceRootClassWithRelation), "RelationClass");
      var endPointInDerivedClass2 = GetRelationEndPointDefinition (derivedClass2, typeof (AboveInheritanceRootClassWithRelation), "RelationClass");

      Assert.That (endPointInDerivedClass1, Is.Not.Null);
      Assert.That (endPointInDerivedClass2, Is.Not.Null);
      Assert.That (endPointInDerivedClass1.RelationDefinition, Is.Not.SameAs (endPointInDerivedClass2.RelationDefinition));

      Assert.That (
          endPointInDerivedClass1.RelationDefinition.ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MappingReflectionIntegrationTests.RelationsAboveInheritanceRoot.DerivedInheritanceRootClass1:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MappingReflectionIntegrationTests.RelationsAboveInheritanceRoot.AboveInheritanceRootClassWithRelation.RelationClass"));
      Assert.That (
          endPointInDerivedClass2.RelationDefinition.ID,
          Is.EqualTo (
              "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MappingReflectionIntegrationTests.RelationsAboveInheritanceRoot.DerivedInheritanceRootClass2:"
              + "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.MappingReflectionIntegrationTests.RelationsAboveInheritanceRoot.AboveInheritanceRootClassWithRelation.RelationClass"));
    }
  }
}