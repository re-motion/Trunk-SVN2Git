// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Reflection;
using System.Runtime.Serialization;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  public class ReflectionBasedVirtualRelationEndPointDefinition : VirtualRelationEndPointDefinition
  {
    [NonSerialized]
    private readonly PropertyInfo _propertyInfo;

    public ReflectionBasedVirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        string propertyTypeName,
        string sortExpression,
        PropertyInfo propertyInfo)
        : base (classDefinition, propertyName, isMandatory, cardinality, propertyTypeName, sortExpression)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
    }

    public ReflectionBasedVirtualRelationEndPointDefinition (
        ClassDefinition classDefinition,
        string propertyName,
        bool isMandatory,
        CardinalityType cardinality,
        Type propertyType,
        string sortExpression,
        PropertyInfo propertyInfo)
      : base (classDefinition, propertyName, isMandatory, cardinality, propertyType, sortExpression)
    {
      ArgumentUtility.CheckNotNull ("propertyInfo", propertyInfo);
      _propertyInfo = propertyInfo;
    }

    public PropertyInfo PropertyInfo
    {
      get { return _propertyInfo; }
    }
  }
}
