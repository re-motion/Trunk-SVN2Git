using System;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;
using Remotion.Web.Development.WebTesting.ControlSelection;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.ControlSelection
{
  /// <summary>
  /// Represents a control selection, selecting the control of the given <typeparamref name="TControlObject"/> type representing the specified
  /// domain object within the given scope.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public class PerDomainPropertyControlSelectionCommand<TControlObject> : IControlSelectionCommand<TControlObject>
      where TControlObject : ControlObject
  {
    private readonly IPerDomainPropertyControlSelector<TControlObject> _controlSelector;
    private readonly string _domainProperty;
    private readonly string _domainClass;

    public PerDomainPropertyControlSelectionCommand (
        [NotNull] IPerDomainPropertyControlSelector<TControlObject> controlSelector,
        [NotNull] string domainProperty,
        [CanBeNull] string domainClass = null)
    {
      ArgumentUtility.CheckNotNull ("controlSelector", controlSelector);
      ArgumentUtility.CheckNotNullOrEmpty ("domainProperty", domainProperty);
      ArgumentUtility.CheckNotEmpty ("domainClass", domainClass);

      _controlSelector = controlSelector;
      _domainProperty = domainProperty;
      _domainClass = domainClass;
    }

    public TControlObject Select (ControlSelectionContext context)
    {
      return _controlSelector.SelectPerDomainProperty (context, _domainProperty, _domainClass);
    }
  }
}