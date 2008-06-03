/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.Globalization
{

/// <summary>
///   A class whose instances know where their resource container is.
/// </summary>
/// <remarks>
///   Used to externally controll resource management.
/// </remarks>
public interface IObjectWithResources
{
  /// <summary>
  ///   Returns an instance of <c>IResourceManager</c> for resource container of the object.
  /// </summary>
  IResourceManager GetResourceManager();
}

}
