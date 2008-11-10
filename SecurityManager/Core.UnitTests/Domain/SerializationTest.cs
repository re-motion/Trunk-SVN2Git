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
      CheckDomainObjectSerializability (delegate { return AccessControlEntry.NewObject (); });
      CheckDomainObjectSerializability (delegate { return StatefulAccessControlList.NewObject (SecurableClassDefinition.NewObject()); });
      CheckDomainObjectSerializability (delegate { return Permission.NewObject (); });
      CheckDomainObjectSerializability (delegate { return StateCombination.NewObject (); });
      CheckDomainObjectSerializability (delegate { return StateUsage.NewObject (); });
      CheckDomainObjectSerializability (delegate { return AbstractRoleDefinition.NewObject (); });
      CheckDomainObjectSerializability (delegate { return AccessTypeDefinition.NewObject (); });
      CheckDomainObjectSerializability (delegate { return AccessTypeReference.NewObject (); });
      CheckDomainObjectSerializability (delegate { return Culture.NewObject ("DE-DE"); });
      CheckDomainObjectSerializability (delegate { return LocalizedName.NewObject ("foo", Culture.NewObject ("DE-DE"), SecurableClassDefinition.NewObject ()); });
      CheckDomainObjectSerializability (delegate { return SecurableClassDefinition.NewObject (); });
      CheckDomainObjectSerializability (delegate { return StateDefinition.NewObject (); });
      CheckDomainObjectSerializability (delegate { return StatePropertyDefinition.NewObject (); });
      CheckDomainObjectSerializability (delegate { return StatePropertyReference.NewObject (); });
      CheckDomainObjectSerializability (delegate { return (Group) RepositoryAccessor.NewObject (typeof (Group)).With (); });
      CheckDomainObjectSerializability (delegate { return (GroupType) RepositoryAccessor.NewObject (typeof (GroupType)).With (); });
      CheckDomainObjectSerializability (delegate { return GroupTypePosition.NewObject (); });
      CheckDomainObjectSerializability (delegate { return (Position) RepositoryAccessor.NewObject (typeof (Position)).With (); });
      CheckDomainObjectSerializability (delegate { return Role.NewObject (); });
      CheckDomainObjectSerializability (delegate { return (Tenant) RepositoryAccessor.NewObject (typeof (Tenant)).With (); });
      CheckDomainObjectSerializability (delegate { return (User) RepositoryAccessor.NewObject (typeof (User)).With(); });
    }

    private void CheckDomainObjectSerializability<T> (Func<T> creator)
        where T: DomainObject
    {
      using (ClientTransaction.CreateRootTransaction().EnterNonDiscardingScope())
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
