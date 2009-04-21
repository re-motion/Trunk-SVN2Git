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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Data.UnitTests.DomainObjects.ObjectBinding.TestDomain;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.BindableObject;
using Remotion.ObjectBinding.BindableObject.Properties;

namespace Remotion.Data.UnitTests.DomainObjects.ObjectBinding.BindableDomainObjectMixinTests
{
  [TestFixture]
  public class DomainObjectSpecificsTest : ObjectBindingBaseTest
  {
    private BindableObjectClass _businessObjectClassWithProperties;
    private BindableObjectClass _businessObjectSampleClass;

    public override void SetUp ()
    {
      base.SetUp ();
      _businessObjectClassWithProperties = BindableObjectProvider.GetBindableObjectClass (typeof (BindableDomainObjectWithProperties));
      _businessObjectSampleClass = BindableObjectProvider.GetBindableObjectClass (typeof (SampleBindableMixinDomainObject));
    }

    [Test]
    public void OrdinaryProperty ()
    {
      Assert.IsTrue (_businessObjectSampleClass.HasPropertyDefinition ("Name"));
    }

    [Test]
    public void UsesBindableDomainObjectMetadataFactory ()
    {
      Assert.That (
        BindableObjectProvider.GetProviderForBindableObjectType(typeof (SampleBindableMixinDomainObject)).MetadataFactory,
        Is.InstanceOfType (typeof (BindableDomainObjectMetadataFactory)));
    }

    [Test]
    public void NoIDProperty ()
    {
      Assert.IsFalse (_businessObjectSampleClass.HasPropertyDefinition ("ID"));
    }

    [Test]
    public void NoPropertyFromDomainObject ()
    {
      PropertyBase[] properties = (PropertyBase[]) _businessObjectSampleClass.GetPropertyDefinitions ();

      foreach (PropertyBase property in properties)
        Assert.AreNotEqual (typeof (DomainObject), property.PropertyInfo.DeclaringType);
    }

    [Test]
    public void PropertyNotInMapping ()
    {
      Assert.IsTrue (_businessObjectClassWithProperties.HasPropertyDefinition ("RequiredPropertyNotInMapping"));
    }

    [Test]
    public void PropertyInMapping ()
    {
      Assert.IsTrue (_businessObjectClassWithProperties.HasPropertyDefinition ("RequiredStringProperty"));
    }

    [Test]
    public void ProtectedPropertyInMapping ()
    {
      Assert.IsFalse (_businessObjectClassWithProperties.HasPropertyDefinition ("ProtectedStringProperty"));
    }

    [Test]
    public void Requiredness ()
    {
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredPropertyNotInMapping").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredStringProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredValueProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredEnumProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredRelatedObjectProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredBidirectionalRelatedObjectProperty").IsRequired);
      Assert.IsTrue (_businessObjectClassWithProperties.GetPropertyDefinition ("RequiredBidirectionalRelatedObjectsProperty").IsRequired);

      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredPropertyNotInMapping").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredStringProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredValueProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredEnumProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredUndefinedEnumProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredRelatedObjectProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredBidirectionalRelatedObjectProperty").IsRequired);
      Assert.IsFalse (_businessObjectClassWithProperties.GetPropertyDefinition ("NonRequiredBidirectionalRelatedObjectsProperty").IsRequired);
    }

    [Test]
    public void MaxLength ()
    {
      Assert.AreEqual (7, ((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("MaxLength7StringProperty")).MaxLength);

      Assert.IsNull (((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("NoMaxLengthStringPropertyNotInMapping")).MaxLength);
      Assert.IsNull (((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("NoMaxLengthStringProperty")).MaxLength);
    }

    [Test]
    public void InheritanceAndOverriding ()
    {
      Assert.IsTrue (_businessObjectClassWithProperties.HasPropertyDefinition ("BasePropertyWithMaxLength3"));
      Assert.IsTrue (_businessObjectClassWithProperties.HasPropertyDefinition ("BasePropertyWithMaxLength4"));

      Assert.AreEqual (33, ((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength3")).MaxLength);
      Assert.AreEqual (4, ((IBusinessObjectStringProperty)
          _businessObjectClassWithProperties.GetPropertyDefinition ("BasePropertyWithMaxLength4")).MaxLength);
    }
  }
}
