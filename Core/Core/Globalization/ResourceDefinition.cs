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
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  public class ResourceDefinition<TAttribute>
      where TAttribute : Attribute, IResourcesAttribute
  {
    private readonly Type _type;
    private readonly TAttribute[] _ownAttributes;
    private readonly List<Tuple<Type, TAttribute[]>> _supplementingAttributes = new List<Tuple<Type, TAttribute[]>>();

    public ResourceDefinition (Type type, TAttribute[] ownAttributes)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("ownAttributes", ownAttributes);
      _type = type;
      _ownAttributes = ownAttributes;
    }

    public Type Type
    {
      get { return _type; }
    }

    public TAttribute[] OwnAttributes
    {
      get { return _ownAttributes; }
    }

    public IEnumerable<Tuple<Type, TAttribute[]>> SupplementingAttributes
    {
      get { return _supplementingAttributes; }
    }

    public bool HasResources
    {
      get { return OwnAttributes.Length > 0 || _supplementingAttributes.Count > 0; }
    }

    public IEnumerable<Tuple<Type, TAttribute[]>> GetAllAttributePairs()
    {
      if (OwnAttributes.Length > 0)
        yield return new Tuple<Type, TAttribute[]> (Type, OwnAttributes);
      foreach (Tuple<Type, TAttribute[]> supplementingAttributes in SupplementingAttributes)
        yield return supplementingAttributes;
    }


    public void AddSupplementingAttributes (Type type, TAttribute[] attributes)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("attributes", attributes);

      _supplementingAttributes.Add (Tuple.NewTuple (type, attributes));
    }

    public void AddSupplementingAttributes (IEnumerable<Tuple<Type, TAttribute[]>> attributePairs)
    {
      ArgumentUtility.CheckNotNull ("attributePairs", attributePairs);

      foreach (Tuple<Type, TAttribute[]> pair in attributePairs)
        AddSupplementingAttributes (pair.A, pair.B);
    }
  }
}
