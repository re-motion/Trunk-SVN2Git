using System;
using System.Collections.Generic;
using System.ComponentModel;
using Remotion.Data.DomainObjects;
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
      return DomainObject.GetObject<SecurableClassDefinition> (id);
    }

    public static SecurableClassDefinition FindByName (string name)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      Query query = new Query ("Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition.FindByName");
      query.Parameters.Add ("@name", name);

      DomainObjectCollection result = ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
      if (result.Count == 0)
        return null;

      return (SecurableClassDefinition) result[0];
    }

    public static DomainObjectCollection FindAll ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition.FindAll");
      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
    }

    public static DomainObjectCollection FindAllBaseClasses ()
    {
      Query query = new Query ("Remotion.SecurityManager.Domain.Metadata.SecurableClassDefinition.FindAllBaseClasses");
      return ClientTransactionScope.CurrentTransaction.QueryManager.GetCollection (query);
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
      AccessControlList accessControlList = (AccessControlList) args.DomainObject;
      DomainObjectCollection accessControlLists = AccessControlLists;
      if (accessControlLists.Count == 1)
        accessControlList.Index = 0;
      else
        accessControlList.Index = ((AccessControlList) accessControlLists[accessControlLists.Count - 2]).Index + 1;
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
          ObjectList<StatePropertyDefinition> stateProperties = new ObjectList<StatePropertyDefinition>();

          foreach (StatePropertyReference propertyReference in StatePropertyReferences)
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
          ObjectList<AccessTypeDefinition> accessTypes = new ObjectList<AccessTypeDefinition>();

          foreach (AccessTypeReference accessTypeReference in AccessTypeReferences)
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
      AccessTypeReference reference = AccessTypeReference.NewObject();
      reference.AccessType = accessType;
      AccessTypeReferences.Add (reference);
      DomainObjectCollection accessTypeReferences = AccessTypeReferences;
      if (accessTypeReferences.Count == 1)
        reference.Index = 0;
      else
        reference.Index = ((AccessTypeReference) accessTypeReferences[accessTypeReferences.Count - 2]).Index + 1;
      Touch();

      _accessTypes = null;
    }

    public void AddStateProperty (StatePropertyDefinition stateProperty)
    {
      StatePropertyReference reference = StatePropertyReference.NewObject();
      reference.StateProperty = stateProperty;

      StatePropertyReferences.Add (reference);
      _stateProperties = null;
    }

    public StateCombination FindStateCombination (List<StateDefinition> states)
    {
      foreach (StateCombination stateCombination in StateCombinations)
      {
        if (stateCombination.MatchesStates (states))
          return stateCombination;
      }

      return null;
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
      SecurableClassValidationResult result = new SecurableClassValidationResult();

      ValidateUniqueStateCombinations (result);

      return result;
    }

    public void ValidateUniqueStateCombinations (SecurableClassValidationResult result)
    {
      Dictionary<StateCombination, StateCombination> stateCombinations = new Dictionary<StateCombination, StateCombination> (new StateCombinationComparer());

      Assertion.IsTrue (State != StateType.Deleted || StateCombinations.Count == 0, "StateCombinations of object are not empty but the object is deleted.", ID);

      foreach (StateCombination stateCombination in StateCombinations)
      {
        if (stateCombinations.ContainsKey (stateCombination))
        {
          result.AddInvalidStateCombination (stateCombinations[stateCombination]);
          result.AddInvalidStateCombination (stateCombination);
        }
        else
        {
          stateCombinations.Add (stateCombination, stateCombination);
        }
      }
    }

    protected override void OnCommitting (EventArgs args)
    {
      SecurableClassValidationResult result = Validate();
      if (!result.IsValid)
      {
        throw new ConstraintViolationException (
            string.Format ("The securable class definition '{0}' contains at least one state combination, which has been defined twice.", Name));
      }

      base.OnCommitting (args);
    }
  }
}