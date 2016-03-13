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
    private int numberOfPasswords;

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
    }

    private string generatePassword(int lenght)
    {
        char[] result = new char[lenght];

        for (int a = 0; a < lenght; a++)
        {
            result[a] = transformIntoHexadecimal(Random.Range(0, 17));
        }

        return result.ToString();
    }

    private char transformIntoHexadecimal(int number)
    {
        if (number >= 0 && number <= 9)
        {
            return (char)(48 + number);
        }
        else if (number >= 10 && number <= 16)
        {
            number -= 9;
            return (char)(65 + number);
        }
        else
            return 'X';
    }

    private bool checktIfThereAreRepetitions(string[] passwords)
    {
        bool everythingInOrder = true;
        int size = passwords.Length;

        for (int a = 0; a < size; a++)
        {
            for (int b = 0; b < size; b++)
            {
                if (passwords[a] == passwords[b])
                {
                    everythingInOrder = false;
                    break;
                }
            }

            if (!everythingInOrder)
                break;
        }

        return everythingInOrder;
    }

    public string[] getAllPasswordsFromAFloor(int floor)
    {
        int size = listOfPasswords.Length;
        Debug.Log(size);
        List<string> newArray = null;

        for (int a = 0; a < size; a++)
        {
            if (floorForEachPassword[a] == floor)
            {
                Debug.Log(floorForEachPassword[a]);
                newArray.Add(listOfPasswords[a]);
            }
        }

        return newArray.ToArray();
    }
}
