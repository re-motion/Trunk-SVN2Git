/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

using System;
using System.Collections.ObjectModel;
using Remotion.ObjectBinding.BindableObject.Properties;
using Remotion.Utilities;

namespace Remotion.ObjectBinding.BindableObject
{
  public class PropertyCollection : KeyedCollection<string, PropertyBase>
  {
    public PropertyCollection ()
    {
    }

    protected override string GetKeyForItem (PropertyBase item)
    {
      return item.Identifier;
    }

    public PropertyBase[] ToArray ()
    {
      return ArrayUtility.Convert (Items);
    }
  }
}
