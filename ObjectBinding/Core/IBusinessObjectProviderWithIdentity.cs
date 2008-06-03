/* Copyright (C) 2005 - 2008 rubicon informationstechnologie gmbh
 *
 * This program is free software: you can redistribute it and/or modify it under 
 * the terms of the re:motion license agreement in license.txt. If you did not 
 * receive it, please visit http://www.re-motion.org/licensing.
 * 
 * Unless otherwise provided, this software is distributed on an "AS IS" basis, 
 * WITHOUT WARRANTY OF ANY KIND, either express or implied. 
 */

namespace Remotion.ObjectBinding
{
  /// <summary>
  /// The <see cref="IBusinessObjectProviderWithIdentity"/> interface defines a getter and a set-method to associate a 
  /// <see cref="BusinessObjectProviderAttribute"/> with the provider. Through this attribute it is possible to identify the relationship
  /// between a provider and its object model.
  /// </summary>
  public interface IBusinessObjectProviderWithIdentity : IBusinessObjectProvider
  {
    BusinessObjectProviderAttribute ProviderAttribute { get; }
  }
}
