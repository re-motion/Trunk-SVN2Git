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
using System.ComponentModel.Design;
using Remotion.BridgeInterfaces;
using Remotion.Implementation;
using Remotion.Utilities;

namespace Remotion.Reflection.TypeDiscovery
{
  /// <summary>
  /// Represents the central entry point into re-motion's type discovery mechanism. All components requiring type discovery should use this class and
  /// its <see cref="GetTypeDiscoveryService"/> method. All components retriving a type by its name that could be executed in the context of the
  /// designer should use <see cref="GetType"/>.
  /// </summary>
  /// <threadsafety static="true" instance="false"/>
  public static class ContextAwareTypeDiscoveryUtility
  {
    private static ITypeDiscoveryService _defaultService = null;
    private static readonly object _defaultServiceLock = new object ();

    /// <summary>
    /// Gets or sets the default non-design mode <see cref="ITypeDiscoveryService"/>. This service is returned whenever 
    /// <see cref="GetTypeDiscoveryService"/> is called unless <see cref="DesignerUtility.IsDesignMode"/> is set to <see langword="true" /> (in which
    /// case the designer's <see cref="ITypeDiscoveryService"/> is used instead). If no custom instance has been set for this property, 
    /// <see cref="T:Remotion.Configuration.TypeDiscovery.TypeDiscoveryConfiguration"/> is used to create a new <see cref="ITypeDiscoveryService"/>
    /// when the property is first retrieved. That instance is stored for later uses.
    /// </summary>
    /// <value>The default non design mode <see cref="ITypeDiscoveryService"/>.</value>
    public static ITypeDiscoveryService DefaultNonDesignModeService
    {
      get
      {
        lock (_defaultServiceLock)
        {
          if (_defaultService == null)
          {
            _defaultService = VersionDependentImplementationBridge<ITypeDiscoveryServiceFactoryImplementation>.Implementation
                .CreateTypeDiscoveryService();
          }
          return _defaultService;
        }
      }
      set
      {
        lock (_defaultServiceLock)
        {
          _defaultService = value;
        }
      }
    }

    /// <summary>
    /// Gets the current context-specific <see cref="ITypeDiscoveryService"/>. If <see cref="DesignerUtility.IsDesignMode"/> is set to 
    /// <see langword="true" />, the designer's <see cref="ITypeDiscoveryService"/> is returned. Otherwise, the 
    /// <see cref="DefaultNonDesignModeService"/> is returned. If no custom instance has been set for the <see cref="DefaultNonDesignModeService"/>
    /// property, <see cref="T:Remotion.Configuration.TypeDiscovery.TypeDiscoveryConfiguration"/> is used to create a new 
    /// <see cref="ITypeDiscoveryService"/> when the property is first retrieved. That instance is stored for later uses.
    /// </summary>
    /// <value>The current context-specific <see cref="ITypeDiscoveryService"/>.</value>
    public static ITypeDiscoveryService GetTypeDiscoveryService ()
    {
      if (DesignerUtility.IsDesignMode)
        return (ITypeDiscoveryService) DesignerUtility.DesignerHost.GetService (typeof (ITypeDiscoveryService));
      else
        return DefaultNonDesignModeService;
    }

    /// <summary>
    /// Gets a type by its name, agnostic of whether the code is executing in the designer or not. If <see cref="DesignerUtility.IsDesignMode"/>
    /// is set to <see langword="true" />, the designer services are used to retrieve the type. Otherwise, <see cref="Type.GetType(string,bool)"/> is 
    /// used. Use <see cref="T:Remotion.Utilities.TypeUtility"/> in order to get types by an abbreviated type name.
    /// </summary>
    /// <param name="typeName">The name of the type to get. This must follow the conventions of <see cref="Type.GetType(string,bool)"/>.</param>
    /// <param name="throwOnError">If <see langword="true" />, a <see cref="TypeLoadException"/> is thrown if the given type cannot be loaded.
    /// Otherwise, <see langword="null" /> is returned.</param>
    /// <returns>The type with the given name, retrieved either from the designer or via <see cref="Type.GetType(string,bool)"/>.</returns>
    public static Type GetType (string typeName, bool throwOnError)
    {
      if (DesignerUtility.IsDesignMode)
      {
        Type type = DesignerUtility.GetDesignModeType (typeName);
        if (type == null && throwOnError)
        {
          string message = string.Format ("Type '{0}' could not be loaded by the designer host.", typeName);
          throw new TypeLoadException (message);
        }
        return type;
      }
      else
        return Type.GetType (typeName, throwOnError);
    }
  }
}