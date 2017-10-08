namespace set_win
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.button1 = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button9 = new System.Windows.Forms.Button();
            this.button10 = new System.Windows.Forms.Button();
            this.button11 = new System.Windows.Forms.Button();
            this.button12 = new System.Windows.Forms.Button();
            this.button13 = new System.Windows.Forms.Button();
            this.notifyIcon1 = new System.Windows.Forms.NotifyIcon(this.components);
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.显示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.退出ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.button14 = new System.Windows.Forms.Button();
            this.txtNodeName = new System.Windows.Forms.TextBox();
            this.button15 = new System.Windows.Forms.Button();
            this.button16 = new System.Windows.Forms.Button();
            this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.添加roo节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.添加子节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.删除选中节点ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.button17 = new System.Windows.Forms.Button();
            this.contextMenuStrip1.SuspendLayout();
            this.contextMenuStrip2.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(494, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(404, 67);
            this.button1.TabIndex = 0;
            this.button1.Text = "获得进程句柄和名称";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // listBox1
            // 
            this.listBox1.FormattingEnabled = true;
            this.listBox1.HorizontalScrollbar = true;
            this.listBox1.ItemHeight = 24;
            this.listBox1.Location = new System.Drawing.Point(12, 12);
            this.listBox1.Name = "listBox1";
            this.listBox1.ScrollAlwaysVisible = true;
            this.listBox1.Size = new System.Drawing.Size(467, 700);
            this.listBox1.TabIndex = 1;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(494, 99);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(404, 67);
            this.button2.TabIndex = 0;
            this.button2.Text = "根据进程句柄获得全路径名称";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(492, 184);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(402, 39);
            this.button3.TabIndex = 0;
            this.button3.Text = "显示进程全路径名称";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(492, 241);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(402, 39);
            this.button4.TabIndex = 0;
            this.button4.Text = "显示所有窗口句柄和标题";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(492, 298);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(402, 39);
            this.button5.TabIndex = 0;
            this.button5.Text = "移动窗口到指定位置并改变大小";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(492, 355);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(402, 39);
            this.button6.TabIndex = 0;
            this.button6.Text = "鼠标全局hook一个,获得窗口句柄";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(12, 889);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(229, 45);
            this.button8.TabIndex = 2;
            this.button8.Text = "清除listbox";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(12, 742);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(467, 35);
            this.textBox1.TabIndex = 3;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(12, 783);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(467, 35);
            this.textBox2.TabIndex = 3;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(12, 824);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(464, 35);
            this.textBox3.TabIndex = 4;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(492, 412);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(402, 39);
            this.button7.TabIndex = 5;
            this.button7.Text = "获得窗口大小位置";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click_1);
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(492, 469);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(402, 39);
            this.button9.TabIndex = 6;
            this.button9.Text = "记住选中窗口位置大小";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(491, 529);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(402, 42);
            this.button10.TabIndex = 7;
            this.button10.Text = "在上面记住的基础上位置复原";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(491, 587);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(401, 43);
            this.button11.TabIndex = 8;
            this.button11.Text = "在记住的基础上，启动方式复原";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(491, 645);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(401, 40);
            this.button12.TabIndex = 9;
            this.button12.Text = "json文件读写";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click_1);
            // 
            // button13
            // 
            this.button13.Location = new System.Drawing.Point(491, 694);
            this.button13.Name = "button13";
            this.button13.Size = new System.Drawing.Size(400, 42);
            this.button13.TabIndex = 10;
            this.button13.Text = "形状改变鼠标";
            this.button13.UseVisualStyleBackColor = true;
            this.button13.Click += new System.EventHandler(this.button13_Click);
            // 
            // notifyIcon1
            // 
            this.notifyIcon1.ContextMenuStrip = this.contextMenuStrip1;
            this.notifyIcon1.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon1.Icon")));
            this.notifyIcon1.Text = "慢慢布局";
            this.notifyIcon1.Visible = true;
            this.notifyIcon1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon1_MouseDoubleClick);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.显示ToolStripMenuItem,
            this.退出ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(137, 76);
            // 
            // 显示ToolStripMenuItem
            // 
            this.显示ToolStripMenuItem.Name = "显示ToolStripMenuItem";
            this.显示ToolStripMenuItem.Size = new System.Drawing.Size(136, 36);
            this.显示ToolStripMenuItem.Text = "显示";
            this.显示ToolStripMenuItem.Click += new System.EventHandler(this.显示ToolStripMenuItem_Click);
            // 
            // 退出ToolStripMenuItem
            // 
            this.退出ToolStripMenuItem.Name = "退出ToolStripMenuItem";
            this.退出ToolStripMenuItem.Size = new System.Drawing.Size(136, 36);
            this.退出ToolStripMenuItem.Text = "退出";
            this.退出ToolStripMenuItem.Click += new System.EventHandler(this.退出ToolStripMenuItem_Click);
            // 
            // treeView1
            // 
            this.treeView1.ContextMenuStrip = this.contextMenuStrip2;
            this.treeView1.Location = new System.Drawing.Point(933, 12);
            this.treeView1.Name = "treeView1";
            this.treeView1.Size = new System.Drawing.Size(583, 422);
            this.treeView1.TabIndex = 12;
            this.treeView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // button14
            // 
            this.button14.Location = new System.Drawing.Point(933, 500);
            this.button14.Name = "button14";
            this.button14.Size = new System.Drawing.Size(100, 48);
            this.button14.TabIndex = 13;
            this.button14.Text = "加根";
            this.button14.UseVisualStyleBackColor = true;
            this.button14.Click += new System.EventHandler(this.button14_Click);
            // 
            // txtNodeName
            // 
            this.txtNodeName.Location = new System.Drawing.Point(933, 448);
            this.txtNodeName.Name = "txtNodeName";
            this.txtNodeName.Size = new System.Drawing.Size(582, 35);
            this.txtNodeName.TabIndex = 14;
            // 
            // button15
            // 
            this.button15.Location = new System.Drawing.Point(1039, 500);
            this.button15.Name = "button15";
            this.button15.Size = new System.Drawing.Size(104, 48);
            this.button15.TabIndex = 13;
            this.button15.Text = "加子";
            this.button15.UseVisualStyleBackColor = true;
            this.button15.Click += new System.EventHandler(this.button15_Click);
            // 
            // button16
            // 
            this.button16.Location = new System.Drawing.Point(1149, 500);
            this.button16.Name = "button16";
            this.button16.Size = new System.Drawing.Size(145, 48);
            this.button16.TabIndex = 13;
            this.button16.Text = "删点";
            this.button16.UseVisualStyleBackColor = true;
            this.button16.Click += new System.EventHandler(this.button16_Click);
            // 
            // contextMenuStrip2
            // 
            this.contextMenuStrip2.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.添加roo节点ToolStripMenuItem,
            this.添加子节点ToolStripMenuItem,
            this.删除选中节点ToolStripMenuItem});
            this.contextMenuStrip2.Name = "contextMenuStrip2";
            this.contextMenuStrip2.Size = new System.Drawing.Size(233, 112);
            // 
            // 添加roo节点ToolStripMenuItem
            // 
            this.添加roo节点ToolStripMenuItem.Name = "添加roo节点ToolStripMenuItem";
            this.添加roo节点ToolStripMenuItem.Size = new System.Drawing.Size(244, 36);
            this.添加roo节点ToolStripMenuItem.Text = "添加roo节点";
            this.添加roo节点ToolStripMenuItem.Click += new System.EventHandler(this.添加roo节点ToolStripMenuItem_Click);
            // 
            // 添加子节点ToolStripMenuItem
            // 
            this.添加子节点ToolStripMenuItem.Name = "添加子节点ToolStripMenuItem";
            this.添加子节点ToolStripMenuItem.Size = new System.Drawing.Size(232, 36);
            this.添加子节点ToolStripMenuItem.Text = "添加子节点";
            // 
            // 删除选中节点ToolStripMenuItem
            // 
            this.删除选中节点ToolStripMenuItem.Name = "删除选中节点ToolStripMenuItem";
            this.删除选中节点ToolStripMenuItem.Size = new System.Drawing.Size(232, 36);
            this.删除选中节点ToolStripMenuItem.Text = "删除选中节点";
            // 
            // button17
            // 
            this.button17.Location = new System.Drawing.Point(1300, 500);
            this.button17.Name = "button17";
            this.button17.Size = new System.Drawing.Size(145, 48);
            this.button17.TabIndex = 13;
            this.button17.Text = "载入list";
            this.button17.UseVisualStyleBackColor = true;
            this.button17.Click += new System.EventHandler(this.button17_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1537, 946);
            this.Controls.Add(this.txtNodeName);
            this.Controls.Add(this.button17);
            this.Controls.Add(this.button16);
            this.Controls.Add(this.button15);
            this.Controls.Add(this.button14);
            this.Controls.Add(this.treeView1);
            this.Controls.Add(this.button13);
            this.Controls.Add(this.button12);
            this.Controls.Add(this.button11);
            this.Controls.Add(this.button10);
            this.Controls.Add(this.button9);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button8);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.button6);
            this.Controls.Add(this.button5);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.contextMenuStrip1.ResumeLayout(false);
            this.contextMenuStrip2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.Button button13;
        private System.Windows.Forms.NotifyIcon notifyIcon1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 显示ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 退出ToolStripMenuItem;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Button button14;
        private System.Windows.Forms.TextBox txtNodeName;
        private System.Windows.Forms.Button button15;
        private System.Windows.Forms.Button button16;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip2;
        private System.Windows.Forms.ToolStripMenuItem 添加roo节点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 添加子节点ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 删除选中节点ToolStripMenuItem;
        private System.Windows.Forms.Button button17;
    }
}

