using UnityEngine;

public class Target : MonoBehaviour
{
    public bool isLit = false;
    public float requiredTime = 1f;

    public GameObject levelCompleteText;

    private float litTimer = 0f;
    private bool levelCompleted = false;

    void Start()
    {
        if (levelCompleteText != null)
            levelCompleteText.SetActive(false);
    }

    void Update()
    {
        if (isLit && !levelCompleted)
        {
            litTimer += Time.deltaTime;

            if (litTimer >= requiredTime)
            {
                levelCompleted = true;

                if (levelCompleteText != null)
                    levelCompleteText.SetActive(true);
            }
        }
        else
        {
            litTimer = 0f;
        }
    }
}
