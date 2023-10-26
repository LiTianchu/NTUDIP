using UnityEngine;
using UnityEngine.UI;

public class GIFAnimator : MonoBehaviour
{
    public Sprite[] gifFrames;
    public float frameRate = 0.1f;

    private Image image;
    private int currentFrame;

    private float timer;

    private void Start()
    {
        image = GetComponent<Image>();
        currentFrame = 0;
        timer = 0f;
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timer >= frameRate)
        {
            timer -= frameRate;
            currentFrame = (currentFrame + 1) % gifFrames.Length;
            image.sprite = gifFrames[currentFrame];
        }
    }
}
