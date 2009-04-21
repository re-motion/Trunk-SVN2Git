// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using Remotion.ObjectBinding.BindableObject;

namespace Remotion.ObjectBinding.UnitTests.Core.TestDomain
{
  public class ManualBusinessObject : IBusinessObject
  {
    private string _stringProperty = "Initial value";

    public string StringProperty
    {
      get { return _stringProperty; }
      set { _stringProperty = value; }
    }

    public object GetProperty (IBusinessObjectProperty property)
    {
      if (property.Identifier == "StringProperty")
        return StringProperty;
      else
        throw new NotSupportedException ();
    }

    public void SetProperty (IBusinessObjectProperty property, object value)
    {
      if (property.Identifier == "StringProperty")
        StringProperty = (string) value;
      else
        throw new NotSupportedException ();
    }

    public string GetPropertyString (IBusinessObjectProperty property, string format)
    {
      return GetProperty (property).ToString ();
    }

    public string DisplayName
    {
      get { return "Manual business object"; }
    }

    public string DisplayNameSafe
    {
      get { return "Manual business object (safe)"; }
    }

    public IBusinessObjectClass BusinessObjectClass
    {
      get { return BindableObjectProvider.GetBindableObjectClass(typeof (ManualBusinessObject)); }
    }
  }
}
