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

namespace Remotion.Mixins.Context.Serialization
{
  public class AttributeMixinContextSerializer : IMixinContextSerializer
  {
    private readonly object[] _values = new object[4];

    public object[] Values
    {
      get { return _values; }
    }

    public void AddMixinType (Type mixinType)
    {
      ArgumentUtility.CheckNotNull ("mixinType", mixinType);
      Values[0] = mixinType;
    }

    public void AddMixinKind(MixinKind mixinKind)
    {
      Values[1] = mixinKind;
    }

    public void AddIntroducedMemberVisibility(MemberVisibility introducedMemberVisibility)
    {
      Values[2] = introducedMemberVisibility;
    }

    public void AddExplicitDependencies(IEnumerable<Type> explicitDependencies)
    {
      Values[3] = EnumerableUtility.ToArray (explicitDependencies);
    }
  }
}