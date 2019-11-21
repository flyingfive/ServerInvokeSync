namespace TestClient
{
    partial class FrmClient
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
            this.btnSend = new System.Windows.Forms.Button();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtSend = new System.Windows.Forms.TextBox();
            this.txtMsg = new System.Windows.Forms.TextBox();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.btnInvoke = new System.Windows.Forms.Button();
            this.btnConnect2 = new System.Windows.Forms.Button();
            this.numTicks = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.numCount = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.numGet = new System.Windows.Forms.NumericUpDown();
            this.numClientCount = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.btnInvokeInOrder = new System.Windows.Forms.Button();
            this.btnDisconnectAll = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.numTicks)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGet)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClientCount)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSend
            // 
            this.btnSend.Location = new System.Drawing.Point(266, 260);
            this.btnSend.Name = "btnSend";
            this.btnSend.Size = new System.Drawing.Size(75, 23);
            this.btnSend.TabIndex = 5;
            this.btnSend.Text = "发送";
            this.btnSend.UseVisualStyleBackColor = true;
            this.btnSend.Click += new System.EventHandler(this.btnSend_Click);
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(175, 260);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 23);
            this.btnConnect.TabIndex = 6;
            this.btnConnect.Text = "单个连接";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtSend
            // 
            this.txtSend.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.txtSend.Location = new System.Drawing.Point(0, 432);
            this.txtSend.Multiline = true;
            this.txtSend.Name = "txtSend";
            this.txtSend.Size = new System.Drawing.Size(800, 107);
            this.txtSend.TabIndex = 4;
            // 
            // txtMsg
            // 
            this.txtMsg.Dock = System.Windows.Forms.DockStyle.Top;
            this.txtMsg.Location = new System.Drawing.Point(0, 0);
            this.txtMsg.Multiline = true;
            this.txtMsg.Name = "txtMsg";
            this.txtMsg.Size = new System.Drawing.Size(800, 234);
            this.txtMsg.TabIndex = 3;
            // 
            // txtServer
            // 
            this.txtServer.Location = new System.Drawing.Point(69, 262);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(100, 21);
            this.txtServer.TabIndex = 7;
            this.txtServer.Text = "127.0.0.1";
            // 
            // btnInvoke
            // 
            this.btnInvoke.Location = new System.Drawing.Point(630, 265);
            this.btnInvoke.Name = "btnInvoke";
            this.btnInvoke.Size = new System.Drawing.Size(140, 23);
            this.btnInvoke.TabIndex = 8;
            this.btnInvoke.Text = "单客户端依次调用";
            this.btnInvoke.UseVisualStyleBackColor = true;
            this.btnInvoke.Click += new System.EventHandler(this.btnInvoke_Click);
            // 
            // btnConnect2
            // 
            this.btnConnect2.Location = new System.Drawing.Point(234, 345);
            this.btnConnect2.Name = "btnConnect2";
            this.btnConnect2.Size = new System.Drawing.Size(75, 23);
            this.btnConnect2.TabIndex = 9;
            this.btnConnect2.Text = "批量连接";
            this.btnConnect2.UseVisualStyleBackColor = true;
            this.btnConnect2.Click += new System.EventHandler(this.btnConnect2_Click);
            // 
            // numTicks
            // 
            this.numTicks.Location = new System.Drawing.Point(527, 311);
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
            this.numTicks.TabIndex = 10;
            this.numTicks.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(423, 318);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(77, 12);
            this.label1.TabIndex = 11;
            this.label1.Text = "调用多少次：";
            // 
            // numCount
            // 
            this.numCount.Location = new System.Drawing.Point(525, 260);
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
            this.numCount.TabIndex = 10;
            this.numCount.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(422, 265);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "每次传递数据：";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(423, 291);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(89, 12);
            this.label3.TabIndex = 16;
            this.label3.Text = "每次获取数据：";
            // 
            // numGet
            // 
            this.numGet.Location = new System.Drawing.Point(527, 286);
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
            this.numGet.TabIndex = 15;
            this.numGet.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // numClientCount
            // 
            this.numClientCount.Location = new System.Drawing.Point(135, 348);
            this.numClientCount.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numClientCount.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numClientCount.Name = "numClientCount";
            this.numClientCount.Size = new System.Drawing.Size(80, 21);
            this.numClientCount.TabIndex = 10;
            this.numClientCount.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 357);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(83, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "多少客户端 ：";
            // 
            // btnInvokeInOrder
            // 
            this.btnInvokeInOrder.Location = new System.Drawing.Point(630, 294);
            this.btnInvokeInOrder.Name = "btnInvokeInOrder";
            this.btnInvokeInOrder.Size = new System.Drawing.Size(140, 23);
            this.btnInvokeInOrder.TabIndex = 8;
            this.btnInvokeInOrder.Text = "批量客户端并发调用";
            this.btnInvokeInOrder.UseVisualStyleBackColor = true;
            this.btnInvokeInOrder.Click += new System.EventHandler(this.btnInvokeInOrder_Click);
            // 
            // btnDisconnectAll
            // 
            this.btnDisconnectAll.Location = new System.Drawing.Point(234, 374);
            this.btnDisconnectAll.Name = "btnDisconnectAll";
            this.btnDisconnectAll.Size = new System.Drawing.Size(75, 23);
            this.btnDisconnectAll.TabIndex = 9;
            this.btnDisconnectAll.Text = "全部断开";
            this.btnDisconnectAll.UseVisualStyleBackColor = true;
            this.btnDisconnectAll.Click += new System.EventHandler(this.btnDisconnectAll_Click);
            // 
            // FrmClient
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 539);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.numGet);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.numCount);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.numClientCount);
            this.Controls.Add(this.numTicks);
            this.Controls.Add(this.btnDisconnectAll);
            this.Controls.Add(this.btnConnect2);
            this.Controls.Add(this.btnInvokeInOrder);
            this.Controls.Add(this.btnInvoke);
            this.Controls.Add(this.txtServer);
            this.Controls.Add(this.btnSend);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.txtSend);
            this.Controls.Add(this.txtMsg);
            this.Name = "FrmClient";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.numTicks)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numGet)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numClientCount)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSend;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtSend;
        private System.Windows.Forms.TextBox txtMsg;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Button btnInvoke;
        private System.Windows.Forms.Button btnConnect2;
        private System.Windows.Forms.NumericUpDown numTicks;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numCount;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown numGet;
        private System.Windows.Forms.NumericUpDown numClientCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnInvokeInOrder;
        private System.Windows.Forms.Button btnDisconnectAll;
    }
}

