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
using Remotion.Data.DomainObjects;
using Remotion.Data.UnitTests.DomainObjects.Factories;

namespace Remotion.Data.UnitTests.DomainObjects.Core.TableInheritance
{
  public class TableInheritanceConfiguration: BaseConfiguration
  {
    private static TableInheritanceConfiguration s_instance;

    public static TableInheritanceConfiguration Instance
    {
      get
      {
        if (s_instance == null)
          throw new InvalidOperationException ("TableInheritanceConfiguration has not been Initialized by invoking Initialize()");
        return s_instance;
      }
    }

    public static void Initialize()
    {
      s_instance = new TableInheritanceConfiguration();
      s_instance._domainObjectIDs = new DomainObjectIDs (s_instance.GetMappingConfiguration());
    }

    private DomainObjectIDs _domainObjectIDs;

    private TableInheritanceConfiguration()
    {
      
    }

    public DomainObjectIDs GetDomainObjectIDs()
    {
      return _domainObjectIDs;
    }
  }
}
