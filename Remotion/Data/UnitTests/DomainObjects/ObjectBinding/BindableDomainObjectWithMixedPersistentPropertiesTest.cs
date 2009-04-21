// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
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
