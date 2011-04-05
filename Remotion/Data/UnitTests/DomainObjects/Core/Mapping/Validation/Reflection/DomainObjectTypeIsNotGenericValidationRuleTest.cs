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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Mapping.Validation.Reflection;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection.DomainObjectTypeIsNotGenericValidationRule;
using Rhino.Mocks;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Reflection
{
  [TestFixture]
  public class DomainObjectTypeIsNotGenericValidationRuleTest : ValidationRuleTestBase
  {
    private DomainObjectTypeIsNotGenericValidationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new DomainObjectTypeIsNotGenericValidationRule();
    }

    [Test]
    public void NoGenericType ()
    {
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeof (NonGenericTypeDomainObject));
      
      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void IsGenericType_IsDomainObjectBase ()
    {
      var typeStub = MockRepository.GenerateStub<Type> ();
      typeStub.Stub (stub => stub.Name).Return ("Test");
      typeStub.Stub (stub => stub.IsGenericType).Return (true);
      typeStub.Stub (stub => stub.Assembly).Return (typeof (DomainObject).Assembly);
      typeStub.Stub (stub => stub.IsSubclassOf (typeof (DomainObject))).Return (true);

      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (typeStub);
      
      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void IsGenericType_IsNotDomainObjectBase ()
    {
      var type = typeof (GenericTypeDomainObject<string>);
      var classDefinition = ClassDefinitionFactory.CreateClassDefinition (type);
     
      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage = "Generic domain objects are not supported.\r\n\r\n"
        +"Declaring type: Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.Reflection."
        +"DomainObjectTypeIsNotGenericValidationRule.GenericTypeDomainObject`1[System.String]";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }
  
  }
}