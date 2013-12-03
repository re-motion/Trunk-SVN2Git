// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Remotion.Collections;
using Remotion.Utilities;

namespace Remotion.Globalization.Implementation
{
  public class ResourceDefinition
  {
    private readonly Type _type;
    private readonly IResourcesAttribute[] _ownAttributes;
    private readonly List<Tuple<Type, IResourcesAttribute[]>> _supplementingAttributes = new List<Tuple<Type, IResourcesAttribute[]>> ();

    public ResourceDefinition (Type type, IResourcesAttribute[] ownAttributes)
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

    public IResourcesAttribute[] OwnAttributes
    {
      get { return _ownAttributes; }
    }

    public ReadOnlyCollection<Tuple<Type, IResourcesAttribute[]>> SupplementingAttributes
    {
      get { return _supplementingAttributes.AsReadOnly(); }
    }

    public bool HasResources
    {
      get { return OwnAttributes.Length > 0 || _supplementingAttributes.Count > 0; }
    }

    public IEnumerable<Tuple<Type, IResourcesAttribute[]>> GetAllAttributePairs ()
    {
      if (OwnAttributes.Length > 0)
        yield return new Tuple<Type, IResourcesAttribute[]> (Type, OwnAttributes);
      foreach (Tuple<Type, IResourcesAttribute[]> supplementingAttributes in SupplementingAttributes)
        yield return supplementingAttributes;
    }


    public void AddSupplementingAttributes (Type type, IResourcesAttribute[] attributes)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNullOrEmpty ("attributes", attributes);

      _supplementingAttributes.Add (Tuple.Create (type, attributes));
    }

    public void AddSupplementingAttributes (IEnumerable<Tuple<Type, IResourcesAttribute[]>> attributePairs)
    {
      ArgumentUtility.CheckNotNull ("attributePairs", attributePairs);

      foreach (Tuple<Type, IResourcesAttribute[]> pair in attributePairs)
        AddSupplementingAttributes (pair.Item1, pair.Item2);
    }
  }
}
