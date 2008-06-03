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
using System.Runtime.Serialization;

namespace Remotion.Data.DomainObjects.Mapping
{
  [Serializable]
  public abstract class SerializableMappingObject : IObjectReference
  {
    public abstract object GetRealObject (StreamingContext context);
    protected abstract bool IsPartOfMapping { get; }
    protected abstract string IDForExceptions { get; }

    [OnSerializing]
    private void CheckWhenSerializing (StreamingContext context)
    {
      if (!IsPartOfMapping)
      {
        string message = string.Format ("The {0} '{1}' cannot be serialized because is is not part of the current mapping.",
            GetType ().Name, IDForExceptions);
        throw new SerializationException (message);
      }
    }
  }
}
