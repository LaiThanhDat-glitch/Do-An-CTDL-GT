using System;
using System.Windows.Forms;
using TowerOfHanoi; // Đảm bảo using đúng namespace chứa Form1

// Đổi namespace nếu cần để phù hợp với project của bạn
namespace ThapHanoi
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1()); // Khởi chạy Form1
        }
    }
}
