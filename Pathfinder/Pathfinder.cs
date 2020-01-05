using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pathfinder
{
    public static class Pathfinder
    {

        static Random rng = new Random();
        public static bool[,] pathMap;

        /// <summary>
        /// Randomly generate a map to navigate. Must be called before finding a path.
        /// </summary>
        public static void BuildMap()
        {
            pathMap = new bool[100, 100];
            for(int i = 0; i < 100; i++)
            {
                for(int j = 0; j < 100; j++)
                {
                    int mapVal = rng.Next(5);

                    if (mapVal == 0)
                        pathMap[i, j] = false;
                    else
                        pathMap[i, j] = true;
                }
            }
        }

        /// <summary>
        /// Find a path between the origin and a randomly-generated goal.
        /// </summary>
        /// <returns>The endpoint of the path.</returns>
        public static PathCell FindPath()
        {
            //define start & goal
            PathCell start = new PathCell(0, 0);

            //generate a goal
            int goalX = rng.Next(0, 99);
            int goalY = rng.Next(0, 99);
            pathMap[goalX, goalY] = true; //make sure its attainable
            PathCell goal = new PathCell(goalX, goalY);

            //prep algorithm
            PathCell current = null;

            List<PathCell> open = new List<PathCell>();
            List<PathCell> closed = new List<PathCell>();

            int g = 0;

            open.Add(start);

            while (open.Count > 0)
            {
                //get square with lowest f
                var lowestF = open.Min(c => c.f);
                current = open.First(c => c.f == lowestF);

                //move current to closed
                closed.Add(current);
                open.Remove(current);

                //if goal is in closed list, we're done
                if (closed.FirstOrDefault(c => c.x == goal.x && c.y == goal.y) != null)
                    break;

                //find adjacent tiles
                List<PathCell> adjacent = GetAdjacentCells(current, open);
                g = current.g + 1;

                //loop through adjacent
                foreach (PathCell a in adjacent)
                {
                    //make sure it isn't already closed
                    if (closed.FirstOrDefault(c => c.x == a.x && c.y == a.y) != null)
                        continue;

                    //create and add to open if not open either
                    if (open.FirstOrDefault(c => c.x == a.x && c.y == a.y) == null)
                    {
                        //set metrics
                        a.g = g;
                        a.h = CalculateH(a, goal);
                        a.f = a.g = a.h;
                        a.parent = current;

                        //add to top of open list
                        open.Insert(0, a);
                    }
                    else //is already open
                    {
                        //update g if this path is more efficient
                        if (g + a.h < a.f)
                        {
                            a.g = g;
                            a.f = a.g + a.h;
                            a.parent = current;
                        }
                    }
                }
            }

            return current;

        }

        /// <summary>
        /// Get all walkable cells adjacent to a specified cell.
        /// </summary>
        /// <param name="focus">The cell to search around.</param>
        /// <param name="open">The list of currently-open cells.</param>
        /// <returns>A list of valid adjacent cells.</returns>
        static List<PathCell> GetAdjacentCells(PathCell focus, List<PathCell> open)
        {
            List<PathCell> adjacent = new List<PathCell>();

            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    //make sure coord is safe
                    if (focus.x + i > 0 &&
                        focus.x + i < pathMap.GetLength(0) &&
                        focus.y + j > 0 &&
                        focus.y + j < pathMap.GetLength(1))
                    {
                        //make sure its walkable
                        if (pathMap[focus.x + i, focus.y + j])
                        {
                            //find node, create it if necessary, and add to returned list
                            PathCell node = open.Find(c => c.x == focus.x + i && c.y == focus.y + j);
                            if (node == null)
                                adjacent.Add(new PathCell(focus.x + i, focus.y + j));
                            else
                                adjacent.Add(node);
                        }
                    }
                }
            }

            return adjacent;
        }

        /// <summary>
        /// Calcualte the h score of a cell.
        /// </summary>
        /// <param name="cell">The cell to calculate for.</param>
        /// <param name="goal">The goal cell.</param>
        /// <returns>The h value.</returns>
        static int CalculateH(PathCell cell, PathCell goal)
        {
            return Math.Abs(goal.x - cell.x) + Math.Abs(goal.y - cell.y);
        }

    }
}
