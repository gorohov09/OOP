using System;
using System.Collections.Generic;

namespace OOP
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
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

        private bool _isShip;
        public bool IsShip
        {
            get => _isShip;
            set => _isShip = value;
        }

        public Ship ship;
    }

    public class Ship
    {
        public int x;
        public int y;

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
        public List<Ship> ships;

        public int size;

        public Point[,] points;

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
            ship.x = x; 
            ship.y = y;
            
            //Отмечаем, что точки принадлежат короблю
            if (ship.isVertical)
            {
                for (int i = y; i < ship.size; i++)
                {
                    points[x, i].IsShip = true;
                    points[x, i].ship = ship;
                }
            }
            else
            {
                for (int i = x; i < ship.size; i++)
                {
                    points[i, y].IsShip = true;
                    points[i, y].ship = ship;
                }
            }

            ships.Add(ship);
            return true;
        }

    }

    public class Player 
    {
        public (int, int) GetShootPoint()
        {
            return (0, 0);
        }
    } 

    public class GameSession
    {
        public Player player1;

        public Player player2;

        public PlayGround playground1;

        public PlayGround playground2;

        public bool isPlayer1;

        public void Start()
        {
            ArrangementShips(playground1);
            ArrangementShips(playground2);

            while (true)
            {
                Shoot();
            }
        }

        public void Shoot(int x, int y)
        {
            if (isPlayer1)
                ShootPlayer(playground2, x, y, false);
            else
                ShootPlayer(playground1, x, y, true);
        }

        private void ArrangementShips(PlayGround playGround)
        {
            for (int i = 0; i < 4; i++)
            {
                var ship = new Ship
                {
                    isVertical = i % 2 == 0 ? true : false,
                    size = i + 1
                };
                playGround.AddShip(ship, i, i + 1);
            }
        }

        private void ShootPlayer(PlayGround playground, int x, int y, bool select)
        {
            var result = playground.Shoot(x, y);
            if (result != ShootResult.Got && result != ShootResult.Kill)
                isPlayer1 = select;
        }
    }
}
