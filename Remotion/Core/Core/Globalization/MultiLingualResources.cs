// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using System.Linq;
using Remotion.Globalization.Implementation;
using Remotion.ServiceLocation;
using Remotion.Utilities;
using Remotion.FunctionalProgramming;

namespace Remotion.Globalization
{
  /// <summary>
  /// Provides the public API for classes working with and analyzing instances of <see cref="MultiLingualResourcesAttribute"/>.
  /// </summary>
  [Obsolete ("Retrieve IGlobalizationService from IoC container instead.")]
  public static class MultiLingualResources
  {
    private static readonly DoubleCheckedLockingContainer<GlobalizationService> s_service =
        new DoubleCheckedLockingContainer<GlobalizationService> (
            () => SafeServiceLocator.Current.GetAllInstances<IGlobalizationService>()
                .OfType<GlobalizationService>()
                .Single (() => new InvalidOperationException ("GlobalizationService is not registered with Service Locator.")));


    /// <summary>
    ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
    ///   in the class declaration of the type.
    /// </summary>
    /// <include file='..\doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='..\doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType" or @name="includeHierarchy"]' />
    public static IResourceManager GetResourceManager (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      ArgumentUtility.CheckNotNull ("includeHierarchy", includeHierarchy);

      var resourceManager = s_service.Value.GetResourceManager (objectType);
      if (resourceManager.IsNull)
      {
        var message = string.Format (
            "Type {0} and its base classes do not define a resource attribute.",
            objectType.FullName);
        throw new ResourceException (message);
      }

      if (includeHierarchy || objectType.BaseType == null)
        return resourceManager;

      var baseResourceManager = s_service.Value.GetResourceManager (objectType.BaseType);
      if (baseResourceManager.IsNull)
        return resourceManager;

      var resourceManagerSet = (ResourceManagerSet) resourceManager;
      var baseResourceManagerSet = (ResourceManagerSet) baseResourceManager;

      return new ResourceManagerSet (
          resourceManagerSet.ResourceManagers.Take (resourceManagerSet.ResourceManagers.Count - baseResourceManagerSet.ResourceManagers.Count));
    }

    /// <summary>
    ///   Returns an instance of <c>IResourceManager</c> for the resource container specified
    ///   in the class declaration of the type.
    /// </summary>
    /// <include file='..\doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/Common/*' />
    /// <include file='..\doc\include\Globalization\MultiLingualResourcesAttribute.xml' path='/MultiLingualResourcesAttribute/GetResourceManager/param[@name="objectType"]' />
    public static IResourceManager GetResourceManager (Type objectType)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      return GetResourceManager (objectType, false);
    }

    /// <summary>
    ///   Loads a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to get the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> The found string resource or an empty string. </returns>
    public static string GetResourceText (Type objectTypeToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      ArgumentUtility.CheckNotNull ("name", name);

      var resourceManager = GetResourceManager (objectTypeToGetResourceFor, false);
      var text = resourceManager.GetString (name);
      if (text == name)
        return String.Empty;
      return text;
    }

    /// <summary>
    ///   Loads a string resource for the object's type, identified by ID.
    /// </summary>
    /// <param name="objectToGetResourceFor">
    ///   The object for whose <see cref="Type"/> to get the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> The found string resource or an empty string. </returns>
    public static string GetResourceText (object objectToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return GetResourceText (objectToGetResourceFor.GetType(), name);
    }

    /// <summary>
    ///   Checks for the existence of a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> <see langword="true"/> if the resource can be found. </returns>
    public static bool ExistsResourceText (Type objectTypeToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      ArgumentUtility.CheckNotNull ("name", name);

      try
      {
        var resourceManager = GetResourceManager (objectTypeToGetResourceFor, false);
        string text = resourceManager.GetString (name);
        return (text != name);
      }
      catch
      {
        return false;
      }
    }

    /// <summary>
    ///   Checks for the existence of a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectToGetResourceFor">
    ///   The object for whose <see cref="Type"/> to check the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> <see langword="true"/> if the resource can be found. </returns>
    public static bool ExistsResourceText (object objectToGetResourceFor, string name)
    {
      ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
      ArgumentUtility.CheckNotNullOrEmpty ("name", name);

      return ExistsResourceText (objectToGetResourceFor.GetType(), name);
    }

    /// <summary>
    ///   Checks for the existence of a resource set for the specified type.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check for the resource set.
    /// </param>
    /// <returns> <see langword="true"/> if the resource ser can be found. </returns>
    public static bool ExistsResource (Type objectTypeToGetResourceFor)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);

      return !GetResourceManager (objectTypeToGetResourceFor, false).IsNull;
    }

    /// <summary>
    ///   Checks for the existence of a resource set for the specified object.
    /// </summary>
    /// <param name="objectToGetResourceFor">
    ///   The object for whose <see cref="Type"/> to check for the resource set.
    /// </param>
    /// <returns> <see langword="true"/> if the resource ser can be found. </returns>
    public static bool ExistsResource (object objectToGetResourceFor)
    {
      ArgumentUtility.CheckNotNull ("objectToGetResourceFor", objectToGetResourceFor);
      return ExistsResource (objectToGetResourceFor.GetType());
    }
  }
}