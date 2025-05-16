using static System.Net.Mime.MediaTypeNames;
using System.Drawing;
using System.Windows.Forms;
using System.Xml.Linq;

namespace TowerOfHanoi
{
    partial class Form1
    {
        private PictureBox FindDiskPictureBox;
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button btnStart;
        private System.Windows.Forms.NumericUpDown numDisks;
        private System.Windows.Forms.Panel panelGame;
        private System.Windows.Forms.Label Step;
        private System.Windows.Forms.Button btnPause;
        private System.Windows.Forms.Button btnReset;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btnStart = new System.Windows.Forms.Button();
            this.numDisks = new System.Windows.Forms.NumericUpDown();
            this.Step = new System.Windows.Forms.Label();
            this.btnPause = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.panelGame = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.numDisks)).BeginInit();
            this.SuspendLayout();
            
            this.btnStart.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnStart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnStart.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnStart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.btnStart.Location = new System.Drawing.Point(346, 539);
            this.btnStart.Name = "btnStart";
            this.btnStart.Size = new System.Drawing.Size(100, 40);
            this.btnStart.TabIndex = 1;
            this.btnStart.Text = "Start";
            this.btnStart.UseVisualStyleBackColor = false;
            
            this.numDisks.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.numDisks.Location = new System.Drawing.Point(25, 549);
            this.numDisks.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
            this.numDisks.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numDisks.Name = "numDisks";
            this.numDisks.Size = new System.Drawing.Size(80, 22);
            this.numDisks.TabIndex = 2;
            this.numDisks.Value = new decimal(new int[] {
            3,
            0,
            0,
            0});
            
            this.Step.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.Step.Font = new System.Drawing.Font("Arial", 10F, System.Drawing.FontStyle.Bold);
            this.Step.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.Step.Location = new System.Drawing.Point(646, 549);
            this.Step.Name = "Step";
            this.Step.Size = new System.Drawing.Size(81, 30);
            this.Step.TabIndex = 4;
            this.Step.Text = "Step: 0";
             
            this.btnPause.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnPause.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnPause.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnPause.Location = new System.Drawing.Point(476, 539);
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(100, 40);
            this.btnPause.TabIndex = 2;
            this.btnPause.Text = "Stop";
            this.btnPause.UseVisualStyleBackColor = false;
            
            this.btnReset.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnReset.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.btnReset.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnReset.Location = new System.Drawing.Point(217, 539);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(100, 40);
            this.btnReset.TabIndex = 3;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = false;
             
            this.panelGame.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panelGame.BackColor = System.Drawing.Color.LightGray;
            this.panelGame.BackgroundImage = global::Tháp_Hà_Nội.Properties.Resources.BackGround3;
            this.panelGame.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panelGame.Location = new System.Drawing.Point(-4, -1);
            this.panelGame.Name = "panelGame";
            this.panelGame.Size = new System.Drawing.Size(803, 517);
            this.panelGame.TabIndex = 3;
             
            this.ClientSize = new System.Drawing.Size(800, 600);
            this.Controls.Add(this.Step);
            this.Controls.Add(this.btnStart);
            this.Controls.Add(this.btnPause);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.numDisks);
            this.Controls.Add(this.panelGame);
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(64)))), ((int)(((byte)(0)))));
            this.Name = "Form1";
            this.Text = "Tháp Hà Nội";
            ((System.ComponentModel.ISupportInitialize)(this.numDisks)).EndInit();
            this.ResumeLayout(false);

        }
    }
}
