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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Globalization; // TODO: adjust DOGen
using Remotion.NullableValueTypes;

namespace DomainSample
{
	[MultiLingualResources("DomainSample.Globalization.Person")] // TODO: adjust DOGen
  public class Person : BindableDomainObject
  {
    // types

    // static members and constants

    public static new Person GetObject (ObjectID id)
    {
      return (Person) DomainObject.GetObject (id);
    }

    public static new Person GetObject (ObjectID id, bool includeDeleted)
    {
      return (Person) DomainObject.GetObject (id, includeDeleted);
    }

    public static new Person GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Person) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Person GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Person) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    private PhoneNumberCollection _phoneNumbersToDelete;

    // construction and disposing

    public Person ()
    {
    }

    public Person (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected Person (DataContainer dataContainer) : base (dataContainer)
    {
    }

    // methods and properties

    public string FirstName
    {
      get { return (string) DataContainer["FirstName"]; }
      set { DataContainer["FirstName"] = value; }
    }

    public string LastName
    {
      get { return (string) DataContainer["LastName"]; }
      set { DataContainer["LastName"] = value; }
    }

    public string EMailAddress
    {
      get { return (string) DataContainer["EMailAddress"]; }
      set { DataContainer["EMailAddress"] = value; }
    }

    public Location Location
    {
      get { return (Location) GetRelatedObject ("Location"); }
      set { SetRelatedObject ("Location", value); }
    }

    public PhoneNumberCollection PhoneNumbers
    {
      get { return (PhoneNumberCollection) GetRelatedObjects ("PhoneNumbers"); }
      set {}
    }

    protected override void OnDeleting(EventArgs args)
    {
      _phoneNumbersToDelete = (PhoneNumberCollection) PhoneNumbers.Clone ();

      base.OnDeleting (args);
    }


    protected override void OnDeleted(EventArgs args)
    {
      foreach (PhoneNumber phoneNumber in _phoneNumbersToDelete)
        phoneNumber.Delete ();

      base.OnDeleted (args);
    }

  //  public new void Delete ()
  //	{
  //		PhoneNumberCollection phoneNumbers = this.PhoneNumbers;
  //
  //		int phoneNumbersCount = phoneNumbers.Count;
  //
  //		for (int j = phoneNumbersCount - 1; j >= 0; j--)
  //			phoneNumbers[j].Delete ();
  //
  //		base.Delete ();
  //	}
  }
}
