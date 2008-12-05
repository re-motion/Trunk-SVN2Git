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
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class LocalizedName : BaseSecurityManagerObject
  {
    public static LocalizedName NewObject (string text, Culture culture, MetadataObject metadataObject)
    {
      return NewObject<LocalizedName> ().With (text, culture, metadataObject);
    }

    protected LocalizedName (string text, Culture culture, MetadataObject metadataObject)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("text", text);
      ArgumentUtility.CheckNotNull ("culture", culture);
      ArgumentUtility.CheckNotNull ("metadataObject", metadataObject);

      Text = text;
      Culture = culture;
      MetadataObject = metadataObject;
    }

    [StringProperty (IsNullable = false)]
    public abstract string Text { get; set; }

    [Mandatory]
    public abstract Culture Culture { get; protected set; }

    [DBBidirectionalRelation ("LocalizedNames")]
    [Mandatory]
    public abstract MetadataObject MetadataObject { get; protected set; }
  }
}
