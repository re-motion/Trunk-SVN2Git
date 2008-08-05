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
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.Linq;
using Remotion.Data.DomainObjects.Queries;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [Serializable]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Culture : BaseSecurityManagerObject
  {
    public static Culture NewObject (string cultureName)
    {
      return NewObject<Culture> ().With (cultureName);
    }

    public static Culture Find (string cultureName)
    {
      ArgumentUtility.CheckNotNull ("cultureName", cultureName);
      
      var result = from c in DataContext.Entity<Culture>()
                   where c.CultureName == cultureName
                   select c;

      return result.ToArray().SingleOrDefault();
    }

    protected Culture (string cultureName)
    {
      ArgumentUtility.CheckNotNull ("cultureName", cultureName);
      
      CultureName = cultureName;
    }

    [StringProperty (IsNullable = false, MaximumLength = 10)]
    public abstract string CultureName { get; protected set; }
  }
}
