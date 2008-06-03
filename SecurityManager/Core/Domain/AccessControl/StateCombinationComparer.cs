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
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class StateCombinationComparer : IEqualityComparer<StateCombination>
  {
    public bool Equals (StateCombination x, StateCombination y)
    {
      List<StateDefinition> statesX = x.GetStates ();
      List<StateDefinition> statesY = y.GetStates ();

      if (statesX.Count != statesY.Count)
        return false;

      foreach (StateDefinition stateX in statesX)
      {
        if (!statesY.Contains (stateX))
          return false;
      }

      return true;
    }

    public int GetHashCode (StateCombination obj)
    {
      int hashCode = obj.Class.GetHashCode ();

      foreach (StateDefinition state in obj.GetStates ())
        hashCode ^= state.GetHashCode ();

      return hashCode;
    }
  }
}
