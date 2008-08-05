using System;
using System.Collections.Generic;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class AclSecurityContextHelper : ISecurityContext
  {
    private Dictionary<string, EnumWrapper> _states = new Dictionary<string, EnumWrapper> ();
    //protected List<StateDefinition> stateDefinitions = new List<StateDefinition> ();
    //protected StateCombination stateCombination = StateCombination.NewObject ();
    private List<StateDefinition> _stateDefinitionList = new List<StateDefinition>();

    

    public AclSecurityContextHelper (string className)
    {
      Class = className;
    }

    public bool Equals (ISecurityContext other)
    {
      throw new System.NotImplementedException();
    }

    public string Class
    {
      get; set;
    }

    public string Owner
    {
      get { throw new System.NotImplementedException(); }
    }

    public string OwnerGroup
    {
      get { throw new System.NotImplementedException(); }
    }

    public string OwnerTenant
    {
      get { throw new System.NotImplementedException(); }
    }

    public EnumWrapper[] AbstractRoles
    {
      get { throw new System.NotImplementedException(); }
    }

    public EnumWrapper GetState (string propertyName)
    {
      return _states[propertyName];
    }

    public bool ContainsState (string propertyName)
    {
      // TODO MGi: Make sure this is the expected behavior.
      return _states.ContainsKey(propertyName);
    }

    public bool IsStateless
    {
      //get { throw new System.NotImplementedException(); }
      get { return _states.Count == 0; }
    }

    public int GetNumberOfStates ()
    {
      //throw new System.NotImplementedException();
      return _states.Count;
    }


    public void AddState(StatePropertyDefinition statePropertyDefinition, StateDefinition stateDefinition)
    {
      //Log ("!!! {0} !!! {1} !!! {2} !!!", stateDefinition.ShortName(), stateDefinition.Name, stateDefinition.ToString ());
      
      _states.Add (statePropertyDefinition.Name, new EnumWrapper (stateDefinition.Name));
      //stateDefinitions.Add (stateDefinition);
      _stateDefinitionList.Add (stateDefinition);
    }

    /*
    public StateDefinition[] GetStateDefinitions ()
    {
      StateDefinition[] stateDefinitionsArray = new StateDefinition[stateDefinitions.Count];
      stateDefinitions.CopyTo (stateDefinitionsArray);
      return stateDefinitionsArray;
    }

    public StateCombination GetStateCombination ()
    {
      // ...create a StateCombination
      var stateCombination = StateCombination.NewObject();
      foreach (var stateDefinition in this.stateDefinitions)
      {
        stateCombination.AttachState (stateDefinition);
      }
      return stateCombination;
    }
    */

    public List<StateDefinition> GetStateDefinitionList ()
    {
      return _stateDefinitionList;
    }


    public override string ToString ()
    {
      return ToTestString();
    }

    public string ToTestString ()
    {
      string s = "";
      s += "{";
      foreach(var pair in _states)
      {
        string statePropertyDefinitionName = pair.Key;
        string stateDefinitionName = pair.Value.Name;
        s += String.Format ("[{0},{1}]", statePropertyDefinitionName, ShortenName(stateDefinitionName));
      }
      s += "} {";
      foreach (var stateDefinition in _stateDefinitionList)
      {
        s += String.Format ("[{0}]", ShortenName(stateDefinition.Name));
      }
      s += "}";
      return s;
    }


    public string ShortenName (string name)
    {
      return name.LeftUntilChar ('|');
    }


    private void Log (string format, params Object[] variables)
    {
      Console.WriteLine (String.Format (format, variables));
    }
  }

  public static class ExtensionMethods
  {
    public static string LeftUntilChar (this string s, char separator)
    {
      int iSeparator = s.IndexOf (separator);
      if (iSeparator > 0)
      {
        return s.Substring (0, iSeparator);
      }
      else
      {
        return s;
      }
    }

    public static string ShortName (this StateDefinition stateDefinition)
    {
      return stateDefinition.Name.LeftUntilChar ('|');
      //string name = stateDefinition.Name;
      //int iSeparator = name.IndexOf ('|');
      //if(iSeparator > 0)
      //{
      //  return name.Substring (0, iSeparator);
      //}
      //else
      //{
      //  return name;
      //}
    }
  }

}