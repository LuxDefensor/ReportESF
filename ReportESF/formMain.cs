using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ReportESF
{
    public partial class formMain: Form
    {
        private DataModel d;
        private XLSExport xl;
        private List<string> selected;
        private bool processChecks = true;
        private char[] invalidChars = Path.GetInvalidFileNameChars();
        private List<TreeNode> found;
        private string lastSearch = string.Empty;

        public formMain()
        {
            InitializeComponent();
            selected = new List<string>();
            found = new List<TreeNode>();
            d = new DataModel();
            xl = new XLSExport();
            this.Load += FormMain_Load;
            calFrom.DateChanged += CalFrom_DateChanged;
            calTill.DateChanged += CalTill_DateChanged;
            btnSettings.Click += BtnSettings_Click;
            treePoints.AfterCheck += TreePoints_AfterCheck;
            btnSelectAll.Click += BtnSelectAll_Click;
            btnDeselectAll.Click += BtnDeselectAll_Click;
            btnSavePreset.Click += BtnSavePreset_Click;
            btnDeletePreset.Click += BtnDeletePreset_Click;
            btnSearch.Click += BtnSearch_Click;
            btnFindNext.Click += BtnFindNext_Click;
            btn2Excel.Click += Btn2Excel_Click;
            lstPresets.DoubleClick += LstPresets_DoubleClick;
            btnCheck.Click += BtnCheck_Click;
            lstReports.SelectedIndexChanged += LstReports_SelectedIndexChanged;
        }

        private void LstReports_SelectedIndexChanged(object sender, EventArgs e)
        {
            ClearCheck();
        }

        private void ClearCheck()
        {
            dgvCheck.DataSource = "";
        }

        private void BtnCheck_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            if (calFrom.SelectionStart <= calTill.SelectionStart)
            {
                switch (lstReports.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                        dgvCheck.DataSource = d.GetPercentMains(selected, calFrom.SelectionStart, calTill.SelectionStart);
                        break;
                    case 5:
                        dgvCheck.DataSource = d.GetPercentNIs(selected, calFrom.SelectionStart, calTill.SelectionStart);
                        break;
                    case 6:
                        dgvCheck.DataSource = d.GetPercentLogs(selected, calFrom.SelectionStart, calTill.SelectionStart);
                        break;
                    default:
                        dgvCheck.DataSource = "";
                        return;
                }
                dgvCheck.Columns[0].FillWeight = 6;
                dgvCheck.Columns[1].FillWeight = 6;
                dgvCheck.Columns[2].FillWeight = 1;
                dgvCheck.Columns[3].FillWeight = 3;
                dgvCheck.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCheck.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCheck.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCheck.Columns[3].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                dgvCheck.Columns[0].HeaderText = "Подстанция";
                dgvCheck.Columns[1].HeaderText = "Присоединение";
                dgvCheck.Columns[2].HeaderText = "%";
                dgvCheck.Columns[3].HeaderText = "Дата последнего значения";

            }
            this.Cursor = Cursors.Default;
        }

        private void Btn2Excel_Click(object sender, EventArgs e)
        {
            if (calFrom.SelectionStart <= calTill.SelectionStart)
            {
                if (lstReports.SelectedIndex >= 0)
                {
                    if (selected.Count > 0)
                    {
                        this.Cursor = Cursors.WaitCursor;
                        switch (lstReports.SelectedIndex)
                        {
                            case 0: // hour values
                                xl.OutputHours(selected, calFrom.SelectionStart, calTill.SelectionStart);
                                break;
                            case 1: // halfhour values
                                xl.OutputHalfhours(selected, calFrom.SelectionStart, calTill.SelectionStart);
                                break;
                            case 2: // daily consumption
                                xl.OutputDaily(selected, calFrom.SelectionStart, calTill.SelectionStart);
                                break;
                            case 3: // fixed values with Ktr
                                xl.OutputFixed(selected, calFrom.SelectionStart, calTill.SelectionStart, true, false);
                                break;
                            case 4: // fixed values without Ktr
                                xl.OutputFixed(selected, calFrom.SelectionStart, calTill.SelectionStart, false, false);
                                break;
                            case 5: // fixed values without Ktr (only measured values)
                                xl.OutputFixed(selected, calFrom.SelectionStart, calTill.SelectionStart, false, true);
                                break;
                            case 6: // meters' logs
                                xl.OutputMeterLogs(selected, calFrom.SelectionStart, calTill.SelectionStart);
                                break;
                        }
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        private void BtnFindNext_Click(object sender, EventArgs e)
        {
            if (found.Count <= 0)
                return;
            int currentIndex = found.IndexOf(treePoints.SelectedNode);
            currentIndex++;
            if (currentIndex < 0 || currentIndex >= found.Count)
                currentIndex = 0;
            treePoints.SelectedNode = found[currentIndex];
            treePoints.SelectedNode.EnsureVisible();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            formInputBox dlg = new formInputBox("Введите строку поиска", "");
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                found.Clear();
                foreach (TreeNode node in treePoints.Nodes)
                    found.AddRange(FoundNodes(node, dlg.Result));
                if (found.Count > 0)
                {
                    treePoints.CollapseAll();
                    found[0].EnsureVisible();
                }
                lastSearch = dlg.Result;
                tipSelectAll.SetToolTip(btnFindNext,
                    "Поиск: " + lastSearch + Environment.NewLine +
                    "Найдено: " + found.Count.ToString() + " узлов");
            }
        }

        private List<TreeNode> FoundNodes(TreeNode root, string criterion)
        {
            List<TreeNode> result = new List<TreeNode>();
            foreach (TreeNode node in root.Nodes)
            {
                if (node.Text.ToLower().Contains(criterion.ToLower()))
                {
                    result.Add(node);
                }
                result.AddRange(FoundNodes(node, criterion));
            }
            return result;
        }

        private void LstPresets_DoubleClick(object sender, EventArgs e)
        {
            string fileName;
            TreeNode[] found;
            if (lstPresets.SelectedIndex >= 0)
            {
                this.Cursor = Cursors.WaitCursor;
                treePoints.CollapseAll();
                fileName = lstPresets.SelectedItem.ToString() + ".pst";
                if (File.Exists(fileName))
                {
                    BtnDeselectAll_Click(sender, e);
                    selected = new List<string>(File.ReadAllLines(fileName));
                    processChecks = false;
                    foreach (string node in selected)
                    {
                        found = treePoints.Nodes.Find("_" + node, true);
                        if (found.Length == 1)
                            found[0].Checked = true;
                    }
                    found = treePoints.Nodes.Find("_" + selected[0], true);
                    if (found.Length == 1)
                        found[0].EnsureVisible();
                    CountChecked();
                    processChecks = true;
                }
                this.Cursor = Cursors.Default;
            }
        }

        private void BtnDeletePreset_Click(object sender, EventArgs e)
        {
            string fileName;
            if (lstPresets.SelectedIndex >= 0)
            {
                fileName = lstPresets.SelectedItem.ToString() + ".pst";
                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                    lstPresets.Items.RemoveAt(lstPresets.SelectedIndex);
                }
            }
        }

        private void BtnSavePreset_Click(object sender, EventArgs e)
        {
            string presetName;
            StringBuilder fileName;
            formInputBox dlg = new formInputBox("Введите название набора", "");
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                presetName = dlg.Result;
                fileName = new StringBuilder(presetName.Length + 4);
                foreach (char c in presetName)
                    if (invalidChars.Contains(c))
                        fileName.Append("_");
                    else
                        fileName.Append(c);
                if (fileName.Length < 1)
                    fileName.Append(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
                if (File.Exists(fileName.ToString() + ".pst"))
                {
                    MessageBox.Show(this, "Набор с именем " + presetName + " уже существует" +
                        Environment.NewLine + "Необходимо ввести уникальное название набора",
                        "Такой набор уже существует", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                File.WriteAllLines(fileName.ToString() + ".pst", selected.ToArray());
                lstPresets.Items.Add(fileName);
            }
        }

        private void BtnDeselectAll_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            foreach (TreeNode node in treePoints.Nodes)
                node.Checked = false;
            this.Cursor = Cursors.Default;
        }

        private void BtnSelectAll_Click(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            foreach (TreeNode node in treePoints.Nodes)
                node.Checked = true;
            this.Cursor = Cursors.Default;
        }

        private void TreePoints_AfterCheck(object sender, TreeViewEventArgs e)
        {
        
            if (processChecks)
            {
                this.Cursor = Cursors.WaitCursor;
                if (e.Node.Checked)
                {
                    if (e.Node.ImageIndex == 9 && !selected.Contains(e.Node.Name.Replace("_", "")))
                        selected.Add(e.Node.Name.Replace("_", ""));
                }
                else
                    selected.Remove(e.Node.Name.Replace("_", ""));
                CheckChildren(e.Node, e.Node.Checked);
                processChecks = true;
                CountChecked();
                ClearCheck();
                this.Cursor = Cursors.Default;
            }
        }
        
        private void CheckChildren(TreeNode parent, bool value)
        {
            processChecks = false;
            foreach (TreeNode child in parent.Nodes)
            {
                child.Checked = value;
                if (value)
                {
                    if (child.ImageIndex == 9 && !selected.Contains(child.Name.Replace("_", "")))
                        selected.Add(child.Name.Replace("_", ""));
                }
                else
                    selected.Remove(child.Name.Replace("_", ""));
                CheckChildren(child, value);
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            formSettings frm = new formSettings();
            frm.ShowDialog(this);
            d = new DataModel();
        }

        private void CalTill_DateChanged(object sender, DateRangeEventArgs e)
        {
            txtDateTill.Text = e.Start.ToShortDateString();
            ClearCheck();
        }

        private void CalFrom_DateChanged(object sender, DateRangeEventArgs e)
        {
            txtDateFrom.Text = e.Start.ToShortDateString();
            ClearCheck();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            this.Cursor = Cursors.WaitCursor;
            this.WindowState = FormWindowState.Maximized;
            calFrom.SetDate(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
            calTill.SetDate(DateTime.Today.AddDays(-1));
            Settings.CheckINIFile();
            FillTree();
            LoadPresets();
            this.Cursor = Cursors.Default;
        }

        private void FillTree()
        {
            TreeNode rootNode, currentNode, parentNode;
            TreeNode[] found;
            treePoints.Nodes.Clear();
            DataTable points, pointInfo, parameters;
            int imageIndex;
            try
            {
                List<int> roots = d.GetRoots();
                foreach (int root in roots)
                {
                    pointInfo = d.PointInfo(root);
                    if (pointInfo.Rows.Count > 0)
                        imageIndex = (int)pointInfo.Rows[0][3];
                    else
                    {
                        string details = Settings.ErrorInfo(new Exception("Запрос информации о параметре вернул пустой набор строк"),
                            "formMain.FillTree");
                        formError err = new formError("Ошибка при построении дерева",
                            "Ошибка!",
                            details + Environment.NewLine + "Добавлено узлов: " + treePoints.GetNodeCount(true) +
                            Environment.NewLine + Environment.NewLine + "Ошибка на корне " + root);
                        err.ShowDialog();
                        continue;
                    }
                    if (imageIndex >= d.NodeTypes.Length)
                        imageIndex = 0;
                    rootNode = treePoints.Nodes.Add(pointInfo.Rows[0][0].ToString(),
                        pointInfo.Rows[0][2].ToString(),
                        d.NodeTypes[imageIndex],
                        d.NodeTypes[imageIndex]);
                    points = d.GetTree(root);
                    foreach (DataRow row in points.Rows)
                    {
                        found = treePoints.Nodes.Find(row[1].ToString(), true);
                        if (found.Length == 1)
                            parentNode = found[0];
                        else
                            continue;
                        imageIndex = (int)row[3];
                        if (imageIndex >= d.NodeTypes.Length)
                            imageIndex = 0;
                        currentNode = parentNode.Nodes.Add(row[0].ToString(),
                            row[2].ToString(),
                            d.NodeTypes[imageIndex],
                            d.NodeTypes[imageIndex]);
                        if (imageIndex == 10 || imageIndex == 9) // если этот узел - Присоединение или ОВ
                        {
                            parameters = d.GetParams(row[0].ToString());
                            foreach (DataRow p in parameters.Rows)
                            {
                                currentNode.Nodes.Add("_" + p[0].ToString(), p[1].ToString(), 9, 9);
                            }
                        } // end of if (currentNode.ImageIndex == 6)
                    } // end of foreach (DataRow row in points.Rows)
                    Application.DoEvents();
                } // end of foreach (int root in roots)
                txtSelectedCount.Text = "0";
            } // end of try
            catch (Exception ex)
            {
                string details = Settings.ErrorInfo(ex, "formMain.FillTree");
                formError err = new formError("Ошибка при построении дерева",
                    "Ошибка!",
                    details + Environment.NewLine + "Добавлено узлов: " + treePoints.GetNodeCount(true));
                err.ShowDialog();
            } // end of catch
        } // end of method FillTree

        private void CountChecked()
        {
            txtSelectedCount.Text = selected.Count.ToString();
        }

        private void LoadPresets()
        {
            string[] presets = Directory.EnumerateFiles(Environment.CurrentDirectory, "*.pst").ToArray();
            if (presets.Length > 0)
            {
                foreach (string s in presets)
                    lstPresets.Items.Add(Path.GetFileNameWithoutExtension(s));
            }
        }

    }
}
