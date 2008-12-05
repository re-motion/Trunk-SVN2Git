// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
