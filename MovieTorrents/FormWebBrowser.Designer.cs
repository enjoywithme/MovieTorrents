// Copyright (C) Microsoft Corporation. All rights reserved.
// Use of this source code is governed by a BSD-style license that can be
// found in the LICENSE file.

namespace MovieTorrents
{
    partial class FormWebBrowser
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebBrowser));
            btnBack = new System.Windows.Forms.Button();
            btnForward = new System.Windows.Forms.Button();
            btnRefresh = new System.Windows.Forms.Button();
            btnStop = new System.Windows.Forms.Button();
            btnGo = new System.Windows.Forms.Button();
            txtUrl = new System.Windows.Forms.TextBox();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            controlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            acceleratorKeysEnabledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            allowExternalDropMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            zoomToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            xToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            xToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            xToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            xToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            backgroundColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            whiteBackgroundColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            redBackgroundColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            blueBackgroundColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            transparentBackgroundColorMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            btnOk = new System.Windows.Forms.Button();
            webView2Control = new Microsoft.Web.WebView2.WinForms.WebView2();
            menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)webView2Control).BeginInit();
            SuspendLayout();
            // 
            // btnBack
            // 
            btnBack.Enabled = false;
            btnBack.Location = new System.Drawing.Point(14, 33);
            btnBack.Margin = new System.Windows.Forms.Padding(4);
            btnBack.Name = "btnBack";
            btnBack.Size = new System.Drawing.Size(88, 30);
            btnBack.TabIndex = 0;
            btnBack.Text = "Back";
            btnBack.UseVisualStyleBackColor = true;
            btnBack.Click += btnBack_Click;
            // 
            // btnForward
            // 
            btnForward.Enabled = false;
            btnForward.Location = new System.Drawing.Point(108, 33);
            btnForward.Margin = new System.Windows.Forms.Padding(4);
            btnForward.Name = "btnForward";
            btnForward.Size = new System.Drawing.Size(88, 30);
            btnForward.TabIndex = 1;
            btnForward.Text = "Forward";
            btnForward.UseVisualStyleBackColor = true;
            btnForward.Click += btnForward_Click;
            // 
            // btnRefresh
            // 
            btnRefresh.Location = new System.Drawing.Point(203, 33);
            btnRefresh.Margin = new System.Windows.Forms.Padding(4);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new System.Drawing.Size(88, 30);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "Refresh";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += BtnRefresh_Click;
            // 
            // btnStop
            // 
            btnStop.Location = new System.Drawing.Point(298, 33);
            btnStop.Margin = new System.Windows.Forms.Padding(4);
            btnStop.Name = "btnStop";
            btnStop.Size = new System.Drawing.Size(88, 30);
            btnStop.TabIndex = 3;
            btnStop.Text = "Stop";
            btnStop.UseVisualStyleBackColor = true;
            // 
            // btnGo
            // 
            btnGo.Location = new System.Drawing.Point(832, 33);
            btnGo.Margin = new System.Windows.Forms.Padding(4);
            btnGo.Name = "btnGo";
            btnGo.Size = new System.Drawing.Size(88, 30);
            btnGo.TabIndex = 5;
            btnGo.Text = "Go";
            btnGo.UseVisualStyleBackColor = true;
            btnGo.Click += BtnGo_Click;
            // 
            // txtUrl
            // 
            txtUrl.Location = new System.Drawing.Point(392, 37);
            txtUrl.Margin = new System.Windows.Forms.Padding(4);
            txtUrl.Name = "txtUrl";
            txtUrl.Size = new System.Drawing.Size(432, 23);
            txtUrl.TabIndex = 4;
            txtUrl.Text = "https://www.bing.com/";
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] { controlToolStripMenuItem, viewToolStripMenuItem });
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new System.Windows.Forms.Padding(4, 1, 0, 1);
            menuStrip1.Size = new System.Drawing.Size(1218, 24);
            menuStrip1.TabIndex = 7;
            menuStrip1.Text = "menuStrip1";
            // 
            // controlToolStripMenuItem
            // 
            controlToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { acceleratorKeysEnabledToolStripMenuItem, allowExternalDropMenuItem });
            controlToolStripMenuItem.Name = "controlToolStripMenuItem";
            controlToolStripMenuItem.Size = new System.Drawing.Size(63, 22);
            controlToolStripMenuItem.Text = "Control";
            // 
            // acceleratorKeysEnabledToolStripMenuItem
            // 
            acceleratorKeysEnabledToolStripMenuItem.Checked = true;
            acceleratorKeysEnabledToolStripMenuItem.CheckOnClick = true;
            acceleratorKeysEnabledToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            acceleratorKeysEnabledToolStripMenuItem.Name = "acceleratorKeysEnabledToolStripMenuItem";
            acceleratorKeysEnabledToolStripMenuItem.Size = new System.Drawing.Size(234, 22);
            acceleratorKeysEnabledToolStripMenuItem.Text = "AcceleratorKeys Enabled";
            // 
            // allowExternalDropMenuItem
            // 
            allowExternalDropMenuItem.Checked = true;
            allowExternalDropMenuItem.CheckOnClick = true;
            allowExternalDropMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            allowExternalDropMenuItem.Name = "allowExternalDropMenuItem";
            allowExternalDropMenuItem.Size = new System.Drawing.Size(234, 22);
            allowExternalDropMenuItem.Text = "AllowExternalDrop Enabled";
            allowExternalDropMenuItem.Click += allowExternalDropMenuItem_Click;
            // 
            // viewToolStripMenuItem
            // 
            viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { zoomToolStripMenuItem, backgroundColorMenuItem });
            viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            viewToolStripMenuItem.Size = new System.Drawing.Size(47, 22);
            viewToolStripMenuItem.Text = "View";
            // 
            // zoomToolStripMenuItem
            // 
            zoomToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { xToolStripMenuItem, xToolStripMenuItem1, xToolStripMenuItem2, xToolStripMenuItem3 });
            zoomToolStripMenuItem.Name = "zoomToolStripMenuItem";
            zoomToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
            zoomToolStripMenuItem.Text = "Zoom";
            // 
            // xToolStripMenuItem
            // 
            xToolStripMenuItem.Name = "xToolStripMenuItem";
            xToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
            xToolStripMenuItem.Text = "0.5x";
            xToolStripMenuItem.Click += xToolStripMenuItem05_Click;
            // 
            // xToolStripMenuItem1
            // 
            xToolStripMenuItem1.Name = "xToolStripMenuItem1";
            xToolStripMenuItem1.Size = new System.Drawing.Size(170, 22);
            xToolStripMenuItem1.Text = "1.0x";
            xToolStripMenuItem1.Click += xToolStripMenuItem1_Click;
            // 
            // xToolStripMenuItem2
            // 
            xToolStripMenuItem2.Name = "xToolStripMenuItem2";
            xToolStripMenuItem2.Size = new System.Drawing.Size(170, 22);
            xToolStripMenuItem2.Text = "2.0x";
            xToolStripMenuItem2.Click += xToolStripMenuItem2_Click;
            // 
            // xToolStripMenuItem3
            // 
            xToolStripMenuItem3.Name = "xToolStripMenuItem3";
            xToolStripMenuItem3.Size = new System.Drawing.Size(170, 22);
            xToolStripMenuItem3.Text = "Get ZoomFactor";
            xToolStripMenuItem3.Click += xToolStripMenuItem3_Click;
            // 
            // backgroundColorMenuItem
            // 
            backgroundColorMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] { whiteBackgroundColorMenuItem, redBackgroundColorMenuItem, blueBackgroundColorMenuItem, transparentBackgroundColorMenuItem });
            backgroundColorMenuItem.Name = "backgroundColorMenuItem";
            backgroundColorMenuItem.Size = new System.Drawing.Size(183, 22);
            backgroundColorMenuItem.Text = "Background Color";
            // 
            // whiteBackgroundColorMenuItem
            // 
            whiteBackgroundColorMenuItem.Name = "whiteBackgroundColorMenuItem";
            whiteBackgroundColorMenuItem.Size = new System.Drawing.Size(146, 22);
            whiteBackgroundColorMenuItem.Text = "White";
            whiteBackgroundColorMenuItem.Click += backgroundColorMenuItem_Click;
            // 
            // redBackgroundColorMenuItem
            // 
            redBackgroundColorMenuItem.Name = "redBackgroundColorMenuItem";
            redBackgroundColorMenuItem.Size = new System.Drawing.Size(146, 22);
            redBackgroundColorMenuItem.Text = "Red";
            redBackgroundColorMenuItem.Click += backgroundColorMenuItem_Click;
            // 
            // blueBackgroundColorMenuItem
            // 
            blueBackgroundColorMenuItem.Name = "blueBackgroundColorMenuItem";
            blueBackgroundColorMenuItem.Size = new System.Drawing.Size(146, 22);
            blueBackgroundColorMenuItem.Text = "Blue";
            blueBackgroundColorMenuItem.Click += backgroundColorMenuItem_Click;
            // 
            // transparentBackgroundColorMenuItem
            // 
            transparentBackgroundColorMenuItem.Name = "transparentBackgroundColorMenuItem";
            transparentBackgroundColorMenuItem.Size = new System.Drawing.Size(146, 22);
            transparentBackgroundColorMenuItem.Text = "Transparent";
            transparentBackgroundColorMenuItem.Click += backgroundColorMenuItem_Click;
            // 
            // btnOk
            // 
            btnOk.Location = new System.Drawing.Point(926, 33);
            btnOk.Margin = new System.Windows.Forms.Padding(4);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(88, 30);
            btnOk.TabIndex = 5;
            btnOk.Text = "OK";
            btnOk.UseVisualStyleBackColor = true;
            btnOk.Click += btnOk_Click;
            // 
            // webView2Control
            // 
            webView2Control.AllowExternalDrop = true;
            webView2Control.CreationProperties = null;
            webView2Control.DefaultBackgroundColor = System.Drawing.Color.White;
            webView2Control.Location = new System.Drawing.Point(0, 90);
            webView2Control.Name = "webView2Control";
            webView2Control.Size = new System.Drawing.Size(537, 391);
            webView2Control.TabIndex = 8;
            webView2Control.ZoomFactor = 1D;
            // 
            // FormWebBrowser
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackgroundImage = (System.Drawing.Image)resources.GetObject("$this.BackgroundImage");
            BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            ClientSize = new System.Drawing.Size(1218, 840);
            Controls.Add(webView2Control);
            Controls.Add(btnOk);
            Controls.Add(btnGo);
            Controls.Add(txtUrl);
            Controls.Add(btnStop);
            Controls.Add(btnRefresh);
            Controls.Add(btnForward);
            Controls.Add(btnBack);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new System.Windows.Forms.Padding(4);
            Name = "FormWebBrowser";
            ShowInTaskbar = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            Text = "BrowserForm";
            Load += FormWebBrowser_Load;
            Resize += Form_Resize;
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)webView2Control).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
        #endregion
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.Button btnForward;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Button btnStop;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem controlToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem acceleratorKeysEnabledToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem zoomToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem xToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem backgroundColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem whiteBackgroundColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem redBackgroundColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem blueBackgroundColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem transparentBackgroundColorMenuItem;
        private System.Windows.Forms.ToolStripMenuItem allowExternalDropMenuItem;
        private System.Windows.Forms.Button btnOk;
        private Microsoft.Web.WebView2.WinForms.WebView2 webView2Control;
    }
}
