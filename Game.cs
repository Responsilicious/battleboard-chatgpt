public enum CellState
{
    Empty,
    Occupied,
    Hit,
    Miss
}

public interface ICell
{
    CellState State { get; set; }
}

public class Cell : ICell
{
    public CellState State { get; set; }

    public Cell()
    {
        State = CellState.Empty;
    }
}

public interface IShip
{
    int Size { get; }
    bool IsSunk { get; }
    IList<ICell> OccupiedCells { get; }
}

public class Ship : IShip
{
    public int Size { get; private set; }
    public IList<ICell> OccupiedCells { get; private set; }

    public bool IsSunk
    {
        get
        {
            return OccupiedCells.All(c => c.State == CellState.Hit);
        }
    }

    public Ship(int size)
    {
        Size = size;
        OccupiedCells = new List<ICell>(size);
    }
}

public interface IBoard
{
    ICell[,] Cells { get; }
    bool PlaceShips();
    bool Shoot(int x, int y, out string result);
}

public class Board : IBoard
{
    private const int BOARD_SIZE = 10;
    private readonly ICell[,] _cells;
    private readonly IList<IShip> _ships;

    public ICell[,] Cells
    {
        get { return _cells; }
    }

    public Board()
    {
        _cells = new ICell[BOARD_SIZE, BOARD_SIZE];
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                _cells[i, j] = new Cell();
            }
        }

        _ships = new List<IShip>()
        {
            new Ship(5),
            new Ship(4),
            new Ship(3),
            new Ship(3),
            new Ship(2)
        };
    }

    public bool PlaceShips()
    {
        Random random = new Random();
        foreach (IShip ship in _ships)
        {
            int x = random.Next(BOARD_SIZE);
            int y = random.Next(BOARD_SIZE);
            bool horizontal = random.Next(2) == 0;
            if (CanPlaceShip(ship, x, y, horizontal))
            {
                PlaceShip(ship, x, y, horizontal);
            }
            else
            {
                return false;
            }
        }
        return true;
    }

    private bool CanPlaceShip(IShip ship, int x, int y, bool horizontal)
    {
        if (horizontal && x + ship.Size > BOARD_SIZE || !horizontal && y + ship.Size > BOARD_SIZE)
        {
            return false;
        }

        for (int i = 0; i < ship.Size; i++)
        {
            if (horizontal && _cells[y, x + i].State == CellState.Occupied ||
                !horizontal && _cells[y + i, x].State == CellState.Occupied)
            {
                return false;
            }
        }

        return true;
    }

    private void PlaceShip(IShip ship, int x, int y, bool horizontal)
    {
        for (int i = 0; i < ship.Size; i++)
        {
            if (horizontal)
            {
                _cells[y, x + i].State = CellState.Occupied;
                ship.OccupiedCells.Add(_cells[y, x + i]);
                        }
        else
        {
            _cells[y + i, x].State = CellState.Occupied;
            ship.OccupiedCells.Add(_cells[y + i, x]);
        }
    }
}

public bool Shoot(int x, int y, out string result)
{
    if (_cells[y, x].State == CellState.Occupied)
    {
        _cells[y, x].State = CellState.Hit;
        IShip ship = _ships.FirstOrDefault(s => s.OccupiedCells.Contains(_cells[y, x]));
        if (ship != null && ship.IsSunk)
        {
            result = "Sunk!";
        }
        else
        {
            result = "Hit!";
        }
        return true;
    }
    else if (_cells[y, x].State == CellState.Empty)
    {
        _cells[y, x].State = CellState.Miss;
        result = "Miss!";
        return true;
    }
    else
    {
        result = "Already hit!";
        return false;
    }
}
