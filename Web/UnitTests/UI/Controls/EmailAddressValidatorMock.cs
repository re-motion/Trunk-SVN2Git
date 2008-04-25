using System;
using System.ComponentModel;
using Remotion.Web.UI.Controls;

namespace Remotion.Web.UnitTests.UI.Controls
{

/// <summary> Exposes non-public members of the <see cref="EmailAddressValidator"/> type. </summary>
[ToolboxItem (false)]
public class EmailAddressValidatorMock: EmailAddressValidator
{
	public new bool IsMatchComplete (string text)
  {
    return base.IsMatchComplete (text);
  }

	public new bool IsMatchUserPart (string text)
  {
    return base.IsMatchUserPart (text);
  }

	public new bool IsMatchDomainPart (string text)
  {
    return base.IsMatchDomainPart (text);
  }
}

}
