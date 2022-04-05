using LiftAlgorithm;


int capacity = 5; //Insert lift capacity
int[][] queues = 
    {
        new int[]{4,5},     // G
        new int[0],         // 1
        new int[]{5,5,5},   // 2
        new int[]{2,6,},    // 3
        new int[0],         // 4
        new int[]{4,0},     // 5
        new int[]{0,0},     // 6
    }; //Insert waiting people's destination floor on each floor

var liftTrace = LiftWAlgorithmClass.TheLift(queues, capacity);

Console.WriteLine("The lift track");
foreach (int n in liftTrace)
    Console.Write(" "+n);

