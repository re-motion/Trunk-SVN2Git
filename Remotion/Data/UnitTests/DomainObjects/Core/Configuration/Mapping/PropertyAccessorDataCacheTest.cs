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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Data.UnitTests.DomainObjects.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Configuration.Mapping
{
  [TestFixture]
  public class PropertyAccessorDataCacheTest : StandardMappingTest
  {
    private PropertyAccessorDataCache _propertyAccessorDataCache;

    public override void SetUp ()
    {
      base.SetUp ();
      _propertyAccessorDataCache = new PropertyAccessorDataCache (DomainObjectIDs.Order1.ClassDefinition);
    }

    [Test]
    public void GetPropertyAccessorData_ValueProperty ()
    {
      var data = _propertyAccessorDataCache.GetPropertyAccessorData (typeof (Order) + ".OrderNumber");

      Assert.That (data, Is.Not.Null);
      Assert.That (data.PropertyIdentifier, Is.EqualTo (typeof (Order) + ".OrderNumber"));
      Assert.That (data.Kind, Is.EqualTo (PropertyKind.PropertyValue));
    }

    [Test]
    public void GetPropertyAccessorData_RelatedObjectProperty_RealSide ()
    {
      var data = _propertyAccessorDataCache.GetPropertyAccessorData (typeof (Order) + ".Customer");

      Assert.That (data, Is.Not.Null);
      Assert.That (data.PropertyIdentifier, Is.EqualTo (typeof (Order) + ".Customer"));
      Assert.That (data.Kind, Is.EqualTo (PropertyKind.RelatedObject));
      Assert.That (data.RelationEndPointDefinition.IsVirtual, Is.False);
    }

    [Test]
    public void GetPropertyAccessorData_RelatedObjectProperty_VirtualSide ()
    {
      var data = _propertyAccessorDataCache.GetPropertyAccessorData (typeof (Order) + ".OrderTicket");

      Assert.That (data, Is.Not.Null);
      Assert.That (data.PropertyIdentifier, Is.EqualTo (typeof (Order) + ".OrderTicket"));
      Assert.That (data.Kind, Is.EqualTo (PropertyKind.RelatedObject));
      Assert.That (data.RelationEndPointDefinition.IsVirtual, Is.True);
    }

    [Test]
    public void GetPropertyAccessorData_RelatedCollectionProperty ()
    {
      var data = _propertyAccessorDataCache.GetPropertyAccessorData (typeof (Order) + ".OrderItems");

      Assert.That (data, Is.Not.Null);
      Assert.That (data.PropertyIdentifier, Is.EqualTo (typeof (Order) + ".OrderItems"));
      Assert.That (data.Kind, Is.EqualTo (PropertyKind.RelatedObjectCollection));
      Assert.That (data.RelationEndPointDefinition.IsVirtual, Is.True);
    }

    [Test]
    public void GetPropertyAccessorData_Unknown ()
    {
      var data = _propertyAccessorDataCache.GetPropertyAccessorData (typeof (Order) + ".OrderSmell");

      Assert.That (data, Is.Null);
    }

    [Test]
    public void GetPropertyAccessorData_ItemsAreCached ()
    {
      var data1 = _propertyAccessorDataCache.GetPropertyAccessorData (typeof (Order) + ".OrderNumber");
      var data2 = _propertyAccessorDataCache.GetPropertyAccessorData (typeof (Order) + ".OrderNumber");

      Assert.That (data1, Is.Not.Null);
      Assert.That (data1, Is.SameAs (data2));
    }
  }
}