using Timer = System.Windows.Forms.Timer;
namespace ProjectSnake
{
    public partial class Form1 : Form
    {
        List<Button> buttons = new List<Button>();
        private List<Point> snake = new List<Point>();
        private Point food;
        private Direction currentDirection = Direction.Right;
        private Timer gameTimer = new Timer();
        bool gameRunning = false;
        private int score = 0;
        private Label scoreLable;
        private int maxSpeed = 200;
        private int intervalSpeed = 5;
        private int minSpeed = 50;
        private Label level;
        private int lvl = 0;
        private enum Direction
        {
            Up,
            Down,
            Left,
            Right
        }
        public Form1()
        {
            InitializeComponent();
            Init();
        }

        void ButtonsInis()
        {
            for (int i = 0; i < 15; i++)
            {
                for (int j = 0; j < 15; j++)
                {
                    Button btn = new Button
                    {
                        Size = new Size(40, 40),
                        Location = new Point(i * 40, j * 40),
                        TabStop = false,   
                        BackColor = Color.LightGray,
                        FlatStyle = FlatStyle.Flat,
                        Enabled = false,
                    };

                    btn.FlatAppearance.BorderSize = 0;
                    buttons.Add(btn);
                    Controls.Add(btn);
                }
            }
        }            
        void Init()
        {
            ClientSize = new Size(600, 650);
            Text = "Змейка";

            scoreLable = new Label
            {
                Text = "Счет: 0",
                Location = new Point(10,610),
                Size = new Size(200,30),
                Font = new Font("Arial", 14),
                ForeColor = Color.Black,                
            };
            Controls.Add(scoreLable);

            level = new Label
            {
                Text = "Уровень: 0",
                Location = new Point(480, 610),
                Size = new Size(200, 30),
                Font = new Font("Arial", 14),
                ForeColor = Color.Black,
            };
            Controls.Add(level);

            ButtonsInis();
            gameTimer.Interval = 200;
            gameTimer.Tick += GameTimer_Tick;
            
            StartGame();
            
        }       

        void GameDisplay()
        {
            foreach (Button btn in buttons)
            {
                btn.BackColor = Color.LightGray;
            }

            for (int i = 0; i < snake.Count; i++)
            {
                int index = snake[i].X * 15 + snake[i].Y;
                if (index >= 0 && index < buttons.Count)
                {
                    if (i == 0)
                    {
                        buttons[index].BackColor = Color.DarkGreen;
                    }
                    else
                    {
                        buttons[index].BackColor= Color.Green;
                    }

                }
                int foodIndex = food.X * 15 + food.Y;
                if (foodIndex >= 0 &&  foodIndex < buttons.Count)
                {
                    buttons[foodIndex].BackColor = Color.Red;
                }
            }
        }
        void InitSnake()
        {
            snake.Clear();

            int startX = 7;
            int startY = 7;

            snake.Add(new Point(startX, startY));
            snake.Add(new Point(startX - 1, startY));
            snake.Add(new Point(startX - 2, startY));

            currentDirection = Direction.Right;

        }
        void GenFood ()
        {
            Random random = new Random();

            do
            {
                food = new Point(random.Next(0, 15), random.Next(0, 15));
            }
            while (snake.Contains(food));
        }
        private void GameTimer_Tick(object? sender, EventArgs e)
        {
           MoveSnake();
           Collision();
           GameDisplay();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            switch (e.KeyCode)
            {
                case Keys.Up:
                    if (currentDirection != Direction.Down)
                    {
                        currentDirection = Direction.Up;
                    }
                    break;
                case Keys.Down:
                    if (currentDirection != Direction.Up)
                    {
                        currentDirection = Direction.Down;
                    }
                    break;
                case Keys.Left:
                    if (currentDirection != Direction.Right)
                    {
                        currentDirection = Direction.Left;
                    }
                    break;
                case Keys.Right:
                    if (currentDirection != Direction.Left)
                    {
                        currentDirection = Direction.Right;
                    }
                    break;
            }

        }
       
        void MoveSnake()
        {
            Point newHead = snake[0];

            switch (currentDirection)
            {
                case Direction.Up:
                    newHead.Y--;
                    break;
                case Direction.Down:
                    newHead.Y++;
                    break;
                case Direction.Left:
                    newHead.X--;
                    break;
                case Direction.Right:
                    newHead.X++;
                    break;
                default:
                    break;
            }
            snake.Insert(0, newHead);

            if (newHead == food)
            {
                score++;
                scoreLable.Text = $"Счет: {score}";
                GenFood();

                if (score % intervalSpeed == 0)
                {
                    int newSpeed = maxSpeed - (score / intervalSpeed) * 20;
                    gameTimer.Interval = newSpeed;

                    if (newSpeed < minSpeed)
                    {
                        newSpeed = minSpeed;
                    }
                }
                
                if (score % 5 == 0)
                {
                    lvl++;
                    scoreLable.Text = $"Счет: {score}";
                    level.Text = $"Уровень: {lvl}";
                }

            }
            else
            {
                snake.RemoveAt(snake.Count - 1);
            }            
        }

        void Collision()
        {
            Point head = snake[0];
            if (head.X < 0 || head.X >= 15 || head.Y < 0 || head.Y >= 15)
            {
               gameTimer.Stop();
               gameRunning = false;
               MessageBox.Show($"Игра окончена! Вы врезались в стену. \nВаш счет: {score}. Ваш уровень: {lvl}");
                DialogResult result = MessageBox.Show("Хотете начать игру заново?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    StartGame();
                }
                if (result == DialogResult.No)
                {
                    Application.Exit();
                }
            }
            for (int i = 1; i < snake.Count; i++)
            {
                if (head == snake[i])
                {
                    gameTimer.Stop();
                    gameRunning = false;
                    MessageBox.Show($"Игра окончена! Вы врезались в тело змейки. \nВаш счет: {score}. Ваш уровень: {lvl}");                   
                    DialogResult result = MessageBox.Show("Хотете начать игру заново?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        StartGame();
                    }
                    if (result == DialogResult.No)
                    {
                        Application.Exit();
                    }

                }
            }
        }
        void StartGame()
        {
            snake.Clear();
            score = 0;
            scoreLable.Text = "Счет: 0";
            level.Text = "Уровень: 0";
            currentDirection = Direction.Right;
            InitSnake();
            GenFood();
            GameDisplay();
            gameRunning = true;
            gameTimer.Start();
        }






    }
}
