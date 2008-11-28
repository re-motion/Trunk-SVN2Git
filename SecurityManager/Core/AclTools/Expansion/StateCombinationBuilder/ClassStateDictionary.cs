// 
//  Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
// 
//  This program is free software: you can redistribute it and/or modify it under 
//  the terms of the re:motion license agreement in license.txt. If you did not 
//  receive it, please visit http://www.re-motion.org/licensing.
//  
//  Unless otherwise provided, this software is distributed on an "AS IS" basis, 
//  WITHOUT WARRANTY OF ANY KIND, either express or implied. 
// 
// 
using System;
using System.Collections.Generic;
using System.Text;
using Remotion.Data.DomainObjects;
using Remotion.Diagnostics.ToText;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.AclTools.Expansion.StateCombinationBuilder
{
  // TODO AE: Remove unused class.
  public class ClassStateDictionary 
  {
    private readonly SortedDictionary<ClassStateTuple, AccessControlList> _classStateToAclMap = new SortedDictionary<ClassStateTuple, AccessControlList>();

    public ClassStateDictionary () : this (SecurableClassDefinition.FindAll ()) { }

    public ClassStateDictionary (IEnumerable<SecurableClassDefinition> classes)
    {
      ArgumentUtility.CheckNotNull ("classes", classes);

      // Initalizes the ClassStateDictionary with the access control lists (ACLs) for each 
      // (SecurableClassDefinition, StateCombination)-combination of the passed classes collection. 
      // After initialization use the GetACL-methods to retrieve
      // the ACL that applies to a given (SecurableClassDefinition, StateCombination)-combination.

      // Create an aclFinder to enable us to query for ACLs for a given (class,state)-combination
      AccessControlListFinder aclFinder = new AccessControlListFinder ();

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
          aclSecurityContextHelper.SetStates (stateCombination.GetStates ());

          // Retrieve the ACL for this (class,state)-combination
          AccessControlList acl = aclFinder.Find (ClientTransactionScope.CurrentTransaction, secuarableClass, aclSecurityContextHelper);

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