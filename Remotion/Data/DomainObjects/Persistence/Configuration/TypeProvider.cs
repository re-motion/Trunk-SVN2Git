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

namespace Remotion.Data.DomainObjects.Persistence.Configuration
{
  public class TypeProvider
  {
    private readonly Dictionary<Type, object> _supportedTypes = new Dictionary<Type, object>();

    public TypeProvider()
    {      
      _supportedTypes.Add (typeof (bool), null);
      _supportedTypes.Add (typeof (byte), null);
      _supportedTypes.Add (typeof (DateTime), null);
      _supportedTypes.Add (typeof (decimal), null);
      _supportedTypes.Add (typeof (double), null);
      _supportedTypes.Add (typeof (Enum), null);
      _supportedTypes.Add (typeof (IExtensibleEnum), null);
      _supportedTypes.Add (typeof (Guid), null);
      _supportedTypes.Add (typeof (short), null);
      _supportedTypes.Add (typeof (int), null);
      _supportedTypes.Add (typeof (long), null);
      _supportedTypes.Add (typeof (float), null);
      _supportedTypes.Add (typeof (string), null);
      _supportedTypes.Add (typeof (ObjectID), null);
      _supportedTypes.Add (typeof (byte[]), null);
    }

    public bool IsTypeSupported (Type type)
    {
      ArgumentUtility.CheckNotNull ("type", type);

      if (type.IsEnum)
        type = typeof (Enum);

      if (ExtensibleEnumUtility.IsExtensibleEnumType (type))
        type = typeof (IExtensibleEnum);

      return _supportedTypes.ContainsKey (type);
    }

    // TODO: Remove
    protected Dictionary<Type, object> SupportedTypes
    {
      get { return _supportedTypes; }
    }
  }
}
