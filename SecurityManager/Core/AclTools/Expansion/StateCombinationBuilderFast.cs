using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  /// <summary>
  /// Creates the outer prodcut of all state-property states of the passed SecurableClassDefinition.
  /// Runtime is currently (2008-08-07) around 20x faster than the LINQ based StateCombinationBuilder implemantation.
  /// </summary>
  
  // Release build:
  // StateCombinationBuilderFast.CalculateOuterProduct3 (numberProperty=8,numberState=4): 195 ms = 0,195 s = 0,00325 min
  // StateCombinationBuilderFast.CalculateOuterProduct4 (numberProperty=8,numberState=4): 189 ms = 0,189 s = 0,00315 min
  // StateCombinationBuilderFast.CalculateOuterProduct5 (numberProperty=8,numberState=4): 166 ms = 0,166 s = 0,00276666666666667 min
  // StateCombinationBuilderFast.CalculateOuterProduct6 (numberProperty=8,numberState=4): 152 ms = 0,152 s = 0,00253333333333333 min
  // StateCombinationBuilder.CreatePropertyProduct (numberProperty=8,numberState=4): 3958 ms = 3,958 s = 0,0659666666666667 min

  public class StateCombinationBuilderFast : IStateCombinationBuilder //: IEnumerator<AclSecurityContextHelper>, IEnumerable<AclSecurityContextHelper>
  {
    public SecurableClassDefinition ClassDefinition
    {
      get { return _securableClass; }
    }

    public int Count
    {
      get { return _outerProductOfStateProperties.Count; }
    }

    private readonly SecurableClassDefinition _securableClass;
    
    private int[] _aStatePropertyDefinedStateIndex;
    private bool _stateOuterProductHasMore;
    //private AclSecurityContextHelper _aclSecurityContextHelper;

    private List<AclSecurityContextHelper> _outerProductOfStateProperties;

    
    public PropertyStateTuple[][] CreatePropertyProduct ()
    {

      return CalculateOuterProduct3();
    }

    // for variant 6
    private readonly int _stateCombinationCount;
    private readonly StateDefinition[][] _stateDefinitions;
    private readonly int _statePropertyCount;
    
    public StateCombinationBuilderFast (SecurableClassDefinition securableClass)
    {
      _securableClass = securableClass;
      _stateDefinitions = GetStateDefinitionsPerProperty ();
      _statePropertyCount = ClassDefinition.StateProperties.Count;
      _stateCombinationCount = CalcOuterProductNrStateCombinations (securableClass);
      //Console.WriteLine ("_stateCombinationCount=" + _stateCombinationCount);
    }

    public PropertyStateTuple[,] CalculateOuterProduct6 ()
    {
      //Console.WriteLine ("_stateCombinationCount=" + _stateCombinationCount);

      if (_stateCombinationCount == 0)
        return null;

      PropertyStateTuple[,] result = new PropertyStateTuple[_stateCombinationCount, _statePropertyCount];
      int[] currentStatePropertyIndices = new int[_statePropertyCount];

      for (int iCurrentStateCombination = 0; iCurrentStateCombination < _stateCombinationCount; ++iCurrentStateCombination)
      {
        CalculateCurrentCombination(result, iCurrentStateCombination, currentStatePropertyIndices);
        PrepareNextCombination(currentStatePropertyIndices);
      }

      return result;
    }

    private void PrepareNextCombination (int[] statePropertyIndices)
    {
      for (int iStateProperty2 = 0; iStateProperty2 < _statePropertyCount; ++iStateProperty2)
      {
        ++statePropertyIndices[iStateProperty2];
        if (statePropertyIndices[iStateProperty2] < _stateDefinitions[iStateProperty2].Length)
          break;
        else
          statePropertyIndices[iStateProperty2] = 0;
      }
    }

    private void CalculateCurrentCombination (PropertyStateTuple[,] result, int currentStateCombination, int[] statePropertyIndices)
    {
      for (int iStateProperty = 0; iStateProperty < _statePropertyCount; ++iStateProperty)
      {
        var stateProperty = ClassDefinition.StateProperties[iStateProperty];
        var stateDefinitionsForProperty = _stateDefinitions[iStateProperty];
        PropertyStateTuple newTuple = new PropertyStateTuple (stateProperty, stateDefinitionsForProperty[statePropertyIndices[_statePropertyCount - 1 - iStateProperty]]);
        result[currentStateCombination, iStateProperty] = newTuple;
      }
    }

    private StateDefinition[][] GetStateDefinitionsPerProperty ()
    {
      var stateDefinitions = new StateDefinition[ClassDefinition.StateProperties.Count][];
      for (int i = 0; i < stateDefinitions.Length; ++i)
        stateDefinitions[i] = ClassDefinition.StateProperties[i].DefinedStates.ToArray ();
      return stateDefinitions;
    }

    public PropertyStateTuple[,] CalculateOuterProduct5 ()
    {
      var stateProperties = ClassDefinition.StateProperties;

      int nrStateCombinations = CalcOuterProductNrStateCombinations (ClassDefinition);
      int numberStateProperties = stateProperties.Count;
      PropertyStateTuple[,] outerProductOfStateProperties = new PropertyStateTuple[numberStateProperties, nrStateCombinations];

      var stateDefinitions = new StateDefinition[numberStateProperties][];
      for (int i = 0; i < numberStateProperties; ++i)
        stateDefinitions[i] = stateProperties[i].DefinedStates.ToArray ();

      int[] aStatePropertyDefinedStateIndex = null;

      if (nrStateCombinations > 0)
      {
        aStatePropertyDefinedStateIndex = new int[numberStateProperties];
      }

      for (int iStateCombinations = 0; iStateCombinations < nrStateCombinations; ++iStateCombinations)
      {
        for (int iStateProperty = 0; iStateProperty < numberStateProperties; ++iStateProperty)
        {
          var stateProperty = stateProperties[iStateProperty];
          var stateDefinitionsForProperty = stateDefinitions[iStateProperty];
          PropertyStateTuple newTuple = new PropertyStateTuple (stateProperty, stateDefinitionsForProperty[aStatePropertyDefinedStateIndex[numberStateProperties - 1 - iStateProperty]]);
          outerProductOfStateProperties[iStateProperty, iStateCombinations] = newTuple;
        }

        // Do the "next"-step in the for-loop-indices array (aStatePropertyDefinedStateIndex)
        // This also updates stateOuterProductHasMore 
        for (int iStateProperty2 = 0; iStateProperty2 < numberStateProperties; ++iStateProperty2)
        {
          ++aStatePropertyDefinedStateIndex[iStateProperty2];
          if (aStatePropertyDefinedStateIndex[iStateProperty2] < stateDefinitions[iStateProperty2].Length)
          {
            break;
          }
          else
          {
            aStatePropertyDefinedStateIndex[iStateProperty2] = 0;
          }
        }
      }

      return outerProductOfStateProperties;
    }

    public PropertyStateTuple[,] CalculateOuterProduct4 ()
    {
      var stateProperties = ClassDefinition.StateProperties;

      int nrStateCombinations = CalcOuterProductNrStateCombinations (ClassDefinition);
      int numberStateProperties = stateProperties.Count;
      PropertyStateTuple[,] outerProductOfStateProperties = new PropertyStateTuple[nrStateCombinations,numberStateProperties];

      var stateDefinitions = new StateDefinition[numberStateProperties][];
      for (int i = 0; i < numberStateProperties; ++i)
        stateDefinitions[i] = stateProperties[i].DefinedStates.ToArray();

      int[] aStatePropertyDefinedStateIndex = null;

      if (nrStateCombinations > 0)
      {
        aStatePropertyDefinedStateIndex = new int[numberStateProperties];
      }

      for (int iStateCombinations = 0; iStateCombinations < nrStateCombinations; ++iStateCombinations)
        {
          for (int iStateProperty = 0; iStateProperty < numberStateProperties; ++iStateProperty)
          {
            var stateProperty = stateProperties[iStateProperty];
            var stateDefinitionsForProperty = stateDefinitions[iStateProperty];
            PropertyStateTuple newTuple = new PropertyStateTuple (stateProperty, stateDefinitionsForProperty[aStatePropertyDefinedStateIndex[numberStateProperties - 1 - iStateProperty]]);
            outerProductOfStateProperties[iStateCombinations, iStateProperty] = newTuple;
          }

          // Do the "next"-step in the for-loop-indices array (aStatePropertyDefinedStateIndex)
          // This also updates stateOuterProductHasMore 
          for (int iStateProperty2 = 0; iStateProperty2 < numberStateProperties; ++iStateProperty2)
          {
            ++aStatePropertyDefinedStateIndex[iStateProperty2];
            if (aStatePropertyDefinedStateIndex[iStateProperty2] < stateDefinitions[iStateProperty2].Length)
            {
              break;
            }
            else
            {
              aStatePropertyDefinedStateIndex[iStateProperty2] = 0;
            }
          }
        }

      return outerProductOfStateProperties;
    }

    public PropertyStateTuple[][] CalculateOuterProduct3 ()
    {
      var stateProperties = ClassDefinition.StateProperties;

      int nrStateCombinations = CalcOuterProductNrStateCombinations (ClassDefinition);
      int numberStateProperties = stateProperties.Count;
      PropertyStateTuple[][] outerProductOfStateProperties = new PropertyStateTuple[nrStateCombinations][];

      var stateDefinitions = new StateDefinition[numberStateProperties][];
      for (int i = 0; i < numberStateProperties; ++i)
        stateDefinitions[i] = stateProperties[i].DefinedStates.ToArray ();

      int[] aStatePropertyDefinedStateIndex = null;

      if (nrStateCombinations > 0)
      {
        aStatePropertyDefinedStateIndex = new int[numberStateProperties];
      }

      for (int iStateCombinations = 0; iStateCombinations < nrStateCombinations; ++iStateCombinations)
      {
        PropertyStateTuple[] propertyStateTuples = new PropertyStateTuple[numberStateProperties];

        for (int iStateProperty = 0; iStateProperty < numberStateProperties; ++iStateProperty)
        {
          var stateProperty = stateProperties[iStateProperty];
          var stateDefinitionsForProperty = stateDefinitions[iStateProperty];
          //propertyStateTuples[iStateProperty] = new PropertyStateTuple (stateProperty, stateProperty.DefinedStates[aStatePropertyDefinedStateIndex[iStateProperty]]);
          propertyStateTuples[iStateProperty] = new PropertyStateTuple (stateProperty, stateDefinitionsForProperty[aStatePropertyDefinedStateIndex[numberStateProperties - 1 - iStateProperty]]);
        }

        outerProductOfStateProperties[iStateCombinations] = propertyStateTuples;

        // Do the "next"-step in the for-loop-indices array (aStatePropertyDefinedStateIndex)
        // This also updates stateOuterProductHasMore 
        for (int iStateProperty2 = 0; iStateProperty2 < numberStateProperties; ++iStateProperty2)
        {
          ++aStatePropertyDefinedStateIndex[iStateProperty2];
          if (aStatePropertyDefinedStateIndex[iStateProperty2] < stateDefinitions[iStateProperty2].Length)
          {
            break;
          }
          else
          {
            aStatePropertyDefinedStateIndex[iStateProperty2] = 0;
          }
        }
      }

      return outerProductOfStateProperties;
    }


    public PropertyStateTuple[][] CalculateOuterProduct2 ()
    {
      int iStateCombinations = 0;
      var stateProperties = ClassDefinition.StateProperties;

      PropertyStateTuple[][] outerProductOfStateProperties = new PropertyStateTuple[CalcOuterProductNrStateCombinations (ClassDefinition)][];

      // To avoid using _stateOuterProductHasMore, the following line needs to be enabled.  
      // (Note however that this leads to less stable code due to a dependency on the order the processing of the _aStatePropertyDefinedStateIndex array):
      // int iStatePropertyLast = _aStatePropertyDefinedStateIndex.Length - 1;
      // if (_aStatePropertyDefinedStateIndex[iStatePropertyLast] < stateProperties[iStatePropertyLast].DefinedStates.Count)

      _stateOuterProductHasMore = false;
      int nStateCombinations = CalcOuterProductNrStateCombinations (ClassDefinition);
      if (nStateCombinations > 0)
      {
        _aStatePropertyDefinedStateIndex = new int[ClassDefinition.StateProperties.Count];
        _stateOuterProductHasMore = true;
      }

      while (_stateOuterProductHasMore)
      {
        int numberStateProperties = stateProperties.Count;
        PropertyStateTuple[] propertyStateTuples = new PropertyStateTuple[numberStateProperties];

        for (int iStateProperty = 0; iStateProperty < numberStateProperties; ++iStateProperty)
        {
          var stateProperty = stateProperties[iStateProperty];
          //Log("stateProperty.Name={0}, iStateProperty={1}, stateProperty={2}", stateProperty.Name, iStateProperty, stateProperty);
          propertyStateTuples[iStateProperty] = new PropertyStateTuple (stateProperty, stateProperty.DefinedStates[_aStatePropertyDefinedStateIndex[iStateProperty]]);
        }
        //Log ("aclSecurityContextHelper={0}", _aclSecurityContextHelper);

        outerProductOfStateProperties[iStateCombinations] = propertyStateTuples;
        ++iStateCombinations;

        // Do the "next"-step in the for-loop-indices array (_aStatePropertyDefinedStateIndex)
        // This also updates _stateOuterProductHasMore 
        _stateOuterProductHasMore = false;
        for (int iStateProperty2 = 0; iStateProperty2 < _aStatePropertyDefinedStateIndex.Length; ++iStateProperty2)
        {
          ++_aStatePropertyDefinedStateIndex[iStateProperty2];
          if (_aStatePropertyDefinedStateIndex[iStateProperty2] < stateProperties[iStateProperty2].DefinedStates.Count)
          {
            _stateOuterProductHasMore = true;
            break;
          }
          else
            _aStatePropertyDefinedStateIndex[iStateProperty2] = 0;
        }
      }

      return outerProductOfStateProperties;
    }



    private void CalculateOuterProduct ()
    {
      var stateProperties = ClassDefinition.StateProperties;

      _outerProductOfStateProperties = new List<AclSecurityContextHelper>();

      // To avoid using _stateOuterProductHasMore, the following line needs to be enabled.  
      // (Note however that this leads to less stable code due to a dependency on the order the processing of the _aStatePropertyDefinedStateIndex array):
      // int iStatePropertyLast = _aStatePropertyDefinedStateIndex.Length - 1;
      // if (_aStatePropertyDefinedStateIndex[iStatePropertyLast] < stateProperties[iStatePropertyLast].DefinedStates.Count)

      _stateOuterProductHasMore = false;
      int nStateCombinations = CalcOuterProductNrStateCombinations (ClassDefinition);
      if (nStateCombinations > 0)
      {
        _aStatePropertyDefinedStateIndex = new int[ClassDefinition.StateProperties.Count];
        _stateOuterProductHasMore = true;
      }

      while (_stateOuterProductHasMore)
      {
        // Create a AclSecurityContextHelper containing the state definitions referenced from the current for-loop-indices array (_aStatePropertyDefinedStateIndex)
        var aclSecurityContextHelper = new AclSecurityContextHelper (ClassDefinition.Name);
        for (int iStateProperty = 0; iStateProperty < stateProperties.Count; ++iStateProperty)
        {
          var stateProperty = stateProperties[iStateProperty];
          //Log("stateProperty.Name={0}, iStateProperty={1}, stateProperty={2}", stateProperty.Name, iStateProperty, stateProperty);
          aclSecurityContextHelper.AddState (stateProperty, stateProperty.DefinedStates[_aStatePropertyDefinedStateIndex[iStateProperty]]);
        }
        //_aclSecurityContextHelper = aclSecurityContextHelper;
        //Log ("aclSecurityContextHelper={0}", _aclSecurityContextHelper);

        _outerProductOfStateProperties.Add (aclSecurityContextHelper);

        // Do the "next"-step in the for-loop-indices array (_aStatePropertyDefinedStateIndex)
        // This also updates _stateOuterProductHasMore 
        _stateOuterProductHasMore = false;
        for (int iStateProperty2 = 0; iStateProperty2 < _aStatePropertyDefinedStateIndex.Length; ++iStateProperty2)
        {
          ++_aStatePropertyDefinedStateIndex[iStateProperty2];
          if (_aStatePropertyDefinedStateIndex[iStateProperty2] < stateProperties[iStateProperty2].DefinedStates.Count)
          {
            _stateOuterProductHasMore = true;
            break;
          }
          else
            _aStatePropertyDefinedStateIndex[iStateProperty2] = 0;
        }
      }
    }


    //public IEnumerator GetEnumerator ()
    //{
    //  //IEnumerator<int> items = (IEnumerator<int>) new[] {1, 2, 3}.GetEnumerator();
    //  //foreach (int i in items)

    //  foreach (var aclSecurityContextHelper in _outerProductOfStateProperties)
    //    yield return aclSecurityContextHelper;
    //}

    //public string ToTestString ()
    //{
    //  string s = "";
    //  foreach (var aclSecurityContextHelper in _outerProductOfStateProperties)
    //    s += Environment.NewLine + aclSecurityContextHelper.ToTestString();
    //  return s;
    //}


    public static int CalcOuterProductNrStateCombinations (SecurableClassDefinition classDefinition)
    {
      var stateProperties = classDefinition.StateProperties;
      if (stateProperties.Count <= 0)
      {
        return 0;
      }
      else
      {
        int nStateCombinations = 1;
        foreach (var statePropertyDefinition in stateProperties)
        {
          int nDefinedStates = statePropertyDefinition.DefinedStates.Count;
          Assertion.IsTrue (nDefinedStates > 0);
          nStateCombinations *= nDefinedStates;
        }
        return nStateCombinations;
      }
    }


    private void Log (string format, params Object[] variables)
    {
      Console.WriteLine (String.Format (format, variables));
    }
  }
}