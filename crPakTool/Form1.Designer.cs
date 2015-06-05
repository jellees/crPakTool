namespace crPakTool
{
    partial class Form1
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
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportDecompressToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editSTRGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convert3DGraphicToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertCMDLToOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertTXTRToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tXTRToPNGToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tXTRToDDSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDATAsize = new System.Windows.Forms.TextBox();
            this.txtRSHDsize = new System.Windows.Forms.TextBox();
            this.txtPakversion = new System.Windows.Forms.TextBox();
            this.lblPakVersion = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.txtLength = new System.Windows.Forms.TextBox();
            this.txtCompression = new System.Windows.Forms.TextBox();
            this.txtFileExtention = new System.Windows.Forms.TextBox();
            this.txtFileId = new System.Windows.Forms.TextBox();
            this.txtFileReference = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.txtLog = new System.Windows.Forms.RichTextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.Location = new System.Drawing.Point(12, 40);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(347, 336);
            this.treeView1.TabIndex = 0;
            this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(627, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
            this.saveAsToolStripMenuItem.Text = "Save As...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // editToolStripMenuItem
            // 
            this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportToolStripMenuItem,
            this.exportDecompressToolStripMenuItem,
            this.importToolStripMenuItem,
            this.toolStripSeparator1,
            this.editSTRGToolStripMenuItem});
            this.editToolStripMenuItem.Name = "editToolStripMenuItem";
            this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
            this.editToolStripMenuItem.Text = "Edit";
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exportToolStripMenuItem.Text = "Export";
            this.exportToolStripMenuItem.Click += new System.EventHandler(this.exportToolStripMenuItem_Click);
            // 
            // exportDecompressToolStripMenuItem
            // 
            this.exportDecompressToolStripMenuItem.Name = "exportDecompressToolStripMenuItem";
            this.exportDecompressToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.exportDecompressToolStripMenuItem.Text = "Export + Decompress";
            this.exportDecompressToolStripMenuItem.Click += new System.EventHandler(this.exportDecompressToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.importToolStripMenuItem.Text = "Replace";
            this.importToolStripMenuItem.Click += new System.EventHandler(this.importToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            this.toolStripSeparator1.Visible = false;
            // 
            // editSTRGToolStripMenuItem
            // 
            this.editSTRGToolStripMenuItem.Name = "editSTRGToolStripMenuItem";
            this.editSTRGToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.editSTRGToolStripMenuItem.Text = "Edit STRG";
            this.editSTRGToolStripMenuItem.Visible = false;
            this.editSTRGToolStripMenuItem.Click += new System.EventHandler(this.editSTRGToolStripMenuItem_Click);
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convert3DGraphicToolStripMenuItem,
            this.convertTXTRToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // convert3DGraphicToolStripMenuItem
            // 
            this.convert3DGraphicToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.convertCMDLToOBJToolStripMenuItem});
            this.convert3DGraphicToolStripMenuItem.Name = "convert3DGraphicToolStripMenuItem";
            this.convert3DGraphicToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.convert3DGraphicToolStripMenuItem.Text = "Convert 3D Graphic";
            // 
            // convertCMDLToOBJToolStripMenuItem
            // 
            this.convertCMDLToOBJToolStripMenuItem.Name = "convertCMDLToOBJToolStripMenuItem";
            this.convertCMDLToOBJToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.convertCMDLToOBJToolStripMenuItem.Text = "CMDL to OBJ";
            this.convertCMDLToOBJToolStripMenuItem.Click += new System.EventHandler(this.convertCMDLToOBJToolStripMenuItem_Click);
            // 
            // convertTXTRToolStripMenuItem
            // 
            this.convertTXTRToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tXTRToPNGToolStripMenuItem,
            this.tXTRToDDSToolStripMenuItem});
            this.convertTXTRToolStripMenuItem.Name = "convertTXTRToolStripMenuItem";
            this.convertTXTRToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            this.convertTXTRToolStripMenuItem.Text = "Convert TXTR";
            // 
            // tXTRToPNGToolStripMenuItem
            // 
            this.tXTRToPNGToolStripMenuItem.Name = "tXTRToPNGToolStripMenuItem";
            this.tXTRToPNGToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.tXTRToPNGToolStripMenuItem.Text = "TXTR to PNG";
            this.tXTRToPNGToolStripMenuItem.Click += new System.EventHandler(this.tXTRToPNGToolStripMenuItem_Click);
            // 
            // tXTRToDDSToolStripMenuItem
            // 
            this.tXTRToDDSToolStripMenuItem.Name = "tXTRToDDSToolStripMenuItem";
            this.tXTRToDDSToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
            this.tXTRToDDSToolStripMenuItem.Text = "TXTR to DDS";
            this.tXTRToDDSToolStripMenuItem.Click += new System.EventHandler(this.tXTRToDDSToolStripMenuItem_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Objects:";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtDATAsize);
            this.groupBox1.Controls.Add(this.txtRSHDsize);
            this.groupBox1.Controls.Add(this.txtPakversion);
            this.groupBox1.Controls.Add(this.lblPakVersion);
            this.groupBox1.Location = new System.Drawing.Point(365, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 99);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Pak Info";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(5, 47);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "DATA size";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(5, 70);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(54, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "File Count";
            // 
            // txtDATAsize
            // 
            this.txtDATAsize.Location = new System.Drawing.Point(113, 45);
            this.txtDATAsize.Margin = new System.Windows.Forms.Padding(2);
            this.txtDATAsize.Name = "txtDATAsize";
            this.txtDATAsize.ReadOnly = true;
            this.txtDATAsize.Size = new System.Drawing.Size(126, 20);
            this.txtDATAsize.TabIndex = 5;
            // 
            // txtRSHDsize
            // 
            this.txtRSHDsize.Location = new System.Drawing.Point(113, 67);
            this.txtRSHDsize.Margin = new System.Windows.Forms.Padding(2);
            this.txtRSHDsize.Name = "txtRSHDsize";
            this.txtRSHDsize.ReadOnly = true;
            this.txtRSHDsize.Size = new System.Drawing.Size(126, 20);
            this.txtRSHDsize.TabIndex = 4;
            // 
            // txtPakversion
            // 
            this.txtPakversion.Location = new System.Drawing.Point(113, 21);
            this.txtPakversion.Margin = new System.Windows.Forms.Padding(2);
            this.txtPakversion.Name = "txtPakversion";
            this.txtPakversion.ReadOnly = true;
            this.txtPakversion.Size = new System.Drawing.Size(126, 20);
            this.txtPakversion.TabIndex = 2;
            // 
            // lblPakVersion
            // 
            this.lblPakVersion.AutoSize = true;
            this.lblPakVersion.Location = new System.Drawing.Point(5, 24);
            this.lblPakVersion.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.lblPakVersion.Name = "lblPakVersion";
            this.lblPakVersion.Size = new System.Drawing.Size(64, 13);
            this.lblPakVersion.TabIndex = 0;
            this.lblPakVersion.Text = "Pak Version";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtAddress);
            this.groupBox2.Controls.Add(this.txtLength);
            this.groupBox2.Controls.Add(this.txtCompression);
            this.groupBox2.Controls.Add(this.txtFileExtention);
            this.groupBox2.Controls.Add(this.txtFileId);
            this.groupBox2.Controls.Add(this.txtFileReference);
            this.groupBox2.Controls.Add(this.label9);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(365, 132);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 170);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "File Info";
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(113, 144);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(2);
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.ReadOnly = true;
            this.txtAddress.Size = new System.Drawing.Size(126, 20);
            this.txtAddress.TabIndex = 13;
            // 
            // txtLength
            // 
            this.txtLength.Location = new System.Drawing.Point(113, 120);
            this.txtLength.Margin = new System.Windows.Forms.Padding(2);
            this.txtLength.Name = "txtLength";
            this.txtLength.ReadOnly = true;
            this.txtLength.Size = new System.Drawing.Size(126, 20);
            this.txtLength.TabIndex = 12;
            // 
            // txtCompression
            // 
            this.txtCompression.Location = new System.Drawing.Point(113, 96);
            this.txtCompression.Margin = new System.Windows.Forms.Padding(2);
            this.txtCompression.Name = "txtCompression";
            this.txtCompression.ReadOnly = true;
            this.txtCompression.Size = new System.Drawing.Size(126, 20);
            this.txtCompression.TabIndex = 11;
            // 
            // txtFileExtention
            // 
            this.txtFileExtention.Location = new System.Drawing.Point(113, 72);
            this.txtFileExtention.Margin = new System.Windows.Forms.Padding(2);
            this.txtFileExtention.Name = "txtFileExtention";
            this.txtFileExtention.ReadOnly = true;
            this.txtFileExtention.Size = new System.Drawing.Size(126, 20);
            this.txtFileExtention.TabIndex = 10;
            // 
            // txtFileId
            // 
            this.txtFileId.Location = new System.Drawing.Point(113, 48);
            this.txtFileId.Margin = new System.Windows.Forms.Padding(2);
            this.txtFileId.Name = "txtFileId";
            this.txtFileId.ReadOnly = true;
            this.txtFileId.Size = new System.Drawing.Size(126, 20);
            this.txtFileId.TabIndex = 9;
            // 
            // txtFileReference
            // 
            this.txtFileReference.Location = new System.Drawing.Point(113, 24);
            this.txtFileReference.Margin = new System.Windows.Forms.Padding(2);
            this.txtFileReference.Name = "txtFileReference";
            this.txtFileReference.ReadOnly = true;
            this.txtFileReference.Size = new System.Drawing.Size(126, 20);
            this.txtFileReference.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(5, 147);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(64, 13);
            this.label9.TabIndex = 5;
            this.label9.Text = "File Address";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(5, 123);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(71, 13);
            this.label8.TabIndex = 4;
            this.label8.Text = "Length of File";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(5, 99);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 13);
            this.label7.TabIndex = 3;
            this.label7.Text = "Compressed";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(5, 75);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 13);
            this.label6.TabIndex = 2;
            this.label6.Text = "File Extention";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 51);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(37, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "File ID";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 27);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "File";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 379);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(627, 22);
            this.statusStrip1.TabIndex = 5;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(39, 17);
            this.toolStripStatusLabel1.Text = "Ready";
            this.toolStripStatusLabel1.Click += new System.EventHandler(this.toolStripStatusLabel1_Click);
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtLog.Location = new System.Drawing.Point(8, 19);
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(231, 40);
            this.txtLog.TabIndex = 7;
            this.txtLog.Text = "";
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.txtLog);
            this.groupBox3.Location = new System.Drawing.Point(365, 308);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 68);
            this.groupBox3.TabIndex = 8;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Log";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(627, 401);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Donkey Kong Country Returns Pak Tool";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Label lblPakVersion;
        private System.Windows.Forms.TextBox txtPakversion;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDATAsize;
        private System.Windows.Forms.TextBox txtRSHDsize;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.TextBox txtLength;
        private System.Windows.Forms.TextBox txtCompression;
        private System.Windows.Forms.TextBox txtFileExtention;
        private System.Windows.Forms.TextBox txtFileId;
        private System.Windows.Forms.TextBox txtFileReference;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ToolStripMenuItem editToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.ToolStripMenuItem exportDecompressToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convert3DGraphicToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertCMDLToOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editSTRGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertTXTRToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tXTRToPNGToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tXTRToDDSToolStripMenuItem;
        private System.Windows.Forms.RichTextBox txtLog;
        private System.Windows.Forms.GroupBox groupBox3;
    }
}

