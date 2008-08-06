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
using Remotion.Collections;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  /// <summary>
  /// A <see cref="StatePropertyDefinition"/> and one of its <see cref="StateDefinition"/> values.
  /// </summary>
  public class PropertyStateTuple : Tuple<StatePropertyDefinition, StateDefinition>
  {
    public PropertyStateTuple (StatePropertyDefinition property, StateDefinition state)
        : base (ArgumentUtility.CheckNotNull ("property", property), state)
    {
    }

    public StatePropertyDefinition Property
    {
      get { return A; }
    }

    public StateDefinition State
    {
      get { return B; }
    }
  }
}