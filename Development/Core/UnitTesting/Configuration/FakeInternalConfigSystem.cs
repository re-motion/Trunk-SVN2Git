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
using System.Collections.Generic;
using System.Configuration.Internal;
using Remotion.Utilities;

namespace Remotion.Development.UnitTesting.Configuration
{
  /// <summary>
  /// Fake implementation of the <see cref="IInternalConfigSystem"/> interface. Used by the <see cref="ConfigSystemHelper"/> to fake the 
  /// configuration.
  /// </summary>
  public class FakeInternalConfigSystem: IInternalConfigSystem
  {
    private Dictionary<string, object> _sections = new Dictionary<string, object>();

    public FakeInternalConfigSystem()
    {
    }

    public object GetSection (string configKey)
    {
      object value;
      if (_sections.TryGetValue (configKey, out value))
        return value;
      return null;
    }

    public void AddSection (string configKey, object section)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("configKey", configKey);
      ArgumentUtility.CheckNotNull ("section", section);

      _sections.Add (configKey, section);
    }

    public void RefreshConfig (string sectionName)
    {
      throw new NotSupportedException();
    }

    public bool SupportsUserConfig
    {
      get { return false; }
    }
  }
}
