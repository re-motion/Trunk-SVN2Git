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
using Remotion.ObjectBinding.Web.UI.Controls;

namespace Remotion.ObjectBinding.UnitTests.Web.UI.Controls.BusinessObjectBoundEditableWebControlTests.TestDomain
{
  public class TestableBusinessObjectBoundEditableWebControl : BusinessObjectBoundEditableWebControl
  {
    private object _value;

    public new bool SaveValueToDomainModel ()
    {
      return base.SaveValueToDomainModel();
    }

    public new object Value
    {
      get { return _value; }
      set { _value = value; }
    }

    public override bool HasValue
    {
      get { return _value != null; }
    }

    protected override object ValueImplementation
    {
      get { return Value; }
      set { Value = value; }
    }

    public override void LoadValue (bool interim)
    {
      throw new NotImplementedException();
    }

    public override void SaveValue (bool interim)
    {
      throw new NotImplementedException();
    }

    public override string[] GetTrackedClientIDs ()
    {
      throw new NotImplementedException();
    }
  }
}