using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class introductionScript : MonoBehaviour {

    public float fadingSpeed = 1;
    public float timeBetweenFading = 0.1f;
    public Image fadingTexture;

    IEnumerator loadLevel(string levelName)
    {
        Color temp = fadingTexture.color;
        while (temp.a < 1)
        {
            temp.a += fadingSpeed;
            fadingTexture.color = temp;
            yield return new WaitForSeconds(timeBetweenFading);
        }
    }

    public void fadeOut(string levelName)
    {
        StartCoroutine(loading(levelName));
    }

    IEnumerator loading(string levelName)
    {
        StartCoroutine(loadLevel(levelName));

        yield return new WaitForSeconds(0f);

        SceneManager.LoadScene(levelName);

        //AsyncOperation async = SceneManager.LoadSceneAsync(levelName);

        //while (!async.isDone)
        //    yield return null;
    }
}
