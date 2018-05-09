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
    // Task A
    // To begin, create a new source code file, containing a new class called Dijkstra’s algorithm needs to maintain the following data:
    // 1. Which nodes(grid locations) are closed
    // 2. A cost value for each node
    // 3. A link for each node(which shows the best way to get there)
    // 4. Which nodes form the final path
    // The easiest way to store this data is to setup a corresponding set of 2D arrays (size 40x40). Each of the can be used to store one of the above data items for
    // every location in the grid. A add the following members to the Dijkstra class:

    class Dijkstra
    {
        public bool[,] closed;  //whether or not location is closed
        public float[,] cost;   //cost value for each location
        public Coord2[,] link;  //link for each location = coords of a neighbouring location
        public bool[,] inPath;  //whether or not a location is in the final path

        public Dijkstra()
        {
            closed = new bool[40, 40];
            cost = new float[40, 40];
            link = new Coord2[40, 40];
            inPath = new bool[40, 40];

            //closed[10, 10] = true; //grid position is closed
            //cost[27,5] = 0; //sets the cost location to 0
            //inPath[12, 3] = true; //sets the rid position as in the path
        }


        // Task B
        // Create a new class function called build(). This function will calculate the path,
        // and fill out the data arrays we defined above.The build function will need to take some parameters:
        // 1. A reference to a bot, to get the starting location
        // 2. A reference to the player, to get the end location
        // 3. A reference to a level, so we can determine which grid locations are blocked by walls
        public void Build(Level level, AiBotBase bot, Player plr)
        {
            // 1. All nodes should be marked as open
            // 2. Each node’s cost should be initialised to a very big number (1, 000, 000 for example)
            // 3. Links are not important at this stage, so just set them all to { -1,-1}
            // 4. Every value of “inPath” should be initialised as false.
            for (int i = 0; i < 40; i++)
            {
                for (int j = 0; j < 40; j++)
                {
                    closed[i, j] = false;
                    cost[i, j] = 1000000;
                    link[i, j] = new Coord2(-1, -1);
                    inPath[i, j] = false;
                }
            }


            // To start the search, the bot’s position should be marked as open, eg:
            closed[bot.GridPosition.X, bot.GridPosition.Y] = false;

            // Similarly, the cost at the bot’s position should be set to zero
            cost[bot.GridPosition.X, bot.GridPosition.Y] = 0;

            //Temp coord position
            Coord2 pos;
            pos.X = 0;
            pos.Y = 0;



            // While the player grid position is not closed  --- or maybe (pos != plr.GridPosition)
            while (!closed[plr.GridPosition.X, plr.GridPosition.Y])
            {
                //Temp minium value for comparison
                float min = 1000000;

                // 1. Find the grid location with the lowest cost, that is not closed, and is not blocked on the map layout(on the first iteration of the loop, this should actually be the bot’s position).
                // If there are locations with equal lowest cost, it doesn’t matter which you choose.
                for (int i = 0; i < 40; i++)
                {
                    for (int j = 0; j < 40; j++)
                    {
                        // If the cost of the searching grid position is less than the min position and is open and valid
                        // update the pos and min cost
                        if (cost[i, j] < min && closed[i, j] == false && level.ValidPosition(new Coord2(i, j)))
                        {
                            min = cost[i, j];
                            pos.X = i;
                            pos.Y = j;
                        }
                    }
                }
                // 2. Mark this location as closed
                closed[pos.X, pos.Y] = true;



                // 3. Calculate a new cost for the 8 locations neighbouring this one: 
                // this will be equal to the cost of the location that was just closed(the parent) + the cost of moving from there to the 
                // neighbour(so, + 1 for left, right, up, down, + 1.4 for diagonals) : BUT only change the cost of the neighbouring square in the cost array IF:
                float pairCost = 0;

                //Loop through neighbouring grid coords
                for (int i = pos.X - 1; i < pos.X + 2; i++)
                {
                    for (int j = pos.Y - 1; j < pos.Y + 2; j++)
                    {
                        // a. The neighbouring location is not blocked on the map layout.
                        if (level.ValidPosition(new Coord2(i, j)) && !closed[i, j])
                        {
                            // Set the cost value for moving to the new location
                            if (i != pos.X && j != pos.Y) pairCost = 1.4f;
                            else if ((i != pos.X && j == pos.Y) || (i == pos.X && j != pos.Y)) pairCost = 1.0f;
                            else pairCost = 0.0f;

                            // b. The new cost value found for the neighbour is lower than its current cost value.
                            if (cost[pos.X, pos.Y] + pairCost < cost[i, j])
                            {
                                // Set the cost value of the neigbour
                                cost[i, j] = cost[pos.X, pos.Y] + pairCost;

                                // 4. If you have set a new cost value for a neighbour, then also set its corresponding link value to be equal to the coordinates of its parent
                                // (the location that was closed) 
                                link[i, j] = pos;
                            }
                        }
                    }
                }

                // 5. This should be repeated until the location occupied by the player is closed, at which point the while loop should terminate.
            }

            // Task C
            // When the loop has terminated (the player’s location has been closed), the array of links should represent a traceable path from the player back to the bot.
            // This path is defined by the values in the link array.The next task is to extract these locations.
            //
            // Starting from the player coordinates, trace back through the links array and mark the corresponding entries in the “inPath” array to true.
            // The code for this should look something like:

            //Set to true when we are back at the bot position 
            bool done = false;

            //Start of path                        
            Coord2 nextClosed = plr.GridPosition;       

            //While not at the bots position
            while (!done)
            {
                //Set in path as true
                inPath[nextClosed.X, nextClosed.Y] = true;

                //Set the next closed coord
                nextClosed = link[nextClosed.X, nextClosed.Y];

                //If the nextclosed is the bots position finish the loop
                if (nextClosed == bot.GridPosition) done = true;
            }
        }
    }
}