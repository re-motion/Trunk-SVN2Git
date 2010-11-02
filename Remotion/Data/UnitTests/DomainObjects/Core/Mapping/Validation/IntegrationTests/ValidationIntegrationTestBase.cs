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
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using Remotion.Data.DomainObjects.ConfigurationLoader.ReflectionBasedConfigurationLoader;
using Remotion.Data.DomainObjects.Mapping;
using Remotion.Development.UnitTesting.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery;
using Remotion.Reflection.TypeDiscovery.AssemblyFinding;
using Remotion.Reflection.TypeDiscovery.AssemblyLoading;

namespace Remotion.Data.UnitTests.DomainObjects.Core.Mapping.Validation.IntegrationTests
{
  public class ValidationIntegrationTestBase
  {
    public void ValidateMapping (string testDomainNamespaceSuffix)
    {
      var testDomainNamespace = "Remotion.Data.UnitTests.DomainObjects.Core.Mapping.TestDomain.Errors.ValidationIntegration."+ testDomainNamespaceSuffix;
      new MappingConfiguration (new MappingReflector (GetTypeDiscoveryService (testDomainNamespace, GetType().Assembly)));
    }

    private ITypeDiscoveryService GetTypeDiscoveryService (string testDomainNamespace, params Assembly[] rootAssemblies)
    {
      var rootAssemblyFinder = new FixedRootAssemblyFinder (rootAssemblies.Select (asm => new RootAssembly (asm, true)).ToArray());
      var assemblyLoader = new FilteringAssemblyLoader (ApplicationAssemblyLoaderFilter.Instance);
      var assemblyFinder = new AssemblyFinder (rootAssemblyFinder, assemblyLoader);
      ITypeDiscoveryService typeDiscoveryService = new AssemblyFinderTypeDiscoveryService (assemblyFinder);

      return FilteringTypeDiscoveryService.CreateFromNamespaceWhitelist (typeDiscoveryService, testDomainNamespace);
    }
  }
}