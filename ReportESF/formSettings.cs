using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportESF
{
    public partial class formSettings : Form
    {
        private bool dirty = false;

        public formSettings()
        {
            InitializeComponent();
            this.Load += FormSettings_Load;
            btnClose.Click += BtnClose_Click;
            btnSave.Click += BtnSave_Click;
            txtServer.TextChanged += Setting_Changed;
            txtDatabase.TextChanged += Setting_Changed;
            txtUser.TextChanged += Setting_Changed;
            txtPassword.TextChanged += Setting_Changed;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            Dictionary<string, string> settings = new Dictionary<string, string>();
            settings["server"] = txtServer.Text;
            settings["database"] = txtDatabase.Text;
            settings["user"] = txtUser.Text;
            settings["password"] = txtPassword.Text;
            Settings.SaveSettings(settings);
            dirty = false;
        }

        private void Setting_Changed(object sender, EventArgs e)
        {
            dirty = true;
        }

        private void BtnClose_Click(object sender, EventArgs e)
        {
            DialogResult result = DialogResult.Yes;
            if (dirty)
            {
                result = MessageBox.Show("Закрыть это окно без сохранения внесённых изменений?",
                    "Настройки были изменены", MessageBoxButtons.YesNo,
                    MessageBoxIcon.Warning);
            }
            if (result == DialogResult.Yes)
                this.Close();
        }

        

        private void FormSettings_Load(object sender, EventArgs e)
        {
            Dictionary<string, string> entries = Settings.Entries;
            txtServer.Text = entries["server"];
            txtDatabase.Text = entries["database"];
            txtUser.Text = entries["user"];
            txtPassword.Text = entries["password"];
            dirty = false;
        }
    }
}
