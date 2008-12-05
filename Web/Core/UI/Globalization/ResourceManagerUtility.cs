// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections;
using System.Collections.Generic;
using System.Web.UI;
using Remotion.Globalization;
using Remotion.Utilities;

namespace Remotion.Web.UI.Globalization
{
/// <summary>
///   Functionality for working with <see cref="IResourceManager"/> in Controls.
/// </summary>
public static class ResourceManagerUtility
{
  private const string c_globalResourceKeyPrefix = "$res:";
  /// <summary> Hashtable&lt;type,IResourceManagers&gt; </summary>
  private static Hashtable s_chachedResourceManagers = new Hashtable();
  /// <summary> Dummy value used mark cached types without a resource manager. </summary>
  private static readonly object s_dummyResourceManager = new object();

  public static bool IsGlobalResourceKey (string elementValue)
  {
    if (StringUtility.IsNullOrEmpty (elementValue))
      return false;
    return elementValue.StartsWith (c_globalResourceKeyPrefix);
  }

  public static string GetGlobalResourceKey (string elementValue)
  {
    if (IsGlobalResourceKey (elementValue))
      return elementValue.Substring (c_globalResourceKeyPrefix.Length);
    else
      return null;
  }

  /// <summary>
  ///   Get resource managers of all controls impementing <see cref="IObjectWithResources"/> in the 
  ///   current control's hierarchy (parents last).
  /// </summary>
  /// <param name="control">
  ///   The <see cref="Control"/> where to start searching for <see cref="IObjectWithResources"/>.
  /// </param>
  /// <returns>
  ///   An <see cref="IResourceManager"/> or <see langname="null"/> if not implemented. If more than
  ///   one resource manager is found, an <see cref="ResourceManagerSet"/> is returned.
  /// </returns>
  /// <remarks> Uses a cache for the individual <see cref="IResourceManager"/> instances. </remarks>
  public static IResourceManager GetResourceManager (Control control)
  {
    return GetResourceManager (control, true);
  }

  /// <summary>
  ///   Get resource managers of all controls impementing <see cref="IObjectWithResources"/> in the 
  ///   current control's hierarchy (parents last).
  /// </summary>
  /// <param name="control">
  ///   The <see cref="Control"/> where to start searching for <see cref="IObjectWithResources"/>.
  /// </param>
  /// <param name="alwaysIncludeParents">
  ///   If true, parent controls' resource managers are included even if a resource manager has already 
  ///   been found in a child control. Default is true.
  /// </param>
  /// <returns>
  ///   An <see cref="IResourceManager"/> or <see langname="null"/> if not implemented. If more than
  ///   one resource manager is found, an <see cref="ResourceManagerSet"/> is returned.
  /// </returns>
  /// <remarks> Uses a cache for the individual <see cref="IResourceManager"/> instances. </remarks>
  public static IResourceManager GetResourceManager (Control control, bool alwaysIncludeParents)
  {
    if (control == null)
      return null;

    List<IResourceManager> resourceManagers = new List<IResourceManager>();

    GetResourceManagersRecursive (control, resourceManagers, alwaysIncludeParents);

    if (resourceManagers.Count == 0)
      return null;
    else if (resourceManagers.Count == 1)
      return (IResourceManager) resourceManagers[0];
    else
      return new ResourceManagerSet ((IResourceManager[]) resourceManagers.ToArray());
  }

  private static void GetResourceManagersRecursive (Control control, List<IResourceManager> resourceManagers, bool alwaysIncludeParents)
  {
    if (control == null)
      return;

    IObjectWithResources objectWithResources  = control as IObjectWithResources;

    if (objectWithResources != null)
    {
      IResourceManager resourceManager = GetResourceManagerFromCache (objectWithResources);
      if (resourceManager != null)
        resourceManagers.Add (resourceManager);
    }

    if (objectWithResources == null || alwaysIncludeParents)
      GetResourceManagersRecursive (control.Parent, resourceManagers, alwaysIncludeParents);
  }

  /// <summary> Gets the (cached) <see cref="IResourceManager"/> for the passed <see cref="IObjectWithResources"/> </summary>
  /// <param name="objectWithResources">
  ///   The <see cref="IObjectWithResources"/> to get the <see cref="IResourceManager"/> from.
  /// </param>
  /// <returns> 
  ///   An <see cref="IResourceManager"/> object or <see langword="null"/> if no resource manager has been returned
  ///   by <see cref="IObjectWithResources.GetResourceManager">IObjectWithResources.GetResourceManager</see>.
  /// </returns>
  private static IResourceManager GetResourceManagerFromCache (IObjectWithResources objectWithResources)
  {
    ArgumentUtility.CheckNotNull ("objectWithResources", objectWithResources);
    Type type = objectWithResources.GetType();

    if (s_chachedResourceManagers[type] == null)
    {
      lock (typeof (ResourceManagerUtility))
      {
        if (s_chachedResourceManagers[type] == null)
        {     
          IResourceManager resourceManager = objectWithResources.GetResourceManager ();
          if (resourceManager == null)
          {
            //  Chache a dummy value if no resources are defined for the page.
            s_chachedResourceManagers[type] = s_dummyResourceManager;
          }
          else
          {
            s_chachedResourceManagers[type] = resourceManager;
          }
        }
      }  
    }
    
    return s_chachedResourceManagers[type] as IResourceManager;
  }
}
}
