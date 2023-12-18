
namespace ToDemo
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.GbxInput = new System.Windows.Forms.GroupBox();
            this.BtnAux = new System.Windows.Forms.Button();
            this.BtnTest = new System.Windows.Forms.Button();
            TbxOutput = new System.Windows.Forms.TextBox();
            this.Pbx = new System.Windows.Forms.PictureBox();
            this.TbxInput = new System.Windows.Forms.TextBox();
            this.tableLayoutPanel1.SuspendLayout();
            this.GbxInput.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pbx)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.GbxInput, 1, 0);
            this.tableLayoutPanel1.Controls.Add(TbxOutput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.Pbx, 1, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(800, 450);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // GbxInput
            // 
            this.GbxInput.Controls.Add(this.TbxInput);
            this.GbxInput.Controls.Add(this.BtnAux);
            this.GbxInput.Controls.Add(this.BtnTest);
            this.GbxInput.Location = new System.Drawing.Point(626, 3);
            this.GbxInput.Name = "GbxInput";
            this.GbxInput.Size = new System.Drawing.Size(171, 219);
            this.GbxInput.TabIndex = 2;
            this.GbxInput.TabStop = false;
            this.GbxInput.Text = "Input";
            // 
            // BtnAux
            // 
            this.BtnAux.Location = new System.Drawing.Point(8, 67);
            this.BtnAux.Name = "BtnAux";
            this.BtnAux.Size = new System.Drawing.Size(157, 32);
            this.BtnAux.TabIndex = 1;
            this.BtnAux.Text = "AUX";
            this.BtnAux.UseVisualStyleBackColor = true;
            this.BtnAux.Click += new System.EventHandler(BtnAux_Click);
            // 
            // BtnTest
            // 
            this.BtnTest.Location = new System.Drawing.Point(8, 29);
            this.BtnTest.Name = "BtnTest";
            this.BtnTest.Size = new System.Drawing.Size(157, 32);
            this.BtnTest.TabIndex = 0;
            this.BtnTest.Text = "Press for test";
            this.BtnTest.UseVisualStyleBackColor = true;
            this.BtnTest.Click += new System.EventHandler(BtnTest_Click);
            // 
            // TbxOutput
            // 
            TbxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            TbxOutput.Location = new System.Drawing.Point(3, 3);
            TbxOutput.Font = new System.Drawing.Font("Consolas, 'Courier New', monospace", 10);
            TbxOutput.Multiline = true;
            TbxOutput.Name = "TbxOutput";
            this.tableLayoutPanel1.SetRowSpan(TbxOutput, 2);
            TbxOutput.Size = new System.Drawing.Size(617, 444);
            TbxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            TbxOutput.TabIndex = 3;
            TbxOutput.PlaceholderText = "here is output";
            TbxOutput.ReadOnly = true;
            // 
            // Pbx
            // 
            this.Pbx.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Pbx.Location = new System.Drawing.Point(626, 228);
            this.Pbx.Name = "Pbx";
            this.Pbx.Size = new System.Drawing.Size(171, 219);
            this.Pbx.TabIndex = 4;
            this.Pbx.TabStop = false;
            // 
            // TbxInput
            // 
            this.TbxInput.Location = new System.Drawing.Point(8, 162);
            this.TbxInput.Name = "TbxInput";
            this.TbxInput.Size = new System.Drawing.Size(157, 30);
            this.TbxInput.TabIndex = 2;
            // 
            // Form1
            // 
            this.AcceptButton = this.BtnTest;
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 23F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "Form1";
            this.Text = "to demo";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(keyDownEvent);
            this.KeyPreview = true;
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.GbxInput.ResumeLayout(false);
            this.GbxInput.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.Pbx)).EndInit();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox GbxInput;
        private System.Windows.Forms.Button BtnAux;
        private System.Windows.Forms.Button BtnTest;
        static private System.Windows.Forms.TextBox TbxOutput;
        private System.Windows.Forms.PictureBox Pbx;
        private System.Windows.Forms.TextBox TbxInput;
    }
}