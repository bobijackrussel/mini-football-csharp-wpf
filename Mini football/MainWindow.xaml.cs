using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Threading;
using System.Windows;


namespace Mini_football
{
    public partial class MainWindow : Window
    {
        private Vector ballVelocity = new Vector(0, 0);
        private DispatcherTimer timer;

        private bool isWPressed, isAPressed, isSPressed, isDPressed;
        private bool isUpPressed, isDownPressed, isLeftPressed, isRightPressed;
        private bool isXPressed, isEnterPressed; // New variables for kicking

        private GameSettings gameSettings;

        public MainWindow(GameSettings settings)
        {
            InitializeComponent();

            this.Loaded += (s, e) =>
            {
                gameSettings = settings;
                SetPlayerControls(player1Controls, gameSettings.Player1Controls);
                SetPlayerControls(player2Controls, gameSettings.Player2Controls);

                ImageBrush newBackground = new ImageBrush
                {
                    ImageSource = new BitmapImage(new Uri(settings.BackgroundImagePath, UriKind.Relative)),
                    Stretch = Stretch.Fill
                };
                footballField.Background = newBackground;
            };

            this.Focus();

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(30);
            timer.Tick += (s, ev) =>
            {
                CheckGoalObstaclesCollisions();
                UpdatePlayerPosition();
                CheckBallCollision();
                UpdateBallPosition();
                CheckGoal();

            };
            timer.Start();

            KeyDown += Window_KeyDown;
            KeyUp += Window_KeyUp;
        }

        private void SetPlayerControls(Canvas canvas, Dictionary<string, Key> controls)
        {
            foreach (UIElement element in canvas.Children)
            {
                if (element is Button button)
                {
                    switch (button.Name)
                    {
                        case "Player1UpButton":
                        case "Player2UpButton":
                            button.Content = GetKeySymbol(controls["Up"]);
                            break;

                        case "Player1DownButton":
                        case "Player2DownButton":
                            button.Content = GetKeySymbol(controls["Down"]);
                            break;

                        case "Player1LeftButton":
                        case "Player2LeftButton":
                            button.Content = GetKeySymbol(controls["Left"]);
                            break;

                        case "Player1RightButton":
                        case "Player2RightButton":
                            button.Content = GetKeySymbol(controls["Right"]);
                            break;

                        case "Player1KickButton":
                        case "Player2KickButton":
                            button.Content = GetKeySymbol(controls["Kick"]);
                            break;
                    }
                }
            }
        }
        private string GetKeySymbol(Key key)
        {
            switch (key)
            {
                case Key.Left: return "←";
                case Key.Right: return "→";
                case Key.Up: return "↑";
                case Key.Down: return "↓";
                case Key.Enter: return "⏎";
                case Key.Space: return "␣";
                default:
                    return key.ToString();
            }
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == gameSettings.Player1Controls["Up"]) isWPressed = true;
            if (e.Key == gameSettings.Player1Controls["Down"]) isSPressed = true;
            if (e.Key == gameSettings.Player1Controls["Left"]) isAPressed = true;
            if (e.Key == gameSettings.Player1Controls["Right"]) isDPressed = true;
            if (e.Key == gameSettings.Player1Controls["Kick"]) isXPressed = true;


            if (e.Key == gameSettings.Player2Controls["Up"]) isUpPressed = true;
            if (e.Key == gameSettings.Player2Controls["Down"]) isDownPressed = true;
            if (e.Key == gameSettings.Player2Controls["Left"]) isLeftPressed = true;
            if (e.Key == gameSettings.Player2Controls["Right"]) isRightPressed = true;
            if (e.Key == gameSettings.Player2Controls["Kick"]) isEnterPressed = true;

        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {

            if (e.Key == gameSettings.Player1Controls["Up"]) { isWPressed = false; player1Controls.Visibility = Visibility.Collapsed; }
            if (e.Key == gameSettings.Player1Controls["Left"]) { isAPressed = false; player1Controls.Visibility = Visibility.Collapsed; }
            if (e.Key == gameSettings.Player1Controls["Down"]) { isSPressed = false; player1Controls.Visibility = Visibility.Collapsed; }
            if (e.Key == gameSettings.Player1Controls["Right"]) { isDPressed = false; player1Controls.Visibility = Visibility.Collapsed; }
            if (e.Key == gameSettings.Player1Controls["Kick"]) { isXPressed = false; player1Controls.Visibility = Visibility.Collapsed; }

            if (e.Key == gameSettings.Player2Controls["Up"]) { isUpPressed = false; player2Controls.Visibility = Visibility.Collapsed; }
            if (e.Key == gameSettings.Player2Controls["Down"]) { isDownPressed = false; player2Controls.Visibility = Visibility.Collapsed; }
            if (e.Key == gameSettings.Player2Controls["Left"]) { isLeftPressed = false; player2Controls.Visibility = Visibility.Collapsed; }
            if (e.Key == gameSettings.Player2Controls["Right"]) { isRightPressed = false; player2Controls.Visibility = Visibility.Collapsed; }
            if (e.Key == gameSettings.Player2Controls["Kick"]) { isEnterPressed = false; player2Controls.Visibility = Visibility.Collapsed; }  // Player 2 kick

        }

        private void UpdatePlayerPosition()
        {
            double player1Left = Canvas.GetLeft(player1);
            double player1Top = Canvas.GetTop(player1);

            double player2Left = Canvas.GetLeft(player2);
            double player2Top = Canvas.GetTop(player2);


            if (isWPressed && player1Top > 0) Canvas.SetTop(player1, player1Top - 5);
            if (isSPressed && player1Top < footballField.ActualHeight - player1.Height) Canvas.SetTop(player1, player1Top + 5);
            if (isAPressed && player1Left > 0) Canvas.SetLeft(player1, player1Left - 5);
            if (isDPressed && player1Left < footballField.ActualWidth - player1.Width) Canvas.SetLeft(player1, player1Left + 5);


            if (isUpPressed && player2Top > 0) Canvas.SetTop(player2, player2Top - 5);
            if (isDownPressed && player2Top < footballField.ActualHeight - player2.Height) Canvas.SetTop(player2, player2Top + 5);
            if (isLeftPressed && player2Left > 0) Canvas.SetLeft(player2, player2Left - 5);
            if (isRightPressed && player2Left < footballField.ActualWidth - player2.Width) Canvas.SetLeft(player2, player2Left + 5);

            CheckPlayerCollision(player1Left, player1Top, player2Left, player2Top);
        }

        private void CheckPlayerCollision(double player1Left, double player1Top, double player2Left, double player2Top)
        {
            double distance = Math.Sqrt(Math.Pow(player2Left - player1Left, 2) + Math.Pow(player2Top - player1Top, 2));
            double collisionDistance = (player1.Width / 2 + player2.Width / 2);

            if (distance < collisionDistance)
            {

                Vector direction = new Vector(player2Left - player1Left, player2Top - player1Top);
                direction.Normalize();


                Canvas.SetLeft(player1, player1Left - direction.X * 5);
                Canvas.SetTop(player1, player1Top - direction.Y * 5);
                Canvas.SetLeft(player2, player2Left + direction.X * 5);
                Canvas.SetTop(player2, player2Top + direction.Y * 5);
            }
        }

        private void CheckBallCollision()
        {
            double ballLeft = Canvas.GetLeft(ball);
            double ballTop = Canvas.GetTop(ball);
            double ballRadius = ball.Width / 2;
            double ballCenterX = ballLeft + ballRadius;
            double ballCenterY = ballTop + ballRadius;

            double player1Left = Canvas.GetLeft(player1);
            double player1Top = Canvas.GetTop(player1);
            double player1Radius = player1.Width / 2;
            double player1CenterX = player1Left + player1Radius;
            double player1CenterY = player1Top + player1Radius;

            double player2Left = Canvas.GetLeft(player2);
            double player2Top = Canvas.GetTop(player2);
            double player2Radius = player2.Width / 2;
            double player2CenterX = player2Left + player2Radius;
            double player2CenterY = player2Top + player2Radius;

            Vector collisionDirection;
            if (IsCollision(ballCenterX, ballCenterY, ballRadius, player1CenterX, player1CenterY, player1Radius, out collisionDirection))
            {
                ReflectBallVelocity(collisionDirection);
            }

            else if (IsCollision(ballCenterX, ballCenterY, ballRadius, player2CenterX, player2CenterY, player2Radius, out collisionDirection))
            {
                ReflectBallVelocity(collisionDirection);
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            PauseWindow pauseWindow = new PauseWindow();

            pauseWindow.Owner = this;
            pauseWindow.ShowDialog();


            if (pauseWindow.UserConfirmedExit)
            {
                pauseWindow.Close();
                this.Close();
            }
        }

        private bool IsCollision(double ballX, double ballY, double ballRadius, double playerX, double playerY, double playerRadius, out Vector collisionDirection)
        {
            double distance = Math.Sqrt(Math.Pow(ballX - playerX, 2) + Math.Pow(ballY - playerY, 2));
            double minDistance = ballRadius + playerRadius;

            if (distance < minDistance) // Collision detected
            {
                collisionDirection = new Vector(ballX - playerX, ballY - playerY);
                collisionDirection.Normalize();
                return true;
            }

            collisionDirection = new Vector(0, 0); // No collision
            return false;
        }

        private void ReflectBallVelocity(Vector collisionDirection)
        {
            // Reflect ball velocity in the direction opposite to the player
            ballVelocity = collisionDirection * Math.Max(ballVelocity.Length, 5); // Ensure minimum speed
        }
        
        private void UpdateBallPosition()
        {
            double newBallLeft = Canvas.GetLeft(ball) + ballVelocity.X;
            double newBallTop = Canvas.GetTop(ball) + ballVelocity.Y;

            if (newBallLeft < 0)
            {
                newBallLeft = 0;
                ballVelocity.X = -ballVelocity.X;
            }
            else if (newBallLeft > footballField.ActualWidth - ball.Width)
            {
                newBallLeft = footballField.ActualWidth - ball.Width;
                ballVelocity.X = -ballVelocity.X;
            }

            if (newBallTop < 0)
            {
                newBallTop = 0;
                ballVelocity.Y = -ballVelocity.Y;
            }
            else if (newBallTop > footballField.ActualHeight - ball.Height)
            {
                newBallTop = footballField.ActualHeight - ball.Height;
                ballVelocity.Y = -ballVelocity.Y;
            }

            // Check for player kick
            if (isXPressed && IsBallNearPlayer(Canvas.GetLeft(player1), Canvas.GetTop(player1)))
            {
                KickBallFromPlayer(player1);
            }
            else if (isEnterPressed && IsBallNearPlayer(Canvas.GetLeft(player2), Canvas.GetTop(player2)))
            {
                KickBallFromPlayer(player2);
            }

            Canvas.SetLeft(ball, newBallLeft);
            Canvas.SetTop(ball, newBallTop);

            // Damping effect
            ballVelocity *= 0.95;
        }

        private void KickBallFromPlayer(UIElement player)
        {
            double playerLeft = Canvas.GetLeft(player);
            double playerTop = Canvas.GetTop(player);
            double ballLeft = Canvas.GetLeft(ball);
            double ballTop = Canvas.GetTop(ball);

            // Calculate the direction vector from player to ball
            Vector direction = new Vector(ballLeft - playerLeft, ballTop - playerTop);
            direction.Normalize(); // Normalize to get a unit vector

            // Set the ball's velocity
            ballVelocity = direction * 10; // Scale by desired kick strength
            ballVelocity.Y += (ballTop - playerTop) * 0.2; // Add some upward velocity based on the player's position
        }


        private bool IsBallNearPlayer(double playerLeft, double playerTop)
        {
            double ballLeft = Canvas.GetLeft(ball);
            double ballTop = Canvas.GetTop(ball);
            double distance = Math.Sqrt(Math.Pow(ballLeft - playerLeft, 2) + Math.Pow(ballTop - playerTop, 2));

            return distance < 50;
        }

        private const double BallWidth = 24;
        private const double GoalDetectionThreshold = BallWidth * 0.75;

        private int scorePlayer1 = 0;
        private int scorePlayer2 = 0;

        private void CheckGoal()
        {
            double ballLeft = Canvas.GetLeft(ball) + BallWidth / 2;
            double ballTop = Canvas.GetTop(ball) + BallWidth / 2;

            // Detekcija gola za igrača 1
            double goal1LineLeft = Canvas.GetLeft(Goal1_line) + GoalDetectionThreshold;
            double goal1TopPost = Canvas.GetTop(Goal1_SidePostTop);
            double goal1BottomPost = Canvas.GetTop(Goal1_SidePostBottom);

            if (ballLeft <= goal1LineLeft &&
                ballTop >= goal1TopPost &&
                ballTop <= goal1BottomPost)
            {
                // Gol za igrača 1 - povećaj rezultat i ažuriraj semafor
                scorePlayer1++;
                ShowGoalMessage();
                UpdateScoreboard();
                ResetGameAfterGoal();
                CheckForWinner("Player1");
                return; // Izlaz da bi se izbegla dvostruka detekcija
            }

            // Detekcija gola za igrača 2
            double goal2LineLeft = Canvas.GetLeft(Goal2_line) - GoalDetectionThreshold;
            double goal2TopPost = Canvas.GetTop(Goal2_sidePostTop);
            double goal2BottomPost = Canvas.GetTop(Goal2_sidePostBottom);

            if (ballLeft >= goal2LineLeft &&
                ballTop >= goal2TopPost &&
                ballTop <= goal2BottomPost)
            {
                // Gol za igrača 2 - povećaj rezultat i ažuriraj semafor
                scorePlayer2++;
                ShowGoalMessage();
                UpdateScoreboard();
                ResetGameAfterGoal();
                CheckForWinner("Player2");
            }
        }

        private void UpdateScoreboard()
        {
            scoreboard.Content = $"{scorePlayer1} - {scorePlayer2}";
        }

        private void ResetGameAfterGoal()
        {
            // Resetuj poziciju lopte
            Canvas.SetLeft(ball, 388);
            Canvas.SetTop(ball, 212);

            // Resetuj pozicije igrača
            Canvas.SetLeft(player1, 105);  // Inicijalna X koordinata za player1
            Canvas.SetTop(player1, 200);   // Inicijalna Y koordinata za player1

            Canvas.SetLeft(player2, 645);  // Inicijalna X koordinata za player2
            Canvas.SetTop(player2, 200);   // Inicijalna Y koordinata za player2

            // Resetuj sve pritiske tastera na false
            isWPressed = false;
            isAPressed = false;
            isSPressed = false;
            isDPressed = false;

            isUpPressed = false;
            isDownPressed = false;
            isLeftPressed = false;
            isRightPressed = false;

            isXPressed = false;
            isEnterPressed = false;
        }
        private async void ShowGoalMessage()
        {
            goal.Visibility = Visibility.Visible;
            Message.Visibility = Visibility.Visible;

            await Task.Delay(1000);

            goal.Visibility = Visibility.Collapsed;
            Message.Visibility = Visibility.Collapsed;

        }
        private void CheckGoalObstaclesCollisions()
        {
            // Koordinate i dimenzije elemenata gola za igrača 1
            CheckCollisionWithObstacle(ball, Goal1_frontPost_Top);
            CheckCollisionWithObstacle(ball, Goal1_frontPost_Bottom);
            CheckCollisionWithObstacle(ball, Goal1_backNet);
            CheckCollisionWithObstacle(ball, Goal1_SidePostTop);
            CheckCollisionWithObstacle(ball, Goal1_SidePostBottom);

            // Koordinate i dimenzije elemenata gola za igrača 2
            CheckCollisionWithObstacle(ball, Goal2_frontPost_Top);
            CheckCollisionWithObstacle(ball, Goal2_frontPost_Bottom);
             CheckCollisionWithObstacle(ball, Goal2_backNet);
            CheckCollisionWithObstacle(ball, Goal2_sidePostTop);
            CheckCollisionWithObstacle(ball, Goal2_sidePostBottom);

            // Proveri da igrači ne prođu kroz golove
            CheckCollisionWithObstacle(player1, Goal1_frontPost_Top);
            CheckCollisionWithObstacle(player1, Goal1_frontPost_Bottom);
            CheckCollisionWithObstacle(player1, Goal1_backNet);
            CheckCollisionWithObstacle(player1, Goal1_SidePostTop);
            CheckCollisionWithObstacle(player1, Goal1_SidePostBottom);

            CheckCollisionWithObstacle(player2, Goal2_frontPost_Top);
            CheckCollisionWithObstacle(player2, Goal2_frontPost_Bottom);
            CheckCollisionWithObstacle(player2, Goal2_backNet);
            CheckCollisionWithObstacle(player2, Goal2_sidePostTop);
            CheckCollisionWithObstacle(player2, Goal2_sidePostBottom);
        }

        private void CheckCollisionWithObstacle(UIElement movingObject, UIElement obstacle)
        {
            // Get positions and dimensions of objects
            double movingLeft = Canvas.GetLeft(movingObject);
            double movingTop = Canvas.GetTop(movingObject);
            double movingWidth = movingObject.RenderSize.Width;
            double movingHeight = movingObject.RenderSize.Height;

            double obstacleLeft = Canvas.GetLeft(obstacle);
            double obstacleTop = Canvas.GetTop(obstacle);
            double obstacleWidth = obstacle.RenderSize.Width;
            double obstacleHeight = obstacle.RenderSize.Height;

            // Check for overlap (collision) with obstacle
            if (movingLeft + movingWidth > obstacleLeft &&
                movingLeft < obstacleLeft + obstacleWidth &&
                movingTop + movingHeight > obstacleTop &&
                movingTop < obstacleTop + obstacleHeight)
            {
                if (movingObject == ball) // Only apply the following logic if the moving object is the ball
                {
                    Vector collisionDirection = new Vector(
                        movingLeft + (movingWidth / 2) - (obstacleLeft + (obstacleWidth / 2)),
                        movingTop + (movingHeight / 2) - (obstacleTop + (obstacleHeight / 2))
                    );
                    collisionDirection.Normalize();

                    if (obstacle == Goal1_frontPost_Top || obstacle == Goal1_frontPost_Bottom)
                    {
                        // Reflect the ball only for front posts
                        ReflectBallVelocity(collisionDirection);
                    }
                    else
                    {
                        // For other obstacles, stop the ball by setting its velocity to zero
                        ballVelocity = new Vector(0, 0);
                    }

                    // Adjust ball position slightly to prevent it from "sticking" to the obstacle
                    double pushOutDistance = 5; // Adjust as necessary
                    Canvas.SetLeft(movingObject, Canvas.GetLeft(movingObject) + collisionDirection.X * pushOutDistance);
                    Canvas.SetTop(movingObject, Canvas.GetTop(movingObject) + collisionDirection.Y * pushOutDistance);
                }
                else
                {
                    // Existing code to stop players from moving through obstacles
                    double playerCenterX = movingLeft + (movingWidth / 2);
                    double playerCenterY = movingTop + (movingHeight / 2);
                    double obstacleCenterX = obstacleLeft + (obstacleWidth / 2);
                    double obstacleCenterY = obstacleTop + (obstacleHeight / 2);

                    Vector directionToObstacle = new Vector(playerCenterX - obstacleCenterX, playerCenterY - obstacleCenterY);
                    directionToObstacle.Normalize();

                    double overlapX = (movingWidth / 2) + (obstacleWidth / 2) - Math.Abs(playerCenterX - obstacleCenterX);
                    double overlapY = (movingHeight / 2) + (obstacleHeight / 2) - Math.Abs(playerCenterY - obstacleCenterY);

                    if (overlapX < overlapY)
                    {
                        if (playerCenterX < obstacleCenterX)
                        {
                            Canvas.SetLeft(movingObject, movingLeft - overlapX);
                        }
                        else
                        {
                            Canvas.SetLeft(movingObject, movingLeft + overlapX);
                        }
                    }
                    else
                    {
                        if (playerCenterY < obstacleCenterY)
                        {
                            Canvas.SetTop(movingObject, movingTop - overlapY);
                        }
                        else
                        {
                            Canvas.SetTop(movingObject, movingTop + overlapY);
                        }
                    }
                }
            }
        }
        int WinningScore = 2;
        private void CheckForWinner(string playerName)
        {
            if (scorePlayer1 == WinningScore || scorePlayer2 == WinningScore)
            {
                var winWindow = new WinWindow(playerName);
                winWindow.Owner = this;
                winWindow.ShowDialog();

                if (winWindow.UserConfirmedExit)
                    Close();

                if (winWindow.UserConfirmedRestart)
                    RestartGame();
            }
        }
        public  void RestartGame()
        {
            scorePlayer1 = 0;
            scorePlayer2 = 0;
            scoreboard.Content = "0-0";

            ResetGameAfterGoal();
        }

    }
}
