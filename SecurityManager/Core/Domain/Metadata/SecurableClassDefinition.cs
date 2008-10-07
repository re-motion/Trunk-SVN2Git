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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;
using Remotion.Data.DomainObjects.Linq;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  public abstract class SecurableClassDefinition : MetadataObject
  {
    // types

    // static members and constants

    public static SecurableClassDefinition NewObject ()
    {
      return NewObject<SecurableClassDefinition>().With();
    }

    public new static SecurableClassDefinition GetObject (ObjectID id)
    {
      return DomainObject.GetObject<SecurableClassDefinition> (id);
    }

    public static SecurableClassDefinition FindByName (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      var result = from c in QueryFactory.CreateQueryable<SecurableClassDefinition>()
                   where c.Name == name
                   select c;

      return result.ToArray ().SingleOrDefault ();
    }

    public static ObjectList<SecurableClassDefinition> FindAll ()
    {
      var result = from c in QueryFactory.CreateQueryable<SecurableClassDefinition>()
                   orderby c.Index
                   select c;

      return result.ToObjectList();
    }

    public static ObjectList<SecurableClassDefinition> FindAllBaseClasses ()
    {
      var result = from c in QueryFactory.CreateQueryable<SecurableClassDefinition>()
                   where c.BaseClass == null
                   orderby c.Index
                   select c;

      return result.ToObjectList();
    }

    // member fields

    private ObjectList<StatePropertyDefinition> _stateProperties;
    private ObjectList<AccessTypeDefinition> _accessTypes;

    // construction and disposing

    protected SecurableClassDefinition ()
    {
      SubscribeCollectionEvents();
    }

    // methods and properties

    //TODO: Add test for initialize during on load
    protected override void OnLoaded (LoadMode loadMode)
    {
      base.OnLoaded (loadMode);
      SubscribeCollectionEvents(); // always subscribe collection events when the object gets a new data container
    }

    private void SubscribeCollectionEvents ()
    {
      AccessControlLists.Added += AccessControlLists_Added;
    }

    private void AccessControlLists_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      var accessControlList = (AccessControlList) args.DomainObject;
      var accessControlLists = AccessControlLists;
      if (accessControlLists.Count == 1)
        accessControlList.Index = 0;
      else
        accessControlList.Index = accessControlLists[accessControlLists.Count - 2].Index + 1;
      Touch();
    }

    [DBBidirectionalRelation ("DerivedClasses")]
    [DBColumn ("BaseSecurableClassID")]
    public abstract SecurableClassDefinition BaseClass { get; set; }

    [DBBidirectionalRelation ("BaseClass", SortExpression = "[Index] ASC")]
    public abstract ObjectList<SecurableClassDefinition> DerivedClasses { get; }

    public void Touch ()
    {
      if (State == StateType.Unchanged)
        MarkAsChanged();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelationAttribute ("Class")]
    protected abstract ObjectList<StatePropertyReference> StatePropertyReferences { get; }

    [StorageClassNone]
    public ObjectList<StatePropertyDefinition> StateProperties
    {
      get
      {
        if (_stateProperties == null)
        {
          var stateProperties = new ObjectList<StatePropertyDefinition>();

          foreach (var propertyReference in StatePropertyReferences)
            stateProperties.Add (propertyReference.StateProperty);

          _stateProperties = new ObjectList<StatePropertyDefinition> (stateProperties, true);
        }

        return _stateProperties;
      }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelationAttribute ("Class", SortExpression = "[Index] ASC")]
    protected abstract ObjectList<AccessTypeReference> AccessTypeReferences { get; }

    [StorageClassNone]
    public ObjectList<AccessTypeDefinition> AccessTypes
    {
      get
      {
        if (_accessTypes == null)
        {
          var accessTypes = new ObjectList<AccessTypeDefinition>();

          foreach (var accessTypeReference in AccessTypeReferences)
            accessTypes.Add (accessTypeReference.AccessType);

          _accessTypes = new ObjectList<AccessTypeDefinition> (accessTypes, true);
        }

        return _accessTypes;
      }
    }

    [DBBidirectionalRelation ("Class")]
    [ObjectBinding (ReadOnly = true)]
    public abstract ObjectList<StateCombination> StateCombinations { get; }

    [DBBidirectionalRelation ("Class", SortExpression = "[Index] ASC")]
    public abstract ObjectList<AccessControlList> AccessControlLists { get; }

    public void AddAccessType (AccessTypeDefinition accessType)
    {
      var reference = AccessTypeReference.NewObject();
      reference.AccessType = accessType;
      AccessTypeReferences.Add (reference);
      var accessTypeReferences = AccessTypeReferences;
      if (accessTypeReferences.Count == 1)
        reference.Index = 0;
      else
        reference.Index = accessTypeReferences[accessTypeReferences.Count - 2].Index + 1;
      Touch();

      _accessTypes = null;
    }

    public void AddStateProperty (StatePropertyDefinition stateProperty)
    {
      var reference = StatePropertyReference.NewObject();
      reference.StateProperty = stateProperty;

      StatePropertyReferences.Add (reference);
      _stateProperties = null;
    }

    public StateCombination FindStateCombination (IList<StateDefinition> states)
    {
      return StateCombinations.Where (sc => sc.MatchesStates (states)).SingleOrDefault ();
    }

    public AccessControlList CreateAccessControlList ()
    {
      AccessControlList accessControlList = AccessControlList.NewObject();
      accessControlList.Class = this;
      accessControlList.CreateStateCombination();
      accessControlList.CreateAccessControlEntry();

      return accessControlList;
    }

    public SecurableClassValidationResult Validate ()
    {
      var result = new SecurableClassValidationResult();

      ValidateUniqueStateCombinations (result);

      return result;
    }

    public void ValidateUniqueStateCombinations (SecurableClassValidationResult result)
    {
      Assertion.IsTrue (State != StateType.Deleted || StateCombinations.Count == 0, "StateCombinations of object are not empty but the object is deleted.", ID);

      var dupblicateStateCombinations = StateCombinations
          .GroupBy (sc => sc, new StateCombinationComparer())
          .Where (g => g.Count() > 1)
          .SelectMany (g => g);

      foreach (StateCombination stateCombination in dupblicateStateCombinations)
        result.AddInvalidStateCombination (stateCombination);
    }

    protected override void OnCommitting (EventArgs args)
    {
      SecurableClassValidationResult result = Validate();
      if (!result.IsValid)
      {
        throw new ConstraintViolationException (
            string.Format ("The securable class definition '{0}' contains at least one state combination that has been defined twice.", Name));
      }

      base.OnCommitting (args);
    }
  }
}
