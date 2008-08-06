using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Tuple of security context class and state, that supports the necessary interfaces to be stored in a Dictionary (<see cref="ClassStateExpander"/>)
  /// </summary>
  public class ClassStateTuple : Tuple<SecurableClassDefinition, List<StateDefinition>>, IComparable<ClassStateTuple>
  {
    public ClassStateTuple (SecurableClassDefinition classDefinition, List<StateDefinition> stateCombinationList) :
      base (classDefinition, stateCombinationList.GetRange (0, stateCombinationList.Count))
    { 
      
    }

    public ClassStateTuple(SecurableClassDefinition classDefinition, StateCombination stateCombination) : 
      base(classDefinition, new List<StateDefinition> (stateCombination.GetStates()))
    {
      //StateList.Sort()
    }

    /// <summary>
    /// Returns the SecurableClassDefinition tuple member.
    /// </summary>
    public SecurableClassDefinition Class 
    {
      get { return this.A; }
    }

    /// <summary>
    /// Returns the StateCombination tuple member.
    /// </summary>
    public List<StateDefinition> StateList
    {
      get { return this.B; }
    }


    public int CompareTo (ClassStateTuple other)
    {
      //throw new System.NotImplementedException();

      int comparedClass = Class.Name.CompareTo (other.Class.Name);

      if (comparedClass != 0)
      {
        return comparedClass;
      }

      int comparedNumberOfStates = StateList.Count.CompareTo (other.StateList.Count);
      if (comparedNumberOfStates != 0)
      {
        return comparedNumberOfStates;
      }

      // !!! The following assumes a sorted StateList !!!
      //for (int i = 0; i < StateList.Count; ++i )
      //{
      //  int comparedState = StateList[i].Name.CompareTo (other.StateList[i].Name);
      //  if (comparedState != 0)
      //  {
      //    return comparedState;
      //  }
      //}

      // The following implementation is very inefficient and is only a placeholder until
      // StateList will automatically be filled sorted as soon as states have a sort order.
      for (int i = 0; i < StateList.Count; ++i)
      {
        for (int i2 = 0; i2 < other.StateList.Count; ++i2)
        {
          int comparedState = StateList[i].Name.CompareTo (other.StateList[i2].Name);
          if (comparedState != 0)
          {
            return comparedState;
          }
        }
      }

      return 0;
    }


    override public string ToString ()
    {
      return string.Format ("({0},{1})", Class.Name, StateList.ToString ());
    }
  
  }
}