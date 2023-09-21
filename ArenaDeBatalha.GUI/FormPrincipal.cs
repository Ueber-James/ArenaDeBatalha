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
using System.Windows.Input;
using System.Windows.Threading;

namespace ArenaDeBatalha.GUI
{
    public partial class FormPrincipal : Form
    {
       

        DispatcherTimer gameLoopTimer {  get; set; }

        DispatcherTimer enemySpawnTimer { get; set; }

        Bitmap  screenBuffer {  get; set; }
        Graphics screenPainter { get; set; }
        Background background {  get; set; }

        Player player { get; set; }

        GameOver gameOver { get; set; }

        List<GameObject> gameObjects { get; set; }

        public Random random { get; set; }

         bool canShoot;
        public FormPrincipal()
        {
            InitializeComponent();

            this.random = new Random();

            this.ClientSize = Media.Background.Size;

            this.screenBuffer = new Bitmap(Media.Background.Width, Media.Background.Height);

            this.screenPainter = Graphics.FromImage(this.screenBuffer);

            this.gameObjects = new List<GameObject>();

            this.background = new Background(this.screenBuffer.Size, this.screenPainter);
            this.player = new Player(this.screenBuffer.Size, this.screenPainter);
            this.gameOver = new GameOver(this.screenBuffer.Size, this.screenPainter);

            this.gameObjects.Add(background);
            this.gameObjects.Add(player);

            

            #region background spawn
            this.gameLoopTimer = new DispatcherTimer(DispatcherPriority.Render);
            this.gameLoopTimer.Interval = TimeSpan.FromMilliseconds(16.66666);
            this.gameLoopTimer.Tick += GameLopp;
            #endregion

            #region enemy spawn

            this.enemySpawnTimer = new DispatcherTimer(DispatcherPriority.Render);

            this.enemySpawnTimer.Interval = TimeSpan.FromMilliseconds(1000);
            this.enemySpawnTimer.Tick += SpawnEnemy;
            #endregion

            StarGame();



        }

        public void StarGame()
        {
            this.gameObjects.Clear();
            this.gameObjects.Add(background);
            this.gameObjects.Add(player);
            this.player.SetStartPosition();
            this.player.Active = true;
            this.gameLoopTimer.Start();
            this.enemySpawnTimer.Start();
            this.canShoot = true;
        }

        public void EndGame()
        {
            this.gameObjects.Clear();
            this.gameLoopTimer.Stop();
            this.enemySpawnTimer.Stop();
            this.gameObjects.Add(background);
            this.gameObjects.Add(gameOver);
            this.background.UpdateObject();
            this.gameOver.UpdateObject();
            Invalidate();
        }

        public void SpawnEnemy(object sender, EventArgs e)
        {
            Point enemyPosition = new Point(this.random.Next(10, this.screenBuffer.Width - 74), -62);

            Enemy enemy = new Enemy(this.screenBuffer.Size, this.screenPainter, enemyPosition);

            this.gameObjects.Add(enemy);
        }

        private void GameLopp(object sender, EventArgs e)
        {
            this.gameObjects.RemoveAll(x => !x.Active);

            this.ProcessControls();
            foreach (GameObject go in this.gameObjects)
            {
                go.UpdateObject();

                if (go.IsOutOfBounds())
                {
                    go.Destroy();
                }

                if (go is Enemy)
                {
                    if (go.IsCollidingWith(player))
                    {
                        player.Destroy();
                        player.PlaySound();
                        EndGame();
                        return;
                    }


                    foreach (GameObject bullet in this.gameObjects.Where(X => X is Bullet))
                    {
                        if (go.IsCollidingWith(bullet))
                        {
                            go.Destroy();
                            bullet.Destroy();
                        }
                    }
                }


            }

            this.Invalidate();
        }

       

        private void FormPrincipal_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.DrawImage(this.screenBuffer, 0, 0);
        }

        private void ProcessControls()
        {
            if (Keyboard.IsKeyDown(Key.Left)) player.MoveLeft();
            if (Keyboard.IsKeyDown(Key.Right)) player.MoveRight();
            if (Keyboard.IsKeyDown(Key.Up)) player.MoveUp();
            if (Keyboard.IsKeyDown(Key.Down)) player.MoveDown();
            if (Keyboard.IsKeyDown(Key.Space) && canShoot)
            {
                this.gameObjects.Insert(1, player.Shoot());
                this.canShoot = false;
                
            }
            if (Keyboard.IsKeyUp(Key.Space)) canShoot = true;
            {
                
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void FormPrincipal_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            if (e.KeyCode == Keys.R)
            {
                StarGame();
            }

            if (e.KeyCode == Keys.Escape)
            {
                Close();
            }
        }
    }
}
