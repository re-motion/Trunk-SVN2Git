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
