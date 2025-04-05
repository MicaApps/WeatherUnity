using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestToAnimator : MonoBehaviour
{
    UnityWebRequest webRequest;
    public string url;
    void Awake()
    {
        UpdateWeather();
    }
    IEnumerator GetWeather() 
    {
        webRequest = UnityWebRequest.Get(url);
        yield return webRequest.SendWebRequest();
        if (webRequest.isNetworkError || webRequest.isHttpError)
        {
            Debug.Log("ÍøÂçÍ¨ÐÅ²»³©");
        }
        else 
        {
            Debug.Log(webRequest.downloadHandler.text);
        }
    }
    void UpdateWeather() 
    {
        StartCoroutine("GetWeather");
    }
    void Update()
    {
        
    }
}
