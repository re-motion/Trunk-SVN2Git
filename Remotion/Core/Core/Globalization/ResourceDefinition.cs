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
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

    public ReadOnlyCollection<Tuple<Type, TAttribute[]>> SupplementingAttributes
    {
      get { return _supplementingAttributes.AsReadOnly(); }
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
