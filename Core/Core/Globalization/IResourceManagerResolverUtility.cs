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

namespace Remotion.Globalization
{
	public interface IResourceManagerResolverUtility
	{
		/// <summary>
		///   Loads a string resource for a given type, identified by ID.
		/// </summary>
		/// <param name="resolver">The resolver to use.</param>
		/// <param name="objectTypeToGetResourceFor">
		///   The type for which to get the resource.
		/// </param>
		/// <param name="name"> The ID of the resource. </param>
		/// <returns> The found string resource or an empty string. </returns>
		string GetResourceText<TAttribute> (ResourceManagerResolver<TAttribute> resolver, Type objectTypeToGetResourceFor, string name)
				where TAttribute: Attribute, IResourcesAttribute;

		/// <summary>
		///   Checks for the existence of a string resource for the specified type, identified by ID.
		/// </summary>
		/// <param name="resolver">The resolver to use.</param>
		/// <param name="objectTypeToGetResourceFor">
		///   The <see cref="Type"/> for which to check the resource.
		/// </param>
		/// <param name="name"> The ID of the resource. </param>
		/// <returns> <see langword="true"/> if the resource can be found. </returns>
		bool ExistsResourceText<TAttribute> (ResourceManagerResolver<TAttribute> resolver, Type objectTypeToGetResourceFor, string name)
				where TAttribute: Attribute, IResourcesAttribute;

		/// <summary>
		///   Checks for the existence of a resource set for the specified type.
		/// </summary>
		/// <param name="resolver">The resolver to use.</param>
		/// <param name="objectTypeToGetResourceFor">
		///   The <see cref="Type"/> for which to check for the resource set.
		/// </param>
		/// <returns> <see langword="true"/> if the resource ser can be found. </returns>
		bool ExistsResource<TAttribute> (ResourceManagerResolver<TAttribute> resolver, Type objectTypeToGetResourceFor)
				where TAttribute: Attribute, IResourcesAttribute;
	}
}
