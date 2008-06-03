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
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  public class SuppressedInterfaceIntroductionDefinition : IVisitableDefinition
  {
    public readonly Type Type;
    public readonly MixinDefinition Implementer;

    private readonly bool _explicitSuppression;

    public SuppressedInterfaceIntroductionDefinition (Type type, MixinDefinition implementer, bool explicitSuppression)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      Type = type;
      Implementer = implementer;
      _explicitSuppression = explicitSuppression;
    }

    public bool IsExplicitlySuppressed
    {
      get { return _explicitSuppression; }
    }

    public bool IsShadowed
    {
      get { return !IsExplicitlySuppressed; }
    }

    public void Accept (IDefinitionVisitor visitor)
    {
      ArgumentUtility.CheckNotNull ("visitor", visitor);
      visitor.Visit (this);
    }

    public string FullName
    {
      get { return Type.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }
  }
}
