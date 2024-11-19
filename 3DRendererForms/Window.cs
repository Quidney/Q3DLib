using Q3DLib;
using Q3DLib.Geometry;
using System.Threading.Tasks.Sources;
using Timer = System.Timers.Timer;

namespace _3DRendererForms
{
    public partial class Window : Form
    {
        /*        Shape3D Cube = new(
                    [
                        new(-1, -1, -1), new(1, -1, -1), new(1, 1, -1), new(-1, 1, -1),
                        new(-1, -1, 1), new(1, -1, 1), new(1, 1, 1), new(-1, 1, 1)
                    ],
                    [
                        0, 1, 1, 2, 2, 3, 3, 0,
                        4, 5, 5, 6, 6, 7, 7, 4,
                        0, 4, 1, 5, 2, 6, 3, 7
                    ],
                    new(0, 0, 0),
                    new(0, 0, 0));
        */

        //Shape3D Shape = Shape3D.FromObj("boat.obj");
        Shape3D Shape = Shape3D.FromObj("lamp.obj");
        //Shape3D Shape = Shape3D.FromObj("Knight.obj");

        readonly Camera Camera;

        Vector2 mouseLockPos;
        Vector2 mouseOffset = new(0, 0);
        bool mouseLocked = true;

        int sWidth;
        int sHeight;

        readonly HashSet<Keys> KeysDown = [];
        HashSet<Keys> KeysDownLock = [];

        //Using a System.Timers.Timer is not good in comparison to Stopwatch, but this is for testing only.
        readonly Timer Timer = new(1000f / 5);

        public Window()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
            StartPosition = FormStartPosition.CenterScreen;

            Camera = new Camera(
                position: new Vector3(x: 0, y: 0, z: -500000),
                fov: 75,
                renderDistance: 1000,
                clipPlane: .1f
            );
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            sWidth = ClientSize.Width;
            sHeight = ClientSize.Height;

            mouseLockPos = new(sWidth / 2f, sHeight / 2f);

            Cursor.Hide();

            ResetMouse();

            KeyDown += (s, e) =>
            {
                switch (e.KeyCode)
                {
                    case Keys.Escape:
                        mouseLocked = !mouseLocked;

                        if (mouseLocked)
                            Cursor.Hide();
                        else
                            Cursor.Show();
                        return;

                    case Keys.F11:
                        WindowState = WindowState == FormWindowState.Maximized ? FormWindowState.Normal : FormWindowState.Maximized;
                        return;
                }

                KeysDown.Add(e.KeyCode);
            };
            KeyUp += (s, e) => KeysDown.Remove(e.KeyCode);

            Timer.Elapsed += (s, e) => FrameUpdate();
            Timer.Start();
        }

        
        private void FrameUpdate()
        {
            LockInput();
            if (InvokeRequired)
                Invoke(() => mouseOffset = ResetMouse());
            else
                mouseOffset = ResetMouse();

            HandleKeyboardInput();
            HandleMouseMovement(mouseOffset);

            if (InvokeRequired)
                Invoke(Refresh);
            else
                Refresh();
        }

        private void HandleMouseMovement(Vector2 mouseOffset)
        {
            mouseOffset /= 1000;
            Camera.Rotate(x: mouseOffset.Y, y: mouseOffset.X);
        }

        private Vector2 ResetMouse()
        {
            if (!mouseLocked)
                return Vector2.Zero;

            Point mouseClientPos = PointToClient(Cursor.Position);
            Vector2 offset = new(MathF.Round(mouseClientPos.X - mouseLockPos.X), MathF.Round(mouseClientPos.Y - mouseLockPos.Y));

            Point newPos = new((int)MathF.Round(mouseLockPos.X), (int)MathF.Round(mouseLockPos.Y));
            Cursor.Position = PointToScreen(newPos);

            return offset;
        }

        private void LockInput() => KeysDownLock = [.. KeysDown];
        private void HandleKeyboardInput()
        {
            float moveSpeed = 100000f;
            foreach (Keys key in KeysDownLock)
            {
                switch (key)
                {
                    case Keys.W:
                        Camera.MoveForward(moveSpeed);
                        break;

                    case Keys.S:
                        Camera.MoveBackward(moveSpeed);
                        break;

                    case Keys.A:
                        Camera.MoveLeft(moveSpeed);
                        break;

                    case Keys.D:
                        Camera.MoveRight(moveSpeed);
                        break;
                }
            }
        }

        protected override void OnClientSizeChanged(EventArgs e)
        {
            base.OnClientSizeChanged(e);

            sWidth = ClientSize.Width;
            sHeight = ClientSize.Height;

            mouseLockPos = new(sWidth / 2f, sHeight / 2f);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            Graphics g = e.Graphics;
            g.Clear(Color.Black);

            Camera.Render(g, Shape, sWidth, sHeight, RenderMode.Wireframe);
        }
    }
}
