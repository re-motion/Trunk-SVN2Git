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
using System.Collections.Generic;

namespace Remotion.Utilities
{
  public struct AttributeWithMetadata
  {
    public static IEnumerable<AttributeWithMetadata> IncludeAll (IEnumerable<AttributeWithMetadata> source, Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      foreach (AttributeWithMetadata attribute in source)
      {
        if (attribute.IsInstanceOfType (attributeType))
          yield return attribute;
      }
    }

    public static IEnumerable<AttributeWithMetadata> ExcludeAll (IEnumerable<AttributeWithMetadata> source, Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      foreach (AttributeWithMetadata attribute in source)
      {
        if (!attribute.IsInstanceOfType (attributeType))
          yield return attribute;
      }
    }

    public static IEnumerable<AttributeWithMetadata> Suppress (IEnumerable<AttributeWithMetadata> source, IEnumerable<AttributeWithMetadata> suppressAttributes)
    {
      ArgumentUtility.CheckNotNull ("source", source);
      ArgumentUtility.CheckNotNull ("suppressAttributes", suppressAttributes);

      AttributeWithMetadata[] suppressAttributesArray = EnumerableUtility.ToArray (suppressAttributes);

      foreach (AttributeWithMetadata attribute in source)
      {
        bool suppressed = false;
        foreach (AttributeWithMetadata suppressAttribute in suppressAttributesArray) // assume that there are only few suppressAttributes, if any
        {
          if (attribute.DeclaringType != suppressAttribute.DeclaringType 
              && attribute.IsInstanceOfType (((SuppressAttributesAttribute)suppressAttribute.AttributeInstance).AttributeBaseType))
            suppressed = true;
        }
        
        if (!suppressed)
          yield return attribute;
      }
    }

    public static IEnumerable<Attribute> ExtractInstances (IEnumerable<AttributeWithMetadata> source)
    {
      foreach (AttributeWithMetadata attribute in source)
        yield return attribute.AttributeInstance;
    }

    private readonly Type _declaringType;
    private readonly Attribute _attribute;

    public AttributeWithMetadata (Type declaringType, Attribute attribute)
    {
      ArgumentUtility.CheckNotNull ("declaringType", declaringType);
      ArgumentUtility.CheckNotNull ("attribute", attribute);

      _declaringType = declaringType;
      _attribute = attribute;
    }

    public Type DeclaringType
    {
      get { return _declaringType; }
    }

    public Attribute AttributeInstance
    {
      get { return _attribute; }
    }

    public bool IsInstanceOfType (Type attributeType)
    {
      ArgumentUtility.CheckNotNull ("attributeType", attributeType);

      return attributeType.IsInstanceOfType (AttributeInstance);
    }
  }
}