namespace ZeroDownBrowser
{
    partial class BrowserForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BrowserForm));
            toolStripContainer = new System.Windows.Forms.ToolStripContainer();
            statusLabel = new System.Windows.Forms.Label();
            outputLabel = new System.Windows.Forms.Label();
            toolStrip1 = new System.Windows.Forms.ToolStrip();
            backButton = new System.Windows.Forms.ToolStripButton();
            forwardButton = new System.Windows.Forms.ToolStripButton();
            tsbAutoDownloadNow = new System.Windows.Forms.ToolStripButton();
            tsbCapture0DayDown = new System.Windows.Forms.ToolStripButton();
            urlTextBox = new System.Windows.Forms.ToolStripTextBox();
            goButton = new System.Windows.Forms.ToolStripButton();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            tsbShowLog = new System.Windows.Forms.ToolStripButton();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            startAutoSaveToWizNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            clearLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            showDevToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripContainer.ContentPanel.SuspendLayout();
            toolStripContainer.TopToolStripPanel.SuspendLayout();
            toolStripContainer.SuspendLayout();
            toolStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripContainer
            // 
            // 
            // toolStripContainer.ContentPanel
            // 
            toolStripContainer.ContentPanel.Controls.Add(statusLabel);
            toolStripContainer.ContentPanel.Controls.Add(outputLabel);
            toolStripContainer.ContentPanel.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            toolStripContainer.ContentPanel.Size = new System.Drawing.Size(1040, 588);
            toolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            toolStripContainer.LeftToolStripPanelVisible = false;
            toolStripContainer.Location = new System.Drawing.Point(0, 27);
            toolStripContainer.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            toolStripContainer.Name = "toolStripContainer";
            toolStripContainer.RightToolStripPanelVisible = false;
            toolStripContainer.Size = new System.Drawing.Size(1040, 613);
            toolStripContainer.TabIndex = 0;
            toolStripContainer.Text = "toolStripContainer1";
            // 
            // toolStripContainer.TopToolStripPanel
            // 
            toolStripContainer.TopToolStripPanel.Controls.Add(toolStrip1);
            // 
            // statusLabel
            // 
            statusLabel.AutoSize = true;
            statusLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            statusLabel.Location = new System.Drawing.Point(0, 554);
            statusLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            statusLabel.Name = "statusLabel";
            statusLabel.Size = new System.Drawing.Size(0, 17);
            statusLabel.TabIndex = 1;
            // 
            // outputLabel
            // 
            outputLabel.AutoSize = true;
            outputLabel.Dock = System.Windows.Forms.DockStyle.Bottom;
            outputLabel.Location = new System.Drawing.Point(0, 571);
            outputLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            outputLabel.Name = "outputLabel";
            outputLabel.Size = new System.Drawing.Size(0, 17);
            outputLabel.TabIndex = 0;
            // 
            // toolStrip1
            // 
            toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { backButton, forwardButton, tsbAutoDownloadNow, tsbCapture0DayDown, urlTextBox, goButton, toolStripSeparator1, tsbShowLog });
            toolStrip1.Location = new System.Drawing.Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Padding = new System.Windows.Forms.Padding(0);
            toolStrip1.Size = new System.Drawing.Size(1040, 25);
            toolStrip1.Stretch = true;
            toolStrip1.TabIndex = 0;
            toolStrip1.Layout += HandleToolStripLayout;
            // 
            // backButton
            // 
            backButton.Enabled = false;
            backButton.Image = Properties.Resources.nav_left_green;
            backButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            backButton.Name = "backButton";
            backButton.Size = new System.Drawing.Size(56, 22);
            backButton.Text = "Back";
            backButton.Click += BackButtonClick;
            // 
            // forwardButton
            // 
            forwardButton.Enabled = false;
            forwardButton.Image = Properties.Resources.nav_right_green;
            forwardButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            forwardButton.Name = "forwardButton";
            forwardButton.Size = new System.Drawing.Size(76, 22);
            forwardButton.Text = "Forward";
            forwardButton.Click += ForwardButtonClick;
            // 
            // tsbAutoDownloadNow
            // 
            tsbAutoDownloadNow.Enabled = false;
            tsbAutoDownloadNow.Image = Properties.Resources.Lightning16;
            tsbAutoDownloadNow.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbAutoDownloadNow.Name = "tsbAutoDownloadNow";
            tsbAutoDownloadNow.Size = new System.Drawing.Size(115, 22);
            tsbAutoDownloadNow.Text = "Download now";
            // 
            // tsbCapture0DayDown
            // 
            tsbCapture0DayDown.Enabled = false;
            tsbCapture0DayDown.Image = Properties.Resources.Database_save_16;
            tsbCapture0DayDown.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbCapture0DayDown.Name = "tsbCapture0DayDown";
            tsbCapture0DayDown.Size = new System.Drawing.Size(125, 22);
            tsbCapture0DayDown.Text = "Save to WizNote";
            tsbCapture0DayDown.Click += tsbCapture0DayDown_Click;
            // 
            // urlTextBox
            // 
            urlTextBox.AutoSize = false;
            urlTextBox.Name = "urlTextBox";
            urlTextBox.Size = new System.Drawing.Size(500, 25);
            urlTextBox.KeyUp += UrlTextBoxKeyUp;
            // 
            // goButton
            // 
            goButton.Image = Properties.Resources.nav_plain_green;
            goButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            goButton.Name = "goButton";
            goButton.Size = new System.Drawing.Size(45, 22);
            goButton.Text = "Go";
            goButton.Click += GoButtonClick;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbShowLog
            // 
            tsbShowLog.Image = Properties.Resources.Column_one_16;
            tsbShowLog.ImageTransparentColor = System.Drawing.Color.Magenta;
            tsbShowLog.Name = "tsbShowLog";
            tsbShowLog.Size = new System.Drawing.Size(50, 22);
            tsbShowLog.Text = "Log";
            tsbShowLog.Click += tsbShowLog_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { fileToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            menuStrip1.Size = new System.Drawing.Size(1040, 27);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { startAutoSaveToWizNoteToolStripMenuItem, clearLogToolStripMenuItem, showDevToolsToolStripMenuItem, toolStripMenuItem1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new System.Drawing.Size(39, 21);
            fileToolStripMenuItem.Text = "File";
            // 
            // startAutoSaveToWizNoteToolStripMenuItem
            // 
            startAutoSaveToWizNoteToolStripMenuItem.Enabled = false;
            startAutoSaveToWizNoteToolStripMenuItem.Name = "startAutoSaveToWizNoteToolStripMenuItem";
            startAutoSaveToWizNoteToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            startAutoSaveToWizNoteToolStripMenuItem.Text = "Enable auto save to WizNote";
            // 
            // clearLogToolStripMenuItem
            // 
            clearLogToolStripMenuItem.Name = "clearLogToolStripMenuItem";
            clearLogToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            clearLogToolStripMenuItem.Text = "Clear log";
            clearLogToolStripMenuItem.Click += clearLogToolStripMenuItem_Click;
            // 
            // showDevToolsToolStripMenuItem
            // 
            showDevToolsToolStripMenuItem.Name = "showDevToolsToolStripMenuItem";
            showDevToolsToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            showDevToolsToolStripMenuItem.Text = "Show DevTools";
            showDevToolsToolStripMenuItem.Click += ShowDevToolsMenuItemClick;
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new System.Drawing.Size(242, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(245, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += ExitMenuItemClick;
            // 
            // BrowserForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1040, 640);
            Controls.Add(toolStripContainer);
            Controls.Add(menuStrip1);
            Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
            Name = "BrowserForm";
            Text = "BrowserForm";
            Load += BrowserForm_Load;
            toolStripContainer.ContentPanel.ResumeLayout(false);
            toolStripContainer.ContentPanel.PerformLayout();
            toolStripContainer.TopToolStripPanel.ResumeLayout(false);
            toolStripContainer.TopToolStripPanel.PerformLayout();
            toolStripContainer.ResumeLayout(false);
            toolStripContainer.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.ToolStripContainer toolStripContainer;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton backButton;
        private System.Windows.Forms.ToolStripButton forwardButton;
        private System.Windows.Forms.ToolStripTextBox urlTextBox;
        private System.Windows.Forms.ToolStripButton goButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.Label outputLabel;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.ToolStripMenuItem showDevToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsbCapture0DayDown;
        private System.Windows.Forms.ToolStripMenuItem startAutoSaveToWizNoteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem clearLogToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton tsbShowLog;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton tsbAutoDownloadNow;
    }
}