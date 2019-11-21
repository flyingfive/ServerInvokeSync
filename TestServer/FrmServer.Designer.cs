namespace TestServer
{
    partial class FrmServer
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
            this.btnInvoke = new System.Windows.Forms.Button();
            this.btnStart = new System.Windows.Forms.Button();
            this.lbClients = new System.Windows.Forms.ListBox();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numTicks = new System.Windows.Forms.NumericUpDown();
            this.numGet = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.btnInvokeInOrder = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTicks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGet)).BeginInit();
            this.SuspendLayout();
            // 
            // btnInvoke
            // 
            this.btnInvoke.Location = new System.Drawing.Point(746, 106);
            this.btnInvoke.Name = "btnInvoke";
            this.btnInvoke.Size = new System.Drawing.Size(125, 23);
            this.btnInvoke.TabIndex = 11;
            this.btnInvoke.Text = "批量调用单个客户端";
            this.btnInvoke.UseVisualStyleBackColor = true;
            this.btnInvoke.Click += new System.EventHandler(this.btnInvoke_Click);
            // 
            // btnStart
            // 
            this.btnStart.Location = new System.Drawing.Point(597, 31);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(75, 23);
            this.btnStart.TabIndex = 10;
            this.btnStart.Text = "启动服务";
            this.btnStart.UseVisualStyleBackColor = true;
            this.btnStart.Click += new System.EventHandler(this.btnStart_Click);
            // 
            // lbClients
            // 
            this.lbClients.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbClients.FormattingEnabled = true;
            this.lbClients.ItemHeight = 12;
            this.lbClients.Location = new System.Drawing.Point(0, 0);
            this.lbClients.Name = "lbClients";
            this.lbClients.Size = new System.Drawing.Size(507, 193);
            this.lbClients.TabIndex = 9;
            // 
            // txtMsg
            // 
            this.txtMsg.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtMsg.Location = new System.Drawing.Point(0, 193);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(942, 257);
            this.txtMsg.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(544, 106);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 14;
            this.label2.Text = "每次传递数据：";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(648, 99);
            this.numCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numCount.Name = "numCount";
            this.numCount.Size = new System.Drawing.Size(80, 21);
            this.numCount.TabIndex = 12;
            this.numCount.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(544, 158);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 15;
            this.label1.Text = "调用多少次：";
            // 
            // numTicks
            // 
            this.numTicks.Location = new System.Drawing.Point(648, 151);
            this.numTicks.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numTicks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numTicks.Name = "numTicks";
            this.numTicks.Size = new System.Drawing.Size(80, 21);
            this.numTicks.TabIndex = 13;
            this.numTicks.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // numGet
            // 
            this.numGet.Location = new System.Drawing.Point(648, 124);
            this.numGet.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numGet.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numGet.Name = "numGet";
            this.numGet.Size = new System.Drawing.Size(80, 21);
            this.numGet.TabIndex = 12;
            this.numGet.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(544, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 14;
            this.label3.Text = "每次获取数据：";
            // 
            // btnInvokeInOrder
            // 
            this.btnInvokeInOrder.Location = new System.Drawing.Point(746, 147);
            this.btnInvokeInOrder.Name = "btnInvokeInOrder";
            this.btnInvokeInOrder.Size = new System.Drawing.Size(125, 23);
            this.btnInvokeInOrder.TabIndex = 11;
            this.btnInvokeInOrder.Text = "并发调用多个客户端";
            this.btnInvokeInOrder.UseVisualStyleBackColor = true;
            this.btnInvokeInOrder.Click += new System.EventHandler(this.btnInvokeInOrder_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(746, 31);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 16;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // FrmServer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(942, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numGet);
            this.Controls.Add(this.numCount);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numTicks);
            this.Controls.Add(this.btnInvokeInOrder);
            this.Controls.Add(this.btnInvoke);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.lbClients);
            this.Controls.Add(this.txtMsg);
            this.Name = "FrmServer";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numTicks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGet)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnInvoke;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.ListBox lbClients;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numTicks;
        private System.Windows.Forms.NumericUpDown numGet;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnInvokeInOrder;
        private System.Windows.Forms.Button button1;
    }
}

