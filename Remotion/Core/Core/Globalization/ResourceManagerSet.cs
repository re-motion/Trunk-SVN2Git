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
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using Remotion.Collections;
using Remotion.Logging;
using Remotion.Text;
using Remotion.Utilities;

namespace Remotion.Globalization
{
  /// <summary>
  ///   Combines one or more <see cref="IResourceManager"/> instances to a set that can be accessed using a single interface.
  /// </summary>
  public class ResourceManagerSet : IResourceManager
  {
    private static readonly ILog s_log = LogManager.GetLogger (typeof (ResourceManagerSet));

    private readonly IResourceManager[] _resourceManagers;
    private readonly string _name;

    /// <summary>
    ///   Combines several IResourceManager instances to a single ResourceManagerSet, starting with the first entry of the first set.
    /// </summary>
    /// <remarks>
    ///   For parameters that are ResourceManagerSet instances, the contained IResourceManagers are added directly.
    /// </remarks>
    /// <example>
    ///   <para>
    ///     Given the following parameter list of resource managers (rm) and resource manager sets (rmset):
    ///   </para><para>
    ///     rm1, rm2, rmset (rm3, rm4, rm5), rm6, rmset (rm7, rm8)
    ///   </para><para>
    ///     The following resource manager set is created:
    ///   </para><para>
    ///     rmset (rm1, rm2, rm3, rm4, rm5, rm6, rm7, rm8)
    ///   </para>
    /// </example>
    /// <param name="resourceManagers"> The resource manager, starting with the most specific. </param>
    public static ResourceManagerSet Create (params IResourceManager[] resourceManagers)
    {
      ArgumentUtility.CheckNotNull ("resourceManagers", resourceManagers);

      return new ResourceManagerSet (resourceManagers.AsEnumerable());
    }

    public ResourceManagerSet (IEnumerable<IResourceManager> resourceManagers)
    {
      _resourceManagers = CreateFlatList(resourceManagers).ToArray();
      SeparatedStringBuilder sb = new SeparatedStringBuilder (", ", 30 * _resourceManagers.Length);
      foreach (var rm in _resourceManagers)
        sb.Append (rm.Name);
      _name = _resourceManagers.Any() ? sb.ToString() : "Empty ResourceManagerSet";
    }

    [Obsolete ("Use ResourceManagerSet.Create instead. NOTE: The order of the ResourceMangers is now reversed. (Version 1.23.211)", true)]
    public ResourceManagerSet (params IResourceManager[] resourceManagers)
    {
      throw new InvalidOperationException ("Use ResourceManagerSet.Create instead. (Version 1.23.211)");
    }

    public IEnumerable<IResourceManager> ResourceManagers
    {
      get { return _resourceManagers.AsReadOnly(); }
    }

    public NameValueCollection GetAllStrings ()
    {
      return GetAllStrings (string.Empty);
    }

    /// <summary>
    ///   Searches for all string resources inside the resource manager whose name is prefixed with a matching tag.
    /// </summary>
    /// <seealso cref="M:Remotion.Globalization.IResourceManager.GetAllStrings(System.String)"/>
    public NameValueCollection GetAllStrings (string prefix)
    {
      var result = new NameValueCollection();
      foreach (var resourceManager in _resourceManagers)
      {
        var strings = resourceManager.GetAllStrings (prefix);
        for (var i = 0; i < strings.Count; i++)
        {
          var key = strings.Keys[i];
          if (result[key] == null)
            result[key] = strings[i];
        }
      }
      return result;
    }

    /// <summary>
    ///   Gets the value of the specified string resource. 
    /// </summary>
    /// <seealso cref="M:Remotion.Globalization.IResourceManager.GetString(System.String)"/>
    public bool TryGetString (string id, out string value)
    {
      //FOR-loop for performance reasons
      // ReSharper disable ForCanBeConvertedToForeach
      for (var i = 0; i < _resourceManagers.Length; i++)
      {
        var s = _resourceManagers[i].GetStringOrDefault (id);
        if (s != null && s != id)
        {
          value = s;
          return true;
        }
      }
      // ReSharper restore ForCanBeConvertedToForeach

      s_log.DebugFormat ("Could not find resource with ID '{0}' in any of the following resource containers '{1}'.", id, _name);

      value = null;
      return false;
    }
    
    private static IEnumerable<IResourceManager> CreateFlatList (IEnumerable<IResourceManager> resourceManagers)
    {
      foreach (var resourceManager in resourceManagers)
      {
        var rmset = resourceManager as ResourceManagerSet;
        if (rmset != null)
        {
          foreach (var rm in rmset.ResourceManagers)
            yield return rm;
        }
        else if (resourceManager != null && !resourceManager.IsNull)
          yield return resourceManager;
      }
    }

    public string Name
    {
      get { return _name; }
    }

    bool INullObject.IsNull
    {
      get { return !_resourceManagers.Any(); }
    }
  }
}