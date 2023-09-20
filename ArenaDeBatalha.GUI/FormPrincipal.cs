using ArenaDeBatalha.GameLogic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Threading;

namespace ArenaDeBatalha.GUI
{
    public partial class FormPrincipal : Form
    {

        DispatcherTimer gameLoopTimer {  get; set; }

        Bitmap  screenBuuffer {  get; set; }
        Graphics screenPainter { get; set; }
        Background background {  get; set; }

        List<GameObject> gameObjects { get; set; }
        public FormPrincipal()
        {
            InitializeComponent();

            this.screenBuuffer = new Bitmap(Media.Background.Width, Media.Background.Height);

            this.screenPainter = Graphics.FromImage(this.screenBuuffer);

            this.gameObjects = new List<GameObject>();

            this.background = new Background(this.screenBuuffer.Size, this.screenPainter);

            this.gameObjects.Add(background);

            this.gameLoopTimer = new DispatcherTimer(DispatcherPriority.Render);

            this.gameLoopTimer.Interval = TimeSpan.FromSeconds(16.66666);
            this.gameLoopTimer.Tick += GameLopp;

            StarGame();



        }

        public void StarGame()
        {
            this.gameLoopTimer.Start();
        }

        private void GameLopp(object sender, EventArgs e)
        {
            foreach (GameObject go in this.gameObjects)
            {
                go.UpdateObject();
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void FormPrincipal_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(this.screenBuuffer, 0, 0);
        }
    }
}
