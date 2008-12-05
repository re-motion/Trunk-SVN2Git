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
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain
{
  [DBTable]
  [Serializable]
  public class DOWithVirtualProperties : DomainObject
  {
    public virtual int PropertyWithGetterAndSetter
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }

    public virtual string PropertyWithGetterOnly
    {
      get { return CurrentProperty.GetValue<string> (); }
    }

    public virtual DateTime PropertyWithSetterOnly
    {
      set { CurrentProperty.SetValue (value); }
    }

    protected virtual int ProtectedProperty
    {
      get { return CurrentProperty.GetValue<int> (); }
      set { CurrentProperty.SetValue (value); }
    }

    public virtual DateTime PropertyThrowing
    {
      get { throw new Exception (); }
      set { throw new Exception (); }
    }

    [StorageClassNone]
    public virtual DateTime PropertyNotInMapping
    {
      get { return CurrentProperty.GetValue<DateTime>(); }
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }

    public string GetAndCheckCurrentPropertyName()
    {
      return CurrentPropertyManager.GetAndCheckCurrentPropertyName();
    }
  }
}
