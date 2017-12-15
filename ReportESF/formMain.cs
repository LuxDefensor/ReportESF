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
using Energosphere;
using Settings;

namespace ReportESF
{
    public partial class formMain: Form
    {
        
        private XLSExport xl;
        private List<Parameter> selected;
        private bool processChecks = true;
        private char[] invalidChars = Path.GetInvalidFileNameChars();
        private List<TreeNode> found;
        private string lastSearch = string.Empty;
        private AIIS aiis;
        private SettingsManager settings;
        Calculator c;

        #region imageIndexes dictionary
        private Dictionary<int, int> imageIndexes = new Dictionary<int, int>()
        {
            {1,0 },
            {2,1 },
            {5,2 },
            {7,3 },
            {8,4 },
            {9,5 },
            {10,6 },
            {12,7 },
            {21,8 },
            {17,9 },
            {39,10 },
            {47,11 },
            {19,12 }
        };

        #endregion

        public formMain()
        {
            InitializeComponent();
            this.Load += FormMain_Load;
            calFrom.DateChanged += CalFrom_DateChanged;
            calTill.DateChanged += CalTill_DateChanged;
            btnSettings.Click += BtnSettings_Click;
            treePoints.AfterCheck += TreePoints_AfterCheck;
            treePoints.BeforeExpand += TreePoints_BeforeExpand;
            treePoints.NodeMouseClick += TreePoints_NodeMouseClick;
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
            menuTree.Click += MenuLoadChildren_Click;
        }

        private void MenuLoadChildren_Click(object sender, EventArgs e)
        {
            if (treePoints.SelectedNode != null)
            {
                Application.DoEvents();
                this.Cursor = Cursors.WaitCursor;
                LoadTreeSection(treePoints.SelectedNode);
                this.Cursor = Cursors.Default;
            }
        }

        private void TreePoints_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            int id;
            if (e.Node.Nodes.Count == 0)
            {
                if (e.Node.Name.Substring(0, 1) != "_")
                {
                    id = int.Parse(e.Node.Name);
                    Energosphere.Point current = aiis.GetPoint(id);
                    if (current == null)
                        throw new PointNotFoundException("В загруженной расчетной схеме нет точки с ID_Point = " + id);
                    if (!current.Loaded)
                        FillTree(e.Node, current.Children);
                }
            }
        }

        private void Aiis_PointsUpdate(PointEventArgs e)
        {
            TreeNode[] nodes = treePoints.Nodes.Find(e.ParentPoint.ID.ToString(), true);
            if (nodes.Length == 1)
            {
                FillTree(nodes[0], e.Points);
            }
        }

        private void TreePoints_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            int id;
            if (e.Node.Name.Substring(0, 1) != "_")
            {
                id = int.Parse(e.Node.Name);
                Energosphere.Point current = aiis.GetPoint(id);
                if (current == null)
                    throw new PointNotFoundException("В загруженной расчетной схеме нет точки с ID_Point = " + id);
                if (!current.Loaded)
                    FillTree(e.Node, current.Children);
            }
        }

        private void LoadTreeSection(TreeNode node)
        {
            string nodeIndex = node.Name;
            if (nodeIndex.Substring(0, 1)!="_")
            {
                Energosphere.Point point = aiis.GetPoint(int.Parse(nodeIndex));
                if (point == null)
                    return;
                aiis.LoadSubtree(point);
                node.Expand();
                node.EnsureVisible();
            }
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
            if (calFrom.SelectionStart > calTill.SelectionStart)
                return;
            if (lstReports.SelectedIndex == -1)
                return;
            if (selected.Count == 0)
                return;
            this.Cursor = Cursors.WaitCursor;
            string sql;
            if (calFrom.SelectionStart <= calTill.SelectionStart)
            {
                switch (lstReports.SelectedIndex)
                {
                    case 0:
                    case 1:
                    case 2:
                    case 3:
                    case 4:
                    case 6:
                        dgvCheck.DataSource = c.GetPercentMains(selected, calFrom.SelectionStart, calTill.SelectionStart);
                        break;
                    case 5:
                        dgvCheck.DataSource = c.GetPercentNIs(selected, calFrom.SelectionStart, calTill.SelectionStart);
                        break;
                    case 7:
                        dgvCheck.DataSource = c.GetPercentLogs(selected, calFrom.SelectionStart, calTill.SelectionStart);
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
                                if (chkTranspose.Checked)
                                    xl.OutputLandscape(selected, Reports.Hours, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromHours(1), "Часовки", false);
                                else
                                    xl.OutputPortrait(selected, Reports.Hours, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromHours(1), "Часовки", false);
                                break;
                            case 1: // halfhour values
                                if (chkTranspose.Checked)
                                    xl.OutputLandscape(selected, Reports.Halfhours, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromMinutes(30), "Получасовки", false);
                                else
                                    xl.OutputPortrait(selected, Reports.Halfhours, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromMinutes(30), "Получасовки", false);
                                break;
                            case 2: // daily consumption
                                if (chkTranspose.Checked)
                                    xl.OutputLandscape(selected, Reports.Daily, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromDays(1), "Потребление", false);
                                else
                                    xl.OutputPortrait(selected, Reports.Daily, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromDays(1), "Потребление", false);
                                break;
                            case 3: // fixed values with Ktr
                                if (chkTranspose.Checked)
                                    xl.OutputLandscape(selected, Reports.Fixed, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromDays(1), "Показания", true);
                                else
                                    xl.OutputPortrait(selected, Reports.Fixed, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromDays(1), "Показания", true);
                                break;
                            case 4: // fixed values without Ktr
                                if (chkTranspose.Checked)
                                    xl.OutputLandscape(selected, Reports.FixedWithoutKtr, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromDays(1), "Показания", true);
                                else
                                    xl.OutputPortrait(selected, Reports.FixedWithoutKtr, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromDays(1), "Показания", true);
                                break;
                            case 5: // fixed values without Ktr (only measured values)
                                if (chkTranspose.Checked)
                                    xl.OutputLandscape(selected, Reports.Measured, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromDays(1), "Показания", true);
                                else
                                    xl.OutputPortrait(selected, Reports.Measured, calFrom.SelectionStart, calTill.SelectionStart,
                                        TimeSpan.FromDays(1), "Показания", true);
                                break;
                            case 6: // fixed values on both ends of the time period
                                xl.OutputLandscape(selected, Reports.PairOfFixed, calFrom.SelectionStart, calTill.SelectionStart,
                                    calTill.SelectionStart - calFrom.SelectionStart, "Показания", true);
                                break;
                            case 7: // meters' logs
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
            List<Energosphere.Point> foundPoints;
            TreeNode[] nodes;
            formInputBox dlg = new formInputBox("Введите строку поиска", "");
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                found.Clear();
                lastSearch = dlg.Result;
                foundPoints = aiis.Search(lastSearch);
                foreach (Energosphere.Point p in foundPoints)
                {
                    nodes = treePoints.Nodes.Find(p.ID.ToString(), true);
                    if (nodes.Length == 1)
                        found.Add(nodes[0]);
                }
                if (found.Count > 0)
                {
                    treePoints.CollapseAll();
                    found[0].EnsureVisible();
                    treePoints.SelectedNode = found[0];
                }
                tipSelectAll.SetToolTip(btnFindNext,
                    "Поиск: " + lastSearch + Environment.NewLine +
                    "Найдено: " + found.Count.ToString() + " узлов");
            }
        }

        private void LstPresets_DoubleClick(object sender, EventArgs e)
        {
            string fileName;
            Parameter current;
            TreeNode[] found;
            if (lstPresets.SelectedIndex >= 0)
            {
                this.Cursor = Cursors.WaitCursor;
                treePoints.CollapseAll();
                fileName = lstPresets.Text + ".pst";
                if (File.Exists(fileName))
                {
                    BtnDeselectAll_Click(sender, e);
                    selected = new List<Parameter>();
                    foreach (string line in File.ReadAllLines(fileName))
                    {
                        current = aiis.AllParameters.FirstOrDefault(p => p.Id.ToString() == line);
                        if (current == null)
                        {
                            current = aiis.LoadParameter(line);
                        }
                        selected.Add(current);
                    }
                    processChecks = false;
                    foreach (Parameter par in selected)
                    {
                        found = treePoints.Nodes.Find("_" + par.Id.ToString(), true);
                        if (found.Length == 1)
                            found[0].Checked = true;
                        else
                        {
                            formError frm = new formError("Ошибка загрузки расчетной схемы", "Ошибка!",
                                Settings.ErrorInfo(new Exception("Parameter isn't in the tree yet, but it should be"),
                                                   "formMain.LstPresets_DoubleClick") +
                                Environment.NewLine + "ip_pp = " + par.Id.ToString());
                            frm.ShowDialog();
                            Application.Exit();
                        }
                    }
                    found = treePoints.Nodes.Find("_" + selected[0], true);
                    if (found.Length == 1)
                    {
                        found[0].EnsureVisible();
                        treePoints.SelectedNode = found[0];
                    }
                    CountChecked();
                    processChecks = true;
                }
                else
                {
                    var details = Settings.ErrorInfo(null, "formMain.LstPresets_DoubleClick") +
                        Environment.NewLine + "Files present: " + Environment.NewLine +
                        string.Join(Environment.NewLine, Directory.GetFiles(Environment.CurrentDirectory));
                    formError frm = new formError("Не найден набор " + lstPresets.Text, "Ошибка!", details);
                    frm.ShowDialog();
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
                File.WriteAllLines(fileName.ToString() + ".pst", selected.Select(p => p.Id.ToString()));
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
            aiis.LoadAllPoints();
            foreach (TreeNode node in treePoints.Nodes)
                node.Checked = true;
            this.Cursor = Cursors.Default;
        }

        private void TreePoints_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (processChecks)
            {
                Parameter current = null;
                Energosphere.Point point = null;
                this.Cursor = Cursors.WaitCursor;
                if (e.Node.Name.Substring(0, 1) != "_")
                {
                    point = aiis.AllPoints.FirstOrDefault(p => p.ID == int.Parse(e.Node.Name));
                    if (point == null)
                        point = aiis.LoadPoint(int.Parse(e.Node.Name));
                }
                else
                {
                    current = aiis.AllParameters.FirstOrDefault(p => p.Id.ToString() == e.Node.Name.Replace("_", string.Empty));
                    if (current == null)
                        current = aiis.LoadParameter(e.Node.Name.Replace("_", string.Empty));
                    point = current.ParentPoint;
                }
                if (e.Node.Checked)
                {
                    if ((point.Type == PointTypes.Feeder || point.Type == PointTypes.FeederWithBypass)
                         && !selected.Contains(current) && current != null)
                        selected.Add(current);
                }
                else if (selected.Count > 0)
                    selected.Remove(selected.FirstOrDefault(p => p.Id.ToString() == e.Node.Name.Replace("_", string.Empty)));
                CheckChildren(e.Node, e.Node.Checked);
                processChecks = true;
                CountChecked();
                ClearCheck();
                this.Cursor = Cursors.Default;
            }
        }
        
        private void CheckChildren(TreeNode parent, bool value)
        {
            Parameter current;
            processChecks = false;
            foreach (TreeNode child in parent.Nodes)
            {
                CheckChildren(child, value);
                if (child.Name.Substring(0, 1) != "_")
                    continue;
                current = aiis.AllParameters.FirstOrDefault(p => p.Id.ToString() == child.Name.Replace("_", string.Empty));
                if (current == null)
                {
                    current = aiis.LoadParameter(child.Name.Replace("_", string.Empty));
                }
                child.Checked = value;
                if (value)
                {
                    if ((current.ParentPoint.Type == PointTypes.Feeder || current.ParentPoint.Type == PointTypes.FeederWithBypass)
                        && !selected.Contains(current))
                        selected.Add(current);
                }
                else
                    selected.Remove(current);
            }
        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            formSettings frm = new formSettings();
            frm.ShowDialog(this);
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
            settings = new SettingsManager(Settings.SettingsFile, new string[] { "roots =1" });
            c = new Calculator(Settings.SettingsFile);
            selected = new List<Parameter>();
            found = new List<TreeNode>();
            xl = new XLSExport(Settings.SettingsFile);
            int[] r = null;
            try
            {
                r = settings["roots"].Split(';').Select(s => int.Parse(s.Trim())).ToArray();
            }
            catch (Exception ex)
            {
                formError frm = new formError("Невозможно прочитать в настройках список корней",
                    "Ошибка!", Settings.ErrorInfo(ex, "formMain.FormMain_Load"));
                frm.ShowDialog();
                Application.Exit();
            }
            try
            {
                aiis = new AIIS(Settings.SettingsFile, r);
            }
            catch (Exception ex)
            {
                formError frm = new formError("Невозможно создать главный объект",
                "Ошибка!", Settings.ErrorInfo(ex, "formMain.FormMain_Load"));
                frm.ShowDialog();
                this.Close();
            }
            this.WindowState = FormWindowState.Maximized;
            calFrom.SetDate(new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1));
            calTill.SetDate(DateTime.Today.AddDays(-1));
            FillTree(null, aiis.Roots);
            LoadPresets();
            aiis.PointsUpdate += Aiis_PointsUpdate;
            this.Cursor = Cursors.Default;
        }

        private void FillTree(TreeNode parent, List<Energosphere.Point> children)
        {
            TreeNode currentNode;
            if (parent == null)
                treePoints.Nodes.Clear();
            int imageIndex;
            try
            {
                foreach (Energosphere.Point point in children)
                {
                    switch (point.Type)
                    {
                        case PointTypes.Abstaract:
                        case PointTypes.Building:
                        case PointTypes.Equipment:
                        case PointTypes.LineLink:
                        case PointTypes.PointLink:
                        case PointTypes.Room:
                        case PointTypes.SupplyPoint:
                        case PointTypes.TN:
                        case PointTypes.TT:
                        case PointTypes.Meter:
                            continue;
                    }
                    if (parent!=null && parent.Nodes.ContainsKey(point.ID.ToString()))
                        continue;
                    if (imageIndexes.ContainsKey((int)point.Type))
                        imageIndex = (int)imageIndexes[(int)point.Type];
                    else
                        imageIndex = 13;
                    if (parent == null)
                        currentNode = treePoints.Nodes.Add(point.ID.ToString(),
                                                        point.Name.ToString(),
                                                        imageIndex,
                                                        imageIndex);
                    else
                        currentNode = parent.Nodes.Add(point.ID.ToString(),
                                                       point.Name.ToString(),
                                                       imageIndex,
                                                       imageIndex);

                    if (point.Type == PointTypes.Feeder || point.Type == PointTypes.FeederWithBypass) // если этот узел - Присоединение или ОВ
                    {
                        foreach (Parameter par in point.Parameters)
                        {
                            currentNode.Nodes.Add("_" + par.Id,
                                                    currentNode.Text + " (" + par.TypeName + ")",
                                                    imageIndexes[(int)PointTypes.PointParameter],
                                                    imageIndexes[(int)PointTypes.PointParameter]);
                        }
                    } // end of if (point.Type == PointTypes.Feeder || point.Type == PointTypes.FeederWithBypass)

                } // end of foreach (Energosphere.Point point in children)
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
