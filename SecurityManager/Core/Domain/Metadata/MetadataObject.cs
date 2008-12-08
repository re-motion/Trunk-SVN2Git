// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// re-strict is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License version 3.0 as
// published by the Free Software Foundation.
// 
// re-strict is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with re-strict; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Remotion.Data.DomainObjects;
using Remotion.Data.DomainObjects.ObjectBinding;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.Metadata
{
  [SecurityManagerStorageGroup]
  [Serializable]
  public abstract class MetadataObject : BaseSecurityManagerObject
  {
    // types

    // static members and constants

    public static MetadataObject Find (string metadataID)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("metadataID", metadataID);

      FindMetadataObjectQueryBuilder queryBuilder = new FindMetadataObjectQueryBuilder ();

      var result = queryBuilder.CreateQuery (metadataID);

      return result.ToArray().SingleOrDefault();
    }

    // member fields

    // construction and disposing

    protected MetadataObject ()
    {
    }

    // methods and properties

    public abstract int Index { get; set; }

    public abstract Guid MetadataItemID { get; set; }

    [StringProperty (IsNullable = false, MaximumLength = 200)]
    public abstract string Name { get; set; }

    [DBBidirectionalRelation ("MetadataObject")]
    [ObjectBinding (ReadOnly = true)]
    public abstract ObjectList<LocalizedName> LocalizedNames { get; }

    public override string DisplayName
    {
      get
      {
        foreach (CultureInfo cultureInfo in GetCultureHierachy (CultureInfo.CurrentUICulture))
        {
          LocalizedName localizedName = GetLocalizedName (cultureInfo.Name);
          if (localizedName != null)
            return localizedName.Text;
        }

        return Name;
      }
    }

    public LocalizedName GetLocalizedName (Culture culture)
    {
      ArgumentUtility.CheckNotNull ("culture", culture);

      return GetLocalizedName (culture.CultureName);
    }

    public LocalizedName GetLocalizedName (string cultureName)
    {
      ArgumentUtility.CheckNotNull ("cultureName", cultureName);

      foreach (LocalizedName localizedName in LocalizedNames)
      {
        if (localizedName.Culture.CultureName.Equals (cultureName, StringComparison.Ordinal))
          return localizedName;
      }

      return null;
    }

    private List<CultureInfo> GetCultureHierachy (CultureInfo cultureInfo)
    {
      List<CultureInfo> cultureHierarchy = new List<CultureInfo> ();

      cultureHierarchy.Add (cultureInfo);
      while (cultureInfo != cultureInfo.Parent) // Invariant culture is its own parent
      {
        cultureInfo = cultureInfo.Parent;
        cultureHierarchy.Add (cultureInfo);
      }

      return cultureHierarchy;
    }
  }
}
