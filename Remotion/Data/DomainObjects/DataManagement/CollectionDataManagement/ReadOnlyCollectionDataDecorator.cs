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
using System.Collections;
using System.Collections.Generic;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.DataManagement.CollectionDataManagement
{
  /// <summary>
  /// This class acts as a read-only decorator for another <see cref="IDomainObjectCollectionData"/> object. Every modifying method 
  /// of the <see cref="IDomainObjectCollectionData"/> interface will throw an <see cref="InvalidOperationException"/> when invoked on this class.
  /// </summary>
  [Serializable]
  public class ReadOnlyCollectionDataDecorator : DomainObjectCollectionDataDecoratorBase
  {
    public ReadOnlyCollectionDataDecorator (IDomainObjectCollectionData wrappedData)
      : base (wrappedData)
    {
    }

    public override void  Clear()
    {
      throw new NotSupportedException ("Cannot clear a read-only collection.");
    }

    public override void Insert (int index, DomainObject domainObject)
    {
      throw new NotSupportedException ("Cannot insert an item into a read-only collection.");
    }

    public override bool Remove (DomainObject domainObject)
    {
      throw new NotSupportedException ("Cannot remove an item from a read-only collection.");
    }

    public override bool Remove (ObjectID objectID)
    {
      throw new NotSupportedException ("Cannot remove an item from a read-only collection.");
    }

    public override void Replace (int index, DomainObject newDomainObject)
    {
      throw new NotSupportedException ("Cannot replace an item in a read-only collection.");
    }
  }
}
