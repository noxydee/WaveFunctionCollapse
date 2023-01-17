namespace WaveFunctionCollapse.WaveFunction
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Runtime.Serialization;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class Collapse : ICollapse
    {
        private Random random = new Random();

        private int outputSizeX2, outputSizeY2;
        private int tileSize2;

        TileGridCell[,] grid { get; set; }

        public Collapse()
        {

        }

        public Bitmap CollapseBitmap(Bitmap source, int outputSizeX, int outputSizeY, int tileSize)
        {
            outputSizeX2 = outputSizeX;
            outputSizeY2 = outputSizeY;
            tileSize2 = tileSize;

            Bitmap outputBitmap = new Bitmap(outputSizeX, outputSizeY);
            List<Tile> tiles = GetTiles(source, tileSize);
            int tileGridX = outputSizeX / tileSize;
            int tileGridY = outputSizeY / tileSize;
            TileGridCell[,] tileCell = FillGridWithTiles(tileGridX, tileGridY, tiles);

            int startingX = random.Next(0, tileGridX - 1);
            int startingY = random.Next(0, tileGridY - 1);
            tileCell = CollapseGrid(tileCell, startingX, startingY, tileGridX, tileGridY);

            using (Graphics graphics = Graphics.FromImage(outputBitmap))
            {
                for (int x = 0; x < tileGridX; x++) 
                {
                    for (int y = 0; y < tileGridY; y++)
                    {
                        if (tileCell[x, y].PickedTile != null)
                        {
                            Rectangle rectangle = new Rectangle(0, 0, tileSize, tileSize);
                            graphics.DrawImage(tileCell[x, y].PickedTile.GetOrientedBitmap(), x*tileSize, y*tileSize, rectangle, GraphicsUnit.Pixel);
                        }
                    }
                }
            }

            outputBitmap.Save("waveFunctionCollapseOutput.bmp", ImageFormat.Bmp);

            Console.WriteLine("Done");

            return outputBitmap;
        }

        public void Draw(TileGridCell[,] tileCell, string fileName)
        {
            Bitmap outputBitmap = new Bitmap(outputSizeX2, outputSizeY2);
            int tileGridX = outputSizeX2 / tileSize2;
            int tileGridY = outputSizeY2 / tileSize2;

            using (Graphics graphics = Graphics.FromImage(outputBitmap))
            {
                for (int x = 0; x < tileGridX; x++)
                {
                    for (int y = 0; y < tileGridY; y++)
                    {
                        if (tileCell[x, y].PickedTile != null)
                        {
                            Rectangle rectangle = new Rectangle(0, 0, tileSize2, tileSize2);
                            graphics.DrawImage(tileCell[x, y].PickedTile.GetOrientedBitmap(), x * tileSize2, y * tileSize2, rectangle, GraphicsUnit.Pixel);
                        }
                    }
                }
            }

            outputBitmap.Save(fileName, ImageFormat.Jpeg);
        }

        public TileGridCell[,] CollapseGrid(TileGridCell[,] grid, int startingX, int startingY, int xSize, int ySize)
        {
            TileGridCell currentCell = grid[startingX, startingY];
            currentCell.PickedTile = currentCell.GetRandomTile();
            currentCell.Collapsed = true;
            //Draw(grid, $"output/{totalCounter}.jpeg");

            if (currentCell.PickedTile != null)
            {
                if (startingY > 0 && grid[startingX, startingY - 1].Collapsed == false)
                {
                    Tile left = GetNeighbourTile(grid, startingX, startingY - 1, Direction.LEFT);
                    Tile right = GetNeighbourTile(grid, startingX, startingY - 1, Direction.RIGHT);
                    Tile top = GetNeighbourTile(grid, startingX, startingY - 1, Direction.UP);
                    Tile bottom = GetNeighbourTile(grid, startingX, startingY - 1, Direction.DOWN);

                    grid[startingX, startingY - 1].RemoveImpossibleTiles(left, top, right, bottom);
                    grid = CollapseGrid(grid, startingX, startingY - 1, xSize, ySize);
                }
                if (startingY < ySize - 1 && grid[startingX, startingY + 1].Collapsed == false)
                {
                    Tile left = GetNeighbourTile(grid, startingX, startingY + 1, Direction.LEFT);
                    Tile right = GetNeighbourTile(grid, startingX, startingY + 1, Direction.RIGHT);
                    Tile top = GetNeighbourTile(grid, startingX, startingY + 1, Direction.UP);
                    Tile bottom = GetNeighbourTile(grid, startingX, startingY + 1, Direction.DOWN);

                    grid[startingX, startingY + 1].RemoveImpossibleTiles(left, top, right, bottom);
                    grid = CollapseGrid(grid, startingX, startingY + 1, xSize, ySize);
                }
                if (startingX > 0 && grid[startingX - 1, startingY].Collapsed == false)
                {
                    Tile left = GetNeighbourTile(grid, startingX - 1, startingY, Direction.LEFT);
                    Tile right = GetNeighbourTile(grid, startingX - 1, startingY, Direction.RIGHT);
                    Tile top = GetNeighbourTile(grid, startingX - 1, startingY, Direction.UP);
                    Tile bottom = GetNeighbourTile(grid, startingX - 1, startingY, Direction.DOWN);

                    grid[startingX - 1, startingY].RemoveImpossibleTiles(left, top, right, bottom);
                    grid = CollapseGrid(grid, startingX - 1, startingY, xSize, ySize);
                }
                if (startingX < xSize - 1 && grid[startingX + 1, startingY].Collapsed == false)
                {
                    Tile left = GetNeighbourTile(grid, startingX + 1, startingY, Direction.LEFT);
                    Tile right = GetNeighbourTile(grid, startingX + 1, startingY, Direction.RIGHT);
                    Tile top = GetNeighbourTile(grid, startingX + 1, startingY, Direction.UP);
                    Tile bottom = GetNeighbourTile(grid, startingX + 1, startingY, Direction.DOWN);

                    grid[startingX + 1, startingY].RemoveImpossibleTiles(left, top, right, bottom);
                    grid = CollapseGrid(grid, startingX + 1, startingY, xSize, ySize);
                }
            }
            
            return grid;
        }

        private Tile GetNeighbourTile(TileGridCell[,] grid, int x, int y, Direction direction)
        {
            try
            {
                switch (direction)
                {
                    case Direction.UP:
                        return grid[x, y - 1].PickedTile;
                    case Direction.DOWN:
                        return grid[x, y + 1].PickedTile;
                    case Direction.RIGHT:
                        return grid[x + 1, y].PickedTile;
                    case Direction.LEFT:
                        return grid[x - 1, y].PickedTile;
                    default:
                        throw new Exception("Direction not implementd");
                }
            }
            catch (IndexOutOfRangeException ex)
            {
                return null;
            }
        }

        private TileGridCell[,] FillGridWithTiles(int xSize, int ySize, List<Tile> tiles)
        {
            TileGridCell[,] grid = new TileGridCell[xSize, ySize];

            for (int x = 0; x < xSize; x++)
            {
                for (int y = 0; y < ySize; y++)
                {
                    grid[x, y] = new TileGridCell(tiles);
                }
            }

            return grid;
        }

        private List<Tile> GetTiles(Bitmap source, int tileSize)
        {
            List<Tile> tiles = new List<Tile>();

            for (int x = 0; x < source.Width; x += tileSize)
            {
                for (int y = 0; y < source.Height; y += tileSize)
                {
                    Bitmap tileBitmap = new Bitmap(tileSize, tileSize);
                    Rectangle rectangle = new Rectangle(x, y, tileSize, tileSize);

                    using (Graphics graphics = Graphics.FromImage(tileBitmap))
                    {
                        graphics.DrawImage(source, 0, 0, rectangle, GraphicsUnit.Pixel);
                    }

                    tiles.Add(new Tile(tileBitmap));

                    for (int i = 0; i < 3; i++)
                    {
                        Tile newTile = tiles.Last();
                        Tile rotatedTile = CreateDeepCopy(newTile);
                        rotatedTile.RotateRight();
                        tiles.Add(rotatedTile);
                    }
                }
            }

            return tiles;
        }

        public static T CreateDeepCopy<T>(T obj)
        {
            using (var ms = new MemoryStream())
            {
                IFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Seek(0, SeekOrigin.Begin);
                return (T)formatter.Deserialize(ms);
            }
        }
    }
}
