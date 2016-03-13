using UnityEngine;
using System.Collections;

public class levelSelection : MonoBehaviour{

    public void loadLevel(string name)
    {
        Application.LoadLevel(name);
    }
}
