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
using Remotion.FunctionalProgramming;
using Remotion.Globalization;
using Remotion.Globalization.Implementation;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.Utilities;

namespace Remotion.Mixins.Globalization
{
  /// <summary>
  /// Provides a variant of <see cref="MultiLingualResources"/> that can be used to have mixins add resource identifiers to a target
  /// class. With this class, attributes are not only retrieved from the class and its base classes, but also from its mixins.
  /// </summary>
  /// <remarks>
  /// The methods of this class do not have overloads taking object - getting and checking resources is always done via the type.
  /// The reason for this is as follows: If an instance was specified, its type would have to be used. Now, if the instance was not created 
  /// by the <see cref="ObjectFactory"/>, we would have to either:
  /// <list type="bullet">
  ///   <item>Fall back to <see cref="MultiLingualResources"/>, because the "new-ed" object doesn't have any mixins, so the type-based resource 
  ///   lookup shouldn't use the mixins either; or </item>
  ///   <item>Be consistent with ExistsResource (obj.GetType()), ie. considering the mixins as well.</item>
  /// </list>
  /// Both possibilities have a certain inconsistency, and none is perfect, so the class leaves it to the user to decide.
  /// </remarks>
  public static class MixedMultiLingualResources
  {
    private static readonly DoubleCheckedLockingContainer<IResourceManagerResolver> s_resolver =
        new DoubleCheckedLockingContainer<IResourceManagerResolver> (() => SafeServiceLocator.Current.GetInstance<IResourceManagerResolver>());

    private static readonly DoubleCheckedLockingContainer<MixinGlobalizationService> s_mixinGlobalizationService =
        new DoubleCheckedLockingContainer<MixinGlobalizationService> (
            () => SafeServiceLocator.Current.GetAllInstances<IGlobalizationService>()
                .OfType<MixinGlobalizationService>()
                .Single (() => new InvalidOperationException ("MixinGlobalizationService is not registered with the IoC container")));

    /// <summary>
    ///   Returns an instance of <see cref="IResourceManager"/> for the resource container specified in the class declaration of the type.
    /// </summary>
    /// <param name="objectType">The type to return an <see cref="IResourceManager"/> for.</param>
    /// <param name="includeHierarchy">If set to true, <see cref="MultiLingualResourcesAttribute"/> applied to base classes and mixins will be
    /// included in the resource manager; otherwise, only the <paramref name="objectType"/> is searched for such attributes.</param>
    /// <returns>An instance of <see cref="IResourceManager"/> for <paramref name="objectType"/>.</returns>
    [Obsolete ("Retrieve IGlobalizationService from IoC container instead and use IGlobalizationService.GetResourceManager (objectType). Note: When using IGlobalizationService, the order of resolution has changed to return resources for mixins first, then the target types. (Version 1.13.223.0)")]
    public static IResourceManager GetResourceManager (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      ArgumentUtility.CheckNotNull ("includeHierarchy", includeHierarchy);

      var targetType = MixinTypeUtility.GetUnderlyingTargetType (objectType);
      var result = s_resolver.Value.Resolve (targetType);
      var mixinResourceManager = s_mixinGlobalizationService.Value.GetResourceManager (TypeAdapter.Create (targetType));

      if (includeHierarchy)
      {
        if (result.IsNull && mixinResourceManager.IsNull)
          throw new ResourceException (string.Format ("Type {0} and its base classes do not define a resource attribute.", objectType.FullName));
        return ResourceManagerSet.Create (result.DefinedResourceManager, mixinResourceManager, result.InheritedResourceManager);
      }

      if (result.IsNull && mixinResourceManager.IsNull)
        throw new ResourceException (string.Format ("Type {0} and its base classes do not define a resource attribute.", objectType.FullName));

      if (result.DefinedResourceManager.IsNull && !mixinResourceManager.IsNull)
        return mixinResourceManager;

      if (result.DefinedResourceManager.IsNull) // we already know there is a resource defined on a base-type so no additional checks are required.
        return MultiLingualResources.GetResourceManager (targetType.BaseType, false);

      return ResourceManagerSet.Create (result.DefinedResourceManager, mixinResourceManager);
    }

    /// <summary>
    ///   Returns an instance of <see cref="IResourceManager"/> for the resource container specified in the class declaration of the type
    ///   that does not include resource managers for base classes and mixins.
    /// </summary>
    /// <param name="objectType">The type to return an <see cref="IResourceManager"/> for.</param>
    /// <returns>An instance of <see cref="IResourceManager"/> for <paramref name="objectType"/>.</returns>
    [Obsolete ("Retrieve IGlobalizationService from IoC container instead and use IGlobalizationService.GetResourceManager (objectType). Note: This method did not include the hierarchy but IGlobalizationService will always include the hierarchy. Note: When using IGlobalizationService, the order of resolution has changed to return resources for mixins first, then the target types. (Version 1.13.223.0)")]
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
    [Obsolete ("Retrieve IGlobalizationService from IoC container instead and use IGlobalizationService.GetResourceManager (objectTypeToGetResourceFor).GetString (name). Note: This method did not include the hierarchy but IGlobalizationService will always include the hierarchy. Note: When using IGlobalizationService, the order of resolution has changed to return resources for mixins first, then the target types. (Version 1.13.223.0)")]
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
    ///   Checks for the existence of a string resource for the specified type, identified by ID.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check the resource.
    /// </param>
    /// <param name="name"> The ID of the resource. </param>
    /// <returns> <see langword="true"/> if the resource can be found. </returns>
    [Obsolete ("Retrieve IGlobalizationService from IoC container instead and test for IGlobalizationService.GetResourceManager (objectTypeToGetResourceFor).ContainsString (name). Note: This method did not include the hierarchy but IGlobalizationService will always include the hierarchy. (Version 1.13.223.0)")]
    public static bool ExistsResourceText (Type objectTypeToGetResourceFor, string name)
    {
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
    ///   Checks for the existence of a resource set for the specified type.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check for the resource set.
    /// </param>
    /// <returns> <see langword="true"/> if the resource set can be found. </returns>
    [Obsolete ("Retrieve IGlobalizationService from IoC container instead and test for IGlobalizationService.GetResourceManager (objectTypeToGetResourceFor).IsNull. Note: This method did not include the hierarchy but IGlobalizationService will always include the hierarchy. (Version 1.13.223.0)")]
    public static bool ExistsResource (Type objectTypeToGetResourceFor)
    {
      try
      {
        return !GetResourceManager (objectTypeToGetResourceFor, false).IsNull;
      }
      catch (ResourceException)
      {
        return false;
      }
    }
  }
}