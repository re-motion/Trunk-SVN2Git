using System;
using NUnit.Framework;
using Remotion.Collections;
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.SecurityManager.Domain;
using Remotion.SecurityManager.Domain.AccessControl;
using Remotion.SecurityManager.Domain.Metadata;
using Remotion.SecurityManager.Domain.OrganizationalStructure;
using Remotion.Data.DomainObjects;
using Remotion.Development.UnitTesting;

namespace Remotion.SecurityManager.UnitTests.Domain
{
  [TestFixture]
  public class SerializationTest
  {
    [Test]
    public void DomainObjectsAreSerializable ()
    {
      CheckDomainObjectSerializability<AccessControlEntry> (delegate { return AccessControlEntry.NewObject (); });
      CheckDomainObjectSerializability<AccessControlList> (delegate { return AccessControlList.NewObject (); });
      CheckDomainObjectSerializability<Permission> (delegate { return Permission.NewObject (); });
      CheckDomainObjectSerializability<StateCombination> (delegate { return StateCombination.NewObject (); });
      CheckDomainObjectSerializability<StateUsage> (delegate { return StateUsage.NewObject (); });
      CheckDomainObjectSerializability<AbstractRoleDefinition> (delegate { return AbstractRoleDefinition.NewObject (); });
      CheckDomainObjectSerializability<AccessTypeDefinition> (delegate { return AccessTypeDefinition.NewObject (); });
      CheckDomainObjectSerializability<AccessTypeReference> (delegate { return AccessTypeReference.NewObject (); });
      CheckDomainObjectSerializability<Culture> (delegate { return Culture.NewObject ("DE-DE"); });
      CheckDomainObjectSerializability<LocalizedName> (delegate { return LocalizedName.NewObject ("foo", Culture.NewObject ("DE-DE"), SecurableClassDefinition.NewObject ()); });
      CheckDomainObjectSerializability<SecurableClassDefinition> (delegate { return SecurableClassDefinition.NewObject (); });
      CheckDomainObjectSerializability<StateDefinition> (delegate { return StateDefinition.NewObject (); });
      CheckDomainObjectSerializability<StatePropertyDefinition> (delegate { return StatePropertyDefinition.NewObject (); });
      CheckDomainObjectSerializability<StatePropertyReference> (delegate { return StatePropertyReference.NewObject (); });
      CheckDomainObjectSerializability<Group> (delegate { return (Group) RepositoryAccessor.NewObject (typeof (Group)).With (); });
      CheckDomainObjectSerializability<GroupType> (delegate { return (GroupType) RepositoryAccessor.NewObject (typeof (GroupType)).With (); });
      CheckDomainObjectSerializability<GroupTypePosition> (delegate { return GroupTypePosition.NewObject (); });
      CheckDomainObjectSerializability<Position> (delegate { return (Position) RepositoryAccessor.NewObject (typeof (Position)).With (); });
      CheckDomainObjectSerializability<Role> (delegate { return Role.NewObject (); });
      CheckDomainObjectSerializability<Tenant> (delegate { return (Tenant) RepositoryAccessor.NewObject (typeof (Tenant)).With (); });
      CheckDomainObjectSerializability<User> (delegate { return (User) RepositoryAccessor.NewObject (typeof (User)).With(); });
    }

    private void CheckDomainObjectSerializability<T> (Func<T> creator)
        where T: DomainObject
    {
      using (ClientTransaction.NewRootTransaction().EnterNonDiscardingScope())
      {
        T instance = creator ();

        Tuple<T, ClientTransaction> deserializedTuple = Serializer.SerializeAndDeserialize (Tuple.NewTuple (instance, ClientTransaction.Current));
        T deserializedT = deserializedTuple.A;
        Assert.IsNotNull (deserializedT);

        IBusinessObject bindableOriginal = (IBusinessObject) instance;
        IBusinessObject bindableDeserialized = (IBusinessObject) deserializedT;

        foreach (IBusinessObjectProperty property in bindableOriginal.BusinessObjectClass.GetPropertyDefinitions())
        {
          Assert.IsNotNull (bindableDeserialized.BusinessObjectClass.GetPropertyDefinition (property.Identifier));

          object value = null;
          bool propertyCanBeRetrieved;
          try
          {
            value = bindableOriginal.GetProperty (property);
            propertyCanBeRetrieved = true;
          }
          catch (Exception)
          {
            propertyCanBeRetrieved = false;
          }

          if (propertyCanBeRetrieved)
          {
            object newValue;
            using (deserializedTuple.B.EnterNonDiscardingScope())
            {
              newValue = bindableDeserialized.GetProperty (property);
            }
            if (value != null && typeof (DomainObject).IsAssignableFrom (property.PropertyType))
              Assert.AreEqual (((DomainObject) value).ID, ((DomainObject) newValue).ID);
            else
              Assert.AreEqual (value, newValue);
          }
        }
      }
    }
  }
}