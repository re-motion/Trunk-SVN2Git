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
