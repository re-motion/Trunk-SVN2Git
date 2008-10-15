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

namespace Remotion.ObjectBinding.BindableObject
{
  /// <summary>
  /// Marks a class to be a base class for the bindable object implementation of <see cref="IBusinessObject"/>. This interface is used by the Remotion
  /// infrastructure, it should not be applied to arbitrary types.
  /// </summary>
  [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
  public class BindableObjectBaseClassAttribute : Attribute
  {
  }
}