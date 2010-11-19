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
using Remotion.Data.DomainObjects.Mapping.Validation.Persistence;
using Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation;
using Remotion.Development.UnitTesting;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.Persistence
{
  [TestFixture]
  public class StorageProviderIDMatchesParentStorageProviderIDValiadationRuleTest : ValidationRuleTestBase
  {
    private StorageProviderIDMatchesParentStorageProviderIDValiadationRule _validationRule;

    [SetUp]
    public void SetUp ()
    {
      _validationRule = new StorageProviderIDMatchesParentStorageProviderIDValiadationRule();
    }

    [Test]
    public void SameStorageProviderID ()
    {
      var baseType = typeof (BaseValidationDomainObjectClass);
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (baseType.Name, baseType.Name, "SPID", baseType, false);
      var type = typeof (DerivedValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false, baseClassDefinition, new Type[0]);

      var validationResult = _validationRule.Validate (classDefinition);

      AssertMappingValidationResult (validationResult, true, null);
    }

    [Test]
    public void DifferentStorageProviderID ()
    {
      var baseType = typeof (BaseValidationDomainObjectClass);
      var baseClassDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (baseType.Name, baseType.Name, "SPID", baseType, false);
      var type = typeof (DerivedValidationDomainObjectClass);
      var classDefinition = ClassDefinitionFactory.CreateReflectionBasedClassDefinition (type.Name, type.Name, "SPID", type, false, baseClassDefinition, new Type[0]);
      PrivateInvoke.SetNonPublicField (classDefinition, "_storageProviderID", "SPID_NEW");

      var validationResult = _validationRule.Validate (classDefinition);

      var expectedMessage =
          "Cannot derive class 'DerivedValidationDomainObjectClass' from base class 'BaseValidationDomainObjectClass' handled by different StorageProviders."
          + "\r\n\r\nDeclaring type: 'Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Validation.DerivedValidationDomainObjectClass'";
      AssertMappingValidationResult (validationResult, false, expectedMessage);
    }


  }
}