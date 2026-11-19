using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApplication.Common;
using Eureka.Stroke;
using Eureka.Stroke.FormatService;

namespace WebApplication
{

public partial class FormatManagerControl : System.Web.UI.UserControl
{
    private Format __format;
    private Format _format
    {
        get
        {
            if (null == __format)
            {
                var formatID = Utils.ToInt(ddlFormat.SelectedValue);
                var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
                __format = formatService.GetFormatByID(formatID);
            }
            return __format;
        }
    }

    protected void Page_Load(object sender, EventArgs e)
    {
        // there is not error or message by default on the page
        boxError.Visible = false;
        boxMessage.Visible = false;
    }

    #region common

    /// <summary>
    /// Show some error message in error box on the page
    /// </summary>
    /// <param name="error"></param>
    protected void ShowError(string error)
    {
        boxError.Text = error;
        boxError.Visible = true;
    }

    /// <summary>
    /// Show some non-error message in message box on the page
    /// </summary>
    /// <param name="error"></param>
    protected void ShowMessage(string error)
    {
        boxMessage.Text = error;
        boxMessage.Visible = true;
    }

    /// <summary>
    /// Catch exception (error) in business layer on the DataSource object level
    /// </summary>
    /// <param name="e"></param>
    protected bool HasDataSourceException(ObjectDataSourceStatusEventArgs e)
    {
        if (null != e.Exception)
        {
            e.ExceptionHandled = true;
            ShowError(e.Exception.InnerException.Message);
            return true;
        }
        return false;
    }

    private static void SetDeleteConfirmation(TableRow row)
    {
        foreach (Control c in row.Cells[0].Controls)
        {
            if (c is LinkButton)
            {
                var btn = (LinkButton)c;
                if (btn.CommandName == DataControlCommands.DeleteCommandName)
                {
                    btn.OnClientClick = "return confirm('Are you sure you want to delete this item?');";
                }
            }
        }
    }

    #endregion

    #region formats

    protected void UpdateCurrentFormat(int formatID)
    {
        btnDeleteFormat.Visible = false;

        if (0 == formatID)
        {
            formatID = Utils.ToInt(ddlFormat.SelectedValue);
        }
        var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
        var format = formatService.GetFormatByID(formatID);
        if (null != format)
        {
            if (0 == format.Status)
                btnDeleteFormat.Visible = true;
        }

    }

    protected void ddlFormat_SelectedIndexChanged(object sender, EventArgs e)
    {
        UpdateCurrentFormat(0);
        RefreshFields();
    }

    
    protected void ddlFormat_Selected(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        HasDataSourceException(e);
    }

    protected void ddlFormat_DataBound(object sender, EventArgs e)
    {
        UpdateCurrentFormat(0);
    }

    /// <summary>
    /// 'Add new format' button handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnAddFormat_Click(object sender, EventArgs e)
    {
        var newFormatName = txtNewFormat.Text.Trim();
        if (string.IsNullOrEmpty(newFormatName))
        {
            ShowError("Provide some unique Name for new Format.");
            return;
        }

        var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

        var format = formatService.AddFormat(newFormatName);
        if (null == format)
        {
            ShowError(formatService.ErrorMessage);
            return;
        }

        ShowMessage("New format added");

        ddlFormat.DataBind();
        ddlFormat.SelectedValue = format.ID.ToString();

        UpdateCurrentFormat(format.ID);
        RefreshFields();
    }

    /// <summary>
    /// 'Delete New format' button handler
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btnDeleteFormat_Click(object sender, EventArgs e)
    {
        // get format service
        var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();

        // disable current format by ID
        var formatID = Utils.ToShort(ddlFormat.SelectedValue);
        if (formatID > 0)
        {
            formatService.DisableFormat(formatID);
            if (3 == formatService.ErrorCode)
                ShowError(formatService.ErrorMessage);
            else
                ShowMessage("Format was deleted");
        }

        // refresh formats drop-down list
        ddlFormat.DataBind();
    }

    #endregion

    #region eureka fields

    protected void RefreshFields()
    {
        gridFields.EditIndex = -1;
        gridFields.SelectedIndex = -1;
        gridFields.DataBind();

        panelFieldDetails.Visible = false;
    }

    protected void UpdateFields()
    {
        var index = gridFields.SelectedIndex;
        gridFields.DataBind();
        gridFields.EditIndex = -1;
        gridFields.SelectedIndex = index;
    }

    protected void RefreshFieldDetails()
    {
        RefreshValues();
        RefreshAlienFields();

        panelFieldDetails.Visible = true;
    }

    protected void gridFields_DataBound(object sender, EventArgs e)
    {
        // check if there is some eureka field selected to show mapping details for it
        // or hide those details in other case
        if (gridFields.SelectedIndex == -1 || gridFields.Rows.Count <= gridFields.SelectedIndex)
        {
            gridFields.SelectedIndex = -1;
            panelFieldDetails.Visible = false;
        } else
        {
            RefreshFieldDetails();
        }
    }

    protected void gridFields_RowDataBound(Object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((e.Row.RowState & (DataControlRowState.Edit)) == 0)
                for (var i = 1; i < e.Row.Cells.Count; ++i)
                    e.Row.Cells[i].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gridFields, "Select$" + e.Row.RowIndex);

            var data = (EurekaField)e.Row.DataItem;

            
            if ((e.Row.RowState & (DataControlRowState.Edit)) != 0)
            {
                var chKeyUpdate = (CheckBox)e.Row.FindControl("chKeyUpdate");
                if (null != chKeyUpdate)
                {
                    var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
                    if (formatService.HasAlienFields(data.ID, _format.ID))
                    {
                        chKeyUpdate.Visible = true;
                        chKeyUpdate.Checked = _format.KeyEurekaField.Contains(data);
                    } else
                    {
                        chKeyUpdate.Visible = false;
                    }

                }
            }
            

            /*
            if ((from k in _format.KeyEurekaField
                where k.ID == data.ID
                select k.ID).Count() > 0)
            {
                var lblKeyEurekaField = (Label) e.Row.FindControl("lblKeyEurekaField");
                if (null != lblKeyEurekaField)
                    lblKeyEurekaField.Text = "Key";
            }
            */

            // for fields with already saved data
            if (!data.IsNew)
            {
                // disable Delete action
                var control = e.Row.FindControl("btDelete");
                if (null != control)
                    control.Visible = false;

                // disable type drop-down
                control = e.Row.FindControl("ddlTypeUpdate");
                if (null != control)
                    //((DropDownList)control).Attributes.Add("disabled", "disabled");
                    control.Visible = false;

                // and show type as label
                control = e.Row.FindControl("labelTypeUpdate");
                if (null != control)
                    ((Label)control).Text = FieldType.Types[data.FieldType].ToString();

            }
        }

    }

    protected void gridFields_SelectedIndexChanged(Object sender, EventArgs e)
    {
        gridFields.EditIndex = -1;

        RefreshFieldDetails();
    }

    protected void gridFields_RowEditing(Object sender, GridViewEditEventArgs e)
    {
        gridFields.SelectedIndex = e.NewEditIndex;

        RefreshFieldDetails();
    }

    protected void gridFields_RowDeleting(Object sender, GridViewDeleteEventArgs e)
    {
        // check if eureka field is still new one
        var eurekaFieldID = gridFields.DataKeys[e.RowIndex].Value.ToInt();
        if (0 != eurekaFieldID)
        {
            var formatService = Eureka.Stroke.Container.DIContainer.Resolve<IFormatService>();
            var field = formatService.GetEurekaFieldByID(eurekaFieldID);
            if (null != field)
            {
                if (field.IsNew) // allow to delete as it's new one
                    return;
            }
        }
        e.Cancel = true; // cancel delete

        // hide Delete action link
        gridFields.Rows[e.RowIndex].FindControl("btDelete").Visible = false;
    }

    protected void gridFields_RowCreated(Object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            SetDeleteConfirmation(e.Row);
        }
    }

    protected void gridFields_RowDeleted(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;

        ShowMessage("Eureka Field was deleted");

        RefreshFields();
    }

    protected void gridFields_Selected(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        HasDataSourceException(e);
    }

    protected void gridFields_RowUpdated(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;

        ShowMessage("Eureka Field was updated");
    }

    /// <summary>
    /// Insert new eureka field record as custom action
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void btInsert_Click(object sender, EventArgs e)
    {
        // call datasource method for insertion
        dsEurekaFields.Insert();
    }

    protected void gridFields_RowUpdating(object sender, ObjectDataSourceMethodEventArgs e)
    {
        if (null != e.InputParameters["Name"])
            e.InputParameters["Name"] = e.InputParameters["Name"].ToString().Trim();
        if (null != e.InputParameters["Description"])
            e.InputParameters["Description"] = e.InputParameters["Description"].ToString().Trim();

        e.InputParameters["keyFormatID"] = _format.ID;
        e.InputParameters["isKeyField"] = ((CheckBox)gridFields.SelectedRow.FindControl("chKeyUpdate")).Checked;

        /*
        if (null != e.InputParameters["IsKeyField"])
            e.InputParameters["isKeyField"] = e.InputParameters["Name"].ToString().Trim();
        */
        /*
        e.AffectedRows()
        var chKeyUpdate = (CheckBox)e.Row.FindControl("chKeyUpdate");
        if (null != chKeyUpdate)
        */

    }

    /// <summary>
    /// Before call insertion method of datasource
    /// Let's copy data values from custom Insertion form to datasource
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gridFields_RowInserting(Object sender, ObjectDataSourceMethodEventArgs e)
    {
        // get footer row with Insert form fields
        TableRow row = gridFields.FooterRow;
        if (null == row)
        {
            var table = (Table)gridFields.FindControl("table");
            row = table.Rows[table.Rows.Count - 1];
        }

        // move the data from fields to datasource
        e.InputParameters["Type"] = ((DropDownList)row.FindControl("ddlTypeInsert")).SelectedValue;
        e.InputParameters["Name"] = ((TextBox) row.FindControl("txtEurekaFieldNameInsert")).Text.Trim();
        e.InputParameters["Description"] = ((TextBox) row.FindControl("txtEurekaFieldDescriptionInsert")).Text.Trim();
    }

    /// <summary>
    /// After Insertion method call
    /// we need to check was it ok or not by Excpetion provided
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    protected void gridFields_RowInserted(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;
        ShowMessage("New Eureka Field was added");

        // insertion was ok
        // so, navigate current record to that inserted row
        // and refresh the grid
        var RowIndex = e.OutputParameters["RowIndex"].ToInt() - 1;
        gridFields.PageIndex = RowIndex / gridFields.PageSize;
        gridFields.EditIndex = -1;
        gridFields.SelectedIndex = RowIndex % gridFields.PageSize;
        gridFields.DataBind();
        RefreshFieldDetails();
    }

    #endregion

    #region alien fields

    protected void gridAlienFields_RowCreated(Object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            SetDeleteConfirmation(e.Row);
        }
    }
    protected void gridAlienFields_RowDataBound(Object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((e.Row.RowState & (DataControlRowState.Edit)) == 0)
                for (var i = 1; i < e.Row.Cells.Count; ++i)
                    e.Row.Cells[i].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gridAlienFields, "Edit$" + e.Row.RowIndex);
        }
    }

    protected void gridAlienFields_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        HasDataSourceException(e);
    }

    protected void gridAlienFields_RowEditing(Object sender, GridViewEditEventArgs e)
    {
        //gridValues.EditIndex = -1;
    }
    protected void RefreshAlienFields()
    {
        gridAlienFields.EditIndex = -1;
        gridAlienFields.SelectedIndex = -1;
        gridAlienFields.DataBind();
    }
    protected void UpdateAlienFields()
    {
        //UpdateFields();
    }

    protected void gridAlienFields_RowDeleted(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;
        ShowMessage("Alien Field was deleted");
        UpdateAlienFields();
    }

    protected void gridAlienFields_RowUpdating(Object sender, ObjectDataSourceMethodEventArgs e)
    {
        if (null != e.InputParameters["Name"])
            e.InputParameters["Name"] = e.InputParameters["Name"].ToString().Trim();
        if (null != e.InputParameters["RegularExpression"])
            e.InputParameters["RegularExpression"] = e.InputParameters["RegularExpression"].ToString().Trim();
    }
    protected void gridAlienFields_RowUpdated(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;
        ShowMessage("Alien Field was updated");
        UpdateAlienFields();
    }
    protected void gridAlienFields_RowInserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;
        ShowMessage("New Alien Field was added");
        UpdateAlienFields();
    }

    protected void btInsertAlien_Click(object sender, EventArgs e)
    {
        dsAlienFields.Insert();
    }
    protected void gridAlienFields_RowInserting(Object sender, ObjectDataSourceMethodEventArgs e)
    {
        TableRow row = gridAlienFields.FooterRow;
        if (null == row)
        {
            var table = (Table) gridAlienFields.FindControl("table");
            row = table.Rows[table.Rows.Count - 1];
        }
        e.InputParameters["Name"] = ((TextBox) row.FindControl("txtAlienFieldInsert")).Text.Trim();
        e.InputParameters["RegularExpression"] = ((TextBox)row.FindControl("txtRegexpInsert")).Text.Trim();
    }

    #endregion

    #region values

    protected void gridValues_RowCreated(Object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            SetDeleteConfirmation(e.Row);
        }
    }

    protected void gridValues_RowDataBound(Object sender, GridViewRowEventArgs e)
    {
        if (e.Row.RowType == DataControlRowType.DataRow)
        {
            if ((e.Row.RowState & (DataControlRowState.Edit)) == 0)
                for (var i = 1; i < e.Row.Cells.Count; ++i)
                    e.Row.Cells[i].Attributes["onclick"] = Page.ClientScript.GetPostBackClientHyperlink(gridValues, "Edit$" + e.Row.RowIndex);
        }
    }

    protected void gridValues_Selected(object sender, ObjectDataSourceStatusEventArgs e)
    {
        HasDataSourceException(e);
    }

    protected void gridValues_RowEditing(Object sender, GridViewEditEventArgs e)
    {
        //gridAlienFields.EditIndex = -1;
    }

    protected void RefreshValues()
    {
        gridValues.EditIndex = -1;
        gridValues.SelectedIndex = -1;
        gridValues.DataBind();
    }

    protected void UpdateValues()
    {
        //UpdateFields();
    }

    protected void gridValues_RowDeleted(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;
        ShowMessage("Values mapping record was deleted");
        UpdateValues();
    }

    protected void gridValues_RowUpdating(Object sender, ObjectDataSourceMethodEventArgs e)
    {
        if (null != e.InputParameters["EurekaValue"])
            e.InputParameters["EurekaValue"] = e.InputParameters["EurekaValue"].ToString().Trim();
        if (null != e.InputParameters["AlienValue"])
            e.InputParameters["AlienValue"] = e.InputParameters["AlienValue"].ToString().Trim();
        if (null != e.InputParameters["EurekaDescription"])
            e.InputParameters["EurekaDescription"] = e.InputParameters["EurekaDescription"].ToString().Trim();
        if (null != e.InputParameters["AlienDescription"])
            e.InputParameters["AlienDescription"] = e.InputParameters["AlienDescription"].ToString().Trim();
    }
    protected void gridValues_RowUpdated(Object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;
        ShowMessage("Values mapping record was updated");
        UpdateValues();
    }

    protected void gridValues_RowInserted(object sender, ObjectDataSourceStatusEventArgs e)
    {
        if (HasDataSourceException(e))
            return;
        ShowMessage("Values mapping record was added");
        UpdateValues();
    }

    protected void btInsertValue_Click(object sender, EventArgs e)
    {
        dsValuesList.Insert();
    }

    protected void gridValues_RowInserting(Object sender, ObjectDataSourceMethodEventArgs e)
    {
        TableRow row = gridValues.FooterRow;
        if (null == row)
        {
            var table = (Table)gridValues.FindControl("table");
            row = table.Rows[table.Rows.Count - 1];
        }

        e.InputParameters["EurekaValue"] = ((TextBox)row.FindControl("txtEurekaValueInsert")).Text.Trim();
        e.InputParameters["AlienValue"] = ((TextBox)row.FindControl("txtAlienValueInsert")).Text.Trim();
        e.InputParameters["EurekaDescription"] = ((TextBox)row.FindControl("txtEurekaDescriptionInsert")).Text.Trim();
        e.InputParameters["AlienDescription"] = ((TextBox)row.FindControl("txtAlienDescriptionInsert")).Text.Trim();
    }

    #endregion

}

}