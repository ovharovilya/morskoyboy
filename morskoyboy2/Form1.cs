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
            Random random = new Random();
            int shipsToPlace = 10;
            int placed = 0;

            while(placed < shipsToPlace)
            {
                int row = random.Next(0, 10);
                int col = random.Next(0, 10);
                if (enemyBoard[row, col] == 0)
                {
                    enemyBoard[row, col] = 1;
                    placed++;
                }
            }

            placed = 0;

            while (placed < shipsToPlace)
            {
                int row = random.Next(0, 10);
                int col = random.Next(0, 10);
                if (playerBoard[row, col] == 0)
                {
                    playerBoard[row, col] = 1;
                    placed++;
                }
            }
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

                if (CheckWin(enemyBoard))
                {
                    labelStatus.Text = "Вы победили!";
                    MessageBox.Show("Вы победили!");
                }
            }
            else
            {
                enemyBoard[row, col] = 3;
                clickedButton.Text = "*";
                labelStatus.Text = "Промах";
            }
            EnemyTurn();
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
            Random random = new Random();
            
            int row, col;

            while (true)
            {
                row = random.Next(0, 10);
                col = random.Next(0, 10);

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
                labelStatus.Text = "Попадание (бот)";

                if (CheckWin(playerBoard))
                {
                    labelStatus.Text = "Вы проиграли!";
                    MessageBox.Show("Вы проиграли!");
                }
            }
            else
            {
                playerBoard[row, col] = 3;
                playerButtons[row, col].BackColor = Color.LightGray;
                playerButtons[row, col].Text = "*";
                labelStatus.Text = "Промах (бот)";
            }

        }
    }
}
