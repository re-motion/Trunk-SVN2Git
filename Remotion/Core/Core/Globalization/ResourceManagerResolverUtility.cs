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
using Remotion.Utilities;

namespace Remotion.Globalization
{
	/// <summary>
	/// Encapsulates the algorithms used to access resource containers defined by resource attributes.
	/// </summary>
	/// <remarks>
	/// This class is an attribute type-agnostic encapsulation of the algorithms used by <see cref="MultiLingualResources"/>.
	/// </remarks>
	public class ResourceManagerResolverUtility : IResourceManagerResolverUtility
	{
		public static readonly ResourceManagerResolverUtility Default = new ResourceManagerResolverUtility();
		private static IResourceManagerResolverUtility s_current = Default;

		public static IResourceManagerResolverUtility Current
		{
			get { return s_current; }
		}

		public static void SetCurrent (IResourceManagerResolverUtility newValue)
		{
			s_current = newValue ?? Default;
		}

		/// <summary>
		///   Loads a string resource for a given type, identified by ID.
		/// </summary>
		/// <param name="resolver">The resolver to use.</param>
		/// <param name="objectTypeToGetResourceFor">
		///   The type for which to get the resource.
		/// </param>
		/// <param name="name"> The ID of the resource. </param>
		/// <returns> The found string resource or an empty string. </returns>
		public string GetResourceText<TAttribute> (ResourceManagerResolver<TAttribute> resolver, Type objectTypeToGetResourceFor, string name)
				where TAttribute: Attribute, IResourcesAttribute
		{
			ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
			ArgumentUtility.CheckNotNull ("name", name);

			IResourceManager rm = resolver.GetResourceManager (objectTypeToGetResourceFor, false);
			string text = rm.GetString (name);
			if (text == name)
				return String.Empty;
			return text;
		}

		/// <summary>
		///   Checks for the existence of a string resource for the specified type, identified by ID.
		/// </summary>
		/// <param name="resolver">The resolver to use.</param>
		/// <param name="objectTypeToGetResourceFor">
		///   The <see cref="Type"/> for which to check the resource.
		/// </param>
		/// <param name="name"> The ID of the resource. </param>
		/// <returns> <see langword="true"/> if the resource can be found. </returns>
		public bool ExistsResourceText<TAttribute> (ResourceManagerResolver<TAttribute> resolver, Type objectTypeToGetResourceFor, string name)
				where TAttribute: Attribute, IResourcesAttribute
		{
			ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
			ArgumentUtility.CheckNotNull ("name", name);

			try
			{
				IResourceManager rm = resolver.GetResourceManager (objectTypeToGetResourceFor, false);
				string text = rm.GetString (name);
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
		/// <param name="resolver">The resolver to use.</param>
		/// <param name="objectTypeToGetResourceFor">
		///   The <see cref="Type"/> for which to check for the resource set.
		/// </param>
		/// <returns> <see langword="true"/> if the resource ser can be found. </returns>
		public bool ExistsResource<TAttribute> (ResourceManagerResolver<TAttribute> resolver, Type objectTypeToGetResourceFor)
				where TAttribute: Attribute, IResourcesAttribute
		{
			ArgumentUtility.CheckNotNull ("objectTypeToGetResourceFor", objectTypeToGetResourceFor);
      return EnumerableUtility.FirstOrDefault (resolver.GetResourceDefinitionStream (objectTypeToGetResourceFor, false)) != null;
		}
	}
}
