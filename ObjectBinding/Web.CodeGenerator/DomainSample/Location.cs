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
using Remotion.Data.DomainObjects.Queries;
using Remotion.Globalization; // TODO: adjust DOGen
using Remotion.NullableValueTypes;

namespace DomainSample
{
	[MultiLingualResources("DomainSample.Globalization.Location")] // TODO: adjust DOGen
  public class Location : BindableDomainObject
  {
    // types

    // static members and constants

    public static new Location GetObject (ObjectID id)
    {
      return (Location) DomainObject.GetObject (id);
    }

    public static new Location GetObject (ObjectID id, bool includeDeleted)
    {
      return (Location) DomainObject.GetObject (id, includeDeleted);
    }

    public static new Location GetObject (ObjectID id, ClientTransaction clientTransaction)
    {
      return (Location) DomainObject.GetObject (id, clientTransaction);
    }

    public static new Location GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
    {
      return (Location) DomainObject.GetObject (id, clientTransaction, includeDeleted);
    }

    // member fields

    // construction and disposing

    public Location ()
    {
    }

    public Location (ClientTransaction clientTransaction) : base (clientTransaction)
    {
    }

    protected Location (DataContainer dataContainer) : base (dataContainer)
    {
    }

    // methods and properties

    public string Name
    {
      get { return (string) DataContainer["Name"]; }
      set { DataContainer["Name"] = value; }
    }

    public string ZipCode
    {
      get { return (string) DataContainer["ZipCode"]; }
      set { DataContainer["ZipCode"] = value; }
    }

    public string City
    {
      get { return (string) DataContainer["City"]; }
      set { DataContainer["City"] = value; }
    }

    public string Street
    {
      get { return (string) DataContainer["Street"]; }
      set { DataContainer["Street"] = value; }
    }

    public Country Country
    {
      get { return (Country) DataContainer["Country"]; }
      set { DataContainer["Country"] = value; }
    }
    
    public override string DisplayName
    {
      get { return Name; }
    }
	
		public static LocationCollection GetAllLocations ()
		{
			Query query = new Query ("AllLocations");

			return (LocationCollection) ClientTransaction.Current.QueryManager.GetCollection (query);
		}

  }
}
