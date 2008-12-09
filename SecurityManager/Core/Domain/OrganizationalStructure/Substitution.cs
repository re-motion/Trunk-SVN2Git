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
using Remotion.Data.DomainObjects;
using Remotion.Globalization;
using Remotion.ObjectBinding;
using Remotion.Utilities;

namespace Remotion.SecurityManager.Domain.OrganizationalStructure
{
  /// <summary>
  /// The <see cref="Substitution"/> type defines the association between two <see cref="User"/>s and optionally a <see cref="Role"/> where the 
  /// <see cref="SubstitutingUser"/> can act as a stand-in for the <see cref="SubstitutedUser"/> and <see cref="SubstitutedRole"/>.
  /// </summary>
  [Serializable]
  [MultiLingualResources ("Remotion.SecurityManager.Globalization.Domain.OrganizationalStructure.Substitution")]
  [PermanentGuid ("5F3FEEE1-38E3-4ecb-AC2F-2D74AFFE3A27")]
  [Instantiable]
  [DBTable]
  [SecurityManagerStorageGroup]
  public abstract class Substitution : OrganizationalStructureObject
  {
    public static Substitution NewObject (User substitutingUser)
    {
      return NewObject<Substitution>().With (substitutingUser);
    }

    protected Substitution (User substitutingUser)
    {
      ArgumentUtility.CheckNotNull ("substitutingUser", substitutingUser);

      // ReSharper disable DoNotCallOverridableMethodsInConstructor
      SubstitutingUser = substitutingUser;
      IsEnabled = true;
      // ReSharper restore DoNotCallOverridableMethodsInConstructor
    }

    [DBBidirectionalRelation ("SubstitutingFor")]
    [Mandatory]
    public abstract User SubstitutingUser { get; protected set; }

    [DBBidirectionalRelation ("SubstitutedBy")]
    [Mandatory]
    public abstract User SubstitutedUser { get; set; }

    [DBBidirectionalRelation ("SubstitutedBy")]
    public abstract Role SubstitutedRole { get; set; }

    [DateProperty]
    public abstract DateTime? BeginDate { get; set; }

    [DateProperty]
    public abstract DateTime? EndDate { get; set; }

    public abstract bool IsEnabled { get; set; }

    /// <summary>
    /// The <see cref="Substitution"/> is only active when the object is <see cref="StateType.Unchanged"/>, the <see cref="IsEnabled"/> flag is set
    /// and the present date is within the range defined by <see cref="BeginDate"/> and <see cref="EndDate"/>.
    /// </summary>
    [StorageClassNone]
    public bool IsActive
    {
      get
      {
        if (State != StateType.Unchanged)
          return false;

        if (!IsEnabled)
          return false;

        if (BeginDate.HasValue && BeginDate.Value.Date > DateTime.Today)
          return false;

        if (EndDate.HasValue && EndDate.Value.Date < DateTime.Today)
          return false;

        return true;
      }
    }
  }
}