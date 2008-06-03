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
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Configuration.StorageProviders
{
  public class TypeProvider
  {
    private Dictionary<Type, object> _supportedTypes = new Dictionary<Type, object>();

    public TypeProvider()
    {      
      _supportedTypes.Add (typeof (bool), null);
      _supportedTypes.Add (typeof (byte), null);
      _supportedTypes.Add (typeof (DateTime), null);
      _supportedTypes.Add (typeof (decimal), null);
      _supportedTypes.Add (typeof (double), null);
      _supportedTypes.Add (typeof (Enum), null);
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

      return _supportedTypes.ContainsKey (type);
    }

    protected Dictionary<Type, object> SupportedTypes
    {
      get { return _supportedTypes; }
    }
  }
}
