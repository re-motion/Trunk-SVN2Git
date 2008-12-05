// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// version 3.0 as published by the Free Software Foundation.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Remotion.Collections;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.Utilities;

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
      return GetObject<SecurableClassDefinition> (id);
    }

    public static SecurableClassDefinition FindByName (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      var result = from c in QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                   where c.Name == name
                   select c;

      return result.ToArray().SingleOrDefault();
    }

    public static ObjectList<SecurableClassDefinition> FindAll ()
    {
      var result = from c in QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
                   orderby c.Index
                   select c;

      return result.ToObjectList();
    }

    public static ObjectList<SecurableClassDefinition> FindAllBaseClasses ()
    {
      var result = from c in QueryFactory.CreateLinqQuery<SecurableClassDefinition>()
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
      StatefulAccessControlLists.Added += StatefulAccessControlLists_Added;
    }

    private void StatefulAccessControlLists_Added (object sender, DomainObjectCollectionChangeEventArgs args)
    {
      var accessControlList = (StatefulAccessControlList) args.DomainObject;
      var accessControlLists = StatefulAccessControlLists;
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
    [DBBidirectionalRelation ("Class")]
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
    [DBBidirectionalRelation ("Class", SortExpression = "[Index] ASC")]
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

    [StorageClassNone]
    [ObjectBinding (ReadOnly = true)]
    public IList<StateCombination> StateCombinations
    {
      get
      {
        var result = from acl in StatefulAccessControlLists 
                     from sc in acl.StateCombinations 
                     select sc;
        return result.ToArray();
      }
    }

    [DBBidirectionalRelation ("MyClass")]
    public abstract StatelessAccessControlList StatelessAccessControlList { get; set; }

    [DBBidirectionalRelation ("MyClass", SortExpression = "[Index] ASC")]
    public abstract ObjectList<StatefulAccessControlList> StatefulAccessControlLists { get; }

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

    /// <summary>Retrieves the <see cref="StatePropertyDefinition"/> with the passed name.</summary>
    /// <param name="propertyName">Name of the <see cref="StatePropertyDefinition"/> to retrieve.Must not be <see langword="null" /> or empty. </param>
    /// <exception cref="ArgumentException">Thrown if the specified property does not exist on this <see cref="SecurableClassDefinition"/>.</exception>
    public StatePropertyDefinition GetStateProperty (string propertyName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("propertyName", propertyName);

      return StateProperties.Single (
          p => p.Name == propertyName,
          () => CreateArgumentException (
                    "propertyName",
                    "A state property with the name '{0}' is not defined for the secureable class definition '{1}'.",
                    propertyName,
                    Name));
    }

    public StateCombination FindStateCombination (IList<StateDefinition> states)
    {
      return StateCombinations.Where (sc => sc.MatchesStates (states)).SingleOrDefault();
    }

    public StatelessAccessControlList CreateStatelessAccessControlList ()
    {
      if (StatelessAccessControlList != null)
        throw new InvalidOperationException ("A SecurableClassDefinition only supports a single StatelessAccessControlList at a time.");

      var accessControlList = StatelessAccessControlList.NewObject();
      accessControlList.Class = this;
      accessControlList.CreateAccessControlEntry ();

      return accessControlList;
    }

    public StatefulAccessControlList CreateStatefulAccessControlList ()
    {
      var accessControlList = StatefulAccessControlList.NewObject ();
      accessControlList.Class = this;
      accessControlList.CreateStateCombination();
      accessControlList.CreateAccessControlEntry();

      return accessControlList;
    }

    public SecurableClassValidationResult Validate ()
    {
      var result = new SecurableClassValidationResult();

      ValidateUniqueStateCombinations (result);

      ValidateStateCombinationsAgainstStateProperties (result);

      return result;
    }

    public void ValidateUniqueStateCombinations (SecurableClassValidationResult result)
    {
      Assertion.IsTrue (
          State != StateType.Deleted || StateCombinations.Count == 0, "StateCombinations of object are not empty but the object is deleted.", ID);

      var duplicateStateCombinations = StateCombinations
          .GroupBy (sc => sc, new StateCombinationComparer())
          .Where (g => g.Count() > 1)
          .SelectMany (g => g);

      foreach (var stateCombination in duplicateStateCombinations)
        result.AddDuplicateStateCombination (stateCombination);
    }

    public void ValidateStateCombinationsAgainstStateProperties (SecurableClassValidationResult result)
    {
      Assertion.IsTrue (
          State != StateType.Deleted || StateCombinations.Count == 0, "StateCombinations of object are not empty but the object is deleted.", ID);

      foreach (var stateCombination in StateCombinations.Where (sc => sc.StateUsages.Count != StateProperties.Count))
        result.AddInvalidStateCombination (stateCombination);
    }

    protected override void OnCommitting (EventArgs args)
    {
      SecurableClassValidationResult result = Validate();
      if (!result.IsValid)
      {
        if (result.DuplicateStateCombinations.Count > 0)
        {
          throw new ConstraintViolationException (
              string.Format ("The securable class definition '{0}' contains at least one state combination that has been defined twice.", Name));
        }
        else
        {
          Assertion.IsTrue (result.InvalidStateCombinations.Count > 0);
          throw new ConstraintViolationException (
              string.Format ("The securable class definition '{0}' contains at least one state combination that does not match the class's properties.", Name));
        }
      }

      base.OnCommitting (args);
    }

    private ArgumentException CreateArgumentException (string argumentName, string format, params object[] args)
    {
      return new ArgumentException (string.Format (format, args), argumentName);
    }
  }
}
