namespace WaveFunctionCollapse.WaveFunction
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading.Tasks;

    public class TileGridCell
    {
        public List<Tile> PossibleTiles { get; set; }
        public bool Collapsed { get; set; }

        public Tile PickedTile { get; set; }

        private readonly Random random = new Random();

        public TileGridCell(List<Tile> tiles)
        {
            PossibleTiles = tiles;
            Collapsed = false;
        }

        public Tile GetRandomTile()
        {
            if (PossibleTiles.Count == 0)
            {
                Console.WriteLine("Found block with no possible tiles left!");
                return null;
            }

            return PossibleTiles.ElementAt(random.Next(0, PossibleTiles.Count - 1));
        }

        public void RemoveImpossibleTiles(Tile previousTile, Direction direction)
        {
            List<Tile> newPossibleTiles = new();

            foreach(Tile tile in PossibleTiles.Where(x => x.CanConnect(previousTile, direction)).ToList()) 
            {
                newPossibleTiles.Add(tile);
            }

            PossibleTiles = newPossibleTiles;
        }

        public void RemoveImpossibleTiles(Tile left, Tile top, Tile right, Tile bottom)
        {
            if (left != null)
            {

                PossibleTiles = PossibleTiles.Where(x => x.CanConnect(left, Direction.LEFT)).ToList();
            }

            if (right != null)
            {
                PossibleTiles = PossibleTiles.Where(x => x.CanConnect(right, Direction.RIGHT)).ToList();
            }

            if (top != null)
            {
                PossibleTiles = PossibleTiles.Where(x => x.CanConnect(top, Direction.UP)).ToList();
            }

            if (bottom != null)
            {
                PossibleTiles = PossibleTiles.Where(x => x.CanConnect(bottom, Direction.DOWN)).ToList();
            }
        }
    }
}
