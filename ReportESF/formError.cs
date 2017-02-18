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
    public partial class formError : Form
    {
        private const string defaultTitle = "Ошибка!";
        private const int defaultHeight = 168;
        private const int expandedHeight = 448;
        private bool expanded = false;

        public formError(string message, string title, string details)
        {
            InitializeComponent();
            this.Height = defaultHeight;
            txtMessage.Text = message;
            this.Text = title;
            txtDetails.Text = details;
            btnDetails.Visible = true;
            btnDetails.Click += BtnDetails_Click;
            btnOK.Click += BtnOK_Click;
        }

        private void BtnOK_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtnDetails_Click(object sender, EventArgs e)
        {
            if (expanded)
            {
                this.Height = defaultHeight;
                btnDetails.Text = "Подробно";
            }
            else
            {
                this.Height = expandedHeight;
                btnDetails.Text = "Скрыть";
            }
            expanded = !expanded;
        }

        public formError(string message, string title)
            : this(message, title, string.Empty)
        {
            btnDetails.Visible = false;
        }

        public formError(string message)
            : this(message, defaultTitle)
        {

        }

        
    }
}
