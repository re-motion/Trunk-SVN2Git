using System;
using JetBrains.Annotations;
using Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.FluentControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.FluentControlSelection
{
  /// <summary>
  /// Default extension methods for all re-motion-provided <see cref="IControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>
  /// implementations.
  /// </summary>
  public static class FluentControlSelectorExtensionsForObjectBinding
  {
    /// <summary>
    /// Extension method for selecting a control by the domain property it represetns (using the
    /// <see cref="PerDomainPropertyControlSelectionCommandBuilder{TControlSelector,TControlObject}"/>).
    /// </summary>
    public static TControlObject ByDomainProperty<TControlSelector, TControlObject> (
        [NotNull] this IFluentControlSelector<TControlSelector, TControlObject> fluentControlSelector,
        [NotNull] string domainProperty,
        [CanBeNull] string domainClass = null)
        where TControlSelector : IPerDomainPropertyControlSelector<TControlObject>
        where TControlObject : ControlObject
    {
      ArgumentUtility.CheckNotNull ("fluentControlSelector", fluentControlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty ("domainClass", domainClass);

      return fluentControlSelector.GetControl (
          new PerDomainPropertyControlSelectionCommandBuilder<TControlSelector, TControlObject> (domainProperty, domainClass));
    }
  }
}