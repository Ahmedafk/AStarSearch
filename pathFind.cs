using System;
using System.Collections;
using System.Collections.Generic;

public abstract class pathFind
{
    private static int straightCost = 2;
    private static int diagonalCost = 3;

    public static Stack<Tile> A(Tile start, Tile goal, Tile[,] tiles)//Add Tile Array from World
    {
        int count = 0;
        // The set of nodes already evaluated.
        List<Tile> closedSet = new List<Tile>();

        // The set of currently discovered nodes still to be evaluated.
        // Initially, only the start node is known
        List<Tile> openSet = new List<Tile>();
        openSet.Add(start);

        // For each node, which node it can most efficiently be reached from.
        // If a node can be reached from many nodes, cameFrom will eventually contain the
        // most efficient previous step.
        var cameFrom = new Dictionary<Tile, Tile>();
        cameFrom[start] = null;

        // For each node, the cost of getting from the start node to that node.
        Dictionary<Tile, int> gScore = new Dictionary<Tile, int>();

        // The cost of going from start to start is zero.
        gScore[start] = 0;

        // For each node, the total cost of getting from the start node to the goal
        // by passing by that node. That value is partly known, partly heuristic.
        Dictionary<Tile, int> fScore = new Dictionary<Tile, int>();

        // For the first node, that value is completely heuristic.
        fScore[start] = heuristic(start, goal, straightCost);

        //node in openSet with lowest fScore[] value
        //currently start node
        Tile current = start;

        Console.WriteLine("Starting Loop...");
        while (openSet.Count != 0)
        {
            Console.WriteLine("Loop (" + count + ")..."); count++;
            //find closest node to goal in openSet
            Console.WriteLine("Finding Closest Tile to goal");
            foreach (Tile node in openSet)
            {
                if (current == null)
                {
                    current = node;
                }
                    
                else if (fScore[node] < fScore[current])
                {
                    current = node;
                }
            }

            //End function here if arrived at destination
            if (current.Equals(goal))
            {
                Console.WriteLine("PathFound, reconstructing...");
                return reconstruct_path(cameFrom, current);
            }

            openSet.Remove(current);
            closedSet.Add(current);

            Tile neighbour;

            //Evaluate all neighbours to find closest to goal
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {                    
                    try
                    {
                        int neighbourX = current.X + x;
                        int neighbourY = current.Y + y;

                        neighbour = tiles[neighbourX, neighbourY];

                        if (closedSet.Contains(neighbour))
                        {
                            //Skip already evaluated neighbour tiles
                            Console.WriteLine("Closed Tile Skipped");
                        }
                        else if (neighbour.Type == Tile.TileTypes.Water)
                        {
                            //Skip water tiles
                            Console.WriteLine("Water Tile Skipped");
                        }
                        else
                        {
                            int tempScore = gScore[current] + cal_Cost(current, neighbour);

                            if (!openSet.Contains(neighbour))//new tile discovered
                            {
                                cameFrom[neighbour] = current;
                                gScore[neighbour] = tempScore;
                                fScore[neighbour] = gScore[neighbour] + heuristic(neighbour, goal, straightCost);

                                openSet.Add(neighbour);
                                Console.WriteLine("New Tile Found");

                            }
                            else if (tempScore < gScore[neighbour])//faster path discovered
                            {
                                cameFrom[neighbour] = current;
                                gScore[neighbour] = tempScore;
                                fScore[neighbour] = gScore[neighbour] + heuristic(neighbour, goal, straightCost);

                                Console.WriteLine("Shorter Path Found to Tile");
                            }
                            else
                            {
                                //Ignore longer route
                                Console.WriteLine("Longer route ignored");
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error in evaluating tile's neighbours!!");
                        Console.WriteLine(e.ToString());
                    }

                }
                Console.WriteLine();
            }
            //Overwrite current Tile to avoid confusion in future iterations
            current = null;
        }

        return null;
    }

    static Stack<Tile> reconstruct_path(Dictionary<Tile, Tile> cameFrom, Tile current)
    {
        // Retrace the steps taken to reach the goal
        Stack<Tile> totalPath = new Stack<Tile>();
        while (current != null)
        {
            totalPath.Push(current);
            current = cameFrom[current];
        }
        return totalPath;
    }

    //Calculate the cost of landing on tile
    //Water = 0(forbidden), straight = 2, diagonal = 3
    static int cal_Cost(Tile current, Tile neighbour)
    {
        if (current.X == neighbour.X && current.Y == neighbour.Y)
        {
            return 0;
        }
        else if (current.X == neighbour.X || current.Y == neighbour.Y)
        {
            return straightCost;
        }
        else
        {
            return diagonalCost;
        }
    }

    //Diagonal Manhattan Distance Approach
    static int heuristic(Tile current, Tile goal, int lowestCost)
    {
        World world = default(World);

        Tile distance = new Tile(world, 0, 0);
        distance.X = Math.Abs(current.X - goal.X);
        distance.Y = Math.Abs(current.Y - goal.Y);
        return lowestCost * Convert.ToInt16(Math.Sqrt(distance.X * distance.X + distance.Y * distance.Y)); // pythagoras theory * cost of shortest possible step 
    }
}
