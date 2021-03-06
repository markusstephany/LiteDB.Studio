﻿using System;
using System.Windows.Forms;
using LiteDB.Studio.ICSharpCode.TextEditor.Model;

namespace LiteDB.Studio.Forms
{
    public partial class SettingsForm : Form
    {
        private readonly ApplicationSettings _appsSettings;

        public SettingsForm(ApplicationSettings appsSettings)
        {
            _appsSettings = appsSettings;
            InitializeComponent();
            loadLastDb.Checked = _appsSettings.LoadLastDbOnStartup;
            maxRecentListItems.Value = _appsSettings.MaxRecentListItems;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            _appsSettings.LoadLastDbOnStartup = loadLastDb.Checked;
            _appsSettings.MaxRecentListItems = (int) maxRecentListItems.Value;
        }
    }
}