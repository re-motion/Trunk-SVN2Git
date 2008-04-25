using System;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Remotion.ObjectBinding;
using Remotion.ObjectBinding.Sample;
using Remotion.ObjectBinding.Web.UI.Controls;
using Remotion.Utilities;
using Remotion.Web.UI.Controls;

namespace OBWTest.IndividualControlTests
{

public class BocListAsGridUserControl : BaseUserControl
{
  protected HtmlTable Table3;
  protected BocTextValue FirstNameField;
  protected BocTextValue LastNameField;
  protected TestBocList ChildrenList;
  protected TestBocList EmptyList;
  protected CheckBox ChildrenListEventCheckBox;
  protected Label ChildrenListEventArgsLabel;
  protected FormGridManager FormGridManager;
  protected BindableObjectDataSourceControl EmptyDataSourceControl;
  protected HtmlTable FormGrid;
  protected TestBocListValidator EmptyListValidator;
  protected TestBocList Testboclist1;
  protected BocList AllColumnsList;
  protected HtmlGenericControl NonVisualControls;
  protected WebButton SwitchToEditModeButton;
  protected WebButton EndEditModeButton;
  protected WebButton AddRowButton;
  protected WebButton AddRowsButton;
  protected BocTextValue NumberOfNewRowsField;
  protected WebButton RemoveRows;
  protected WebButton CancelEditModeButton;
  protected BindableObjectDataSourceControl CurrentObject;

  protected override void RegisterEventHandlers ()
  {
    base.RegisterEventHandlers();

    SwitchToEditModeButton.Click += new EventHandler(SwitchToEditModeButton_Click);
    EndEditModeButton.Click += new EventHandler(EndEditModeButton_Click);
    CancelEditModeButton.Click += new EventHandler(CancelEditModeButton_Click);

    AddRowButton.Click += new EventHandler (AddRowButton_Click);
    AddRowsButton.Click += new EventHandler (AddRowsButton_Click);
    RemoveRows.Click += new EventHandler (RemoveRows_Click);
    
    ChildrenList.ListItemCommandClick += new BocListItemCommandClickEventHandler(this.ChildrenList_ListItemCommandClick);
    ChildrenList.MenuItemClick += new WebMenuItemClickEventHandler(this.ChildrenList_MenuItemClick);
    
    ChildrenList.DataRowRender += new BocListDataRowRenderEventHandler(this.ChildrenList_DataRowRender);

    ChildrenList.EditableRowChangesCanceling += new BocListEditableRowChangesEventHandler (ChildrenList_EditableRowChangesCanceling);
    ChildrenList.EditableRowChangesCanceled += new BocListItemEventHandler (ChildrenList_EditableRowChangesCanceled);
    ChildrenList.EditableRowChangesSaving += new BocListEditableRowChangesEventHandler (ChildrenList_EditableRowChangesSaving);
    ChildrenList.EditableRowChangesSaved += new BocListItemEventHandler (ChildrenList_EditableRowChangesSaved);
  }

  public override IBusinessObjectDataSourceControl DataSource
  {
    get { return CurrentObject; }
  }

	override protected void OnInit(EventArgs e)
	{
		InitializeComponent();
    base.OnInit (e);
    InitializeMenuItems();
  }

  private void InitializeMenuItems()
  {
    BocMenuItem menuItem = null;

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Copy";
    menuItem.Category = "Edit";
    menuItem.Text = "Copy";
    menuItem.Icon.Url = "~/Images/CopyItem.gif";
    menuItem.RequiredSelection = RequiredSelection.ExactlyOne;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);
    ChildrenList.OptionsMenuItems.Add (menuItem);

    menuItem = new BocMenuItem();
    menuItem.ItemID = "Paste";
    menuItem.Category = "Edit";
    menuItem.Text = "Paste";
    menuItem.IsDisabled = false;
    menuItem.Command.Type = CommandType.Event;
    ChildrenList.ListMenuItems.Add (menuItem);
    ChildrenList.OptionsMenuItems.Add (menuItem);
  }

  override protected void OnLoad (EventArgs e)
  {
    base.OnLoad (e);

    IBusinessObjectProperty dateOfBirth = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("DateOfBirth");
    IBusinessObjectProperty dateOfDeath = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("DateOfDeath");
    IBusinessObjectProperty height = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Height");
    IBusinessObjectProperty gender = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Gender");
    IBusinessObjectProperty cv = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("CV");
    IBusinessObjectProperty income = CurrentObject.BusinessObjectClass.GetPropertyDefinition ("Income");


    //  Additional columns, in-code generated

    BocSimpleColumnDefinition birthdayColumnDefinition = new BocSimpleColumnDefinition();
    birthdayColumnDefinition.ColumnTitle = "Birthday";
    birthdayColumnDefinition.SetPropertyPath (dateOfBirth.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[]{dateOfBirth}));

    BocSimpleColumnDefinition dayofDeathColumnDefinition = new BocSimpleColumnDefinition();
    dayofDeathColumnDefinition.ColumnTitle = "Day of Death";
    dayofDeathColumnDefinition.SetPropertyPath (dateOfDeath.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[] { dateOfDeath }));
    dayofDeathColumnDefinition.Width = Unit.Parse ("4em");
    dayofDeathColumnDefinition.EnforceWidth = true;

    BocSimpleColumnDefinition heightColumnDefinition = new BocSimpleColumnDefinition();
    heightColumnDefinition.SetPropertyPath (height.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[] { height }));

    BocSimpleColumnDefinition genderColumnDefinition = new BocSimpleColumnDefinition();
    genderColumnDefinition.SetPropertyPath (gender.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[] { gender }));

    BocSimpleColumnDefinition cvColumnDefinition = new BocSimpleColumnDefinition();
    cvColumnDefinition.SetPropertyPath (cv.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[] { cv }));

    BocSimpleColumnDefinition incomeColumnDefinition = new BocSimpleColumnDefinition();
    incomeColumnDefinition.SetPropertyPath (cv.BusinessObjectProvider.CreatePropertyPath (new IBusinessObjectProperty[] { income }));

    BocListView datesView = new BocListView();
    datesView.Title = "Dates";
    datesView.ColumnDefinitions.AddRange (
          new BocColumnDefinition[] {birthdayColumnDefinition, dayofDeathColumnDefinition});

    BocListView statsView = new BocListView();
    statsView.Title = "Stats";
    statsView.ColumnDefinitions.AddRange (
        new BocColumnDefinition[] {heightColumnDefinition, genderColumnDefinition});

    BocListView cvView = new BocListView();
    cvView.Title = "CV";
    cvView.ColumnDefinitions.AddRange (
        new BocColumnDefinition[] {cvColumnDefinition});

    BocListView incomeView = new BocListView();
    incomeView.Title = "Income";
    incomeView.ColumnDefinitions.AddRange (
        new BocColumnDefinition[] {incomeColumnDefinition});

    ChildrenList.AvailableViews.AddRange (new BocListView[] {
      datesView,
      statsView,
      cvView,
      incomeView});

    if (! IsPostBack)
      ChildrenList.SelectedView = datesView;

    if (!IsPostBack)
    {
//      ChildrenList.SetSortingOrder (
//          new BocListSortingOrderEntry[] {
//              new BocListSortingOrderEntry ((BocColumnDefinition) ChildrenList.FixedColumns[7], SortingDirection.Ascending) });
    }

    if (! IsPostBack)
      ChildrenList.SwitchListIntoEditMode();

    if (! IsPostBack)
      NumberOfNewRowsField.Value = 1;
    NumberOfNewRowsField.IsDirty = false;
  }

  protected override void OnPreRender(EventArgs e)
  {
    base.OnPreRender (e);

    SwitchToEditModeButton.Enabled = ! ChildrenList.IsListEditModeActive;
    EndEditModeButton.Enabled = ChildrenList.IsListEditModeActive;
    EndEditModeButton.Enabled = ChildrenList.IsListEditModeActive;
    CancelEditModeButton.Enabled = ChildrenList.IsListEditModeActive;
  }

  private void SwitchToEditModeButton_Click(object sender, EventArgs e)
  {
    ChildrenList.SwitchListIntoEditMode ();
  }

  private void EndEditModeButton_Click(object sender, EventArgs e)
  {
    ChildrenList.EndListEditMode (true);
  }

  private void CancelEditModeButton_Click(object sender, EventArgs e)
  {
    ChildrenList.EndListEditMode (false);
  }

  private void AddRowButton_Click(object sender, EventArgs e)
  {
    Person person = Person.CreateObject (Guid.NewGuid());
    ChildrenList.AddRow ((IBusinessObject) person);
  }

  private void AddRowsButton_Click(object sender, EventArgs e)
  {
    int count = 0;
    
    if (NumberOfNewRowsField.Validate())
      count = (int) NumberOfNewRowsField.Value;

    Person[] persons = new Person[count];
    for (int i = 0; i < count; i++)
      persons[i] = Person.CreateObject (Guid.NewGuid());

    ChildrenList.AddRows ((IBusinessObjectWithIdentity[]) ArrayUtility.Convert (persons, typeof (IBusinessObjectWithIdentity)));
  }

  private void RemoveRows_Click(object sender, EventArgs e)
  {
    IBusinessObject[] selectedBusinessObjects = ChildrenList.GetSelectedBusinessObjects();
    ChildrenList.RemoveRows (selectedBusinessObjects);
  }

  private void ChildrenList_ListItemCommandClick (object sender, BocListItemCommandClickEventArgs e)
  {
    ChildrenListEventCheckBox.Checked = true;
    ChildrenListEventArgsLabel.Text += string.Format ("ColumnID: {0}<br>", e.Column.ItemID);
    if (e.BusinessObject is IBusinessObjectWithIdentity)
      ChildrenListEventArgsLabel.Text += string.Format ("BusinessObjectID: {0}<br>", ((IBusinessObjectWithIdentity) e.BusinessObject).UniqueIdentifier);
    ChildrenListEventArgsLabel.Text += string.Format ("ListIndex: {0}<br>", e.ListIndex);
  }

  private void ChildrenList_MenuItemClick (object sender, WebMenuItemClickEventArgs e)
  {
    ChildrenListEventArgsLabel.Text = e.Item.ItemID;
  }

  private void ChildrenList_DataRowRender (object sender, BocListDataRowRenderEventArgs e)
  {
    if (e.ListIndex == 3)
      e.IsEditableRow = false;
  }

  private void ChildrenList_EditableRowChangesCanceling(object sender, BocListEditableRowChangesEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesCanceled(object sender, BocListItemEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesSaving(object sender, BocListEditableRowChangesEventArgs e)
  {
  }

  private void ChildrenList_EditableRowChangesSaved(object sender, BocListItemEventArgs e)
  {
  }

	#region Web Form Designer generated code	
	/// <summary>
	///		Required method for Designer support - do not modify
	///		the contents of this method with the code editor.
	/// </summary>
	private void InitializeComponent()
	{

  }
  #endregion
}

}
