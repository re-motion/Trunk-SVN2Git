using System;
using System.Web.UI;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls.NumericValidatorTests
{
  public class NumericValidatorMock : NumericValidator
  {
    private readonly Control _namingContainer;

    public NumericValidatorMock (Control namingContainer)
    {
      ArgumentUtility.CheckNotNull ("namingContainer", namingContainer);
      _namingContainer = namingContainer;
    }

    public override Control NamingContainer
    {
      get { return _namingContainer; }
    }
  }
}