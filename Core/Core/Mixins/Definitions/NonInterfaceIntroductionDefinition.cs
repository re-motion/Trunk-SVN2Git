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
using System.Diagnostics;
using Remotion.Utilities;

namespace Remotion.Mixins.Definitions
{
  [DebuggerDisplay ("{FullName}, not introduced by {Implementer.FullName}")]
  public class NonInterfaceIntroductionDefinition : IVisitableDefinition
  {
    public NonInterfaceIntroductionDefinition (Type type, MixinDefinition implementer, bool explicitSuppression)
    {
      ArgumentUtility.CheckNotNull ("type", type);
      ArgumentUtility.CheckNotNull ("implementer", implementer);

      InterfaceType = type;
      Implementer = implementer;
      IsExplicitlySuppressed = explicitSuppression;
    }

    public Type InterfaceType { get; private set; }
    public MixinDefinition Implementer { get; private set; }
    public bool IsExplicitlySuppressed { get; private set; }

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
      get { return InterfaceType.FullName; }
    }

    public IVisitableDefinition Parent
    {
      get { return Implementer; }
    }
  }
}
