using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace morskoyboy2
{
    public partial class Form1 : Form
    {
        private Button[,] playerButtons = new Button[10, 10];
        private Button[,] enemyButtons = new Button[10, 10];
        // 0 - пустая 1 - корабль 2 - попал 3 - промах
        private int[,] playerBoard = new int[10, 10];
        private int[,] enemyBoard = new int[10, 10];

        // 1 - 4, 2 - 3, 3 - 2, 4 - 1

        private Random random = new Random();

        public Form1()
        {
            InitializeComponent();
            CreateBoards();
            PlaceShips();
            DrawPlayerShips();
        }

        private void CreateBoards()
        {
            int cellSize = 30;
            int leftMargin = 20;
            int topMargin = 20;

            string[] letters = { "A", "Б", "В", "Г", "Д", "Е", "Ж", "З", "И", "К" };

            for (int i = 0; i < 10; i++)
            {
                Label playerLetter = new Label();
                playerLetter.Text = letters[i];
                playerLetter.AutoSize = false;
                playerLetter.Size = new Size(cellSize, topMargin);
                playerLetter.TextAlign = ContentAlignment.MiddleCenter;
                playerLetter.Location = new Point(leftMargin + i * cellSize, 0);
                panelPlayer.Controls.Add(playerLetter);

                Label enemyLetter = new Label();
                enemyLetter.Text = letters[i];
                enemyLetter.AutoSize = false;
                enemyLetter.Size = new Size(cellSize, topMargin);
                enemyLetter.TextAlign = ContentAlignment.MiddleCenter;
                enemyLetter.Location = new Point(leftMargin + i * cellSize, 0);
                panelEnemy.Controls.Add(enemyLetter);
            }

            for (int i = 0; i < 10; i++)
            {
                Label playerNum = new Label();
                playerNum.Text = (i + 1).ToString();
                playerNum.AutoSize = false;
                playerNum.Size = new Size(leftMargin, cellSize);
                playerNum.TextAlign = ContentAlignment.MiddleCenter;
                playerNum.Location = new Point(0, topMargin + i * cellSize);
                panelPlayer.Controls.Add(playerNum);

                Label enemyNum = new Label();
                enemyNum.Text = (i + 1).ToString();
                enemyNum.AutoSize = false;
                enemyNum.Size = new Size(leftMargin, cellSize);
                enemyNum.TextAlign = ContentAlignment.MiddleCenter;
                enemyNum.Location = new Point(0, topMargin + i * cellSize);
                panelEnemy.Controls.Add(enemyNum);
            }

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Button playerButton = new Button();
                    playerButton.Size = new Size(cellSize, cellSize);
                    playerButton.Location = new Point(col * cellSize + leftMargin, row * cellSize + topMargin);
                    playerButton.Enabled = false;
                    panelPlayer.Controls.Add(playerButton);
                    playerButtons[row, col] = playerButton;

                    Button enemyButton = new Button();
                    enemyButton.Size = new Size(cellSize, cellSize);
                    enemyButton.Location = new Point(col * cellSize + leftMargin, row * cellSize + topMargin);
                    enemyButton.Enabled = true;
                    enemyButton.Tag = new Point(row, col);
                    enemyButton.Click += enemyButton_Click;

                    panelEnemy.Controls.Add(enemyButton);
                    enemyButtons[row, col] = enemyButton;

                }
            }
        }

        private void PlaceShips()
        {
            PlaceShipForBoard(enemyBoard);
            PlaceShipForBoard(playerBoard);

        }

        private void PlaceShipForBoard(int[,] board)
        {
            int[] ships = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            
            foreach(int shipSize in ships)
            {
                bool placed = false;

                while(!placed)
                {
                    int row = this.random.Next(0, 10);
                    int col = this.random.Next(0, 10);

                    bool horizontal = this.random.Next(2) == 0;

                    if (CanPlaceShip(board, row, col, shipSize, horizontal))
                    {
                        for (int i = 0; i < shipSize; i++)
                        {
                            if (horizontal)
                            {
                                board[row, col + i] = 1;
                            }
                            else
                            {
                                board[row + i, col] = 1;
                            }
                          
                            placed = true;
                        }
                    }
                }
            }
        }

        private bool CanPlaceShip(int[,] board, int row, int col, int size, bool horizontal)
        {

            int endRow = horizontal ? row : row + size - 1;
            int endCol = horizontal ? col + size - 1 : col;

            if (endRow > 9 || endCol > 9)
                return false;


            for (int r = row - 1; r <= endRow + 1; r++)
            {
                for (int c = col - 1; c <= endCol + 1; c++)
                {
                    if (r < 0 || r > 9 || c < 0 || c > 9)
                        continue;

                    if (board[r, c] != 0)
                        return false;
                }
            }
            return true;
        }


        private void DrawPlayerShips()
        {
            for (int row = 0; row < 10; row++)
            {
                for(int col = 0; col < 10; col++)
                {
                    if (playerBoard[row, col] == 1)
                    {
                        playerButtons[row, col].BackColor = Color.Gray;
                    }
                }
            }
        }

        private void enemyButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;
            Point point = (Point)clickedButton.Tag;

            int row = point.X;
            int col = point.Y;

            if (enemyBoard[row, col] == 2 || enemyBoard[row, col] == 3)
            {
                return;
            }

            if (enemyBoard[row, col] == 1)
            {
                enemyBoard[row, col] = 2;
                clickedButton.BackColor = Color.Red;
                clickedButton.Text = "X";
                labelStatus.Text = "Попадание";

                if (IsShipSunk(enemyBoard, row, col))
                {
                    MarkAroundShipSunk(enemyBoard, enemyButtons, row, col);
                    labelStatus.Text = "Корабль убит";
                }

                if (CheckWin(enemyBoard))
                {
                    labelStatus.Text = "Вы победили!";
                    MessageBox.Show("Вы победили!");
                }
            }
            else
            {
                enemyBoard[row, col] = 3;
                enemyButtons[row, col].BackColor = SystemColors.ControlLight;
                clickedButton.Text = "*";
                labelStatus.Text = "Промах";
            }

            if (!CheckWin(enemyBoard))
            {
                EnemyTurn();
            }
        }

        private bool CheckWin(int[,] board)
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    if (board[row, col] == 1)
                        return false;
                }
            }
            return true;
        }

        private void EnemyTurn()
        {
            int row, col;

            while (true)
            {
                row = this.random.Next(0, 10);
                col = this.random.Next(0, 10);

                if (playerBoard[row, col] == 2 || playerBoard[row, col] == 3)
                {
                    continue;
                }
                break;
            }

            if (playerBoard[row, col] == 1)
            {
                playerBoard[row, col] = 2;
                playerButtons[row, col].BackColor = Color.Red;
                playerButtons[row, col].Text = "X";
                
                //labelStatus.Text = "Попадание (бот)";

                if (IsShipSunk(playerBoard, row, col))
                {
                    MarkAroundShipSunk(playerBoard, playerButtons, row, col);
                    labelStatus.Text = "Корабль убит (бот)";
                }

                if (CheckWin(playerBoard))
                {
                    labelStatus.Text = "Вы проиграли!";
                    MessageBox.Show("Вы проиграли!");
                }
            }
            else
            {
                playerBoard[row, col] = 3;
                playerButtons[row, col].BackColor = SystemColors.ControlLight;
                playerButtons[row, col].ForeColor = SystemColors.ControlText;
                playerButtons[row, col].Text = "*";
            
                //labelStatus.Text = "Промах (бот)";
            }
        }

        private bool IsShipSunk(int[,] board, int row, int col)
        {
            List<Point> shipCells = GetShipCells(board, row, col);
            foreach (Point cell in shipCells)
            {
                if (board[cell.X, cell.Y] != 2)
                {
                    return false;
                }
            }
            return true;
        }


        private List<Point> GetShipCells(int[,] board, int row, int col)
        {
            List<Point> cells = new List<Point>();
            cells.Add(new Point(row, col));

            for (int r = row - 1; r >= 0 && (board[r, col] == 1 || board[r, col] == 2); r--)
            {
                cells.Add(new Point(r, col));
            }

            for (int r = row + 1; r < 10 && (board[r, col] == 1 || board[r, col] == 2); r++)
            {
                cells.Add(new Point(r, col));
            }

            for (int c = col - 1; c >= 0 && (board[row, c] == 1 || board[row, c] == 2); c--)
            {
                cells.Add(new Point(row, c));
            }

            for (int c = col + 1; c < 10 && (board[row, c] == 1 || board[row, c] == 2); c++)
            {
                cells.Add(new Point(row, c));
            }

            return cells;
        }

        private void MarkAroundShipSunk(int[,] board, Button[,] buttons, int row, int col)
        {
            List<Point> shipCells = GetShipCells(board, row, col);

            foreach (Point cell in shipCells)
            {
                for (int r = cell.X - 1; r <= cell.X + 1; r++)
                {
                    for (int c = cell.Y - 1; c <= cell.Y + 1; c++)
                    {
                        if (r < 0 || r > 9 || c < 0 || c > 9)
                        {
                            continue;
                        }

                        if (board[r, c] == 0)
                        {
                            board[r, c] = 3;
                            buttons[r, c].BackColor = SystemColors.ControlLight;
                            buttons[r, c].ForeColor = SystemColors.ControlText;
                            buttons[r, c].Text = "*";
                            
                        }
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            playerBoard = new int[10, 10];
            enemyBoard = new int[10, 10];

            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    playerButtons[row, col].BackColor = SystemColors.ControlLight;
                    playerButtons[row, col].Text = "";
                    

                    enemyButtons[row, col].BackColor = SystemColors.ControlLight;
                    enemyButtons[row, col].Text = "";
                    enemyButtons[row, col].Enabled = true;
                }
            }

            PlaceShips();
            DrawPlayerShips();
            labelStatus.Text = "Ход игрока";
        }
    }
}
