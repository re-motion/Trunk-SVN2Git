// This file is part of re-strict (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.FunctionalProgramming;
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
      return NewObject<SecurableClassDefinition>();
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

      return result.SingleOrDefault();
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

    private ReadOnlyCollection<StatePropertyDefinition> _stateProperties;
    private ReadOnlyCollection<AccessTypeDefinition> _accessTypes;

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
    }

    [DBBidirectionalRelation ("DerivedClasses")]
    [DBColumn ("BaseSecurableClassID")]
    public abstract SecurableClassDefinition BaseClass { get; set; }

    [DBBidirectionalRelation ("BaseClass", SortExpression = "Index ASC")]
    public abstract ObjectList<SecurableClassDefinition> DerivedClasses { get; }

    public void Touch ()
    {
      if (State == StateType.Unchanged || State == StateType.NotLoadedYet)
        MarkAsChanged();
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("Class")]
    protected abstract ObjectList<StatePropertyReference> StatePropertyReferences { get; }

    [StorageClassNone]
    public ReadOnlyCollection<StatePropertyDefinition> StateProperties
    {
      get
      {
        if (_stateProperties == null)
        {
          var stateProperties = new List<StatePropertyDefinition>();

          foreach (var propertyReference in StatePropertyReferences)
            stateProperties.Add (propertyReference.StateProperty);

          _stateProperties = stateProperties.AsReadOnly();
        }

        return _stateProperties;
      }
    }

    [EditorBrowsable (EditorBrowsableState.Never)]
    [DBBidirectionalRelation ("Class", SortExpression = "Index ASC")]
    protected abstract ObjectList<AccessTypeReference> AccessTypeReferences { get; }

    [StorageClassNone]
    public ReadOnlyCollection<AccessTypeDefinition> AccessTypes
    {
      get
      {
        if (_accessTypes == null)
          _accessTypes = AccessTypeReferences.Select (accessTypeReference => accessTypeReference.AccessType).ToList().AsReadOnly();

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

    [DBBidirectionalRelation ("MyClass", SortExpression = "Index ASC")]
    public abstract ObjectList<StatefulAccessControlList> StatefulAccessControlLists { get; }

    /// <summary>
    /// Adds an <see cref="AccessTypeDefinition"/> at end of the <see cref="AccessTypes"/> list.
    /// </summary>
    /// <param name="accessType">The <see cref="AccessTypeDefinition"/> to be added. Must not be <see langword="null" />.</param>
    /// <remarks> Also updates all <see cref="AccessControlEntry"/> objects associated with the <see cref="SecurableClassDefinition"/> 
    /// to include a <see cref="Permission"/> entry for the new <see cref="AccessTypeDefinition"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="accessType"/> already exists on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    public void AddAccessType (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      InsertAccessType (AccessTypeReferences.Count, accessType);
    }

    /// <summary>
    /// Inserts an <see cref="AccessTypeDefinition"/> at the specified <paramref name="index"/>. 
    /// </summary>
    /// <param name="index">The zero-based index at which the <paramref name="accessType"/> should be inserted.</param>
    /// <param name="accessType">The <see cref="AccessTypeDefinition"/> to be inserted. Must not be <see langword="null" />.</param>
    /// <remarks> Also updates all <see cref="AccessControlEntry"/> objects associated with the <see cref="SecurableClassDefinition"/> 
    /// to include a <see cref="Permission"/> entry for the new <see cref="AccessTypeDefinition"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="accessType"/> already exists on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <para><paramref name="index"/> is less than 0.</para>
    /// <para> -or-</para>
    /// <para><paramref name="index"/> is greater than the total number of <see cref="AccessTypes"/>.</para>
    /// </exception>
    public void InsertAccessType (int index, AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);
      if (index < 0 || index > AccessTypeReferences.Count)
      {
        throw CreateArgumentOutOfRangeException (
            "index", index, "The index must not be less than 0 or greater than the total number of access types for this securable class definition.");
      }

      if (AccessTypeReferences.Where (r => r.AccessType == accessType).Any())
      {
        throw CreateArgumentException (
            "accessType", "The access type '{0}' has already been added to this securable class definition.", accessType.Name);
      }

      _accessTypes = null;

      var reference = AccessTypeReference.NewObject();
      reference.AccessType = accessType;
      AccessTypeReferences.Insert (index, reference);
      for (int i = 0; i < AccessTypeReferences.Count; i++)
        AccessTypeReferences[i].Index = i;

      foreach (var ace in GetAccessControlLists().SelectMany (acl => acl.AccessControlEntries))
        ace.AddAccessType (accessType);

      Touch();
    }

    /// <summary>
    /// Removes an <see cref="AccessTypeDefinition"/> at from of the <see cref="AccessTypes"/> list.
    /// </summary>
    /// <param name="accessType">The <see cref="AccessTypeDefinition"/> to be removed. Must not be <see langword="null" />.</param>
    /// <remarks> Also updates all <see cref="AccessControlEntry"/> objects associated with the <see cref="SecurableClassDefinition"/> 
    /// to remove the <see cref="Permission"/> entry for the <see cref="AccessTypeDefinition"/>.
    /// </remarks>
    /// <exception cref="ArgumentException">
    /// The <paramref name="accessType"/> does not exist on the <see cref="SecurableClassDefinition"/>.
    /// </exception>
    public void RemoveAccessType (AccessTypeDefinition accessType)
    {
      ArgumentUtility.CheckNotNull ("accessType", accessType);

      _accessTypes = null;

      var accessTypeReference = AccessTypeReferences.SingleOrDefault (r => r.AccessType == accessType);
      if (accessTypeReference == null)
      {
        throw CreateArgumentException (
            "accessType", "The access type '{0}' is not associated with this securable class definition.", accessType.Name);
      }

      accessTypeReference.Delete();
      for (int i = 0; i < AccessTypeReferences.Count; i++)
        AccessTypeReferences[i].Index = i;

      foreach (var ace in GetAccessControlLists().SelectMany (acl => acl.AccessControlEntries))
        ace.RemoveAccessType (accessType);

      Touch();
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
          State != StateType.Deleted || StateCombinations.Count == 0, "StateCombinations of object '{0}' are not empty but the object is deleted.", ID);

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
          State != StateType.Deleted || StateCombinations.Count == 0, "StateCombinations of object '{0}' are not empty but the object is deleted.", ID);

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

      Touch();
    }

    private ArgumentException CreateArgumentException (string argumentName, string format, params object[] args)
    {
      return new ArgumentException (string.Format (format, args), argumentName);
    }
    
    private ArgumentException CreateArgumentOutOfRangeException (string argumentName, object actualValue, string format, params object[] args)
    {
      return new ArgumentOutOfRangeException(argumentName, actualValue, string.Format (format, args));
    }

    private IEnumerable<AccessControlList> GetAccessControlLists()
    {
      if (StatelessAccessControlList != null)
        yield return StatelessAccessControlList;

      foreach (var acl in StatefulAccessControlLists)
        yield return acl;
    }
  }
}
