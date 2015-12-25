using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GirlTalkInABox
{
    class Visualizer: PictureBox
    {
        private EchoNestAnalysisResponse _info;

        private int tileSize = 10;
        private int tilePadding = 2;
        private int barPadding = 10;

        public void setInfo(EchoNestAnalysisResponse info)
        {
            _info = info;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            if (_info == null)
            {
                return;
            }

            var barCount = _info.bars.Count();
            var g = e.Graphics;

            var container = g.BeginContainer();
            g.SetClip(e.ClipRectangle);

            var tileX = 0;
            var tileY = 0;
            var tileBrush = new SolidBrush(Color.FromArgb(119, 255, 170));
            
            for (var barIndex = 0; barIndex < barCount; barIndex++)
            {
                var bar = _info.bars.ElementAt(barIndex);

                var biggestTileX = tileX + ((tileSize + tilePadding) * bar.beats.Count());

                if (biggestTileX > g.ClipBounds.Width)
                {
                    tileX = 0;
                    tileY = tileY + tileSize + barPadding;
                }

                for (var tile = 0; tile < bar.beats.Count(); tile++)
                {
                    var beat = bar.beats.ElementAt(tile);
                    var tileRect = new Rectangle(tileX, tileY, tileSize, tileSize);
                    g.FillRectangle(tileBrush, tileRect);

                    tileX = tileX + tileSize + tilePadding;
                }

                tileX += barPadding;
            }

            g.EndContainer(container);
        }
    }
}
