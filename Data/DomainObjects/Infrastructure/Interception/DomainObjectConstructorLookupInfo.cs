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
using Remotion.Reflection;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.Interception
{
  public class DomainObjectConstructorLookupInfo : ConstructorLookupInfo
  {
    private readonly Type _baseType;

    public DomainObjectConstructorLookupInfo (Type baseType, Type definingType, BindingFlags bindingFlags)
        : base (ArgumentUtility.CheckNotNull ("definingType", definingType), bindingFlags)
    {
      ArgumentUtility.CheckNotNull ("baseType", baseType);

      _baseType = baseType;
    }

    public override Delegate GetDelegate (Type delegateType)
    {
      try
      {
        return base.GetDelegate (delegateType);
      }
      catch (MissingMethodException ex)
      {
        Type[] parameterTypes = GetParameterTypes (delegateType);
        string message = string.Format ("Type {0} does not support the requested constructor with signature ({1}).",
                                        _baseType.FullName, SeparatedStringBuilder.Build (", ", parameterTypes, delegate (Type t) { return t.FullName; })); 
        throw new MissingMethodException (message, ex);
      }
    }
  }
}