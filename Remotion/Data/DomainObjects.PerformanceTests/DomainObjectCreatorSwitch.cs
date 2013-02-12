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
using Remotion.Data.DomainObjects.Infrastructure;
using Remotion.Data.DomainObjects.Infrastructure.ObjectLifetime;
using Remotion.Data.DomainObjects.Infrastructure.TypePipe;
using Remotion.Reflection;
using Remotion.ServiceLocation;
using Remotion.TypePipe;

namespace Remotion.Data.DomainObjects.PerformanceTests
{
  // TODO 5370: This class is a temporary hack for using the old code generation in tests that need serialization/deserialization capabilities.
  public class DomainObjectCreatorSwitch : IDomainObjectCreator
  {
    public static bool UseLegacyCodeGeneration { get; set; }

    private readonly InterceptedDomainObjectCreator _legacyCreator = InterceptedDomainObjectCreator.Instance;
    private readonly TypePipeBasedDomainObjectCreator _typePipeCreator =
        new TypePipeBasedDomainObjectCreator (SafeServiceLocator.Current.GetInstance<IObjectFactory>());

    public DomainObject CreateObjectReference (IObjectInitializationContext objectInitializationContext, ClientTransaction clientTransaction)
    {
      return SelectCreator().CreateObjectReference (objectInitializationContext, clientTransaction);
    }

    public DomainObject CreateNewObject (IObjectInitializationContext objectInitializationContext, ParamList constructorParameters, ClientTransaction clientTransaction)
    {
      return SelectCreator().CreateNewObject (objectInitializationContext, constructorParameters, clientTransaction);
    }

    private IDomainObjectCreator SelectCreator ()
    {
      return UseLegacyCodeGeneration ? (IDomainObjectCreator) _legacyCreator : _typePipeCreator;
    }
  }
}