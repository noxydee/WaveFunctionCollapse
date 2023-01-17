namespace WaveFunctionCollapse.WaveFunction
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    [Serializable]
    public class Tile
    {
        private Bitmap TileBitmap { get; set; }

        private readonly List<Color> _topColors;
        private readonly List<Color> _bottomColors;
        private readonly List<Color> _rightColors;
        private readonly List<Color> _leftColors;

        public Tile(Bitmap tile)
        {
            TileBitmap = tile;
            _topColors = new();
            _bottomColors = new();
            _rightColors = new();
            _leftColors = new();

            for (int i = 0; i < tile.Width; i++)
            {
                _topColors.Add(tile.GetPixel(i, 0));
                _bottomColors.Add(tile.GetPixel(i, tile.Height - 1));
                _leftColors.Add(tile.GetPixel(0, i));
                _rightColors.Add(tile.GetPixel(tile.Width - 1, i));
            }
        }

        public Bitmap GetOrientedBitmap()
        {
            //TODO rotate if needed
            return TileBitmap;
        }

        public bool CanConnect(Tile tile, Direction direction, int maximumError=10)
        {
                List<Color> colors = null;
                List<Color> colorsToCompare = null;

                switch (direction)
                {
                    case Direction.UP:
                        colors = tile._bottomColors;
                        colorsToCompare = _topColors;
                        break;
                    case Direction.DOWN:
                        colors = tile._topColors;
                        colorsToCompare = _bottomColors;
                        break;
                    case Direction.RIGHT:
                        colors = tile._leftColors;
                        colorsToCompare = _rightColors;
                        break;
                    case Direction.LEFT:
                        colors = tile._rightColors;
                        colorsToCompare = _leftColors;
                        break;
                }

                for (int i = 0; i < colors.Count; i++)
                {
                    if (!IsColorSimilar(colors[i], colorsToCompare[i], maximumError))
                    {
                        return false;
                    }
                }

            return true;
        }

        public void RotateRight()
        {
            TileBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

            _topColors.RemoveAll(x => true);
            _bottomColors.RemoveAll(x => true);
            _rightColors.RemoveAll(x => true);
            _leftColors.RemoveAll(x => true);

            for (int i = 0; i < TileBitmap.Width; i++)
            {
                _topColors.Add(TileBitmap.GetPixel(i, 0));
                _bottomColors.Add(TileBitmap.GetPixel(i, TileBitmap.Height - 1));
                _leftColors.Add(TileBitmap.GetPixel(0, i));
                _rightColors.Add(TileBitmap.GetPixel(TileBitmap.Width - 1, i));
            }
        }

        public bool IsColorSimilar(Color first, Color second, int maximumError)
        {
            int redDifference = Math.Abs(first.R - second.R);
            int greenDifference = Math.Abs(first.G - second.G);
            int blueDifference = Math.Abs(first.B - second.B);

            return redDifference < maximumError && greenDifference < maximumError && blueDifference < maximumError;
        }
    }
}
