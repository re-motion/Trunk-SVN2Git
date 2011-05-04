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
using System.Reflection;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Reflection;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping
{
  public class TestablePropertyDefinition : PropertyDefinition
  {
    public TestablePropertyDefinition (ClassDefinition classDefinition, string propertyName, int? maxLength, StorageClass storageClass)
        : base (classDefinition, new PropertyInfoAdapter (typeof(string).GetProperty("Length")), propertyName, typeof(object), false, maxLength, storageClass)
    {
    }
    
    public TestablePropertyDefinition (ClassDefinition classDefinition, PropertyInfo propertyInfo, int? maxLength, StorageClass storageClass)
      : base (classDefinition, new PropertyInfoAdapter (propertyInfo), propertyInfo.Name, propertyInfo.PropertyType, false, maxLength, storageClass)
    {
    }

    public TestablePropertyDefinition (ClassDefinition classDefinition, IPropertyInformation propertyInfo, int? maxLength, StorageClass storageClass)
      : base (classDefinition, propertyInfo, propertyInfo.Name, propertyInfo.PropertyType, false, maxLength, storageClass)
    {
    }
  }
}