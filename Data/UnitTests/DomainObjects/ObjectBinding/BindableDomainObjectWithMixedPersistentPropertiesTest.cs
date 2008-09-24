/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using System.Linq;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding
{
  [TestFixture]
  public class BindableDomainObjectWithMixedPersistentPropertiesTest : ObjectBindingBaseTest
  {
    [Test]
    public void MixedProperty_Exists ()
    {
      var instance = BindableDomainObjectWithMixedPersistentProperties.NewObject();
      IBusinessObject instanceAsBusinessObject = instance;
      var boClass = instanceAsBusinessObject.BusinessObjectClass;

      Assert.That (boClass.GetPropertyDefinitions ().Select (p => p.Identifier ).ToArray(),
          List.Contains ("MixedProperty"));
    }

    [Test]
    public void MixedProperty_DefaultValue ()
    {
      var instance = BindableDomainObjectWithMixedPersistentProperties.NewObject ();
      IBusinessObject instanceAsBusinessObject = instance;
      var boClass = instanceAsBusinessObject.BusinessObjectClass;

      IBusinessObjectProperty mixedProperty = boClass.GetPropertyDefinition ("MixedProperty");
      Assert.That (instanceAsBusinessObject.GetProperty (mixedProperty), Is.Null);
    }

    [Test]
    public void MixedProperty_NonDefaultValue ()
    {
      var instance = BindableDomainObjectWithMixedPersistentProperties.NewObject ();
      IBusinessObject instanceAsBusinessObject = instance;
      var boClass = instanceAsBusinessObject.BusinessObjectClass;

      IBusinessObjectProperty mixedProperty = boClass.GetPropertyDefinition ("MixedProperty");
      var dateTime = new DateTime(2008, 08, 01);
      ((IMixinAddingPersistentProperties) instance).MixedProperty = dateTime;
      Assert.That (instanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }

    [Test]
    public void MixedProperty_NonDefaultValue_WithUnchangedValue ()
    {
      var instance = BindableDomainObjectWithMixedPersistentProperties.NewObject ();
      IBusinessObject instanceAsBusinessObject = instance;
      var boClass = instanceAsBusinessObject.BusinessObjectClass;

      IBusinessObjectProperty mixedProperty = boClass.GetPropertyDefinition ("MixedProperty");
      var dateTime = ((IMixinAddingPersistentProperties) instance).MixedProperty;
      ((IMixinAddingPersistentProperties) instance).MixedProperty = dateTime;
      Assert.That (instanceAsBusinessObject.GetProperty (mixedProperty), Is.EqualTo (dateTime));
    }
  }
}