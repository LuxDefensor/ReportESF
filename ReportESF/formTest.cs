using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MyDataControls;

namespace ReportESF
{
    public partial class formTest : Form
    {
        private DataModel d;
        private TreePicker tree;
        public formTest()
        {
            InitializeComponent();
            d = new DataModel();
            tree = new TreePicker();
            tree.Left = 20;
            tree.Top = 20;
            tree.Dock = DockStyle.Fill;
            this.Controls.Add(tree);
            this.Load += FormTest_Load;
            tree.OnNeedChildren += Tree_OnNeedChildren;
            tree.ValueChanged += Tree_ValueChanged;
        }

        private void Tree_ValueChanged(TreeEventArgs<string> e)
        {
            tree.Path = d.PointPath(e != null ? e.SelectedItem.Index : 1, " / ");
        }

        private void Tree_OnNeedChildren(TreeEventArgs<string> e)
        {
            DataTable children = d.GetChildren(e.SelectedItem.Index);
            Node<string>[] nodes = new Node<string>[children.Rows.Count];
            for (int i = 0; i < nodes.Length; i++)
            {
                nodes[i] = new Node<string>()
                {
                    Index = (int)children.Rows[i][0],
                    Parent = e.SelectedItem,
                    Value = children.Rows[i][1].ToString()
                };
            }
            e.SelectedItem.AddChildren(nodes);

        }

        private void FormTest_Load(object sender, EventArgs e)
        {
            List<int> rootIDs = d.GetRoots();
            tree.Roots = rootIDs.Select(r => new Node<string>() { Index = r, Parent = null, Value = d.PointName(r) }).ToArray();
        }
    }
}
