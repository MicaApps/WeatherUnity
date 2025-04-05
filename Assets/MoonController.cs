using UnityEngine;
using System.Collections;

public class MoonController : MonoBehaviour
{
    public int moon = -1;
    [Tooltip("请设置为30个，以对应每天的月亮状态")]
    public Sprite[] sprites;
    public SpriteRenderer SpriteRenderer;

    private void Start()
    {
        if (SpriteRenderer == null)
        {
            SpriteRenderer = GetComponent<SpriteRenderer>();
            if (SpriteRenderer == null)
            {
                Debug.LogError("SpriteRenderer not found on this GameObject.");
            }
        }
        if (sprites.Length != 30)
            Debug.LogError("The parameter is not 30.");
        StartCoroutine("LookAtCamear");
    }

    public void UpdateSprite()
    {
        if (moon >= 0 && moon < 30)
            SpriteRenderer.sprite = sprites[moon];
    }

    private IEnumerator LookAtCamear()
    {
        while (true)
        {
            if (gameObject.activeSelf)
            {
                transform.LookAt(Camera.main.transform);
            }
            yield return null;
        }
    }
}