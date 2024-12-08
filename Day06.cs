namespace AoC;

public class Day06 : IDay
{
    private enum Direction { Up, Down, Left, Right }
    private enum Tile { Free, Obstacle }
    
    private record struct Position(int Row, int Col)
    {
        public static Position operator +(Position a, Position b) =>
            new(a.Row + b.Row, a.Col + b.Col);
    }
    
    private record Guard(Position Position, Direction Direction);

    public string Part1(string input)
    {
        var (map, guard) = ParseInput(input);
        return GetGuardPath(guard, map)
            .Select(p => p.Position)
            .Distinct()
            .Count()
            .ToString();
    }

    public string Part2(string input)
    {
        var (map, guard) = ParseInput(input);
        return map
            .GetAllPossibleMaps(guard)
            .AsParallel()
            .Count(m => HasLoop(guard, m))
            .ToString();
    }

    private static Direction RotateRight(Direction direction) => direction switch
    {
        Direction.Up => Direction.Right,
        Direction.Right => Direction.Down,
        Direction.Down => Direction.Left,
        Direction.Left => Direction.Up,
        _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, null)
    };

    private static Position GetPositionDifference(Direction direction) => direction switch
    {
        Direction.Up => new Position(-1, 0),
        Direction.Right => new Position(0, 1),
        Direction.Down => new Position(1, 0),
        Direction.Left => new Position(0, -1),
        _ => throw new ArgumentOutOfRangeException()
    };

    private static Guard UpdateGuard(Guard guard, Map map)
    {
        var nextPosition = guard.Position + GetPositionDifference(guard.Direction);
        return map.GetTileOrDefault(nextPosition) is Tile.Free or null
            ? guard with { Position = nextPosition }
            : guard with { Direction = RotateRight(guard.Direction) };
    }

    private static IEnumerable<Guard> GetGuardPath(Guard guard, Map map)
    {
        while (map.IsInBounds(guard.Position))
        {
            yield return guard;
            guard = UpdateGuard(guard, map);
        }
    }

    private static bool HasLoop(Guard guard, Map map)
    {
        var seen = new HashSet<Guard>();
        while (map.IsInBounds(guard.Position))
        {
            if (!seen.Add(guard))
            {
                return true;
            }
            guard = UpdateGuard(guard, map);
        }
        return false;
    }

    private class Map(IReadOnlyList<List<Tile>> tiles)
    {
        public bool IsInBounds(Position position) =>
            position.Row >= 0 && position.Row < tiles.Count && 
            position.Col >= 0 && position.Col < tiles[position.Row].Count;

        public Tile? GetTileOrDefault(Position position) =>
            IsInBounds(position) 
                ? tiles[position.Row][position.Col] 
                : null;

        public IEnumerable<Map> GetAllPossibleMaps(Guard guard) =>
            from pos in GetAllFreePositions(guard)
            let newTiles = DeepCopyTiles()
            let _ = newTiles[pos.Row][pos.Col] = Tile.Obstacle
            select new Map(newTiles);

        private IEnumerable<Position> GetAllFreePositions(Guard guard) =>
            from row in Enumerable.Range(0, tiles.Count)
            from col in Enumerable.Range(0, tiles[row].Count)
            let pos = new Position(row, col)
            where pos != guard.Position && tiles[pos.Row][pos.Col] == Tile.Free
            select pos;

        private List<List<Tile>> DeepCopyTiles() =>
            tiles.Select(row => row.ToList()).ToList();
    }

    private static (Map Map, Guard Guard) ParseInput(string input)
    {
        var rows = input.Split(Environment.NewLine);
        return (ParseMap(rows), ParseGuard(rows));
    }

    private static Map ParseMap(IEnumerable<string> rows) =>
        new(rows.Select(ParseRow).ToList());

    private static List<Tile> ParseRow(string row) =>
        row.Select(ParseTile).ToList();
    
    private static Guard ParseGuard(IEnumerable<string> rows) =>
        (from indexedRow in rows.Select((rowContent, rowIndex) => (rowContent, rowIndex))
         from indexedChar in indexedRow.rowContent.Select((symbol, colIndex) => (symbol, colIndex))
         where IsGuard(indexedChar.symbol)
         select new Guard(
             new Position(indexedRow.rowIndex, indexedChar.colIndex),
             GetDirection(indexedChar.symbol)))
        .Single();

    private static Tile ParseTile(char t) => t switch
    {
        '.' or '^' or '>' or 'v' or 'V' or '<' => Tile.Free,
        '#' => Tile.Obstacle,
        _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
    };

    private static bool IsGuard(char t) => 
        t is '^' or '>' or 'v' or 'V' or '<';

    private static Direction GetDirection(char t) => t switch
    {
        '^' => Direction.Up,
        '>' => Direction.Right,
        'v' or 'V' => Direction.Down,
        '<' => Direction.Left,
        _ => throw new ArgumentOutOfRangeException(nameof(t), t, null)
    };
}
