using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TowerOfHanoi
{
    public partial class Form1 : Form
    {
        private bool isResetting = false;
        private int moveCount = 0;  

        private Size formOriginalSize;
        private Dictionary<Control, Rectangle> originalControlBounds = new Dictionary<Control, Rectangle>();
        private Dictionary<Control, Rectangle> originalPanelGameControlBounds = new Dictionary<Control, Rectangle>();

        private Stopwatch stopwatch = new Stopwatch();
        private System.Windows.Forms.Timer Thoigianchay;

        private List<Label> diskLabels = new List<Label>();
        private Tower[] towers;
        private bool isPaused = false;
        private bool isRunning = false;

        private void TickHandler(object sender, EventArgs e)
        {
            Step.Text = $"Step: {moveCount}";
        }
        private void PanelGame_Resize_Handler(object sender, EventArgs e)
        {
            panelGame.Invalidate();
        }
        public Form1()
        {
            InitializeComponent();

            typeof(Panel)
                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
                .SetValue(panelGame, true, null);

            this.Load += new EventHandler(Form1_Load);
            btnStart.Click += new EventHandler(btnStart_Click);
            btnPause.Click += new EventHandler(btnPause_Click);
            btnReset.Click += new EventHandler(btnReset_Click);

            Thoigianchay = new System.Windows.Forms.Timer();
            Thoigianchay.Interval = 500;
            Thoigianchay.Tick += TickHandler;

            this.Resize += new EventHandler(Form1_Resize);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            btnPause.Visible = true;
            btnPause.Enabled = false;
            numDisks.Minimum = 1;
            numDisks.Maximum = 10;
            numDisks.Value = 3;

            panelGame.BackColor = Color.LightGray;

            DrawPegs();
            originalPanelGameControlBounds.Clear();
            foreach (Control ctrl in panelGame.Controls)
            {
                originalPanelGameControlBounds[ctrl] = ctrl.Bounds;
            }

            formOriginalSize = this.ClientSize;

            originalControlBounds.Clear();
            foreach (Control ctl in this.Controls)
            {
                originalControlBounds[ctl] = ctl.Bounds;
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (formOriginalSize.Width == 0 || formOriginalSize.Height == 0)
                return;

            float scaleX = (float)this.ClientSize.Width / formOriginalSize.Width;
            float scaleY = (float)this.ClientSize.Height / formOriginalSize.Height;

            foreach (KeyValuePair<Control, Rectangle> pair in originalControlBounds)
            {
                Control ctl = pair.Key;
                Rectangle original = pair.Value;
                if (ctl == panelGame)
                    continue;
                ctl.SetBounds(
                    (int)(original.X * scaleX),
                    (int)(original.Y * scaleY),
                    (int)(original.Width * scaleX),
                    (int)(original.Height * scaleY)
                );
            }
            foreach (KeyValuePair<Control, Rectangle> pair in originalPanelGameControlBounds)
            {
                Control ctl = pair.Key;
                Rectangle original = pair.Value;

                ctl.SetBounds(
                    (int)(original.X * scaleX),
                    (int)(original.Y * scaleY),
                    (int)(original.Width * scaleX),
                    (int)(original.Height * scaleY)
                );
            }
           
            List<Control> toRemove = new List<Control>();
            foreach (Control ctrl in panelGame.Controls)
            {
                if (ctrl is Panel)  
                    toRemove.Add(ctrl);
            }
            foreach (Control ctrl in toRemove)
                panelGame.Controls.Remove(ctrl);

            DrawPegs();
            UpdateDisksPosition();
        }

        private async void btnStart_Click(object sender, EventArgs e)
        {
            if (isRunning) return;

            isRunning = true;
            btnStart.Enabled = false;
            btnPause.Enabled = true;
            btnPause.Text = "Pause";
            isPaused = false;
            isResetting = false;

            towers = new Tower[3];
            for (int i = 0; i < 3; i++)
                towers[i] = new Tower(i);

            foreach (Label d in diskLabels)
            {
                panelGame.Controls.Remove(d);
                d.Dispose();
            }
            diskLabels.Clear();

            int diskCount = (int)numDisks.Value;
            int diskHeight = 35;
            int baseY = panelGame.Height - diskHeight;

            for (int size = diskCount; size >= 1; size--)
            {
                towers[0].disks.Push(size);

                Color diskColor = (size % 2 == 0) ? Color.DarkGreen : Color.Green;

                Label disk = new Label
                {
                    Size = new Size(50 + size * 20, diskHeight),
                    BackColor = diskColor,
                    Text = size.ToString(),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Left = (panelGame.Width / 4) - ((50 + size * 20) / 2),
                    Top = baseY - (diskCount - size) * diskHeight
                };
                panelGame.Controls.Add(disk);
                diskLabels.Add(disk);
            }
            UpdateDisksPosition();

            moveCount = 0;
            Step.Text = "Step: 0";
            stopwatch.Reset();
            stopwatch.Start();
            Thoigianchay.Start();

            await SolveHanoiAsync(diskCount, 0, 2, 1);

            if (!isResetting)
            {
                stopwatch.Stop();
                Thoigianchay.Stop();
                MessageBox.Show("Hoàn thành giải bài toán Tháp Hà Nội!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                MessageBox.Show("Thời gian giải bài toán: " + stopwatch.Elapsed.ToString(@"mm\:ss"),
                    "Thời gian giải bài toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            btnStart.Enabled = true;
            btnPause.Enabled = false;
            isRunning = false;
        }

        private async Task SolveHanoiAsync(int n, int source, int destination, int auxiliary)
        {
            if (n <= 0) return;

            await SolveHanoiAsync(n - 1, source, auxiliary, destination);
            await WaitWhilePausedAsync();

            if (isResetting) return;

            MoveDisk(source, destination);
            moveCount++;
            Step.Text = $"Step: {moveCount}";
            await Task.Delay(500);

            await SolveHanoiAsync(n - 1, auxiliary, destination, source);
        }

        private async Task WaitWhilePausedAsync()
        {
            while (isPaused)
            {
                await Task.Delay(100);
                if (isResetting) break;
            }
        }

        private Label FindDiskLabel(List<Label> labels, string size)
        {
            foreach (Label label in labels)
            {
                if (label.Text == size)
                    return label;
            }
            return null;
        }

        private void MoveDisk(int from, int to)
        {
            if (towers[from].disks.Count() == 0) return;

            int diskSize = (int)towers[from].disks.Pop();
            towers[to].disks.Push(diskSize);

            Label disk = FindDiskLabel(diskLabels, diskSize.ToString());
            if (disk != null)
            {
                int pegX = (to + 1) * (panelGame.Width / 4) - (disk.Width / 2);
                int diskHeight = 35;
                int baseY = panelGame.Height - diskHeight;

                disk.Left = pegX;
                disk.Top = baseY - ((towers[to].disks.Count() - 1) * diskHeight);
            }
        }

        private void DrawPegs()
        {
            int pegWidth = 30;
            int pegHeight = 350;
            int pegY = panelGame.Height - pegHeight;
            int peg1_X = panelGame.Width / 4;
            int peg2_X = panelGame.Width / 2;
            int peg3_X = 3 * panelGame.Width / 4;

            panelGame.Controls.Add(new Panel()
            {
                Size = new Size(pegWidth, pegHeight),
                Location = new Point(peg1_X - pegWidth / 2, pegY),
                BackColor = Color.BurlyWood
            });

            panelGame.Controls.Add(new Panel()
            {
                Size = new Size(pegWidth, pegHeight),
                Location = new Point(peg2_X - pegWidth / 2, pegY),
                BackColor = Color.BurlyWood
            });

            panelGame.Controls.Add(new Panel()
            {
                Size = new Size(pegWidth, pegHeight),
                Location = new Point(peg3_X - pegWidth / 2, pegY),
                BackColor = Color.BurlyWood
            });
        }

        private void UpdateDisksPosition()
        {
            if (towers == null) return;

            int diskHeight = 35;
            int pegSpacing = panelGame.Width / 4;
            int maxDiskWidth = pegSpacing * 2 / 3;

            for (int pegIndex = 0; pegIndex < 3; pegIndex++)
            {
                MyStack stack = towers[pegIndex].disks;
                Label[] disksOnPeg = new Label[stack.Count()];
                int count = stack.Count();

                object[] diskArray = stack.ToArray();
                Array.Reverse(diskArray);

                for (int j = 0; j < count; j++)
                {
                    int size = (int)diskArray[j];
                    Label d = FindDiskLabel(diskLabels, size.ToString());
                    if (d != null)
                    {
                        int pegX = (pegIndex + 1) * pegSpacing - (d.Width / 2);
                        d.Left = pegX;
                        d.Top = panelGame.Height - diskHeight * (j + 1);
                    }
                }
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (!isRunning) return;

            if (!isPaused)
            {
                isPaused = true;
                btnPause.Text = "Continue";
                stopwatch.Stop();
            }
            else
            {
                isPaused = false;
                btnPause.Text = "Pause";
                stopwatch.Start();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            isResetting = true;
            isPaused = false;
            isRunning = false;

            stopwatch.Reset();
            Thoigianchay.Stop();
            Step.Text = "Step: 0";

            foreach (Label d in diskLabels)
            {
                panelGame.Controls.Remove(d);
                d.Dispose();
            }
            diskLabels.Clear();

            List<Control> toRemove = new List<Control>();
            foreach (Control ctrl in panelGame.Controls)
            {
                if (ctrl is Panel)
                    toRemove.Add(ctrl);
            }
            foreach (Control ctrl in toRemove)
                panelGame.Controls.Remove(ctrl);
            DrawPegs();
            btnStart.Enabled = true;
            btnPause.Enabled = false;
            btnPause.Text = "Pause";
            moveCount = 0;
            UpdateDisksPosition();
        }

        class Tower
        {
            public int Id { get; set; }
            public MyStack disks { get; set; }
            public Tower(int id)
            {
                Id = id;
                disks = new MyStack();
            }
        }
    }

    public class Node
    {
        public object Data;
        public Node Next;
        public Node(object data)
        {
            Data = data;
            Next = null;
        }
    }

    public class MyStack
    {
        public Node top;

        public void Push(object data)
        {
            Node newNode = new Node(data);
            newNode.Next = top;
            top = newNode;
        }

        public object Pop()
        {
            if (IsEmpty()) return null;
            object data = top.Data;
            top = top.Next;
            return data;
        }

        public object Peek()
        {
            if (IsEmpty()) return null;
            return top.Data;
        }

        public bool IsEmpty()
        {
            return top == null;
        }

        public object[] ToArray()
        {
            List<object> list = new List<object>();
            Node current = top;
            while (current != null)
            {
                list.Add(current.Data);
                current = current.Next;
            }
            return list.ToArray();
        }

        public int Count()
        {
            int count = 0;
            Node current = top;
            while (current != null)
            {
                count++;
                current = current.Next;
            }
            return count;
        }
    }
}
