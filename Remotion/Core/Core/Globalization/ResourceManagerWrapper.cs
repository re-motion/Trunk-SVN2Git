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
using System.Collections;
using System.Collections.Specialized;
using System.Globalization;
using System.Resources;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  ///   A wrapper for the .net Framework <c>ResourceManager</c> implementation.
  /// </summary>
  /// <remarks>
  ///   <para>
  ///     Limited to accessing string resources.
  ///     Limited on resources for the current UI culture and its less specific cultures.
  ///   </para><para>
  ///     If multiple Resource Managers are added which belonging to derived types, 
  ///     make sure to sort the resource managers in the order of inheritance before wrapping them.
  ///   </para>
  /// </remarks>
  public class ResourceManagerWrapper : IResourceManager
  {
    //  static members

    private static readonly ILog s_log = LogManager.GetLogger (typeof (ResourceManagerWrapper));

    public static ResourceManagerSet CreateWrapperSet (ResourceManager[] resourceManagers)
    {
      ResourceManagerWrapper[] wrappers = new ResourceManagerWrapper[resourceManagers.Length];
      for (int i = 0; i < wrappers.Length; ++i)
        wrappers[i] = new ResourceManagerWrapper (resourceManagers[i]);

      return new ResourceManagerSet (wrappers);
    }

    // member fields

    private readonly ResourceManager _resourceManager;
    private readonly InterlockedCache<Tuple<CultureInfo, string>, NameValueCollection> _cachedResourceSet = 
        new InterlockedCache<Tuple<CultureInfo, string>, NameValueCollection> ();

    // construction and disposing

    /// <summary>
    ///   Constructor for wrapping multiple resource managers
    /// </summary>
    /// <include file='doc\include\Globalization\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/Constructor/param[@name="resourceManager"]' />
    public ResourceManagerWrapper (ResourceManager resourceManager)
    {
      ArgumentUtility.CheckNotNull ("resourceManager", resourceManager);
      _resourceManager = resourceManager;
    }

    // methods and properties

    /// <summary>
    ///   Gets the wrapped <c>ResourceManager</c> instance. 
    /// </summary>
    public ResourceManager ResourceManager
    {
      get { return _resourceManager; }
    }

    /// <summary>
    ///   Gets the root names of the resource files that the <c>IResourceManager</c>
    ///   searches for resources. Multiple roots are separated by a comma.
    /// </summary>
    string IResourceManager.Name
    {
      get { return _resourceManager.BaseName; }
    }

    /// <summary>
    ///   Returns all string resources inside the wrapped resource managers.
    /// </summary>
    /// <returns>
    ///   A collection of string pairs, the key being the resource's ID, the vale being the string.
    /// </returns>
    public NameValueCollection GetAllStrings ()
    {
      return GetAllStrings (string.Empty);
    }

    /// <summary>
    ///   Searches for all string resources inside the resource manager whose name is prefixed 
    ///   with a matching tag.
    /// </summary>
    /// <seealso cref="M:Remotion.Globalization.IResourceManager.GetAllStrings(System.String)"/>
    /// <include file='doc\include\Globalization\ResourceManagerWrapper.xml' path='ResourceManagerWrapper/GetAllStrings/remarks' />
    public NameValueCollection GetAllStrings (string prefix)
    {
      return _cachedResourceSet.GetOrCreateValue (
          Tuple.NewTuple (CultureInfo.CurrentUICulture, StringUtility.NullToEmpty (prefix)),
          key =>
          {
            //  Loop through all entries in the resource managers
            CultureInfo[] cultureHierarchy = GetCultureHierarchy (key.A);

            // Loop from most neutral to current UICulture
            // Copy the resources into a collection
            NameValueCollection result = new NameValueCollection ();
            for (int i = 0; i < cultureHierarchy.Length; i++)
            {
              CultureInfo culture = cultureHierarchy[i];
              ResourceSet resourceSet = _resourceManager.GetResourceSet (culture, true, false);
              if (resourceSet != null)
              {
                foreach (DictionaryEntry entry in resourceSet)
                {
                  string entryKey = (string) entry.Key;
                  if (entryKey.StartsWith (key.B))
                    result[entryKey] = (string) entry.Value;
                }
              }
            }

            return result;
          });
    }

    /// <summary>
    ///   Gets the value of the specified string resource. 
    /// </summary>
    /// <seealso cref="M:Remotion.Globalization.IResourceManager.GetString(System.Enum)"/>
    public string GetString (Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);
      return GetString (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
    }

    /// <summary>
    ///   Gets the value of the specified string resource. 
    /// </summary>
    /// <seealso cref="M:Remotion.Globalization.IResourceManager.GetString(System.String)"/>
    public string GetString (string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);

      string result = _resourceManager.GetString (id);
      if (result != null)
        return result;

      s_log.Debug ("Could not find resource with ID '" + id + "' in resource container '" + _resourceManager.BaseName + "'.");
      return id;
    }

    /// <summary>Tests whether the <see cref="ResourceManagerWrapper"/> contains the specified resource.</summary>
    /// <param name="id">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="ResourceManagerWrapper"/> contains the specified resource.</returns>
    public bool ContainsResource (string id)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("id", id);
      return _resourceManager.GetObject (id) != null;
    }

    /// <summary>Tests whether the <see cref="ResourceManagerWrapper"/> contains the specified resource.</summary>
    /// <param name="enumValue">The ID of the resource to look for.</param>
    /// <returns><see langword="true"/> if the <see cref="ResourceManagerWrapper"/> contains the specified resource.</returns>
    public bool ContainsResource (Enum enumValue)
    {
      ArgumentUtility.CheckNotNull ("enumValue", enumValue);
      return ContainsResource (ResourceIdentifiersAttribute.GetResourceIdentifier (enumValue));
    }

    /// <summary>
    ///   Returns the culture hierarchy, starting with the most specialized culture.
    /// </summary>
    /// <param name="mostSpecialized">
    ///   The starting point for walking the culture tree upwards. Must not be <see langame="null"/>.
    /// </param>
    /// <returns>
    ///   The cultures, starting with the invariant culture, ending with the most specialized culture.
    /// </returns>
    public static CultureInfo[] GetCultureHierarchy (CultureInfo mostSpecialized)
    {
      ArrayList hierarchyTopDown = new ArrayList();

      CultureInfo currentLevel = mostSpecialized;

      do
      {
        hierarchyTopDown.Add (currentLevel);
        currentLevel = currentLevel.Parent;
      } while (currentLevel != CultureInfo.InvariantCulture);

      if (mostSpecialized != CultureInfo.InvariantCulture)
        hierarchyTopDown.Add (currentLevel);

      hierarchyTopDown.Reverse();

      return (CultureInfo[]) hierarchyTopDown.ToArray (typeof (CultureInfo));
    }

    bool INullObject.IsNull
    {
      get { return false; }
    }
  }
}
