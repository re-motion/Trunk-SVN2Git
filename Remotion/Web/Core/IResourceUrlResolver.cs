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
using System.Web.UI;

namespace Remotion.Web
{
/// <summary>
///   Resolve the relative image URL into an absolute image url.
/// </summary>
public interface IResourceUrlResolver
{
  /// <summary>
  ///   Resolves a relative URL into an absolute URL.
  /// </summary>
  /// <param name="control"> 
  ///   The current <see cref="Control"/>. Used to detect design time.
  /// </param>
  /// <param name="relativeUrl">
  ///   The relative URL to be resolved into an absolute URL.
  /// </param>
  /// <param name="definingType"> 
  ///   The type that defines the resource. If the resource instance is not defined by a type, 
  ///   this is <see langword="null"/>. 
  /// </param>
  /// <param name="resourceType">
  ///   The type of resource to get. 
  /// </param>
  /// <returns>
  ///   The absulute URL.
  /// </returns>
	string GetResourceUrl (Control control, Type definingType, ResourceType resourceType, string relativeUrl);
}

public class ResourceType
{
  public static readonly ResourceType Image = new ResourceType ("Image");
  public static readonly ResourceType Html = new ResourceType ("Html");
  public static readonly ResourceType UI = new ResourceType ("UI");
  public static readonly ResourceType HelpPage = new ResourceType ("HelpPage");

  private string _name;

  public ResourceType (string name)
  {
    _name = name;
  }

  public string Name
  { 
    get { return _name; }
  }
}

}
