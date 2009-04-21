// This file is part of re-strict (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using Remotion.Data.DomainObjects.DataManagement;
using Remotion.Data.DomainObjects.Persistence.Rdbms;

namespace Remotion.SecurityManager.Persistence
{
  public class SecurityManagerSqlProvider : SqlProvider
  {
    // constants

    // types

    // static members

    // member fields

    private RevisionStorageProviderExtension _revisionExtension;

    // construction and disposing

    public SecurityManagerSqlProvider (RdbmsProviderDefinition definition) 
      : base (definition)
    {
      _revisionExtension = new RevisionStorageProviderExtension ();
    }

    // methods and properties

    public override void Save (DataContainerCollection dataContainers)
    {
      _revisionExtension.Saving (Connection, Transaction , dataContainers);
      base.Save (dataContainers);
    }
  }
}
