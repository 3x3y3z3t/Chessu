// ;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Chessu
{
    public partial class Form1 : Form
    {
        private GamePanel gamePanel;


        public Form1()
        {
            InitializeComponent();

            SuspendLayout();
            gamePanel = new GamePanel
            {
                Location = new Point(0, 0),
                Name = "gamePanel",
                Size = new Size(620, 620),
                TabIndex = 0,
            };
            Controls.Add(gamePanel);
            ResumeLayout(false);

            gamePanel.BringToFront();
            gamePanel.Parent = this;








            gamePanel.Update();


        }

        private void button1_Click(object sender, System.EventArgs e)
        {
            gamePanel.Game.Switch();
        }
    }

    public class GamePanel : Panel
    {
        public Dictionary<string, Image> SpriteCache { get; set; }
        public CBack.Game Game { get; set; }

        private Point startDrag;
        private Point cursorOffs;
        private bool dragging;

        private float minFrametime;
        private float maxFrametime;

        private const int boardEdge = 30;
        private const int cellSize = 70;
        private const int pcsSize = 62;
        private const int offs = 5;

        private static Brush overlayBrush = new SolidBrush(Color.FromArgb(0x44FFFFFF));
        private static Pen cyanPen = new Pen(Color.Cyan, 3);
        private static Pen darkRedPen = new Pen(Color.DarkRed, 3);
        private static Pen redPen = new Pen(Color.Red, 3);
        private static Pen yellowPen = new Pen(Color.Yellow, 3);

        public GamePanel()
        {
            this.DoubleBuffered = true;
            this.MouseClick += GamePanel_MouseClick;
            this.MouseDown += GamePanel_MouseDown;
            this.MouseUp += GamePanel_MouseUp;
            this.MouseMove += GamePanel_MouseMove;
            this.Paint += GamePanel_Paint;

            startDrag = new Point(-1, -1);
            cursorOffs = new Point(-1, -1);
            dragging = false;

            minFrametime = 10000.0f; // assume 10 seconds;;
            maxFrametime = 0.0f;


            SpriteCache = new Dictionary<string, Image>
            {
                { "board", Image.FromFile(@"res\img\board.png") },
                { "sheet", Image.FromFile(@"res\img\piecesheet.png") },
                { "1_6", Image.FromFile(@"res\img\70px_1_6.png") },
                { "1_5", Image.FromFile(@"res\img\70px_1_5.png") },
                { "1_4", Image.FromFile(@"res\img\70px_1_4.png") },
                { "1_3", Image.FromFile(@"res\img\70px_1_3.png") },
                { "1_2", Image.FromFile(@"res\img\70px_1_2.png") },
                { "1_1", Image.FromFile(@"res\img\70px_1_1.png") },
                { "2_6", Image.FromFile(@"res\img\70px_2_6.png") },
                { "2_5", Image.FromFile(@"res\img\70px_2_5.png") },
                { "2_4", Image.FromFile(@"res\img\70px_2_4.png") },
                { "2_3", Image.FromFile(@"res\img\70px_2_3.png") },
                { "2_2", Image.FromFile(@"res\img\70px_2_2.png") },
                { "2_1", Image.FromFile(@"res\img\70px_2_1.png") },
            };

            Game = new CBack.Game(new CBack.HumanPlayer(CBack.PieceColor.White), new CBack.HumanPlayer(CBack.PieceColor.Black));

        }

        private bool IsBitSet(int _num, int _pos)
        {
            return (_num & (1 << _pos)) != 0;
        }

        #region Conversion Helper Methods
        private int CalcXFromCol(int _col)
        {
            return _col * cellSize;
        }

        private int CalcYFromRow(int _row)
        {
            return (CBack.Game.ColumnSize - 1 - _row) * cellSize;
        }

        private int CalcXFromIndex(int _index)
        {
            return (_index % CBack.Game.RowSize) * cellSize;
        }

        private int CalcYFromIndex(int _index)
        {
            return (CBack.Game.ColumnSize - 1 - (_index / CBack.Game.ColumnSize)) * cellSize;
        }

        private int CalcRowFromY(int _y)
        {
            if (_y < 30 || _y > 590)
                return -1;
            return CBack.Game.ColumnSize - 1 - (_y - 30) / 70;
        }

        private int CalcColFromX(int _x)
        {
            if (_x < 30 || _x > 590)
                return -1;
            return (_x - 30) / 70;
        }

        private int CalcIndexFromPoint(Point _point)
        {
            if (_point.X < 30 || _point.X > 590 || _point.Y < 30 || _point.Y > 590)
                return -1;
            int row = CBack.Game.ColumnSize - 1 - (_point.Y - 30) / 70;
            int col = (_point.X - 30) / 70;
            return row * CBack.Game.ColumnSize + col;
        }
        #endregion

        private void GamePanel_MouseClick(object sender, MouseEventArgs e)
        {
            int row = CalcRowFromY(e.Y);
            int col = CalcColFromX(e.X);
            if (Game.SelectCell(row, col))
            {
                //Console.WriteLine("Pieced!");
            }
            else
            {
                //Console.WriteLine("Unpieced!");

            }
            Invalidate();
        }

        private void GamePanel_MouseDown(object sender, MouseEventArgs e)
        {
            return;
            if (Game.SelectedPiece != null)
                return;

            startDrag = e.Location;

            Console.WriteLine("Down----->");

        }

        private void GamePanel_MouseUp(object sender, MouseEventArgs e)
        {
            return;

            Console.WriteLine("Up----->");


            startDrag = new Point(-1, -1);
        }

        private void GamePanel_MouseMove(object sender, MouseEventArgs e)
        {
            cursorOffs = e.Location;
            //Console.WriteLine(cursorOffs);
            //Invalidate(new Rectangle(cursorOffs.X - 105, cursorOffs.Y - 105, 210, 210));
            Invalidate();
            //Update();
            return;
            if (e.Button != MouseButtons.Left)
                return;

            if (dragging || e.X - startDrag.X > 5 || e.Y - startDrag.Y > 5)
            {
                dragging = true;
                cursorOffs = e.Location;
                Console.WriteLine("Drag...");

            }
        }

        private void GamePanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;

            long start = DateTime.Now.Ticks;

            g.DrawImageUnscaled(SpriteCache["board"], 0, 0, 620, 620);

            foreach (CBack.Pieces.Piece pcs in Game.Table)
            {
                if (pcs == null)
                    continue;

                string key = (int)pcs.Color + "_" + (int)pcs.Type;
                if (!SpriteCache.ContainsKey(key))
                {
                    //Console.WriteLine($"Sprite not found for piece({pcs}). Expected filename \"70px_{key}.png\"");
                    continue;
                }
                if (pcs.Equals(Game.SelectedPiece))
                {
                    if (dragging)
                    {
                        // TODO: draw the piece at the cursor;
                        g.DrawImage(SpriteCache[key], new Rectangle(cursorOffs.X, cursorOffs.Y, pcsSize, pcsSize));


                        continue;
                    }
                }

                // (image, destRect, srcRect);
                g.DrawImage(SpriteCache[key],
                    new Rectangle(boardEdge + 5 + CalcXFromCol(pcs.Column), boardEdge + 5 + CalcYFromRow(pcs.Row), pcsSize, pcsSize));


            }

            for (int i = 0; i < Game.TableStatus.Length; ++i)
            {
                //Console.Write(" " + Game.TableStatus[i]);
                //if (i % 8 == 7)
                //Console.WriteLine();

                if (Game.IsFlagSetAtCell(i, CBack.CellStatus.LastMove))
                {
                    // TODO: draw yellow curved rect;
                    g.DrawRectangle(yellowPen, boardEdge + CalcXFromIndex(i) + offs, boardEdge + CalcYFromIndex(i) + offs, 70 - offs - offs, 70 - offs - offs);
                }
                if (Game.IsFlagSetAtCell(i, CBack.CellStatus.Targetable))
                {
                    // TODO: draw red curved rect;
                    g.DrawRectangle(darkRedPen, boardEdge + CalcXFromIndex(i) + offs, boardEdge + CalcYFromIndex(i) + offs, 70 - offs - offs, 70 - offs - offs);
                }
                if (Game.IsFlagSetAtCell(i, CBack.CellStatus.Movable))
                {
                    // TODO: draw cyan curved rect;
                    g.DrawRectangle(cyanPen, boardEdge + CalcXFromIndex(i) + offs, boardEdge + CalcYFromIndex(i) + offs, 70 - offs - offs, 70 - offs - offs);
                }
                if (Game.IsFlagSetAtCell(i, CBack.CellStatus.Checking))
                {
                    g.DrawRectangle(redPen, boardEdge + CalcXFromIndex(i) + offs, boardEdge + CalcYFromIndex(i) + offs, 70 - offs - offs, 70 - offs - offs);
                }
                if (Game.IsFlagSetAtCell(i, CBack.CellStatus.Selecting))
                {
                    g.FillRectangle(overlayBrush, boardEdge + CalcXFromIndex(i) + offs, boardEdge + CalcYFromIndex(i) + offs, 70 - offs - offs, 70 - offs - offs);
                }
            }

            int r = CalcRowFromY(cursorOffs.Y);
            int c = CalcColFromX(cursorOffs.X);
            //Console.WriteLine("r = " + r + " c = " + c);
            if (r >= 0 && r < CBack.Game.ColumnSize && c >= 0 && c < CBack.Game.RowSize)
            {
                g.FillRectangle(overlayBrush, boardEdge + CalcXFromCol(c), boardEdge + CalcYFromRow(r), 70, 70);
            }

            long end = DateTime.Now.Ticks;
            TimeSpan ts = new TimeSpan(end - start);
            if (ts.TotalMilliseconds < minFrametime)
                minFrametime = (float)ts.TotalMilliseconds;
            if (ts.TotalMilliseconds > maxFrametime)
                maxFrametime = (float)ts.TotalMilliseconds;

            //Console.WriteLine($"Last Frametime: {ts.TotalMilliseconds:F}ms (min {minFrametime:F}ms, max {maxFrametime:F}ms).");
            Form1 f = Parent as Form1;

            f.lblLog.Text = $"Last Frametime: {ts.TotalMilliseconds:F}ms (min {minFrametime:F}ms, max {maxFrametime:F}ms).";



        }
    }
}
