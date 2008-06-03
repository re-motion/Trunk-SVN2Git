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
