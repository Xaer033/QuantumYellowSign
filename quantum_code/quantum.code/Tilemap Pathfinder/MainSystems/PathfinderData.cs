using System.Collections.Generic;
using System.ComponentModel;

namespace Quantum.Tiles
{
    public class PathfinderData
    {
        public Dictionary<int, PathData> TempPath = new Dictionary<int, PathData>();
        public PriorityQueueBinaryHeap Frontier = new PriorityQueueBinaryHeap();
    }

    public struct PathData {
	    public int CameFrom;
	    public int Cost;
    }
}
