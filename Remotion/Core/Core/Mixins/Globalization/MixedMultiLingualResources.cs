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
using Remotion.Globalization;
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
  public class MixedMultiLingualResources
  {
  	private static readonly MixedResourceManagerResolver<MultiLingualResourcesAttribute> s_resolver =
        new MixedResourceManagerResolver<MultiLingualResourcesAttribute> ();

		/// <summary>
		/// Gets the resolver object used by the methods of this class.
		/// </summary>
		/// <value>The resolver object used by <see cref="MultiLingualResources"/>.</value>
		public static MixedResourceManagerResolver<MultiLingualResourcesAttribute> Resolver
		{
			get { return s_resolver; }
		}

    /// <summary>
    ///   Returns an instance of <see cref="IResourceManager"/> for the resource container specified in the class declaration of the type.
    /// </summary>
    /// <param name="objectType">The type to return an <see cref="IResourceManager"/> for.</param>
    /// <param name="includeHierarchy">If set to true, <see cref="MultiLingualResourcesAttribute"/> applied to base classes and mixins will be
    /// included in the resource manager; otherwise, only the <paramref name="objectType"/> is searched for such attributes.</param>
    /// <returns>An instance of <see cref="IResourceManager"/> for <paramref name="objectType"/>.</returns>
    public static IResourceManager GetResourceManager (Type objectType, bool includeHierarchy)
    {
      ArgumentUtility.CheckNotNull ("objectType", objectType);
      ArgumentUtility.CheckNotNull ("includeHierarchy", includeHierarchy);

      return s_resolver.GetResourceManager (objectType, includeHierarchy);
    }

		/// <summary>
    ///   Returns an instance of <see cref="IResourceManager"/> for the resource container specified in the class declaration of the type
		///   that does not include resource managers for base classes and mixins.
    /// </summary>
    /// <param name="objectType">The type to return an <see cref="IResourceManager"/> for.</param>
    /// <returns>An instance of <see cref="IResourceManager"/> for <paramref name="objectType"/>.</returns>
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

			return ResourceManagerResolverUtility.Current.GetResourceText (s_resolver, objectTypeToGetResourceFor, name);
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

			return ResourceManagerResolverUtility.Current.ExistsResourceText (s_resolver, objectTypeToGetResourceFor, name);
		}

    /// <summary>
    ///   Checks for the existence of a resource set for the specified type.
    /// </summary>
    /// <param name="objectTypeToGetResourceFor">
    ///   The <see cref="Type"/> for which to check for the resource set.
    /// </param>
    /// <returns> <see langword="true"/> if the resource set can be found. </returns>
    public static bool ExistsResource (Type objectTypeToGetResourceFor)
    {
      ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
			return ResourceManagerResolverUtility.Current.ExistsResource (s_resolver, objectTypeToGetResourceFor);
    }
  }
}
