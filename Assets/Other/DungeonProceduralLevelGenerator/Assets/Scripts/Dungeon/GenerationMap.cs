using UnityEngine;
using Unity.Netcode;

namespace ProceduralLevelGenerator
{
    public class GenerationMap : NetworkBehaviour
    {
        public static int[,] myArr;
        
        public static int maxRoom;
        [Header("maximum branching level")]
        public int maxRoomInspector;

        public static int maxLength;
        private string str;
        private int randomDoor, randomRoom;

        [HideInInspector] public NetworkVariable<string> finalMatrix = new NetworkVariable<string>();

        public void GenerationMatrix()
        {      
            maxRoom = maxRoomInspector ;

            // the maximum length of the array is built from the maximum number of possible rooms
            // the value of the maximum rooms is multiplied by the dimension of the room and multiplied by two sides and summed up with the dimension of the starting room
            maxLength = maxRoom * 3 * 2 + 3;
            myArr = new int[maxLength, maxLength];

            // rearrangement of doors in 4 directions
            One(maxLength / 2 + 1, maxLength / 2, maxLength / 2, maxLength / 2, 1, 1);
            One(maxLength / 2 - 1, maxLength / 2, maxLength / 2, maxLength / 2, 1, 1);
            One(maxLength / 2, maxLength / 2 + 1, maxLength / 2, maxLength / 2, 1, 1);
            One(maxLength / 2, maxLength / 2 - 1, maxLength / 2, maxLength / 2, 1, 1);

            //collecting the array into a string and passing the string
            for (int i = 0; i < maxLength; i++)
            {
                for (int j = 0; j < maxLength; j++)
                {
                    str = str + myArr[i, j];
                }
                str = str + "\n";
            }

            Debug.Log(str); //debug matrix
            finalMatrix.Value = str;
        }

        public void One(int xNew, int yNew, int xOld, int yOld, int door, int room)
        {
            //put 1, this means the door at the new coordinate
            myArr[xNew, yNew] = 1;

            // if we went up or down then
            if (Mathf.Abs(yNew - yOld) != 0)
            {
                // then there should be a wall on the side of the door, set 0
                myArr[xNew - 1, yNew] = 0;
                myArr[xNew + 1, yNew] = 0;

                // if we went down, then it is necessary to change the value of the old and new coordinate points and increase the value
                if (yNew > yOld)
                {
                    yOld = yNew;
                    yNew++;
                }

                // if we went up, then it is necessary to change the value of the old and new coordinate points and decrease the coordinate
                else
                {
                    yOld = yNew;
                    yNew--;
                }
            }

            // if we walked left or right
            else
            {
                // if we go left or right, then there should be a wall above and below the door, set 0
                myArr[xNew, yNew - 1] = 0;
                myArr[xNew, yNew + 1] = 0;

                // if we go to the right, we need to change the value of the old and new coordinate points and increase the value
                if (xNew > xOld)
                {
                    xOld = xNew;
                    xNew++;
                }

                // if to the left, then you need to change the value of the old and new coordinate points and decrease the coordinate
                else
                {
                    xOld = xNew;
                    xNew--;
                }
            }

            // for the algorithm to work, the room must be represented as a 3x3 array, 
            // therefore the exit from the room must put another door, 
            // and then set 0
            if (door == 1)
            {
                door--;
                One(xNew, yNew, xOld, yOld, door, room);
            }
            else
            {
                Zero(xNew, yNew, xOld, yOld, room);
            }
        }

        public void Zero(int xNew, int yNew, int xOld, int yOld, int room)
        {
            //put 0, which means the center of the room
            myArr[xNew, yNew] = 0;

            // the more we have the maximum number of rooms, the greater the chance of a new room appearing
            // if we have reached the maximum number of rooms, then we do not start the distribution of rooms further
            randomRoom = maxRoomInspector;
            if (room < randomRoom)
            {
                // if the building algorithm goes down then
                if (Mathf.Abs(yNew - yOld) != 0)
                {
                    // then we increase the number of rooms and with a probability of 50% we start building doors in different directions
                    room++;

                    // if we went down then
                    if (yNew > yOld)
                    {
                        // then with a probability of 50% we start the construction of doors in different directions
                        randomDoor = Random.Range(0, 2);

                        // building down
                        if (myArr[xNew, yNew + 1] != 1 && randomDoor == 1)
                            One(xNew, yNew + 1, xOld, yOld + 1, 1, room);
                        randomDoor = Random.Range(0, 2);

                        // building to the right
                        if (myArr[xNew + 1, yNew] != 1 && randomDoor == 1)
                            One(xNew + 1, yNew, xOld, yOld + 1, 1, room);
                        randomDoor = Random.Range(0, 2);

                        // building to the left
                        if (myArr[xNew - 1, yNew] != 1 && randomDoor == 1)
                            One(xNew - 1, yNew, xOld, yOld + 1, 1, room);
                    }

                    // if we went up then
                    else
                    {
                        // we increase the number of rooms and with a probability of 50 % we start building doors in different directions
                        randomDoor = Random.Range(0, 2);

                        // building up
                        if (myArr[xNew, yNew - 1] != 1 && randomDoor == 1)
                            One(xNew, yNew - 1, xOld, yOld - 1, 1, room);
                        randomDoor = Random.Range(0, 2);

                        // building to the right
                        if (myArr[xNew + 1, yNew] != 1 && randomDoor == 1)
                            One(xNew + 1, yNew, xOld, yOld - 1, 1, room);
                        randomDoor = Random.Range(0, 2);

                        // building to the left
                        if (myArr[xNew - 1, yNew] != 1 && randomDoor == 1)
                            One(xNew - 1, yNew, xOld, yOld - 1, 1, room);
                    }
                }
                else
                {
                    room++;
                    // if you walked to the right then
                    if (xNew > xOld)
                    {
                        // building to the right
                        randomDoor = Random.Range(0, 2);
                        if (myArr[xNew + 1, yNew] != 1 && randomDoor == 1)
                            One(xNew + 1, yNew, xOld + 1, yOld, 1, room);
                        randomDoor = Random.Range(0, 2);

                        // building down
                        if (myArr[xNew, yNew + 1] != 1 && randomDoor == 1)
                            One(xNew, yNew + 1, xOld + 1, yOld, 1, room);
                        randomDoor = Random.Range(0, 2);

                        // building up
                        if (myArr[xNew, yNew - 1] != 1 && randomDoor == 1)
                            One(xNew, yNew - 1, xOld + 1, yOld, 1, room);
                    }

                    // if you walked to the left
                    else
                    {
                        // building to the left
                        randomDoor = Random.Range(0, 2);
                        if (myArr[xNew - 1, yNew] != 1 && randomDoor == 1)
                            One(xNew - 1, yNew, xOld - 1, yOld, 1, room);
                        randomDoor = Random.Range(0, 2);

                        // building down
                        if (myArr[xNew, yNew + 1] != 1 && randomDoor == 1)
                            One(xNew, yNew + 1, xOld - 1, yOld, 1, room);
                        randomDoor = Random.Range(0, 2);

                        // building up
                        if (myArr[xNew, yNew - 1] != 1 && randomDoor == 1)
                            One(xNew, yNew - 1, xOld - 1, yOld, 1, room);
                    }
                }
            }
        }
    }
}