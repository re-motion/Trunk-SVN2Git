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
using Remotion.Data.DomainObjects;
using Remotion.Security;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.AccessControl
{
  public class AccessControlListFinder : IAccessControlListFinder
  {
    /// <exception cref="AccessControlException">
    ///   The <see cref="SecurableClassDefinition"/> is not found.<br/>- or -<br/>
    ///   A matching <see cref="AccessControlList"/> is not found.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and a <see cref="StatePropertyDefinition"/> is missing.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and contains an invalid state for a <see cref="StatePropertyDefinition"/>.
    /// </exception>
    public AccessControlList Find (ClientTransaction transaction, ISecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("context", context);

      SecurableClassDefinition classDefinition;
      using (transaction.EnterNonDiscardingScope())
      {
        classDefinition = SecurableClassDefinition.FindByName (context.Class);
      }
      if (classDefinition == null)
        throw CreateAccessControlException ("The securable class '{0}' cannot be found.", context.Class);

      return Find (transaction, classDefinition, context);
    }

    /// <exception cref="AccessControlException">
    ///   A matching <see cref="AccessControlList"/> is not found.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and a <see cref="StatePropertyDefinition"/> is missing.<br/>- or -<br/>
    ///   <paramref name="context"/> is not state-less and contains an invalid state for a <see cref="StatePropertyDefinition"/>.
    /// </exception>
    public AccessControlList Find (ClientTransaction transaction, SecurableClassDefinition classDefinition, ISecurityContext context)
    {
      ArgumentUtility.CheckNotNull ("transaction", transaction);
      ArgumentUtility.CheckNotNull ("classDefinition", classDefinition);
      ArgumentUtility.CheckNotNull ("context", context);

      using (transaction.EnterNonDiscardingScope())
      {
        AccessControlList foundAccessControlList = null;

        while (foundAccessControlList == null && classDefinition != null)
        {
          if (context.IsStateless)
          {
            foundAccessControlList = classDefinition.StatelessAccessControlList;
          }
          else
          {
            var foundStateCombination = FindStateCombination (classDefinition, context);
            if (foundStateCombination != null)
              foundAccessControlList = foundStateCombination.AccessControlList;
          }

          classDefinition = classDefinition.BaseClass;
        }

        if (foundAccessControlList == null)
          throw CreateAccessControlException ("The ACL for the securable class '{0}' could not be found.", context.Class);

        return foundAccessControlList;
      }
    }

    private StateCombination FindStateCombination (SecurableClassDefinition classDefinition, ISecurityContext context)
    {
      List<StateDefinition> states = GetStates (classDefinition.StateProperties, context);
      if (states == null)
        return null;

      return classDefinition.FindStateCombination (states);
    }

    private List<StateDefinition> GetStates (IList<StatePropertyDefinition> stateProperties, ISecurityContext context)
    {
      List<StateDefinition> states = new List<StateDefinition>();

      if (context.GetNumberOfStates() > stateProperties.Count)
        return null;

      foreach (var stateProperty in stateProperties)
        states.Add (GetState (stateProperty, context));

      return states;
    }

    private StateDefinition GetState (StatePropertyDefinition property, ISecurityContext context)
    {
      if (!context.ContainsState (property.Name))
        throw CreateAccessControlException ("The state '{0}' is missing in the security context.", property.Name);

      EnumWrapper enumWrapper = context.GetState (property.Name);

      if (!property.ContainsState (enumWrapper.Name))
      {
        throw CreateAccessControlException (
            "The state '{0}' is not defined for the property '{1}' of the securable class '{2}' or its base classes.",
            enumWrapper.Name,
            property.Name,
            context.Class);
      }

      return property.GetState (enumWrapper.Name);
    }

    private AccessControlException CreateAccessControlException (string message, params object[] args)
    {
      return new AccessControlException (string.Format (message, args));
    }
  }
}