// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// The re-motion Core Framework is free software; you can redistribute it 
// and/or modify it under the terms of the GNU Lesser General Public License 
// as published by the Free Software Foundation; either version 2.1 of the 
// License, or (at your option) any later version.
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
