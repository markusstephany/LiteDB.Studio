﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using LiteDB.Studio.ICSharpCode.TextEditor.Util;

namespace LiteDB.Studio.Forms
{
    public partial class ConnectionForm : Form
    {
        private const long MB = 1024 * 1024;

        public ConnectionString ConnectionString = new ConnectionString();

        public ConnectionForm(ConnectionString cs)
        {
            InitializeComponent();

            txtFilename.Text = cs.Filename;
            chkReadonly.Checked = cs.ReadOnly;
            txtInitialSize.Text = (cs.InitialSize / MB).ToString();

            chkUpgrade.Checked = cs.Upgrade;

            cmbCulture.DataSource =
                CultureInfo.GetCultures(CultureTypes.AllCultures)
                    .Select(x => x.LCID)
                    .Distinct()
                    .Where(x => x != 4096)
                    .Select(x => CultureInfo.GetCultureInfo(x).Name)
                    .ToList();

            var sort = new List<string> {""};
            sort.AddRange(Enum.GetNames(typeof(CompareOptions)));

            cmbSort.DataSource = sort;

            if (cs.Collation != null)
            {
                cmbCulture.SelectedIndex = cmbCulture.FindString(cs.Collation.Culture.Name);
                cmbSort.SelectedIndex = cmbSort.FindString(cs.Collation.SortOptions.ToString());
            }
        }

        private void BtnConnect_Click(object sender, EventArgs e)
        {
            ConnectionString.Connection =
                radModeDirect.Checked ? ConnectionType.Direct :
                radModeShared.Checked ? ConnectionType.Shared : ConnectionType.Direct;

            ConnectionString.Filename = txtFilename.Text;
            ConnectionString.ReadOnly = chkReadonly.Checked;
            ConnectionString.Upgrade = chkUpgrade.Checked;
            ConnectionString.Password = txtPassword.Text.Trim().Length > 0 ? txtPassword.Text.Trim() : null;

            if (int.TryParse(txtInitialSize.Text, out var initialSize)) ConnectionString.InitialSize = initialSize * MB;

            if (cmbCulture.SelectedIndex > 0)
            {
                var collation = cmbCulture.SelectedItem.ToString();

                if (cmbSort.SelectedIndex > 0) collation += "/" + cmbSort.SelectedItem;

                ConnectionString.Collation = new Collation(collation);
            }

            DialogResult = DialogResult.OK;
            // make it last db
            AppSettingsManager.ApplicationSettings.LastConnectionStrings = ConnectionString;
            // add to recent list
            AppSettingsManager.AddToRecentList(ConnectionString);
            Close();
        }

        private void BtnOpen_Click(object sender, EventArgs e)
        {
            openFileDialog.FileName = txtFilename.Text;

            openFileDialog.CheckFileExists = false;

            if (openFileDialog.ShowDialog() == DialogResult.OK) txtFilename.Text = openFileDialog.FileName;
        }

        private void ChkReadonly_CheckedChanged(object sender, EventArgs e)
        {
            if (chkReadonly.Checked)
            {
                chkUpgrade.Checked = false;
                chkUpgrade.Enabled = false;
            }
            else
            {
                chkUpgrade.Enabled = true;
            }
        }
    }
}