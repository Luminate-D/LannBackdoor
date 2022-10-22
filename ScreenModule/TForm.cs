// namespace ScreenModule;
//
// public partial class TForm : Form {
//     public Image imgToDraw;
//     public Rectangle renderScale;
//
//     public TForm() {
//         InitializeComponent();
//         imgToDraw = null;
//
//         renderScale = new Rectangle(0, 0, 300, 300);
//     }
//
//     protected override void OnPaintBackground(PaintEventArgs a) {
//         // Nothing - Prevents flicker
//     }
//
//     protected override void OnPaint(PaintEventArgs e) {
//         base.OnPaint(e);
//         if (imgToDraw != null) e.Graphics.DrawImage(imgToDraw, renderScale);
//     }
//
//     public static void RunnerThread() {
//         ScreenModuleImpl.Form1.ShowDialog();
//     }
// }