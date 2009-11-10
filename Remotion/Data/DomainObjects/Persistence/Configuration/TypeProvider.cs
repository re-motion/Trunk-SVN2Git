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
using System.Collections.Generic;
using Remotion.ExtensibleEnums;
using Remotion.Utilities;
using System.Linq;

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public class TypeProvider
  {
    private readonly HashSet<Type> _supportedTypes = new HashSet<Type>();
    private readonly List<Type> _supportedBaseTypes = new List<Type> ();

    public TypeProvider()
    {      
      AddSupportedType (typeof (bool));
      AddSupportedType (typeof (byte));
      AddSupportedType (typeof (DateTime));
      AddSupportedType (typeof (decimal));
      AddSupportedType (typeof (double));
      AddSupportedType (typeof (Guid));
      AddSupportedType (typeof (short));
      AddSupportedType (typeof (int));
      AddSupportedType (typeof (long));
      AddSupportedType (typeof (float));
      AddSupportedType (typeof (string));
      AddSupportedType (typeof (ObjectID));
      AddSupportedType (typeof (byte[]));

      AddSupportedBaseType (typeof (Enum));
      AddSupportedBaseType (typeof (IExtensibleEnum));
    }

    public bool IsTypeSupported (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      return _supportedTypes.Contains (type) 
          || _supportedBaseTypes.Any (t => t != type && t.IsAssignableFrom (type));
    }

    public void AddSupportedType (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      _supportedTypes.Add (type);
    }

    public void AddSupportedBaseType (Type baseTypeOrInterface)
    {
      ArgumentUtility.CheckNotNull ("baseTypeOrInterface", baseTypeOrInterface);

      _supportedBaseTypes.Add (baseTypeOrInterface);
    }

    [Obsolete ("This API is obsolete since version 1.13.34, use IsTypeSupported, AddSupportedType, and AddSupportedBaseType instead.")]
    protected Dictionary<Type, object> SupportedTypes
    {
      get { throw new NotImplementedException ("Obsolete"); }
    }
  }
}
