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
using System.ComponentModel;
using System.ComponentModel.Design;
using Remotion.Data.DomainObjects.ConfigurationLoader;
using Remotion.Reflection;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Design
{
  /// <summary>
  /// Design mode attribute used to associate a design mode specifc mapping loader with it's run-time version.
  /// </summary>
  [AttributeUsage (AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
  public sealed class DesignModeMappingLoaderAttribute: Attribute
  {
    private readonly Type _type;

    /// <summary>
    /// Initializes a new instance of the <see cref="DesignModeMappingLoaderAttribute"/> class with a design mode mapping loader
    /// </summary>
    /// <param name="type">
    /// A <see cref="Type"/> implementing <see cref="IMappingLoader"/>. The <paramref name="type"/> must also have a constructor receiving an 
    /// instance of a <see cref="Type"/> implementing <see cref="ISite"/>.
    /// </param>
    public DesignModeMappingLoaderAttribute (Type type)
    {
      ArgumentUtility.CheckNotNullAndTypeIsAssignableFrom ("type", type, typeof (IMappingLoader));
      _type = type;
    }

    public Type Type
    {
      get { return _type; }
    }

    public IMappingLoader CreateInstance (IDesignerHost designerHost)
    {
      ArgumentUtility.CheckNotNull ("designerHost", designerHost);
      return (IMappingLoader) TypesafeActivator.CreateInstance (_type).With (designerHost);
    }
  }
}
