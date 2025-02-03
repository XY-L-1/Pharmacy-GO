using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class QuestionUnit : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void SetImage(Question question)
    {
        if (question.image.Length > 0)
        {
            StartCoroutine(requestImage(question.image));
        }
    }

    private IEnumerator requestImage(string url)
    {
        UnityWebRequest texture = UnityWebRequestTexture.GetTexture ("https://tornquisterik.github.io/Webpage/IMG_20210208_111233479.jpg");
        yield return texture.SendWebRequest ();
        Texture2D question = DownloadHandlerTexture.GetContent (texture);
        Rect rect = new Rect(0, 0, question.width, question.height);
        image.sprite = Sprite.Create (question, rect, new Vector2(), 1);
    }
}
