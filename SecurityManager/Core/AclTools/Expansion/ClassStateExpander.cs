using System;
using System.Collections.Generic;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion
{
  public class ClassStateExpander //: IClassStateExpander
  {
    private SortedDictionary<ClassStateTuple, AccessControlList> _classStateToAclMap = new SortedDictionary<ClassStateTuple, AccessControlList>();

    public ClassStateExpander ()
    {
      _Init();
    }

    /// <summary>
    /// Initalizes the ClassStateExpander with the access control lists (ACLs) for each 
    /// (SecurableClassDefinition, StateCombination)-combination. 
    /// After initialization use the GetACL-methods to retrieve
    /// the ACL that applies to a given (SecurableClassDefinition, StateCombination)-combination.
    /// </summary>
    private void _Init ()
    {
      _classStateToAclMap.Clear();

      // Create an aclFinder to enable us to query for ACLs for a given (class,state)-combination
      AccessControlListFinder aclFinder = new AccessControlListFinder();

      // Retrieve all class definitions from the DB
      ObjectList<SecurableClassDefinition> classList = SecurableClassDefinition.FindAll();

      // For each of the retrieved class definitions
      foreach (var securableClass in classList)
      {
        // Initialize a OuterProductOfStateProperties-object that generates all possible state combinations
        // of a given class.
        var outerProductOfStateProperties = new OuterProductOfStateProperties (securableClass);

        // Iterate over all state combinations
        foreach (AclSecurityContextHelper aclSecurityContextHelper in outerProductOfStateProperties)
        {
          // Retrieve the ACL for this (class,state)-combination
          AccessControlList acl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, securableClass, aclSecurityContextHelper);

          // Store the retrieved ACL in a map, indexed by (class, state-definition-list)
          _classStateToAclMap.Add (new ClassStateTuple (securableClass, aclSecurityContextHelper.GetStateDefinitionList ()), acl);
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
      return _classStateToAclMap[new ClassStateTuple(securableClass, state.GetStates())];
    }

   
  }
}
