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