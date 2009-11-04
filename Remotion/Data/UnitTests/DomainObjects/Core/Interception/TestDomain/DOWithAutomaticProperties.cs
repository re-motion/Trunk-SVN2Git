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
using System.Runtime.CompilerServices;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Infrastructure;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Interception.TestDomain
{
  [DBTable]
  public class DOWithAutomaticProperties : DomainObject
  {
    public virtual int PropertyWithGetterAndSetter
    {
      [CompilerGenerated]
      get { throw new NotImplementedException (); }
      [CompilerGenerated]
      set { throw new NotImplementedException (); } 
    }

    public virtual string PropertyWithGetterOnly
    {
      [CompilerGenerated]
      get { throw new NotImplementedException (); }
    }

    public virtual DateTime PropertyWithSetterOnly
    {
      [CompilerGenerated]
      set { throw new NotImplementedException (); }
    }

    protected virtual int ProtectedProperty
    {
      [CompilerGenerated]
      get { throw new NotImplementedException (); }
      [CompilerGenerated]
      set { throw new NotImplementedException (); }
    }

    [StorageClassNone]
    public new PropertyIndexer Properties
    {
      get { return base.Properties; }
    }
  }
}
