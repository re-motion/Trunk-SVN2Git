/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

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
