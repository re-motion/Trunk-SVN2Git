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
using System.Collections.Generic;
using System.Linq;
using Remotion.Mixins.Context;
using Remotion.Utilities;
using Remotion.Logging;

namespace Remotion.Mixins.MixerTool
{
  public class ClassContextFinder
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ClassContextFinder));

    private readonly MixinConfiguration _configuration;
    private readonly IEnumerable<Type> _types;

    public ClassContextFinder (MixinConfiguration configuration, IEnumerable<Type> types)
    {
      ArgumentUtility.CheckNotNull ("configuration", configuration);
      ArgumentUtility.CheckNotNull ("types", types);

      _configuration = configuration;
      _types = types;
    }

    public IEnumerable<ClassContext> FindClassContexts ()
    {
      var inheritedContexts = from t in _types
                              where !t.IsDefined (typeof (IgnoreForMixinConfigurationAttribute), false)
                              let configuredContext = _configuration.ClassContexts.GetExact (t)
                              where configuredContext == null
                              let inheritedContext = _configuration.ClassContexts.GetWithInheritance (t)
                              where inheritedContext != null
                              select inheritedContext;

      return _configuration.ClassContexts.Concat (inheritedContexts).Where (ShouldProcessContext);
    }

    private bool ShouldProcessContext (ClassContext context)
    {
      if (context.Type.IsGenericTypeDefinition)
      {
        s_log.WarnFormat ("Type {0} is a generic type definition and is thus ignored.", context.Type);
        return false;
      }

      if (context.Type.IsInterface)
      {
        s_log.WarnFormat ("Type {0} is an interface and is thus ignored.", context.Type);
        return false;
      }

      return true;
    }
  }
}