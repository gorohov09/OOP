using System;
using System.Collections.Generic;

namespace OOP
{
    public class Program
    {
        static void Main(string[] args)
        {
            GameSession session = new GameSession();

            var result = session.Start("A", "B", 12);
            Console.WriteLine(result);
        }
    }

    public class Point
    {
        public int X;
        public int Y;
        private bool _shooted;
        public bool Shooted
        {
            get => _shooted;
            set => _shooted = value;
        }

        public bool IsShip
        {
            get => ship != null;
        }

        public Ship ship;
    }

    public class Ship
    {
        public int size;

        public bool isVertical;

        public int countHits;

        public bool IsDestruct()
        {
            return countHits == size;
        }
    }

    public enum ShootResult
    {
        Got,
        Missied,
        Kill,
        Impossible
    }

    public class PlayGround
    {
        public List<Ship> ships = new List<Ship>();

        public int size;

        public Point[,] points;

        public PlayGround(int size)
        {
            this.size = size;
            InitPoints();
        }

        public ShootResult Shoot(int x, int y)
        {
            if (x < 0 || y < 0 || x >= size || y >= size) 
                return ShootResult.Impossible;

            if (points[x, y].Shooted)
                return ShootResult.Missied;

            points[x, y].Shooted = true;

            if (points[x, y].IsShip)
            {
                var ship = points[x, y].ship;

                ship.countHits++;
                if (ship.IsDestruct())
                    return ShootResult.Kill;
                else
                    return ShootResult.Got;
            }

            return ShootResult.Missied;
        }

        public bool AddShip(Ship ship, int x, int y)
        {
            //Отмечаем, что точки принадлежат короблю
            if (ship.isVertical)
            {
                if (ship.size + y > size)
                    return false;


                if (!IsCheckShipRect(x, y, 1, ship.size))
                    return false;

                for (int i = y; i < y + ship.size; i++)
                {
                    points[x, i].ship = ship;
                }
            }
            else
            {
                if (ship.size + x > size)
                    return false;

                if (!IsCheckShipRect(x, y, ship.size, 1))
                    return false;

                for (int i = x; i < x + ship.size; i++)
                {
                    points[i, y].ship = ship;
                }
            }

            ships.Add(ship);
            return true;
        }

        public bool IsAllShipsDestroyed()
        {
            foreach (var ship in ships)
            {
                if (!ship.IsDestruct())
                    return false;
            }

            return true;
        }

        public void InitPoints()
        {
            points = new Point[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    points[i, j] = new Point { X = i, Y = j };
                }
            }
        }

        private bool IsCheckShipRect(int x, int y, int sizeX, int sizeY)
        {
            for (int i = x - 1; i < x + sizeX + 1; i++)
            {
                for (int j = y - 1; j < y + sizeY + 1; j++)
                {
                    if (x > 0 && y > 0 && x < size - sizeX && y < size - sizeY)
                    {
                        if (points[i, j].ship != null)
                            return false;
                    }
                }
            }

            return true;
        }

    }

    public class Player 
    {
        public string name;

        public PlayGround playgroundShoot;

        public PlayGround playgroundSet;

        public (int, int) TakeShootPoint()
        {
            Console.WriteLine($"{name} В какую точку произвести выстрел?");
            int x = int.Parse(Console.ReadLine());
            int y = int.Parse(Console.ReadLine());
            return (x, y);
        }

        public void ReceiveResult(ShootResult shootResult)
        {
            Console.WriteLine($"Результат выстрела {name}: {shootResult}");
        }
    } 

    public class GameSession
    {
        public Player player1;

        public Player player2;

        public Player currentPlayer;

        public string Start(string name1, string name2, int size)
        {
            var playground1 = new PlayGround(size);
            var playground2 = new PlayGround(size);

            player1 = new Player 
            { 
                name = name1,
                playgroundSet = playground1,
                playgroundShoot = playground2,
            };

            player2 = new Player 
            { 
                name = name2,
                playgroundSet = playground2,
                playgroundShoot = playground1,
            };

            currentPlayer = player1;

            ArrangementShips(player1.playgroundSet);
            ArrangementShips(player2.playgroundSet);

            while (true)
            {
                var result = Shoot(currentPlayer);
                if (result != ShootResult.Got && result != ShootResult.Kill)
                {
                    if (currentPlayer == player1)
                        currentPlayer = player2;
                    else
                        currentPlayer = player1;
                }
                else
                {
                    if (currentPlayer.playgroundShoot.IsAllShipsDestroyed())
                        return currentPlayer.name;
                }

                currentPlayer.ReceiveResult(result);
            }
        }

        public ShootResult Shoot(Player player)
        {
            var point = player.TakeShootPoint();
            var result = player.playgroundShoot.Shoot(point.Item1, point.Item2);

            return result;
        }

        private void ArrangementShips(PlayGround playGround)
        {
            Random random = new Random();

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < i + 1; j++)
                {
                    var ship = new Ship
                    {
                        isVertical = (i + j) % 2 == 0,
                        size = 4 - i
                    };

                    bool result = false;
                    do
                    {
                        var pointX = random.Next(0, playGround.size);
                        var pointY = random.Next(0, playGround.size);
                        result = playGround.AddShip(ship, pointX, pointY);
                        if (result)
                            Console.WriteLine($"Корабль {ship.size} x = {pointX} y = {pointY}. Расположение вертикальное = {ship.isVertical}");

                    } while (!result);
                }
            }
        }

        //private void DrawSession(Player player)
        //{
        //    var points = player.playgroundS
        //}
    }
}
