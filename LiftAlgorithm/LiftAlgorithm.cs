using System;
using System.Collections.Generic;
using System.Linq;

namespace LiftAlgorithm;
public class LiftWAlgorithmClass
{
    private static List<int>[] makeTableOfLists(int[][] queues)
    {
        var ret = new List<int>[queues.GetLength(0)];

        Enumerable.Range(0, queues.GetLength(0)).ToList().
          ForEach(x => ret[x] = queues[x].ToList());

        return ret;
    }

    private static void printLift(List<int> peopleAbordQueuesList)
    {
        System.Console.Write("<==LIFT==( ");
        foreach (var i in peopleAbordQueuesList)
        {
            System.Console.Write(i + "  ");
        }
        System.Console.Write($")");
    }

    private static void printFloors(List<int>[] makeTableOfLists, List<int> peopleAbordQueuesList, int actualFloor, int liftDirection)
    {
        System.Console.WriteLine();
        for (int i = makeTableOfLists.GetLength(0) - 1; i >= 0; i--)
        {
            System.Console.WriteLine("--||=====================||  ");
            System.Console.Write(i + " ||");

            System.Console.Write("  ");
            foreach (var j in makeTableOfLists[i])
            {
                System.Console.Write(j + " ");
            }
            System.Console.SetCursorPosition(25, Console.CursorTop);
            System.Console.Write("||   ");
            if (actualFloor == i)
            {
                printLift(peopleAbordQueuesList);
                var res = liftDirection == 1 ? " up" : " down";
                System.Console.Write(res);
            }
            System.Console.WriteLine();
        }
    }

    private static void checkPreesedKeys(int[] floorsKeysPressedList, List<int>[] floorsQueuesList)
    {
        Enumerable.Range(0, floorsQueuesList.GetLength(0)).ToList().
          ForEach(x => Enumerable.Range(0, floorsQueuesList[x].Count()).ToList().
           ForEach(y =>
            {
                if ((floorsKeysPressedList[x] == 2 && floorsQueuesList[x][y] > x)
                 || (floorsKeysPressedList[x] == 1 && floorsQueuesList[x][y] < x))
                    floorsKeysPressedList[x] = 3;
                else if (floorsQueuesList[x][y] > x && floorsKeysPressedList[x] != 3) floorsKeysPressedList[x] = 1;
                else if (floorsKeysPressedList[x] != 3) floorsKeysPressedList[x] = 2;
            }));
    }

    private static void passangersLeave(ref int peopleAbord, ref List<int> peopleAbordQueuesList, int actualFloor)
    {
        peopleAbord -= peopleAbordQueuesList.Count(x => x == actualFloor); //Calculate nomber of people abord 
        peopleAbordQueuesList = peopleAbordQueuesList.Where(x => x != actualFloor).ToList(); //Update people's abord direction floors
    }

    private static void takePassangers(ref int peopleAbord, List<int> floorsQueuesList, int actualFloor, int liftDirection, ref int floorsKeysPressedList, int capacity,
                                     List<int> peopleAbordQueuesList)
    {
        List<int> passangersWaiting = new List<int>();

        if (liftDirection == 1) //when list arrive, take list of passangers that waiting in lift's direction
            passangersWaiting = floorsQueuesList.Where(x => x > actualFloor).ToList(); //making list of passanges that want to enter lift
        else
            passangersWaiting = floorsQueuesList.Where(x => x < actualFloor).ToList(); //making list of passanges that want to enter lift

        for (int x = 0; x < passangersWaiting.Count(); x++) //People entering the lift - process
        {
            if (peopleAbord < capacity)
            {
                peopleAbord++; //One more passanger inside the lift
                peopleAbordQueuesList.Add(passangersWaiting[x]); //Add new lift stop for person that just enter it
                var removeVal = floorsQueuesList.First(y => y == passangersWaiting[x]);
                floorsQueuesList.Remove(removeVal); //Remove person from waiting to enter lift queue       
            }
        }
        if (floorsKeysPressedList == 3 && floorsQueuesList.All(x => x < actualFloor)) floorsKeysPressedList = 2; //Floor lift key ststus change
        else if (floorsKeysPressedList == 3 && floorsQueuesList.All(x => x > actualFloor)) floorsKeysPressedList = 1;
        else if (floorsQueuesList.Count() == 0) floorsKeysPressedList = 0;
    }

    private static bool liftGo(ref int actualFloor, ref int liftDirection, ref int peopleAbord, int[] floorsKeysPressedList, List<int>[] floorsQueuesList,
                              ref List<int> peopleAbordQueuesList, List<int> liftTrack, int capacity)
    {
        bool isStop = false;
        var copy1 = actualFloor;
        var copy2 = liftDirection;

        //If lift is empty, and nobody is waiting on future floor, and nobady that enter want to go in current direction
        if ((peopleAbord == 0 || peopleAbordQueuesList.All(x => x == copy1))
          && floorsKeysPressedList.Where((key, index) => index > copy1 && key > 0).Count() == 0
          && floorsKeysPressedList[actualFloor] != liftDirection && floorsKeysPressedList[actualFloor] != 3
          && liftDirection == 1)
            liftDirection = 2;
        else if (peopleAbord == 0
           && floorsKeysPressedList.Where((key, index) => index < copy1 && key > 0).Count() == 0
           && floorsKeysPressedList.Sum() != 0
           && floorsKeysPressedList[actualFloor] != liftDirection && floorsKeysPressedList[actualFloor] != 3
           && liftDirection == 2)
            liftDirection = 1;

        if (actualFloor == floorsKeysPressedList.GetLength(0) - 1) liftDirection = 2; //If lift is on top or bottom change direction
        if (actualFloor == 0) liftDirection = 1;

        if (peopleAbordQueuesList.Any(x => x == copy1))  ////People are leaving the lift
        {
            passangersLeave(ref peopleAbord, ref peopleAbordQueuesList, actualFloor);
            isStop = true;
        }

        if (floorsKeysPressedList[actualFloor] == liftDirection || floorsKeysPressedList[actualFloor] == 3) //People are entering the lift
        {
            takePassangers(ref peopleAbord, floorsQueuesList[actualFloor], actualFloor,
                        liftDirection, ref floorsKeysPressedList[actualFloor], capacity,
                        peopleAbordQueuesList);
            isStop = true;
        }

        if (peopleAbord == 0 && floorsKeysPressedList.Sum() == 0) liftDirection = 2; //If lift is empty, and nobody is waiting -> go down

        return isStop;
    }

    public static int[] TheLift(int[][] queues, int capacity)
    {
        System.Console.WriteLine("Capacity:" + capacity);

        int peopleAbord = 0;
        int actualFloor = 0;
        int liftDirection = 1; //Which direction the lift is actual going 1-up, 2-down

        var liftTrack = new List<int>();                      //Where the lift already been
        var peopleAbordQueuesList = new List<int>();          //Where people abord the lift want to go
        var floorsQueuesList = makeTableOfLists(queues);      //Where the people outside the lift want to go
        var floorsKeysPressedList = new int[queues.GetLength(0)];  //Whitch direction key is prees on each floor 0-none 1-up 2-down 3-all

        checkPreesedKeys(floorsKeysPressedList, floorsQueuesList);

        liftTrack.Add(0);
        printFloors(floorsQueuesList, peopleAbordQueuesList, actualFloor, liftDirection);

        while (!(floorsKeysPressedList.All(x => x == 0) && actualFloor == 0)) //Main loop of lift work
        {
            var checkMove = liftGo(ref actualFloor, ref liftDirection, ref peopleAbord, floorsKeysPressedList, floorsQueuesList, ref peopleAbordQueuesList, liftTrack, capacity);

            if (checkMove && liftTrack.Last() != actualFloor)
            {
                liftTrack.Add(actualFloor); //If there was a stop add it to list
                printFloors(floorsQueuesList, peopleAbordQueuesList, actualFloor, liftDirection);
            }

            var result = liftDirection == 1 ? actualFloor++ : actualFloor--; //Lift ride one floor (up or down)     
        }

        if (liftTrack.Last() != 0)
        {
            liftTrack.Add(0);
            printFloors(floorsQueuesList, peopleAbordQueuesList, actualFloor, liftDirection);
        }

        return liftTrack.ToArray();
    }
}