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
using Remotion.TypePipe.Caching;
using Remotion.Utilities;

namespace Remotion.Data.DomainObjects.Infrastructure.TypePipe
{
  /// <summary>
  /// The <see cref="ICacheKeyProvider"/> used by the <see cref="DomainObjectParticipant"/>.
  /// </summary>
  public class DomainObjectParticipantCacheKeyProvider : ICacheKeyProvider
  {
    private readonly ITypeDefinitionProvider _typeDefinitionProvider;

    public DomainObjectParticipantCacheKeyProvider (ITypeDefinitionProvider typeDefinitionProvider)
    {
      ArgumentUtility.CheckNotNull ("typeDefinitionProvider", typeDefinitionProvider);

      _typeDefinitionProvider = typeDefinitionProvider;
    }

    public ITypeDefinitionProvider TypeDefinitionProvider
    {
      get { return _typeDefinitionProvider; }
    }

    public object GetCacheKey (Type requestedType)
    {
      ArgumentUtility.CheckNotNull ("requestedType", requestedType);

      // TODO 5370: This will change when TypePipe is integrated with re-mix.
      var domainObjectType = _typeDefinitionProvider.GetPublicDomainObjectType (requestedType);
      var classDefinition = _typeDefinitionProvider.GetTypeDefinition (domainObjectType);

      return classDefinition;
    }

    public object RebuildCacheKey (Type generatedProxyType)
    {
      ArgumentUtility.CheckNotNull ("generatedProxyType", generatedProxyType);

      // TODO Review2!

      // TODO 5370: This will (maybe??) change when TypePipe is integrated with re-mix.
      var domainObjectType = _typeDefinitionProvider.GetPublicDomainObjectType (generatedProxyType);
      var classDefinition = _typeDefinitionProvider.GetTypeDefinition (domainObjectType);

      return classDefinition;
    }
  }
}