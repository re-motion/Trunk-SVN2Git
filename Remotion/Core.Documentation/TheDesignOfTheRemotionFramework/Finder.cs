// This file is part of the re-motion Core Framework (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
using System.Collections.Generic;
using Remotion.Reflection;
using Remotion.Security;
using Remotion.SecurityManager.Domain.AccessControl;

namespace Core.Documentation.TheDesignOfTheRemotionFramework
{
  internal partial class AaaInfoAboutTheDesignOfTheRemotionFramework
  {
    /// <summary>
    /// Finder classes (e.g. <see cref="AccessControlListFinder"/>, <see cref="AssemblyFinder"/>) have the
    /// following characteristics:
    /// <list type="number">
    /// <item>
    /// Finder classes are called "SomethingFinder" (e.g. <see cref="AccessControlListFinder"/>).
    /// </item>
    /// <item>
    /// The methods executing the searches are called "FindSomething" or just "Find" 
    /// (e.g. <see cref="AccessControlListFinder"/>.<see cref="AccessControlListFinder.Find(Remotion.Data.DomainObjects.ClientTransaction,ISecurityContext)"/>
    /// <see cref="AssemblyFinder.FindAssemblies"/>).
    /// </item>
    /// <item>
    /// The search methods execute potentially costly searches, returning a collection of the found objects.
    /// This collection is normally of type <see cref="Array"/> or <see cref="List{T}"/>.
    /// </item>
    /// <item>
    /// They do not cache the search result, i.e. every call to a Find-method initiates a new search.
    /// Classes are expected to cache the resulting collection themselves, if required.
    /// </item>
    /// </list>
    /// </summary>
    private class Finder {}
  }
}
