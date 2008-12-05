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
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.Globalization; // TODO: adjust DOGen
using Remotion.NullableValueTypes;

namespace DomainSample
{
[MultiLingualResources("DomainSample.Globalization.PhoneNumber")] // TODO: adjust DOGen
public class PhoneNumber : BindableDomainObject
{
  // types

  // static members and constants

  public static new PhoneNumber GetObject (ObjectID id)
  {
    return (PhoneNumber) DomainObject.GetObject (id);
  }

  public static new PhoneNumber GetObject (ObjectID id, bool includeDeleted)
  {
    return (PhoneNumber) DomainObject.GetObject (id, includeDeleted);
  }

  public static new PhoneNumber GetObject (ObjectID id, ClientTransaction clientTransaction)
  {
    return (PhoneNumber) DomainObject.GetObject (id, clientTransaction);
  }

  public static new PhoneNumber GetObject (ObjectID id, ClientTransaction clientTransaction, bool includeDeleted)
  {
    return (PhoneNumber) DomainObject.GetObject (id, clientTransaction, includeDeleted);
  }

  // member fields

  // construction and disposing

  public PhoneNumber ()
  {
  }

  public PhoneNumber (ClientTransaction clientTransaction) : base (clientTransaction)
  {
  }

  protected PhoneNumber (DataContainer dataContainer) : base (dataContainer)
  {
  }

  // methods and properties

  public string CountryCode
  {
    get { return (string) DataContainer["CountryCode"]; }
    set { DataContainer["CountryCode"] = value; }
  }

  public string AreaCode
  {
    get { return (string) DataContainer["AreaCode"]; }
    set { DataContainer["AreaCode"] = value; }
  }

  public string Number
  {
    get { return (string) DataContainer["Number"]; }
    set { DataContainer["Number"] = value; }
  }

  public string Extension
  {
    get { return (string) DataContainer["Extension"]; }
    set { DataContainer["Extension"] = value; }
  }

  public Person Person
  {
    get { return (Person) GetRelatedObject ("Person"); }
    set { SetRelatedObject ("Person", value); }
  }

  public override string ToString()
  {
    string number = String.Empty;

    if (CountryCode != null && CountryCode.Trim() != String.Empty)
      number = "++" + CountryCode.Trim() + " ";

    if (AreaCode != null && AreaCode.Trim() != String.Empty)
      number += "(" + AreaCode.Trim() + ") ";

    if (Number != null && Number.Trim() != String.Empty)
      number += Number.Trim() + " - ";

    if (Extension != null && Extension.Trim() != String.Empty)
      number += Extension.Trim();

    return number;
  }

	public override string DisplayName
	{
		get { return ToString(); }
	}
}
}
