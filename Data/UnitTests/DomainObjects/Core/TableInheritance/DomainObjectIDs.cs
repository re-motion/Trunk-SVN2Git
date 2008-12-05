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
using Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  public class DomainObjectIDs
  {
    public readonly ObjectID Customer = new ObjectID (typeof (Customer), new Guid ("{623016F9-B525-4CAE-A2BD-D4A6155B2F33}"));
    public readonly ObjectID Client = new ObjectID (typeof (Client), new Guid ("{F7AD91EF-AC75-4fe3-A427-E40312B12917}"));
    public readonly ObjectID ClassWithUnidirectionalRelation = new ObjectID (typeof (ClassWithUnidirectionalRelation),
        new Guid ("{7E7E4002-19BF-4e8b-9525-4634A8D0FCB5}"));
    public readonly ObjectID Person = new ObjectID (typeof (Person), new Guid ("{21E9BEA1-3026-430a-A01E-E9B6A39928A8}"));
    public readonly ObjectID PersonForUnidirectionalRelationTest = new ObjectID (typeof (Person), new Guid ("{084010C4-82E5-4b0d-AE9F-A953303C03A4}"));
    public readonly ObjectID Region = new ObjectID (typeof (Region), new Guid ("{7905CF32-FBC2-47fe-AC40-3E398BEEA5AB}"));
    public readonly ObjectID Order = new ObjectID (typeof (Order), new Guid ("{6B88B60C-1C91-4005-8C60-72053DB48D5D}"));
    public readonly ObjectID HistoryEntry1 = new ObjectID (typeof (HistoryEntry), new Guid ("{0A2A6302-9CCB-4ab2-B006-2F1D89526435}"));
    public readonly ObjectID HistoryEntry2 = new ObjectID (typeof (HistoryEntry), new Guid ("{02D662F0-ED50-49b4-8A26-BB6025EDCA8C}"));
    public readonly ObjectID OrganizationalUnit = new ObjectID (typeof (OrganizationalUnit), new Guid ("{C6F4E04D-0465-4a9e-A944-C9FD26E33C44}")); 

    public DomainObjectIDs ()
    {
    }
  }
}
