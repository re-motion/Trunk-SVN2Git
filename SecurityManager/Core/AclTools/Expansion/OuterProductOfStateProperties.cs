﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  // Create outer prodcut of all state-property states of the passed SecurableClassDefinition
  public class OuterProductOfStateProperties //: IEnumerator<AclSecurityContextHelper>, IEnumerable<AclSecurityContextHelper>
  {
    public SecurableClassDefinition SecurableClass
    {
      get { return _securableClass; }
      set
      {
        _securableClass = value;
        CalculateOuterProduct ();
      }
    }

    public int Count
    {
      get { return _outerProductOfStateProperties.Count; }
    }

    private SecurableClassDefinition _securableClass;
    private int[] _aStatePropertyDefinedStateIndex;
    private bool _stateOuterProductHasMore;
    //private AclSecurityContextHelper _aclSecurityContextHelper;

    private List<AclSecurityContextHelper> _outerProductOfStateProperties;
    //private List<List<PropertyStateTuple>> _outerProductOfStateProperties2;


    public OuterProductOfStateProperties (SecurableClassDefinition securableClass)
    {
      _securableClass = securableClass;
      CalculateOuterProduct ();
    }



    public PropertyStateTuple[][] CalculateOuterProduct3 ()
    {
      var stateProperties = SecurableClass.StateProperties;

      int nrStateCombinations = CalcOuterProductNrStateCombinations (SecurableClass);
      int numberStateProperties = stateProperties.Count;
      PropertyStateTuple[][] outerProductOfStateProperties = new PropertyStateTuple[nrStateCombinations][];

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
      var stateProperties = SecurableClass.StateProperties;

      //_outerProductOfStateProperties = new List<AclSecurityContextHelper> ();
      //_outerProductOfStateProperties2 = new List<List<PropertyStateTuple>>();

      PropertyStateTuple[][] outerProductOfStateProperties = new PropertyStateTuple[CalcOuterProductNrStateCombinations (SecurableClass)][];

      // To avoid using _stateOuterProductHasMore, the following line needs to be enabled.  
      // (Note however that this leads to less stable code due to a dependency on the order the processing of the _aStatePropertyDefinedStateIndex array):
      // int iStatePropertyLast = _aStatePropertyDefinedStateIndex.Length - 1;
      // if (_aStatePropertyDefinedStateIndex[iStatePropertyLast] < stateProperties[iStatePropertyLast].DefinedStates.Count)

      _stateOuterProductHasMore = false;
      int nStateCombinations = CalcOuterProductNrStateCombinations (SecurableClass);
      if (nStateCombinations > 0)
      {
        _aStatePropertyDefinedStateIndex = new int[SecurableClass.StateProperties.Count];
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
      var stateProperties = SecurableClass.StateProperties;

      _outerProductOfStateProperties = new List<AclSecurityContextHelper>();

      // To avoid using _stateOuterProductHasMore, the following line needs to be enabled.  
      // (Note however that this leads to less stable code due to a dependency on the order the processing of the _aStatePropertyDefinedStateIndex array):
      // int iStatePropertyLast = _aStatePropertyDefinedStateIndex.Length - 1;
      // if (_aStatePropertyDefinedStateIndex[iStatePropertyLast] < stateProperties[iStatePropertyLast].DefinedStates.Count)

      _stateOuterProductHasMore = false;
      int nStateCombinations = CalcOuterProductNrStateCombinations (SecurableClass);
      if (nStateCombinations > 0)
      {
        _aStatePropertyDefinedStateIndex = new int[SecurableClass.StateProperties.Count];
        _stateOuterProductHasMore = true;
      }

      while (_stateOuterProductHasMore)
      {
        // Create a AclSecurityContextHelper containing the state definitions referenced from the current for-loop-indices array (_aStatePropertyDefinedStateIndex)
        var aclSecurityContextHelper = new AclSecurityContextHelper (SecurableClass.Name);
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


    public IEnumerator GetEnumerator ()
    {
      //IEnumerator<int> items = (IEnumerator<int>) new[] {1, 2, 3}.GetEnumerator();
      //foreach (int i in items)

      foreach (var aclSecurityContextHelper in _outerProductOfStateProperties)
        yield return aclSecurityContextHelper;
    }

    public string ToTestString ()
    {
      string s = "";
      foreach (var aclSecurityContextHelper in _outerProductOfStateProperties)
        s += Environment.NewLine + aclSecurityContextHelper.ToTestString();
      return s;
    }


    public static int CalcOuterProductNrStateCombinations (SecurableClassDefinition classDefinition)
    {
      var stateProperties = classDefinition.StateProperties;
      if (stateProperties.Count <= 0)
        return 0;
      else
      {
        int nStateCombinations = 1;
        foreach (var statePropertyDefinition in stateProperties)
        {
          int nDefinedStates = statePropertyDefinition.DefinedStates.Count;
          //Assertion.IsTrue (nDefinedStates > 0);
          nStateCombinations *= nDefinedStates;
        }
        return nStateCombinations;
      }
    }

#if(false)
    #region Implementation of IEnumerator

    public bool MoveNext ()
    {
      return _MoveNext();
    }

    public void Reset ()
    {
      ArgumentUtility.CheckNotNull ("SecurableClass", SecurableClass);
      _stateOuterProductHasMore = false;
      _aclSecurityContextHelper = null;
      int nStateCombinations = CalcOuterProductNrStateCombinations (SecurableClass);
      if (nStateCombinations > 0)
      {
        _aStatePropertyDefinedStateIndex = new int[SecurableClass.StateProperties.Count];
        _stateOuterProductHasMore = true;
      }
    }

    AclSecurityContextHelper IEnumerator<AclSecurityContextHelper>.Current
    {
      get
      {
        return _aclSecurityContextHelper;
      }
    }

    public object Current
    {
      get
      {
        return _aclSecurityContextHelper;
      }
    }


    #endregion

    public void Dispose ()
    {
      // empty
    }


    #region Implementation of IEnumerable

    public IEnumerator<AclSecurityContextHelper> GetEnumerator ()
    {
      return this;
    }

    IEnumerator IEnumerable.GetEnumerator ()
    {
      return this;
    }

    #endregion
#endif

    private void Log (string format, params Object[] variables)
    {
      Console.WriteLine (String.Format (format, variables));
    }
  }
}