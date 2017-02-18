using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ReportESF
{
    public partial class formInputBox : Form
    {
        private const string defaultTitle = "Введите значение";

        public formInputBox() : this(defaultTitle, string.Empty)
        {
        }

        public formInputBox(string defaultValue) : this(defaultTitle, defaultValue)
        {

        }

        public formInputBox(string title, string defaultValue)
        {
            InitializeComponent();
            this.Text = title;
            txtInput.Text = defaultValue;
        }

        public string Result
        {
        get
            {
                return txtInput.Text;
            }
        }
    }
}
