using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Game
{

    public static Game current;

    public string nameOfPlayer;

    #region settingsVariables
    public float SFXVolume;
    public float MusicVolume;
    public float MasterVolume;
    //public Input keybinds;
    #endregion

    #region passwordSystemVariables
    public float[] bestTimes;
    public int[] lessDamageReceivedByFloor;
    public string[] listOfPasswords;
    public int[] floorForEachPassword;
    public int healthKits;

    private bool[] unlockedPasswords;
    private bool[] unlockedFloors;
    private string[] messages = new string[11];
    private int numberOfFloors = 3;
    private int numberOfPasswords = 11;
    private const int lenghtOfPasswords = 9;
    #endregion

    public Game()
    {
        MusicVolume = 1f;
        SFXVolume = 1f;
        MasterVolume = 1f;
        healthKits = 1;
        listOfPasswords = new string[numberOfPasswords];
        unlockedPasswords = new bool[numberOfPasswords];
        bestTimes = new float[numberOfFloors];
        lessDamageReceivedByFloor = new int[numberOfFloors];
        bool[] temp = { true, false, false };
        unlockedFloors = temp;

        for (int a = 0; a < numberOfFloors; a++)
        {
            bestTimes[a] = 999999999;
            lessDamageReceivedByFloor[a] = 999999999;
        }

        newPasswords();

        unlockedPasswords[0] = true;
        setMessages();        
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

    public bool checkPassword(string pass)
    {
        if (checkValidityOfString(pass))
        {
            pass = transformPassword(pass);
            int l = listOfPasswords.Length;

            for (int a = 0; a < l; a++)
            {
                //Debug.Log(pass + " vs " + listOfPasswords[a]);
                if (pass == listOfPasswords[a] && !unlockedPasswords[a])
                {
                    unlockedPasswords[a] = true;
                    SaveLoad.Save();
                    //Debug.Log("Password " + a + " is now " + unlockedPasswords[a]);
                    return true;
                }
            }
            return false;
        }
        else
        {
            return false;
        }
    }

    bool checkValidityOfString(string input)
    {
        int lenght = input.Length;

        for (int a = 0; a < lenght; a++)
        {
            if (!checkIfValidCharacter(input[a]))
                return false;
        }

        return true;
    }

    bool checkIfValidCharacter(char input)
    {
        if (((int)input > 47 && (int)input < 58) || ((int)input > 64 && (int)input < 91) || ((int)input > 96 && (int)input < 123))
        {
            return true;
        }
        else
            return false;
    }

    string transformPassword(string input)
    {
        string temp = "";
        int l = input.Length;

        for (int a = 0; a < l; a++)
        {
            if (((int)input[a] > 96 && (int)input[a] < 123))
                temp += (char)(input[a] - 32);
            else
                temp += input[a];
        }
        Debug.Log("pass is " + temp);
        return temp;
    }

    public string returnMessage(int num)
    {
        if (unlockedPasswords[num])
        {
            return messages[num];
        }
        else
        {
            return "ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR ERROR";
        }
    }

    void setMessages()
    {
        floorForEachPassword = new int[messages.Length];

        messages[0] = "(You found a coded quartz. Bashrans used them to store big chunks of information for almost unlimited periods of time. However, the information encoded inside is encrypted with multiple keys. Passwords are required to decrypt it.\nBy unlocking information you may also unlock energy locked in the quartz that you may use for healing.)";
        floorForEachPassword[0] = 0;

        messages[1] = "Translating information.\n\nSecurity Level 2 - 21201028\n\n\nThe Bashran Council got the best technicians on the planet and put them all together in a room. Our mission is to build a machine to save our civilization.\n\nAnd they put me in charge of keeping a complete registry of the development of this machine.\n\nThis will be interesting to say the least.";
        floorForEachPassword[1] = 1;

        messages[2] = "Translating information.\n\nSecurity Level 1 - 21341211\n\n\nRegardless of the cuts, development on the Vasili continues and is kept safe in the deepest parts of this building. \n\nIt seems like it will be finished on time.";
        floorForEachPassword[2] = 1;

        messages[3] = "Translating information.\n\nSecurity Level 4 -  21340000\n\n\nWe just received the information that NASA discovered primitive signs of life in a moon of Jupiter. Primitive is used lightly here, these organisms are fully developed, multicellular creatures, and some pictures taken by the space rover Fermidox reveal their size and shape to be roughly humanoid. \n\nThe reason why a marine species may have developed legs is still a mystery to us, it would be fascinating to explore the moon and its geography. \n\nThere are so many secrets of the universe we have yet to understand. I refuse to let humanity die after all this, to private them from the wonders of the unseen.\nWe will complete the Vasili.";
        floorForEachPassword[3] = 1;

        messages[4] = "Translating information.\n\nSecurity Level 2 - 21341220\n\n\nThe colonies were hit by a flare this morning.\n\nAll communications were cut, we have no idea how many communities were affected. This becomes more worrisome considering the planet is still deadly without the protection of magnetic shields.\n\nIf the solar flare hit the main communities, I am afraid we have even less time than we tought.";
        floorForEachPassword[4] = 2;

        messages[5] = "Translating information.\n\nSecurity Level 2 -  21201126\n\n\nI was not surprised when they told us that we are all going to die soon.\n\nWe have lost control of our own reproduction and the issue was never handled correctly. Everyone knows the problem but nobody ever talks about it and the Bashran Council have negated it for so long that it was not until now that they decided to take any action.\n\nI will never understand how those [untranslatable] got to where they are.";
        floorForEachPassword[5] = 2;

        messages[6] = "Translating information.\n\nSecurity Level 1 - 21341228\n\n\nOur time is running out. Everything is crumbling in chaos and despair as a response to the consideration of our extinction. \n\nIt is ironic how the fear of extinction brings us closer to it. And how we are the only ones that can fix everything before it is too late. \n\nVasili needs to be completed.";
        floorForEachPassword[6] = 2;

        messages[7] = "Translating information.\n\nSecurity Level 3 - 21210201\n\n\nMost things are made of attractor and repulsive particles joined together and chained with other unions of particles of the same qualities.\nHowever, even if everything is made of the same, most things are also useless. Nothing more than filth that cannot be exploited.\n\nBut what if that could be changed.\nWhat if it was possible to dissolve these unions and join them together to form other materials.\n\nI thought of this idea, and in a moment of enlightenment, came up with a theory behind the process.\n\nI think I know how to save us.";
        floorForEachPassword[7] = 2;
        
        messages[8] = "Translating information.\n\nSecurity Level 3 - 21210415\n\n\nGetting the rest of the team to accept the idea of a transmutator as our little science project took some time, but I managed to convince everyone. The Bashran Council seemed skeptic at first, but they finally agreed to the construction of the machine.\n\nWe decided to call the project Vasili, in honor of Vasili Arkhipov, the soviet navy that prevented the armageddon during the Cold War over 150 years ago. We agreed that the name sounded simple and fitted perfectly with the situation.\n\nI cannot believe we are going to save humanity with an Atom Transmutator. Guess those science fiction novels I read as a kid served some purpose in the end.";
        floorForEachPassword[8] = 3;

        messages[9] = "Translating information.\n\nSecurity Level 1 - 21350000\n\n\nVasili is functionally complete, we just need to make some modifications before sending it back. \n\nThe materials used will make it durable and safe, but some security overdrives need to be done, else the disconnection from the central terminal may result in data cleansing that would leave the apparatus useless.\n\nAt least the hardest part is done. We are saved.";
        floorForEachPassword[9] = 3;

        messages[10] = "Translating information.\n\nSecurity Level 3 - 21350001\n\n\nFate can be cruel.\nNo… there is no “can” here. Fate is a [untranslatable]. \n\nThe last hope of humanity… a machine that can turn atoms into other atoms. We were just one step away from disconnecting it from the main frame. I do not know if the operation was completed actually.\n\nSuddenly, the lab entered some sort of quarantine protocol. Some sort of glitch in the system. Every oxygen producer, temperature normalizer, everything is shut down now. We tried contacting the Chiefs, but we have no idea if the message reached them.\n\nIf it didn’t, we are dead. Minor detail of establishing a laboratory to save humanity in the [untranslatable] south pole: People die if the technology does not work properly.\nGuess I now know how the martians felt.\n\nMaybe we will live through this. Maybe the message will be received on time and transportation will arrive before the lack of oxygen kills us. In the very least, even if we are dead, they will be able to recover Vasili and take it back. They all should have IDs so the security protocols will not activate themselves. \n\n...Or maybe the message will never go through the snow storm and we will die, rot and banish into the unstoppable flow of time in these cold heartless facilities in the middle of nowhere. ...Our names unknown to anyone, our lives mere stains in the fabric of existence.\n\n...It’s weird… just the thought of dying scares the [untranslatable] outta me. We worked so far… come so far… but in the end… it may not matter.\n\n...I guess it’s easy to forget, in the middle of all the knowledge that comes with understanding a fragment of the universe… that we are mortals too… That regardless of how intelligent we are, there is an end to our story…\n\n...But I refuse to die here and now...\n\n...Please Allah… I was never a religious person… but please… let my work be for something. Let it work… and let me live enough to see it save everything…\n\n...Let me get through this… let me come back and see the world turn into a better place… let me fix the mistakes that us selfish humans have unleashed upon the Earth… There is so much I want to see… so much I want to do… Please Allah don’t let me die here...\n\n\n\n...I don’t want to die...";
        floorForEachPassword[10] = 3;
    
    }

    void setVolumes(float sfx, float music, float master)
    {
        SFXVolume = sfx * master;
        MusicVolume = music * master;
    }

    public void setUnlockFloor(int floor, bool value)
    {
        unlockedFloors[floor - 1] = value;
    }

    public void setNewTimeRecord(float newTime, int floor)
    {
        bestTimes[floor] = newTime;
    }

    public void setDamageRecord(int newDamage, int floor)
    {
        lessDamageReceivedByFloor[floor] = newDamage;
    }

    public bool[] returnUnlockedFloors()
    {
        return unlockedFloors;
    }

    public bool[] returnUnlockedPasswords()
    {
        return unlockedPasswords;
    }

    public void newPasswords()
    {
        do
        {
            for (int a = 1; a < numberOfPasswords; a++)
            {
                listOfPasswords[a] = generatePassword(lenghtOfPasswords);
                unlockedPasswords[a] = false;
            }

        } while (!checktIfThereAreRepetitions(listOfPasswords));
    }

    public void unlockAllPasswords()
    {
        int l = unlockedPasswords.Length;

        for (int a = 0; a < l; a++)
        {
            unlockedPasswords[a] = true;
        }
    }

    public void unlockAllFloors()
    {
        int l = unlockedFloors.Length;

        for (int a = 0; a < l; a++)
        {
            unlockedFloors[a] = true;
        }
    }
}
