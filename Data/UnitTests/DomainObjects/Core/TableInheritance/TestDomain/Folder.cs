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

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance.TestDomain
{
  [ClassID ("TI_Folder")]
  [DBTable ("TableInheritance_Folder")]
  [Instantiable]
  public abstract class Folder: FileSystemItem
  {
    public static Folder NewObject()
    {
      return NewObject<Folder>().With();
    }

    public static Folder GetObject (ObjectID id)
    {
      return GetObject<Folder> (id);
    }

    protected Folder()
    {
    }

    [DBBidirectionalRelation ("ParentFolder", SortExpression = "Name ASC")]
    public abstract ObjectList<FileSystemItem> FileSystemItems { get; }

    [DBColumn ("FolderCreatedAt")]
    public abstract DateTime CreatedAt { get; set; }
  }
}
