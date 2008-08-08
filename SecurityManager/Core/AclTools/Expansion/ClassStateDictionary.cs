using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class ClassStateDictionary 
  {
    private readonly SortedDictionary<ClassStateTuple, AccessControlList> _classStateToAclMap = new SortedDictionary<ClassStateTuple, AccessControlList>();

    public ClassStateDictionary (IEnumerable<SecurableClassDefinition> classes)
    {
      _Init (classes);
    }

    public ClassStateDictionary () : this(null) {}


    /// <summary>
    /// Initalizes the ClassStateDictionary with the access control lists (ACLs) for each 
    /// (SecurableClassDefinition, StateCombination)-combination of the passed classes collection. 
    /// After initialization use the GetACL-methods to retrieve
    /// the ACL that applies to a given (SecurableClassDefinition, StateCombination)-combination.
    /// </summary>
    private void _Init (IEnumerable<SecurableClassDefinition> classes)
    {
      _classStateToAclMap.Clear ();

      // Create an aclFinder to enable us to query for ACLs for a given (class,state)-combination
      AccessControlListFinder aclFinder = new AccessControlListFinder();

      // Retrieve all class definitions from the DB
      //IEnumerable<SecurableClassDefinition> classes = SecurableClassDefinition.FindAll();
      if (classes == null)
      {
        classes = SecurableClassDefinition.FindAll();
      }

      // For each of the retrieved class definitions
      foreach (var secuarableClass in classes)
      {
        // Initialize a StateCombinationBuilderFast-object that generates all possible state combinations
        // of a given class.
        //var outerProductOfStateProperties = new StateCombinationBuilderFast (securableClass);

        // Iterate over all state combinations
        //foreach (AclSecurityContextHelper aclSecurityContextHelper in outerProductOfStateProperties)
        foreach (var stateCombination in secuarableClass.StateCombinations)
        {
          // Create an AclSecurityContextHelper and initialize it with the StateDefinition|s of 
          // the current foreach StateCombination
          var aclSecurityContextHelper = new AclSecurityContextHelper (secuarableClass.Name);
          aclSecurityContextHelper.SetStates (stateCombination.GetStates());

          // Retrieve the ACL for this (class,state)-combination
          AccessControlList acl = 
            aclFinder.Find (ClientTransactionScope.CurrentTransaction, secuarableClass, aclSecurityContextHelper);

          // Store the retrieved ACL in a map, indexed by (class, state-definition-list)
          _classStateToAclMap.Add (new ClassStateTuple (secuarableClass, aclSecurityContextHelper.GetStateDefinitionList ()), acl);
        } 
      }
    }
    
    /// <summary>
    /// ACL retrieval through ClassStateTuple-key (main funtionality of class).
    /// </summary>
    /// <param name="classStateTuple">tuple containing (SecurableClassDefinition, StateCombination)</param>
    /// <returns>Access control list corresponding to the passed (class,state)-tuple.</returns>
    public AccessControlList GetACL(ClassStateTuple classStateTuple)
    {
      ArgumentUtility.CheckNotNull ("classStateTuple", classStateTuple);
      ArgumentUtility.CheckNotNull ("_classStateToAclMap", _classStateToAclMap);
      return _classStateToAclMap[classStateTuple];
    }


    /// <summary>
    /// ACL retrieval through (SecurableClassDefinition, StateCombination)-key (main funtionality of class).
    /// </summary>
    /// <param name="securableClass"></param>
    /// <param name="state"></param>
    /// <returns>ACL for the passed (SecurableClassDefinition, StateCombination)-tuple</returns>
    public AccessControlList GetACL (SecurableClassDefinition securableClass, StateCombination state)
    {
      ArgumentUtility.CheckNotNull ("class", securableClass);
      ArgumentUtility.CheckNotNull ("state", state);
      ArgumentUtility.CheckNotNull ("_classStateToAclMap", _classStateToAclMap);
      return _classStateToAclMap[new ClassStateTuple(securableClass, new List<StateDefinition>(state.GetStates()))];
    }

    public override string ToString ()
    {
      var stringbuilder = new StringBuilder();
      foreach(var mapEntry in _classStateToAclMap)
      {
        stringbuilder.AppendLine (String.Format ("[{0},{1}]", To.Text(mapEntry.Key), mapEntry.Value));
      }
      return stringbuilder.ToString();
    }
    
  }
}
