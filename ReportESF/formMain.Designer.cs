namespace ReportESF
{
    partial class formMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(formMain));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtSelectedCount = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.treePoints = new System.Windows.Forms.TreeView();
            this.ilTree = new System.Windows.Forms.ImageList(this.components);
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.btn2Excel = new System.Windows.Forms.Button();
            this.lstReports = new System.Windows.Forms.ListBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnFindNext = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnDeselectAll = new System.Windows.Forms.Button();
            this.btnSelectAll = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btnSettings = new System.Windows.Forms.Button();
            this.txtDateTill = new System.Windows.Forms.TextBox();
            this.txtDateFrom = new System.Windows.Forms.TextBox();
            this.calTill = new System.Windows.Forms.MonthCalendar();
            this.calFrom = new System.Windows.Forms.MonthCalendar();
            this.lstPresets = new System.Windows.Forms.ListBox();
            this.panel5 = new System.Windows.Forms.Panel();
            this.btnDeletePreset = new System.Windows.Forms.Button();
            this.btnSavePreset = new System.Windows.Forms.Button();
            this.tipSelectAll = new System.Windows.Forms.ToolTip(this.components);
            this.tipPresets = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 40F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 60F));
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel2, 2, 1);
            this.tableLayoutPanel1.Controls.Add(this.panel3, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.panel4, 2, 0);
            this.tableLayoutPanel1.Controls.Add(this.lstPresets, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.panel5, 1, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 240F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(755, 557);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.txtSelectedCount);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.treePoints);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.tableLayoutPanel1.SetRowSpan(this.panel1, 2);
            this.panel1.Size = new System.Drawing.Size(276, 431);
            this.panel1.TabIndex = 0;
            // 
            // txtSelectedCount
            // 
            this.txtSelectedCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtSelectedCount.Location = new System.Drawing.Point(115, 406);
            this.txtSelectedCount.Name = "txtSelectedCount";
            this.txtSelectedCount.Size = new System.Drawing.Size(83, 20);
            this.txtSelectedCount.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 409);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Выбрано каналов:";
            // 
            // treePoints
            // 
            this.treePoints.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treePoints.CheckBoxes = true;
            this.treePoints.HideSelection = false;
            this.treePoints.ImageIndex = 0;
            this.treePoints.ImageList = this.ilTree;
            this.treePoints.Location = new System.Drawing.Point(9, 9);
            this.treePoints.Name = "treePoints";
            this.treePoints.SelectedImageIndex = 0;
            this.treePoints.Size = new System.Drawing.Size(264, 393);
            this.treePoints.TabIndex = 0;
            // 
            // ilTree
            // 
            this.ilTree.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("ilTree.ImageStream")));
            this.ilTree.TransparentColor = System.Drawing.Color.Transparent;
            this.ilTree.Images.SetKeyName(0, "Node_Object.bmp");
            this.ilTree.Images.SetKeyName(1, "Node_Part.bmp");
            this.ilTree.Images.SetKeyName(2, "Node_Subst.bmp");
            this.ilTree.Images.SetKeyName(3, "Node_Cross.bmp");
            this.ilTree.Images.SetKeyName(4, "Node_Bus.bmp");
            this.ilTree.Images.SetKeyName(5, "Node_Sw_Obh.bmp");
            this.ilTree.Images.SetKeyName(6, "Node_Conn.bmp");
            this.ilTree.Images.SetKeyName(7, "Node_ConnOV.bmp");
            this.ilTree.Images.SetKeyName(8, "Node_Meter.bmp");
            this.ilTree.Images.SetKeyName(9, "Node_Param.bmp");
            this.ilTree.Images.SetKeyName(10, "Node_Neighbour.bmp");
            this.ilTree.Images.SetKeyName(11, "Node_Server.bmp");
            this.ilTree.Images.SetKeyName(12, "Node_USPD.bmp");
            this.ilTree.Images.SetKeyName(13, "Node_Other.bmp");
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.btn2Excel);
            this.panel2.Controls.Add(this.lstReports);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(335, 243);
            this.panel2.Name = "panel2";
            this.tableLayoutPanel1.SetRowSpan(this.panel2, 2);
            this.panel2.Size = new System.Drawing.Size(417, 311);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 8;
            this.label2.Text = "Выберите отчёт:";
            // 
            // btn2Excel
            // 
            this.btn2Excel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btn2Excel.Font = new System.Drawing.Font("Wingdings", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btn2Excel.ForeColor = System.Drawing.Color.Green;
            this.btn2Excel.Location = new System.Drawing.Point(372, 269);
            this.btn2Excel.Name = "btn2Excel";
            this.btn2Excel.Size = new System.Drawing.Size(38, 36);
            this.btn2Excel.TabIndex = 7;
            this.btn2Excel.Text = "2";
            this.tipSelectAll.SetToolTip(this.btn2Excel, "Выгрузить отчёт в Excel");
            this.btn2Excel.UseVisualStyleBackColor = true;
            // 
            // lstReports
            // 
            this.lstReports.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lstReports.FormattingEnabled = true;
            this.lstReports.Items.AddRange(new object[] {
            "Часовки за период",
            "Получасовки за период",
            "Показания на начало суток"});
            this.lstReports.Location = new System.Drawing.Point(3, 28);
            this.lstReports.Name = "lstReports";
            this.lstReports.Size = new System.Drawing.Size(363, 277);
            this.lstReports.TabIndex = 0;
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnFindNext);
            this.panel3.Controls.Add(this.btnSearch);
            this.panel3.Controls.Add(this.btnDeselectAll);
            this.panel3.Controls.Add(this.btnSelectAll);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(285, 3);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(44, 234);
            this.panel3.TabIndex = 2;
            this.tipSelectAll.SetToolTip(this.panel3, "Новый поиск");
            // 
            // btnFindNext
            // 
            this.btnFindNext.Font = new System.Drawing.Font("Wingdings", 18.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnFindNext.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnFindNext.Location = new System.Drawing.Point(4, 137);
            this.btnFindNext.Name = "btnFindNext";
            this.btnFindNext.Size = new System.Drawing.Size(38, 36);
            this.btnFindNext.TabIndex = 3;
            this.btnFindNext.Text = "К";
            this.tipSelectAll.SetToolTip(this.btnFindNext, "Результаты поиска");
            this.btnFindNext.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Font = new System.Drawing.Font("Webdings", 18.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSearch.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnSearch.Location = new System.Drawing.Point(4, 95);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(38, 36);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "L";
            this.tipSelectAll.SetToolTip(this.btnSearch, "Поиск");
            this.btnSearch.UseVisualStyleBackColor = true;
            // 
            // btnDeselectAll
            // 
            this.btnDeselectAll.Font = new System.Drawing.Font("Wingdings", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDeselectAll.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnDeselectAll.Location = new System.Drawing.Point(4, 53);
            this.btnDeselectAll.Name = "btnDeselectAll";
            this.btnDeselectAll.Size = new System.Drawing.Size(38, 36);
            this.btnDeselectAll.TabIndex = 1;
            this.btnDeselectAll.Text = "o";
            this.tipSelectAll.SetToolTip(this.btnDeselectAll, "Отменить всё");
            this.btnDeselectAll.UseVisualStyleBackColor = true;
            // 
            // btnSelectAll
            // 
            this.btnSelectAll.Font = new System.Drawing.Font("Wingdings", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSelectAll.ForeColor = System.Drawing.Color.Green;
            this.btnSelectAll.Location = new System.Drawing.Point(4, 12);
            this.btnSelectAll.Name = "btnSelectAll";
            this.btnSelectAll.Size = new System.Drawing.Size(38, 36);
            this.btnSelectAll.TabIndex = 0;
            this.btnSelectAll.Text = "ю";
            this.tipSelectAll.SetToolTip(this.btnSelectAll, "Выбрать всё");
            this.btnSelectAll.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.btnSettings);
            this.panel4.Controls.Add(this.txtDateTill);
            this.panel4.Controls.Add(this.txtDateFrom);
            this.panel4.Controls.Add(this.calTill);
            this.panel4.Controls.Add(this.calFrom);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(335, 3);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(417, 234);
            this.panel4.TabIndex = 3;
            // 
            // btnSettings
            // 
            this.btnSettings.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSettings.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnSettings.Font = new System.Drawing.Font("Webdings", 19F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSettings.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnSettings.Location = new System.Drawing.Point(372, 15);
            this.btnSettings.Name = "btnSettings";
            this.btnSettings.Size = new System.Drawing.Size(38, 36);
            this.btnSettings.TabIndex = 4;
            this.btnSettings.Text = "@";
            this.tipSelectAll.SetToolTip(this.btnSettings, "Настройки");
            this.btnSettings.UseVisualStyleBackColor = false;
            // 
            // txtDateTill
            // 
            this.txtDateTill.Location = new System.Drawing.Point(202, 189);
            this.txtDateTill.Name = "txtDateTill";
            this.txtDateTill.ReadOnly = true;
            this.txtDateTill.Size = new System.Drawing.Size(164, 20);
            this.txtDateTill.TabIndex = 3;
            this.txtDateTill.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // txtDateFrom
            // 
            this.txtDateFrom.Location = new System.Drawing.Point(20, 189);
            this.txtDateFrom.Name = "txtDateFrom";
            this.txtDateFrom.ReadOnly = true;
            this.txtDateFrom.Size = new System.Drawing.Size(164, 20);
            this.txtDateFrom.TabIndex = 2;
            this.txtDateFrom.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // calTill
            // 
            this.calTill.Location = new System.Drawing.Point(202, 15);
            this.calTill.MaxSelectionCount = 1;
            this.calTill.Name = "calTill";
            this.calTill.TabIndex = 1;
            // 
            // calFrom
            // 
            this.calFrom.Location = new System.Drawing.Point(20, 15);
            this.calFrom.MaxSelectionCount = 1;
            this.calFrom.Name = "calFrom";
            this.calFrom.TabIndex = 0;
            // 
            // lstPresets
            // 
            this.lstPresets.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstPresets.FormattingEnabled = true;
            this.lstPresets.Location = new System.Drawing.Point(3, 440);
            this.lstPresets.Name = "lstPresets";
            this.lstPresets.Size = new System.Drawing.Size(276, 114);
            this.lstPresets.TabIndex = 4;
            this.tipPresets.SetToolTip(this.lstPresets, "Двойной щелчок - загрузить набор\r\n\r\nВ именах наборов НЕЛЬЗЯ использовать символы:" +
        "\r\n* . ? [ ] / \\ | %");
            // 
            // panel5
            // 
            this.panel5.Controls.Add(this.btnDeletePreset);
            this.panel5.Controls.Add(this.btnSavePreset);
            this.panel5.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel5.Location = new System.Drawing.Point(285, 440);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(44, 114);
            this.panel5.TabIndex = 5;
            // 
            // btnDeletePreset
            // 
            this.btnDeletePreset.Font = new System.Drawing.Font("Wingdings", 20F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDeletePreset.ForeColor = System.Drawing.Color.Red;
            this.btnDeletePreset.Location = new System.Drawing.Point(4, 45);
            this.btnDeletePreset.Name = "btnDeletePreset";
            this.btnDeletePreset.Size = new System.Drawing.Size(38, 36);
            this.btnDeletePreset.TabIndex = 6;
            this.btnDeletePreset.Text = "";
            this.tipSelectAll.SetToolTip(this.btnDeletePreset, "Удалить набор из списка");
            this.btnDeletePreset.UseVisualStyleBackColor = true;
            // 
            // btnSavePreset
            // 
            this.btnSavePreset.Font = new System.Drawing.Font("Wingdings", 18.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnSavePreset.ForeColor = System.Drawing.Color.Green;
            this.btnSavePreset.Location = new System.Drawing.Point(4, 3);
            this.btnSavePreset.Name = "btnSavePreset";
            this.btnSavePreset.Size = new System.Drawing.Size(38, 36);
            this.btnSavePreset.TabIndex = 5;
            this.btnSavePreset.Text = "<";
            this.tipSelectAll.SetToolTip(this.btnSavePreset, "Сохранить выбранные точки в наборе");
            this.btnSavePreset.UseVisualStyleBackColor = true;
            // 
            // tipPresets
            // 
            this.tipPresets.IsBalloon = true;
            this.tipPresets.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            this.tipPresets.ToolTipTitle = "Сохранённые наборы точек";
            // 
            // formMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(755, 557);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "formMain";
            this.Text = "Выгрузки из <Энергосферы>";
            this.tableLayoutPanel1.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TreeView treePoints;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox txtSelectedCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnSelectAll;
        private System.Windows.Forms.Button btnDeselectAll;
        private System.Windows.Forms.ToolTip tipSelectAll;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btnSettings;
        private System.Windows.Forms.TextBox txtDateTill;
        private System.Windows.Forms.TextBox txtDateFrom;
        private System.Windows.Forms.MonthCalendar calTill;
        private System.Windows.Forms.MonthCalendar calFrom;
        private System.Windows.Forms.ImageList ilTree;
        private System.Windows.Forms.ListBox lstPresets;
        private System.Windows.Forms.ToolTip tipPresets;
        private System.Windows.Forms.Button btnSavePreset;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Button btnDeletePreset;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnFindNext;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btn2Excel;
        private System.Windows.Forms.ListBox lstReports;
    }
}

