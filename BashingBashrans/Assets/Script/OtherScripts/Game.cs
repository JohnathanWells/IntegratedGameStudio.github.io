using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Game
{

    public static Game current;

    public string nameOfPlayer;
    public float[] bestTimes;
    public int[] lessDamageReceivedByFloor;
    public string[] listOfPasswords;
    public int[] floorForEachPassword = {1, 1, 1, 1, 2, 2, 2, 2, 3, 3, 3};
    public bool[] unlockedPasswords;
    public bool[] unlockedFloors;

    private int numberOfFloors = 3;
    private int numberOfPasswords = 11;

    public Game()
    {
        listOfPasswords = new string[numberOfPasswords];
        unlockedPasswords = new bool[numberOfPasswords];
        bestTimes = new float[numberOfFloors];
        lessDamageReceivedByFloor = new int[numberOfFloors];
        bool[] temp = { true, false, false };
        unlockedFloors = temp;

        for (int a = 0; a < numberOfFloors; a++)
        {
            bestTimes[a] = 10000000000000;
            lessDamageReceivedByFloor[a] = 100000000;
        }

        do
        {
            for (int a = 0; a < numberOfPasswords; a++)
            {
                listOfPasswords[a] = generatePassword(10);
                unlockedPasswords[a] = false;
            }

        } while (!checktIfThereAreRepetitions(listOfPasswords));

        //for (int a = 0; a < numberOfPasswords; a++)
        //    Debug.Log("Password " + a + ": " + listOfPasswords[a]);
    }

    private string generatePassword(int lenght)
    {
        string result = "";
        char temp;

        for (int a = 0; a < lenght; a++)
        {
            temp = transformIntoHexadecimal(Random.Range(0, 17));
            //Debug.Log(temp);
            result += temp;
            //Debug.Log(result[a]);
        }

        //Debug.Log(result + "/");
        
        return result;
    }

    private char transformIntoHexadecimal(int number)
    {
        if (number >= 0 && number <= 9)
        {
            //Debug.Log((char)(48 + number));
            return (char)(48 + number);
        }
        else if (number >= 10 && number <= 16)
        {
            number -= 9;
            //Debug.Log((char)(65 + number));
            return (char)(65 + number);
        }
        else
        {
            //Debug.Log("X");
            return 'X';
        }
    }

    private bool checktIfThereAreRepetitions(string[] passwords)
    {
        bool repetitionFound = false;
        int size = passwords.Length;

        for (int a = 0; a < size; a++)
        {
            for (int b = 0; b < size; b++)
            {
                if (passwords[a] == passwords[b])
                {
                    repetitionFound = true;
                    break;
                }
            }

            if (repetitionFound)
                break;
        }

        return repetitionFound;
    }

    public string[] getAllPasswordsFromAFloor(int floor)
    {
        int size = listOfPasswords.Length;
        //Debug.Log(size);
        List<string> newArray = new List<string>();

        for (int a = 0; a < size; a++)
        {
            if (floorForEachPassword[a] == floor)
            {
                //Debug.Log(listOfPasswords[a]);
                newArray.Add(listOfPasswords[a]);
            }
        }

        //Debug.Log(newArray);
        return newArray.ToArray();
    }
}
