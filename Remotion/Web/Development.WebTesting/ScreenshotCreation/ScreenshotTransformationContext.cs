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
using System.Drawing;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ScreenshotCreation
{
  /// <summary>
  /// A context for <see cref="IScreenshotTransformation{T}"/> to apply their transformations on.
  /// </summary>
  public class ScreenshotTransformationContext<T> : IDisposable
  {
    public static ScreenshotTransformationContext<T> CreateForTransformation (
        ScreenshotManipulation manipulation,
        [NotNull] Graphics graphics,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] T target,
        CoordinateSystem coordinateSystem,
        [NotNull] IScreenshotTransformation<T> transformation,
        [NotNull] IBrowserContentLocator locator)
    {
      ArgumentUtility.CheckNotNull ("graphics", graphics);
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("transformation", transformation);
      ArgumentUtility.CheckNotNull ("locator", locator);

      var resolvedElement = Resolve (resolver, target, locator, coordinateSystem);
      var context = new ScreenshotTransformationContext<T> (manipulation, graphics, resolver, target, resolvedElement, transformation);

      context = transformation.BeginApply (context);

      var parentBounds = context.ResolvedElement.ParentBounds;
      if (parentBounds.HasValue)
      {
        return
            context.CloneWith (
                resolvedElement:
                resolvedElement.CloneWith (elementBounds: Rectangle.Intersect (parentBounds.Value, context.ResolvedElement.ElementBounds)));
      }

      return context;
    }

    private readonly ScreenshotManipulation _manipulation;
    private readonly Graphics _graphics;
    private readonly IScreenshotElementResolver<T> _resolver;
    private readonly T _target;
    private readonly ResolvedScreenshotElement _resolvedElement;
    private readonly IScreenshotTransformation<T> _transformation;

    public ScreenshotTransformationContext (
        ScreenshotManipulation manipulation,
        [NotNull] Graphics graphics,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] T target,
        [NotNull] ResolvedScreenshotElement resolvedElement)
        : this (manipulation, graphics, resolver, target, resolvedElement, null)
    {
      ArgumentUtility.CheckNotNull ("graphics", graphics);
      ArgumentUtility.CheckNotNull ("resolver", resolver);
      ArgumentUtility.CheckNotNull ("target", target);
      ArgumentUtility.CheckNotNull ("resolvedElement", resolvedElement);
    }

    private ScreenshotTransformationContext (
        ScreenshotManipulation manipulation,
        [NotNull] Graphics graphics,
        [NotNull] IScreenshotElementResolver<T> resolver,
        [NotNull] T target,
        [NotNull] ResolvedScreenshotElement resolvedElement,
        [CanBeNull] IScreenshotTransformation<T> transformation)
    {
      _manipulation = manipulation;
      _graphics = graphics;
      _resolver = resolver;
      _target = target;
      _resolvedElement = resolvedElement;
      _transformation = transformation;
    }

    /// <summary>
    /// The action that is currently in process.
    /// </summary>
    public ScreenshotManipulation Manipulation
    {
      get { return _manipulation; }
    }

    /// <summary>
    /// The <see cref="System.Drawing.Graphics"/> used to draw the <see cref="IScreenshotAnnotation"/>.
    /// </summary>
    public Graphics Graphics
    {
      get { return _graphics; }
    }

    /// <summary>
    /// The resolver used to resolve <see cref="Target"/> to <see cref="ResolvedElement"/>.
    /// </summary>
    public IScreenshotElementResolver<T> Resolver
    {
      get { return _resolver; }
    }

    /// <summary>
    /// The <see cref="ResolvedScreenshotElement"/> that will be passed to the <see cref="IScreenshotAnnotation"/>.
    /// </summary>
    public ResolvedScreenshotElement ResolvedElement
    {
      get { return _resolvedElement; }
    }

    /// <summary>
    /// The target that was resolved to <see cref="ResolvedElement"/>.
    /// </summary>
    public T Target
    {
      get { return _target; }
    }

    /// <summary>
    /// Clones the current process overriding all specified properties.
    /// </summary>
    public ScreenshotTransformationContext<T> CloneWith (
        IScreenshotElementResolver<T> resolver = null,
        OptionalParameter<T> target = default (OptionalParameter<T>),
        ResolvedScreenshotElement resolvedElement = null,
        IScreenshotTransformation<T> transformation = null)
    {
      if (target.HasValue && target.Value == null)
        throw new ArgumentNullException ("target", "Value of optional parameter cannot be null.");

      return new ScreenshotTransformationContext<T> (
          _manipulation,
          _graphics,
          resolver ?? _resolver,
          Assertion.IsNotNull (target.GetValueOrDefault (_target)),
          resolvedElement ?? _resolvedElement,
          transformation ?? _transformation);
    }

    public void Dispose ()
    {
      _transformation?.EndApply (this);
    }

    private static ResolvedScreenshotElement Resolve (
        IScreenshotElementResolver<T> resolver,
        T target,
        IBrowserContentLocator locator,
        CoordinateSystem coordinateSystem)
    {
      switch (coordinateSystem)
      {
        case CoordinateSystem.Browser:
          return resolver.ResolveBrowserCoordinates (target);
        case CoordinateSystem.Desktop:
          return resolver.ResolveDesktopCoordinates (target, locator);
        default:
          throw new ArgumentOutOfRangeException ("coordinateSystem", coordinateSystem, null);
      }
    }
  }
}