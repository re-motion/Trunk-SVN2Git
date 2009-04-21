// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.ComponentModel;
using Remotion.Configuration;
using Remotion.Data.DomainObjects.Configuration;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Design;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Design
{
  /// <summary>
  /// The <see cref="DomainObjectsDesignModeHelper"/> is inteded to encapsulate design mode specific initialization code for <see cref="IComponent"/>
  /// implementations in the <see cref="N:Remotion.Data.DomainObjects"/> namespace, such as data sources.
  /// </summary>
  public class DomainObjectsDesignModeHelper
  {
    private readonly IDesignModeHelper _designModeHelper;

    public DomainObjectsDesignModeHelper (IDesignModeHelper designModeHelper)
    {
      ArgumentUtility.CheckNotNull ("designModeHelper", designModeHelper);

      _designModeHelper = designModeHelper;
    }

    public void InitializeConfiguration ()
    {
      System.Configuration.Configuration configuration = _designModeHelper.GetConfiguration();
      if (configuration != null)
      {
        ConfigurationWrapper.SetCurrent (ConfigurationWrapper.CreateFromConfigurationObject (configuration));
        DomainObjectsConfiguration.SetCurrent (new DomainObjectsConfiguration ());
        MappingConfiguration.SetCurrent (null);
      }
    }
  }
}
