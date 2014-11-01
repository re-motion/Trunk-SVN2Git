using System;
using Coypu;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Contract.DiagnosticMetadata;
using Remotion.Web.Development.WebTesting.ControlSelection;
using Remotion.Web.Development.WebTesting.Utilities;

namespace Remotion.Web.Development.WebTesting.ControlObjects.Selectors
{
  /// <summary>
  /// Base implementation for <see cref="ControlObject"/> selector implementations which can identify the <typeparamref name="TControlObject"/> via
  /// <see cref="DiagnosticMetadataAttributes.ControlType"/> metadata.
  /// </summary>
  /// <typeparam name="TControlObject">The specific <see cref="ControlObject"/> type to select.</typeparam>
  public abstract class TypedControlSelectorBase<TControlObject>
      : ControlSelectorBase<TControlObject>,
          IFirstControlSelector<TControlObject>,
          IPerIndexControlSelector<TControlObject>,
          ISingleControlSelector<TControlObject>
      where TControlObject : RemotionControlObject
  {
    private readonly string _controlType;

    /// <param name="controlType">The <see cref="DiagnosticMetadataAttributes.ControlType"/> identifying the <typeparamref name="TControlObject"/>.</param>
    protected TypedControlSelectorBase ([NotNull] string controlType)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("controlType", controlType);

      _controlType = controlType;
    }

    /// <summary>
    /// Returns the <see cref="DiagnosticMetadataAttributes.ControlType"/> identifying the <typeparamref name="TControlObject"/>.
    /// </summary>
    protected string ControlType
    {
      get { return _controlType; }
    }

    public TControlObject SelectFirst (WebTestObjectContext context)
    {
      var scope = context.Scope.FindDMA ("*", DiagnosticMetadataAttributes.ControlType, _controlType);
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectSingle (WebTestObjectContext context)
    {
      var scope = context.Scope.FindDMA ("*", DiagnosticMetadataAttributes.ControlType, _controlType);
      scope.ElementFinder.Options.Match = Match.Single;
      return CreateControlObject (context, scope);
    }

    public TControlObject SelectPerIndex (WebTestObjectContext context, int index)
    {
      var hasAttributeCheck = XPathUtils.CreateHasAttributeCheck (DiagnosticMetadataAttributes.ControlType, _controlType);
      var scope = context.Scope.FindXPath (string.Format ("(.//*{0})[{1}]", hasAttributeCheck, index));
      return CreateControlObject (context, scope);
    }
  }
}