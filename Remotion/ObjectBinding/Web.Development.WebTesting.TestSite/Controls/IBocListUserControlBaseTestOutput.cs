using System;

namespace Remotion.ObjectBinding.Web.Development.WebTesting.TestSite.Controls
{
  public interface IBocListUserControlBaseTestOutput
  {
    void SetActionPerformed (string bocListId, int rowIndex, string action, string parameter);
  }
}