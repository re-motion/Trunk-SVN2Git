using System;
using ActaNova.WebTesting.ControlObjects;
using ActaNova.WebTesting.PageObjects;
using JetBrains.Annotations;
using Remotion.Utilities;
using Remotion.Web.Development.WebTesting;

namespace ActaNova.WebTesting.ActaNovaExtensions
{
  /// <summary>
  /// ActaNova-specific extension methods for interacting with the additional commands menu on the form page.
  /// </summary>
  public static class ActaNovaAdditionalCommandsExtensions
  {
    public static ActaNovaSignaturePopupWindowPageObject Abschliessen ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Abschlie&#223;en").ExpectSignaturePage();
    }

    public static ActaNovaSignaturePopupWindowPageObject Umprotokollieren ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Umprotokollieren").ExpectSignaturePage();
    }

    public static ActaNovaMainPageObject Protokollieren ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return
          formPage.SelectItem (
              "Protokollieren zu bestehendem Gesch&#228;ftsfall (<img border=\"0\" title=\"Erledigt zugeordnete Aktivität &#39;Bürgeranliegen prüfen und bearbeiten&#39; in &#39;Eigener AV&#39;\" alt=\"Erledigt zugeordnete Aktivität &#39;Bürgeranliegen prüfen und bearbeiten&#39; in &#39;Eigener AV&#39;\" src=\"/ActaNova_trunk_com/web/res/Rubicon.Workflow.Web/Image/CompletesActivity.gif\" />)",
              Continue.When (Wxe.PostBackCompletedInParent (formPage)))
              .ExpectMainPage();
    }

    public static ActaNovaSignaturePopupWindowPageObject Stornieren ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return
          formPage.SelectItem (
              "Stornieren (<img border=\"0\" title=\"Erledigt zugeordnete Aktivität &#39;Bürgeranliegen prüfen und bearbeiten&#39; in &#39;Eigener AV&#39;\" alt=\"Erledigt zugeordnete Aktivität &#39;Bürgeranliegen prüfen und bearbeiten&#39; in &#39;Eigener AV&#39;\" src=\"/ActaNova_trunk_com/web/res/Rubicon.Workflow.Web/Image/CompletesActivity.gif\" />)")
              .ExpectSignaturePage();
    }

    public static ActaNovaSignaturePopupWindowPageObject Unterschreiben ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Unterschreiben").ExpectSignaturePage();
    }

    public static ActaNovaSignaturePopupWindowPageObject Heranholen ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Heranholen").ExpectSignaturePage();
    }

    public static ActaNovaMainPageObject BarcodeDrucken ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Barcode drucken").Expect<ActaNovaMessageBoxPageObject>().Confirm().ExpectMainPage();
    }

    public static ActaNovaMainPageObject MailVersenden ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Mail versenden", Continue.When (Wxe.PostBackCompletedInParent (formPage))).ExpectMainPage();
    }

    public static ActaNovaSignaturePopupWindowPageObject Sperren ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Sperren").ExpectSignaturePage();
    }

    public static ActaNovaMainPageObject SpeichernUndWeiterleiten ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Speichern und weiterleiten").ExpectMainPage();
    }

    public static ActaNovaMainPageObject Gesamtdruck ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Gesamtdruck", Continue.When (Wxe.PostBackCompletedInParent (formPage))).ExpectMainPage();
    }

    public static ActaNovaPopupWindowPageObject ExportierenNachEDIAKT ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Exportieren (EDIAKT)").ExpectNewPopupWindow<ActaNovaPopupWindowPageObject> ("EDIAKT exportieren");
    }

    public static ActaNovaEditObjectPanelControlObject BesitzUebernehmen ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Besitz &#252;bernehmen").ExpectMainPage().FormPage.GetDialog();
    }

    public static ActaNovaEditObjectPanelControlObject BesitzUebergeben ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Besitz &#252;bergeben").ExpectMainPage().FormPage.GetDialog();
    }

    public static ActaNovaMainPageObject SicherheitsvererbungBearbeiten ([NotNull] this ActaNovaFormPageObject formPage)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);

      return formPage.SelectItem ("Sicherheitsvererbung bearbeiten").ExpectMainPage();
    }

    private static UnspecifiedPageObject SelectItem (
        [NotNull] this ActaNovaFormPageObject formPage,
        [NotNull] string displayText,
        [CanBeNull] ICompletionDetection completionDetection = null)
    {
      ArgumentUtility.CheckNotNull ("formPage", formPage);
      ArgumentUtility.CheckNotNullOrEmpty ("displayText", displayText);

      return formPage.GetAdditionalCommandsMenu().SelectItem().WithDisplayText (displayText, completionDetection);
    }

    private static ActaNovaSignaturePopupWindowPageObject ExpectSignaturePage ([NotNull] this UnspecifiedPageObject page)
    {
      return page.ExpectNewPopupWindow<ActaNovaSignaturePopupWindowPageObject> ("Unterschreiben");
    }
  }
}