using System.Diagnostics;

namespace Shared.Game;

public class Pong
{
    public async Task Run()
    {
        var width = Console.WindowWidth;
        var height = Console.WindowHeight;
        var multiplier = 1.1f;
        var delay = TimeSpan.FromMilliseconds(10);
        var enemyInputDelay = TimeSpan.FromMilliseconds(100);
        var paddleSize = height / 4;
        Stopwatch stopwatch = new();
        Stopwatch enemyStopwatch = new();
        var scoreA = 0;
        var scoreB = 0;
        Ball ball;
        var paddleA = height / 3;
        var paddleB = height / 3;

        Console.Clear();
        stopwatch.Restart();
        enemyStopwatch.Restart();
        Console.CursorVisible = false;
        while (scoreA < 3 && scoreB < 3)
        {
            ball = CreateNewBall();
            while (true)
            {
                #region Update Ball

                // Compute Time And New Ball Position
                var time = (float)stopwatch.Elapsed.TotalSeconds * 15;
                var (X2, Y2) = (ball.X + time * ball.dX, ball.Y + time * ball.dY);

                // Collisions With Up/Down Walls
                if (Y2 < 0 || Y2 > height)
                {
                    ball.dY = -ball.dY;
                    Y2 = ball.Y + ball.dY;
                }

                // Collision With Paddle A
                if (Math.Min(ball.X, X2) <= 2 && 2 <= Math.Max(ball.X, X2))
                {
                    var ballPathAtPaddleA =
                        height - (int)GetLineValue(((ball.X, height - ball.Y), (X2, height - Y2)), 2);
                    ballPathAtPaddleA = Math.Max(0, ballPathAtPaddleA);
                    ballPathAtPaddleA = Math.Min(height - 1, ballPathAtPaddleA);
                    if (paddleA <= ballPathAtPaddleA && ballPathAtPaddleA <= paddleA + paddleSize)
                    {
                        ball.dX = -ball.dX;
                        ball.dX *= multiplier;
                        ball.dY *= multiplier;
                        X2 = ball.X + time * ball.dX;
                    }
                }

                // Collision With Paddle B
                if (Math.Min(ball.X, X2) <= width - 2 && width - 2 <= Math.Max(ball.X, X2))
                {
                    var ballPathAtPaddleB =
                        height - (int)GetLineValue(((ball.X, height - ball.Y), (X2, height - Y2)), width - 2);
                    ballPathAtPaddleB = Math.Max(0, ballPathAtPaddleB);
                    ballPathAtPaddleB = Math.Min(height - 1, ballPathAtPaddleB);
                    if (paddleB <= ballPathAtPaddleB && ballPathAtPaddleB <= paddleB + paddleSize)
                    {
                        ball.dX = -ball.dX;
                        ball.dX *= multiplier;
                        ball.dY *= multiplier;
                        X2 = ball.X + time * ball.dX;
                    }
                }

                // Collisions With Left/Right Walls
                if (X2 < 0)
                {
                    scoreB++;
                    break;
                }

                if (X2 > width)
                {
                    scoreA++;
                    break;
                }

                // Updating Ball Position
                Console.SetCursorPosition((int)ball.X, (int)ball.Y);
                Console.Write(' ');
                ball.X += time * ball.dX;
                ball.Y += time * ball.dY;
                Console.SetCursorPosition((int)ball.X, (int)ball.Y);
                Console.Write('O');

                #endregion

                #region Update Player Paddle

                if (Console.KeyAvailable)
                    switch (Console.ReadKey(true).Key)
                    {
                        case ConsoleKey.UpArrow:
                            paddleA = Math.Max(paddleA - 1, 0);
                            break;
                        case ConsoleKey.DownArrow:
                            paddleA = Math.Min(paddleA + 1, height - paddleSize - 1);
                            break;
                        case ConsoleKey.Escape:
                            Console.Clear();
                            Console.Write("Pong was closed.");
                            return;
                    }

                while (Console.KeyAvailable) Console.ReadKey(true);

                #endregion

                #region Update Computer Paddle

                if (enemyStopwatch.Elapsed > enemyInputDelay)
                {
                    if (ball.Y < paddleB + paddleSize / 2 && ball.dY < 0)
                        paddleB = Math.Max(paddleB - 1, 0);
                    else if (ball.Y > paddleB + paddleSize / 2 && ball.dY > 0)
                        paddleB = Math.Min(paddleB + 1, height - paddleSize - 1);
                    enemyStopwatch.Restart();
                }

                #endregion

                #region Render Paddles

                for (var i = 0; i < height; i++)
                {
                    Console.SetCursorPosition(2, i);
                    Console.Write(paddleA <= i && i <= paddleA + paddleSize ? '█' : ' ');
                    Console.SetCursorPosition(width - 2, i);
                    Console.Write(paddleB <= i && i <= paddleB + paddleSize ? '█' : ' ');
                }

                #endregion

                stopwatch.Restart();
                Thread.Sleep(delay);
            }

            Console.SetCursorPosition((int)ball.X, (int)ball.Y);
            Console.Write(' ');
        }

        Console.Clear();
        if (scoreA > scoreB) Console.Write("You win.");
        if (scoreA < scoreB) Console.Write("You lose.");

        Ball CreateNewBall()
        {
            var randomFloat = (float)Random.Shared.NextDouble() * 2f;
            var dx = Math.Max(randomFloat, 1f - randomFloat);
            var dy = 1f - dx;
            float x = width / 2;
            float y = height / 2;
            if (Random.Shared.Next(2) is 0) dx = -dx;
            if (Random.Shared.Next(2) is 0) dy = -dy;
            return new Ball
            {
                X = x,
                Y = y,
                dX = dx,
                dY = dy
            };
        }

        float GetLineValue(((float X, float Y) A, (float X, float Y) B) line, float x)
        {
            // order points from least to greatest X
            if (line.B.X < line.A.X) (line.A, line.B) = (line.B, line.A);
            // find the slope
            var slope = (line.B.Y - line.A.Y) / (line.B.X - line.A.X);
            // find the y-intercept
            var yIntercept = line.A.Y - line.A.X * slope;
            // find the function's value at parameter "x"
            return x * slope + yIntercept;
        }
    }
}

public class Ball
{
    public float dX;
    public float dY;
    public float X;
    public float Y;
}