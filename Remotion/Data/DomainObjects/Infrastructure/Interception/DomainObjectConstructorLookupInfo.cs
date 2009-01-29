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
    private readonly Type _publicDomainObjectType;

    public DomainObjectConstructorLookupInfo (Type publicDomainObjectType, Type constructedType, BindingFlags bindingFlags)
      : base (ArgumentUtility.CheckNotNull ("constructedType", constructedType), bindingFlags)
    {
      ArgumentUtility.CheckNotNull ("publicDomainObjectType", publicDomainObjectType);

      _publicDomainObjectType = publicDomainObjectType;
    }

    public Type PublicDomainObjectType
    {
      get { return _publicDomainObjectType; }
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
                                        _publicDomainObjectType.FullName, SeparatedStringBuilder.Build (", ", parameterTypes, t => t.FullName)); 
        throw new MissingMethodException (message, ex);
      }
    }
  }
}
