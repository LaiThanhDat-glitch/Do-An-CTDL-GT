#region Code Hải Long
//using System;
//using System.Collections;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Drawing;
//using System.Reflection;
//using System.Windows.Forms;

//namespace TowerOfHanoi
//{
//    public partial class Form1 : Form
//    {
//        private bool isResetting = false;
//        private int moveCount = 0;  // Đếm số lần di chuyển đĩa

//        // Kích thước form gốc (chỉnh theo kích thước thiết kế ban đầu)
//        private Size formOriginalSize;
//        // Lưu vị trí và kích thước ban đầu của các control trên form (cả panel, button, label,...)
//        private Dictionary<Control, Rectangle> originalControlBounds = new Dictionary<Control, Rectangle>();
//        // Lưu vị trí và kích thước ban đầu của các control trong panelGame (3 cái cọc)
//        private Dictionary<Control, Rectangle> originalPanelGameControlBounds = new Dictionary<Control, Rectangle>();

//        private System.Windows.Forms.Timer Thoigianchay;
//        private int Time = 0;
//        private List<Label> diskLabels = new List<Label>();
//        private Tower[] towers;
//        private bool isPaused = false; // Thêm biến isPaused
//        private bool isRunning = false;

//        private void TickHandler(object sender, EventArgs e)
//        {
//            Time++;
//        }
//        private void PanelGame_Resize_Handler(object sender, EventArgs e)
//        {
//            panelGame.Invalidate();
//        }
//        public Form1()
//        {
//            InitializeComponent();

//            typeof(Panel)
//                .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
//                .SetValue(panelGame, true, null);

//            this.Load += new EventHandler(Form1_Load);
//            btnStart.Click += new EventHandler(btnStart_Click);
//            btnPause.Click += new EventHandler(btnPause_Click);
//            btnReset.Click += new EventHandler(btnReset_Click);


//            // Khởi tạo Thoigianchay để đếm thời gian giải thuật
//            Thoigianchay = new System.Windows.Forms.Timer();
//            Thoigianchay.Interval = 1000; // 1 giây
//            Thoigianchay.Tick += TickHandler;

//            this.Resize += new EventHandler(Form1_Resize);

//        }

//        private void Form1_Load(object sender, EventArgs e)
//        {

//            btnPause.Visible = true;
//            btnPause.Enabled = false;
//            numDisks.Minimum = 1;
//            numDisks.Maximum = 10;
//            numDisks.Value = 3;

//            panelGame.BackColor = Color.LightGray;

//            DrawPegs();
//            // Lưu vị trí và kích thước ban đầu của controls trong panelGame (3 cái cọc)
//            originalPanelGameControlBounds.Clear();
//            foreach (Control ctrl in panelGame.Controls)
//            {
//                originalPanelGameControlBounds[ctrl] = ctrl.Bounds;
//            }
//            // Lưu kích thước form lúc load
//            formOriginalSize = this.ClientSize;

//            // Lưu vị trí và kích thước ban đầu của controls trên form (chỉ lấy controls cha trực tiếp, nếu bạn có nhiều control lồng nhau thì cần xử lý thêm)
//            originalControlBounds.Clear();
//            foreach (Control ctl in this.Controls)
//            {
//                originalControlBounds[ctl] = ctl.Bounds;
//            }
//        }
//        private void Form1_Resize(object sender, EventArgs e)
//        {
//            if (formOriginalSize.Width == 0 || formOriginalSize.Height == 0)
//                return; // Tránh chia cho 0

//            float scaleX = (float)this.ClientSize.Width / formOriginalSize.Width;
//            float scaleY = (float)this.ClientSize.Height / formOriginalSize.Height;

//            // Resize controls trên form
//            foreach (KeyValuePair<Control, Rectangle> pair in originalControlBounds)
//            {
//                Control ctl = pair.Key;
//                Rectangle original = pair.Value;

//                ctl.SetBounds(
//                    (int)(original.X * scaleX),
//                    (int)(original.Y * scaleY),
//                    (int)(original.Width * scaleX),
//                    (int)(original.Height * scaleY)
//                );
//            }
//            // Resize controls trong panelGame (các cọc)
//            foreach (KeyValuePair<Control, Rectangle> pair in originalPanelGameControlBounds)
//            {
//                Control ctl = pair.Key;
//                Rectangle original = pair.Value;

//                ctl.SetBounds(
//                    (int)(original.X * scaleX),
//                    (int)(original.Y * scaleY),
//                    (int)(original.Width * scaleX),
//                    (int)(original.Height * scaleY)
//                );
//            }

//            // Gọi UpdateDisksPosition() để cập nhật vị trí đĩa khi form resize
//            UpdateDisksPosition();
//        }


//        private void btnStart_Click(object sender, EventArgs e)
//        {
//            btnStart.Enabled = false;
//            isResetting = false;
//            // Khởi tạo 3 cột trống (Tower) để khỏi null
//            towers = new Tower[3];
//            for (int i = 0; i < 3; i++)
//                towers[i] = new Tower(i);

//            // Xóa hết các Label đĩa cũ trên panel
//            foreach (Label d in diskLabels)
//            {
//                panelGame.Controls.Remove(d);
//                d.Dispose();
//            }
//            diskLabels.Clear();

//            // Lấy số đĩa, tạo và vẽ Label cho cột đầu tiên
//            int diskCount = (int)numDisks.Value;
//            int diskHeight = 35;
//            int baseY = panelGame.Height - diskHeight;
//            for (int size = diskCount; size >= 1; size--)
//            {
//                // Đẩy dữ liệu logic
//                towers[0].disks.Push(size);

//                // Chọn màu xen kẽ 2 màu
//                Color diskColor;
//                if (size % 2 == 0)
//                    diskColor = Color.DarkGreen;
//                else
//                    diskColor = Color.Green;

//                // Tạo Label và thêm vào giao diện
//                Label disk = new Label
//                {
//                    Size = new Size(50 + size * 20, diskHeight),
//                    BackColor = diskColor,
//                    Text = size.ToString(),
//                    TextAlign = ContentAlignment.MiddleCenter,
//                    Left = (panelGame.Width / 4) - ((50 + size * 20) / 2),
//                    Top = baseY - (diskCount - size) * diskHeight
//                };
//                panelGame.Controls.Add(disk);
//                diskLabels.Add(disk);
//            }
//            UpdateDisksPosition();

//            // Start gameStep để bắt đầu đếm thời gian chơi
//            btnPause.Enabled = true;
//            btnPause.Visible = true;
//            btnPause.BringToFront();

//            // Start thuật toán và gọi SolveHanoi một lần duy nhất
//            Time = 0;
//            Thoigianchay.Start();
//            Debug.WriteLine(">> SolveHanoi bắt đầu với n=" + diskCount);

//            SolveHanoi(diskCount, 0, 2, 1);

//            // Reset biến đếm số lần di chuyển
//            moveCount = 0;
//            Step.Text = "Step: 0";
//        }



//        void SolveHanoi(int n, int source, int destination, int auxiliary)
//        {
//            // Dùng stack để mô phỏng lời gọi đệ quy
//            MyStack<Tuple<int, int, int, int, bool>> stack = new MyStack<Tuple<int, int, int, int, bool>>();

//            // Đẩy trạng thái ban đầu vào stack
//            stack.Push(new Tuple<int, int, int, int, bool>(n, source, destination, auxiliary, false));

//            while (!stack.IsEmpty())
//            {
//                Tuple<int, int, int, int, bool> state = (Tuple<int, int, int, int, bool>)stack.Pop();

//                int disk = state.Item1;
//                int from = state.Item2;
//                int to = state.Item3;
//                int aux = state.Item4;
//                bool stepDone = state.Item5;

//                // Nếu không còn đĩa để xử lý thì bỏ qua
//                if (disk <= 0)
//                {
//                    continue;
//                }

//                if (stepDone)
//                {
//                    // Chờ nếu đang tạm dừng
//                    while (isPaused)
//                    {
//                        Application.DoEvents();
//                        System.Threading.Thread.Sleep(100);
//                    }

//                    // Di chuyển đĩa
//                    MoveDisk(from, to);
//                    Application.DoEvents();
//                    System.Threading.Thread.Sleep(500);
//                }
//                else
//                {
//                    // Mô phỏng ba lời gọi đệ quy theo thứ tự ngược lại

//                    // Bước 3: chuyển n-1 đĩa từ cọc phụ sang cọc đích
//                    stack.Push(new Tuple<int, int, int, int, bool>(disk - 1, aux, to, from, false));

//                    // Bước 2: chuyển đĩa từ cọc nguồn sang cọc đích
//                    stack.Push(new Tuple<int, int, int, int, bool>(disk, from, to, aux, true));

//                    // Bước 1: chuyển n-1 đĩa từ cọc nguồn sang cọc phụ
//                    stack.Push(new Tuple<int, int, int, int, bool>(disk - 1, from, aux, to, false));
//                }
//            }

//            // Dừng bộ đếm thời gian khi hoàn tất
//            Thoigianchay.Stop();

//            // Hiển thị thông báo bằng cách dùng delegate kiểu truyền thống
//            this.Invoke(new Action(ShowFinishMessage));
//            if (!isResetting)
//            {
//                this.Invoke(new Action(ShowFinishMessage));
//            }
//        }

//        // Hàm hiển thị thông báo khi hoàn thành (không dùng lambda)
//        private void ShowFinishMessage()
//        {
//            MessageBox.Show("Hoàn thành giải bài toán Tháp Hà Nội!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            MessageBox.Show("Thời gian chạy thuật toán: " + Time + " giây", "Thời gian thuật toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
//            btnStart.Enabled = true;
//        }

//        private Label FindDiskLabel(List<Label> labels, string size)
//        {
//            foreach (Label label in labels)
//            {
//                if (label.Text == size)
//                {
//                    return label;
//                }
//            }
//            return null; // hoặc có thể ném ngoại lệ nếu không tìm thấy
//        }

//        private void MoveDisk(int from, int to)
//        {
//            if (towers[from].disks.Count() == 0) return;

//            int diskSize = (int)towers[from].disks.Pop();
//            towers[to].disks.Push(diskSize);

//            Label disk = FindDiskLabel(diskLabels, diskSize.ToString());
//            if (disk != null)
//            {
//                int pegX = (to + 1) * (panelGame.Width / 4) - (disk.Width / 2);
//                int diskHeight = 35;
//                int baseY = panelGame.Height - diskHeight;

//                disk.Left = pegX;
//                disk.Top = baseY - ((towers[to].disks.Count() - 1) * diskHeight);
//            }
//            // Tăng biến đếm số lần di chuyển
//            moveCount++;

//            // Cập nhật label hiển thị số lần di chuyển (thay lblTimer)
//            Step.Text = $"Step: {moveCount}";
//        }

//        private void DrawPegs()
//        {
//            int pegWidth = 30;
//            int pegHeight = 350;
//            int pegY = panelGame.Height - pegHeight;
//            int peg1_X = panelGame.Width / 4;
//            int peg2_X = panelGame.Width / 2;
//            int peg3_X = 3 * panelGame.Width / 4;

//            panelGame.Controls.Add(new Panel()
//            {
//                Size = new Size(pegWidth, pegHeight),
//                Location = new Point(peg1_X - pegWidth / 2, pegY),
//                BackColor = Color.BurlyWood
//            });

//            panelGame.Controls.Add(new Panel()
//            {
//                Size = new Size(pegWidth, pegHeight),
//                Location = new Point(peg2_X - pegWidth / 2, pegY),
//                BackColor = Color.BurlyWood
//            });

//            panelGame.Controls.Add(new Panel()
//            {
//                Size = new Size(pegWidth, pegHeight),
//                Location = new Point(peg3_X - pegWidth / 2, pegY),
//                BackColor = Color.BurlyWood
//            });
//        }
//        private Label FindFirstDiskLabel(List<Label> labels, string size)
//        {
//            foreach (Label label in labels)
//            {
//                if (label.Text == size)
//                {
//                    return label;
//                }
//            }
//            return null; // hoặc có thể ném ngoại lệ nếu không tìm thấy
//        }

//        private void UpdateDisksPosition()
//        {
//            if (towers == null) return;

//            int diskHeight = 35;
//            int pegSpacing = panelGame.Width / 4;
//            int maxDiskWidth = pegSpacing * 2 / 3;
//            int minDiskWidth = maxDiskWidth / 3;

//            for (int i = 0; i < 3; i++)
//            {
//                int stackCount = towers[i].disks.Count();
//                int baseY = panelGame.Height - diskHeight;
//                int pegX = (i + 1) * pegSpacing;
//                int index = 0;

//                foreach (int diskSize in towers[i].disks)
//                {
//                    Label disk = FindFirstDiskLabel(diskLabels, diskSize.ToString());

//                    if (disk != null)
//                    {
//                        // Tính toán chiều rộng mới của đĩa
//                        int diskWidth = minDiskWidth + (diskSize - 1) * (maxDiskWidth - minDiskWidth) / ((int)numDisks.Value - 1);
//                        disk.Size = new Size(diskWidth, diskHeight);

//                        // Căn giữa đĩa trên cọc
//                        disk.Left = pegX - (disk.Width / 2);
//                        disk.Top = baseY - (stackCount - 1 - index) * diskHeight;
//                    }
//                    index++;
//                }
//            }
//        }

//        private void btnPause_Click(object sender, EventArgs e)
//        {
//            if (isPaused)
//            {
//                isPaused = false;
//                btnPause.Text = "Stop";
//                Thoigianchay.Start();
//            }
//            else
//            {
//                isPaused = true;
//                btnPause.Text = "Continue";
//                Thoigianchay.Stop();
//            }
//        }

//        private void btnReset_Click(object sender, EventArgs e)
//        {
//            // Đặt cờ reset
//            isResetting = true;

//            // Dừng timer
//            Thoigianchay.Stop();

//            // Reset bộ đếm bước và bộ đếm thời gian
//            moveCount = 0;
//            Time = 0;
//            Step.Text = "Step: 0";

//            // Reset trạng thái tạm dừng
//            isPaused = false;
//            btnPause.Text = "Stop";  // hoặc "Pause" tùy UI bạn thích
//            btnPause.Enabled = false; // vì chưa bắt đầu chơi lại

//            // Xóa hết Label đĩa cũ trên panel
//            foreach (Label d in diskLabels)
//            {
//                panelGame.Controls.Remove(d);
//                d.Dispose();
//            }
//            diskLabels.Clear();
//            // Enable nút Start, disable nút Pause vì chưa chơi
//            btnStart.Enabled = true;
//            btnPause.Enabled = false;
//            moveCount = 0;
//            Step.Text = "Step: 0"; ;
//            towers = new Tower[] { new Tower(0), new Tower(1), new Tower(2) };
//            UpdateDisksPosition();
//        }

//        public class MyStack<T> : IEnumerable<T>
//        {
//            private Node top;

//            public void Push(T data)
//            {
//                Node newNode = new Node(data);
//                newNode.Next = top;
//                top = newNode;
//            }

//            public T Pop()
//            {
//                if (IsEmpty())
//                {
//                    throw new InvalidOperationException("Stack is empty");
//                }
//                T data = top.Data;
//                top = top.Next;
//                return data;
//            }

//            public T Peek()
//            {
//                if (IsEmpty())
//                {
//                    throw new InvalidOperationException("Stack is empty");
//                }
//                return top.Data;
//            }

//            public bool IsEmpty()
//            {
//                return top == null;
//            }

//            public int Count()
//            {
//                int count = 0;
//                Node current = top;
//                while (current != null)
//                {
//                    count++;
//                    current = current.Next;
//                }
//                return count;
//            }

//            private class Node
//            {
//                public T Data;
//                public Node Next;
//                public Node(T data)
//                {
//                    Data = data;
//                    Next = null;
//                }
//            }
//            public T[] ToArray()
//            {
//                List<T> list = new List<T>();
//                Node current = top;
//                while (current != null)
//                {
//                    list.Add(current.Data);
//                    current = current.Next;
//                }
//                return list.ToArray();
//            }

//            public IEnumerator<T> GetEnumerator()
//            {
//                Node current = top;
//                while (current != null)
//                {
//                    yield return current.Data;
//                    current = current.Next;
//                }
//            }

//            IEnumerator IEnumerable.GetEnumerator()
//            {
//                return GetEnumerator();
//            }
//        }
//        public class Tower
//        {
//            public MyStack<int> disks = new MyStack<int>();
//            public int Index;
//            public Tower(int index) { Index = index; }

//            public void MoveTopTo(Tower destination)
//            {
//                if (disks.Count() > 0)
//                {
//                    int top = disks.Pop();
//                    destination.disks.Push(top);
//                }
//            }
//        }
//    }
//}
#endregion

#region Code Tuấn Đạt
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace TowerOfHanoi
{
    public partial class Form1 : Form
    {
        private bool isResetting = false;
        private int moveCount = 0;  // Đếm số lần di chuyển đĩa

        // Kích thước form gốc (chỉnh theo kích thước thiết kế ban đầu)
        private Size formOriginalSize;
        // Lưu vị trí và kích thước ban đầu của các control trên form (cả panel, button, label,...)
        private Dictionary<Control, Rectangle> originalControlBounds = new Dictionary<Control, Rectangle>();
        // Lưu vị trí và kích thước ban đầu của các control trong panelGame (3 cái cọc)
        private Dictionary<Control, Rectangle> originalPanelGameControlBounds = new Dictionary<Control, Rectangle>();

        private System.Windows.Forms.Timer Thoigianchay;
        private int Time = 0;
        private List<Label> diskLabels = new List<Label>();
        private Tower[] towers;
        private bool isPaused = false; // Thêm biến isPaused
        private bool isRunning = false;

        private void TickHandler(object sender, EventArgs e)
        {
            Time++;
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


            // Khởi tạo Thoigianchay để đếm thời gian giải thuật
            Thoigianchay = new System.Windows.Forms.Timer();
            Thoigianchay.Interval = 1000; // 1 giây
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
            // Lưu vị trí và kích thước ban đầu của controls trong panelGame (3 cái cọc)
            originalPanelGameControlBounds.Clear();
            foreach (Control ctrl in panelGame.Controls)
            {
                originalPanelGameControlBounds[ctrl] = ctrl.Bounds;
            }
            // Lưu kích thước form lúc load
            formOriginalSize = this.ClientSize;

            // Lưu vị trí và kích thước ban đầu của controls trên form (chỉ lấy controls cha trực tiếp, nếu bạn có nhiều control lồng nhau thì cần xử lý thêm)
            originalControlBounds.Clear();
            foreach (Control ctl in this.Controls)
            {
                originalControlBounds[ctl] = ctl.Bounds;
            }
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (formOriginalSize.Width == 0 || formOriginalSize.Height == 0)
                return; // Tránh chia cho 0

            float scaleX = (float)this.ClientSize.Width / formOriginalSize.Width;
            float scaleY = (float)this.ClientSize.Height / formOriginalSize.Height;

            // Resize controls trên form
            foreach (KeyValuePair<Control, Rectangle> pair in originalControlBounds)
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
            // Resize controls trong panelGame (các cọc)
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

            // Gọi UpdateDisksPosition() để cập nhật vị trí đĩa khi form resize
            UpdateDisksPosition();
        }


        private void btnStart_Click(object sender, EventArgs e)
        {
            btnStart.Enabled = false;
            isResetting = false;
            // Khởi tạo 3 cột trống (Tower) để khỏi null
            towers = new Tower[3];
            for (int i = 0; i < 3; i++)
                towers[i] = new Tower(i);

            // Xóa hết các Label đĩa cũ trên panel
            foreach (Label d in diskLabels)
            {
                panelGame.Controls.Remove(d);
                d.Dispose();
            }
            diskLabels.Clear();

            // Lấy số đĩa, tạo và vẽ Label cho cột đầu tiên
            int diskCount = (int)numDisks.Value;
            int diskHeight = 35;
            int baseY = panelGame.Height - diskHeight;
            for (int size = diskCount; size >= 1; size--)
            {
                // Đẩy dữ liệu logic
                towers[0].disks.Push(size);

                // Chọn màu xen kẽ 2 màu
                Color diskColor;
                if (size % 2 == 0)
                    diskColor = Color.DarkGreen;
                else
                    diskColor = Color.Green;

                // Tạo Label và thêm vào giao diện
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

            // Start gameStep để bắt đầu đếm thời gian chơi
            btnPause.Enabled = true;
            btnPause.Visible = true;
            btnPause.BringToFront();

            // Start thuật toán và gọi SolveHanoi một lần duy nhất
            Time = 0;
            Thoigianchay.Start();
            Debug.WriteLine(">> SolveHanoi bắt đầu với n=" + diskCount);

            SolveHanoi(diskCount, 0, 2, 1);

            // Reset biến đếm số lần di chuyển
            moveCount = 0;
            Step.Text = "Step: 0";
        }


        void SolveHanoi(int n, int source, int destination, int auxiliary)
        {
            // Dùng stack để mô phỏng lời gọi đệ quy
            MyStack<Tuple<int, int, int, int, bool>> stack = new MyStack<Tuple<int, int, int, int, bool>>();

            // Đẩy trạng thái ban đầu vào stack
            stack.Push(new Tuple<int, int, int, int, bool>(n, source, destination, auxiliary, false));

            while (!stack.IsEmpty())
            {
                Tuple<int, int, int, int, bool> state = (Tuple<int, int, int, int, bool>)stack.Pop();

                int disk = state.Item1;
                int from = state.Item2;
                int to = state.Item3;
                int aux = state.Item4;
                bool stepDone = state.Item5;

                // Nếu không còn đĩa để xử lý thì bỏ qua
                if (disk <= 0)
                {
                    continue;
                }

                if (stepDone)
                {
                    // Chờ nếu đang tạm dừng
                    while (isPaused)
                    {
                        Application.DoEvents();
                        System.Threading.Thread.Sleep(100);
                    }

                    // Di chuyển đĩa
                    MoveDisk(from, to);
                    Application.DoEvents();
                    System.Threading.Thread.Sleep(500);
                }
                else
                {
                    // Mô phỏng ba lời gọi đệ quy theo thứ tự ngược lại

                    // Bước 3: chuyển n-1 đĩa từ cọc phụ sang cọc đích
                    stack.Push(new Tuple<int, int, int, int, bool>(disk - 1, aux, to, from, false));

                    // Bước 2: chuyển đĩa từ cọc nguồn sang cọc đích
                    stack.Push(new Tuple<int, int, int, int, bool>(disk, from, to, aux, true));

                    // Bước 1: chuyển n-1 đĩa từ cọc nguồn sang cọc phụ
                    stack.Push(new Tuple<int, int, int, int, bool>(disk - 1, from, aux, to, false));
                }
            }

            // Dừng bộ đếm thời gian khi hoàn tất
            Thoigianchay.Stop();

            // Hiển thị thông báo bằng cách dùng delegate kiểu truyền thống
            if (!isResetting)
            {
                this.Invoke(new Action(ShowFinishMessage));
            }

        }

        // Hàm hiển thị thông báo khi hoàn thành (không dùng lambda)
        private void ShowFinishMessage()
        {
            MessageBox.Show("Hoàn thành giải bài toán Tháp Hà Nội!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            MessageBox.Show("Thời gian chạy thuật toán: " + Time + " giây", "Thời gian thuật toán", MessageBoxButtons.OK, MessageBoxIcon.Information);
            btnStart.Enabled = true;
        }

        private Label FindDiskLabel(List<Label> labels, string size)
        {
            foreach (Label label in labels)
            {
                if (label.Text == size)
                {
                    return label;
                }
            }
            return null; // hoặc có thể ném ngoại lệ nếu không tìm thấy
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
            // Tăng biến đếm số lần di chuyển
            moveCount++;

            // Cập nhật label hiển thị số lần di chuyển (thay lblTimer)
            Step.Text = $"Step: {moveCount}";
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
        private Label FindFirstDiskLabel(List<Label> labels, string size)
        {
            foreach (Label label in labels)
            {
                if (label.Text == size)
                {
                    return label;
                }
            }
            return null; // hoặc có thể ném ngoại lệ nếu không tìm thấy
        }

        private void UpdateDisksPosition()
        {
            if (towers == null) return;

            int diskHeight = 35;
            int pegSpacing = panelGame.Width / 4;
            int maxDiskWidth = pegSpacing * 2 / 3;
            int minDiskWidth = maxDiskWidth / 3;

            for (int i = 0; i < 3; i++)
            {
                int stackCount = towers[i].disks.Count();
                int baseY = panelGame.Height - diskHeight;
                int pegX = (i + 1) * pegSpacing;
                int index = 0;

                foreach (int diskSize in towers[i].disks)
                {
                    Label disk = FindFirstDiskLabel(diskLabels, diskSize.ToString());

                    if (disk != null)
                    {
                        // Tính toán chiều rộng mới của đĩa
                        int diskWidth = minDiskWidth + (diskSize - 1) * (maxDiskWidth - minDiskWidth) / ((int)numDisks.Value - 1);
                        disk.Size = new Size(diskWidth, diskHeight);

                        // Căn giữa đĩa trên cọc
                        disk.Left = pegX - (disk.Width / 2);
                        disk.Top = baseY - (stackCount - 1 - index) * diskHeight;
                    }
                    index++;
                }
            }
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            if (isPaused)
            {
                isPaused = false;
                btnPause.Text = "Stop";
                Thoigianchay.Start();
            }
            else
            {
                isPaused = true;
                btnPause.Text = "Continue";
                Thoigianchay.Stop();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Đặt cờ reset
            isResetting = true;
            // Dừng timer
            Thoigianchay.Stop();

            // Reset bộ đếm bước và bộ đếm thời gian
            moveCount = 0;
            Time = 0;
            Step.Text = "Step: 0";

            // Reset trạng thái tạm dừng
            isPaused = false;
            btnPause.Text = "Stop";  // hoặc "Pause" tùy UI bạn thích
            btnPause.Enabled = false; // vì chưa bắt đầu chơi lại

            // Xóa hết Label đĩa cũ trên panel
            foreach (Label d in diskLabels)
            {
                panelGame.Controls.Remove(d);
                d.Dispose();
            }
            diskLabels.Clear();
            // Enable nút Start, disable nút Pause vì chưa chơi
            btnStart.Enabled = true;
            btnPause.Enabled = false;
            moveCount = 0;
            Step.Text = "Step: 0"; ;
            towers = new Tower[] { new Tower(0), new Tower(1), new Tower(2) };
            UpdateDisksPosition();
        }

        public class MyStack<T> : IEnumerable<T>
        {
            private Node top;

            public void Push(T data)
            {
                Node newNode = new Node(data);
                newNode.Next = top;
                top = newNode;
            }

            public T Pop()
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("Stack is empty");
                }
                T data = top.Data;
                top = top.Next;
                return data;
            }

            public T Peek()
            {
                if (IsEmpty())
                {
                    throw new InvalidOperationException("Stack is empty");
                }
                return top.Data;
            }

            public bool IsEmpty()
            {
                return top == null;
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

            private class Node
            {
                public T Data;
                public Node Next;
                public Node(T data)
                {
                    Data = data;
                    Next = null;
                }
            }
            public T[] ToArray()
            {
                List<T> list = new List<T>();
                Node current = top;
                while (current != null)
                {
                    list.Add(current.Data);
                    current = current.Next;
                }
                return list.ToArray();
            }

            public IEnumerator<T> GetEnumerator()
            {
                Node current = top;
                while (current != null)
                {
                    yield return current.Data;
                    current = current.Next;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
        public class Tower
        {
            public MyStack<int> disks = new MyStack<int>();
            public int Index;
            public Tower(int index) { Index = index; }

            public void MoveTopTo(Tower destination)
            {
                if (disks.Count() > 0)
                {
                    int top = disks.Pop();
                    destination.disks.Push(top);
                }
            }
        }
    }
}
#endregion