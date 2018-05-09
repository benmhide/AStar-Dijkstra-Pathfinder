using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using System.IO;
namespace Pathfinder
{
    class AiBotSimple2 : AiBotBase
    {
        public AiBotSimple2(int x, int y) : base(x, y)
        {
        }

        protected override void ChooseNextGridLocation(Level level, Player plr)
        {
            Coord2 desiredPos;
            Coord2 currentPos;

            //bool ok = false;
            currentPos = GridPosition;
            desiredPos = GridPosition;

            do
            {
                if (plr.GridPosition.X > GridPosition.X)
                {
                    desiredPos.X += 1;
                }

                else if (plr.GridPosition.X < GridPosition.X)
                {
                    desiredPos.X -= 1;
                }

                else if (plr.GridPosition.Y > GridPosition.Y)
                {
                    desiredPos.Y += 1;
                }

                else if (plr.GridPosition.Y < GridPosition.Y)
                {
                    desiredPos.Y -= 1;
                }

            } while (level.ValidPosition(desiredPos));
        }
    }
}