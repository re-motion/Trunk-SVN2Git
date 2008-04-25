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
