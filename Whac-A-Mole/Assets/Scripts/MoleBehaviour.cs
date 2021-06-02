using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoleBehaviour : MonoBehaviour
{
    Vector3 originalPosition;
    float timeToStartMoving = 1.0f;
    bool isShown = false;
    public float initTimeMin = 0.5f;
    public float initTimeMax = 3.5f;
    private bool moleIsRunning = true;


    // Start is called before the first frame update
    void Awake()
    {
        originalPosition = transform.position;
        ResetMole(initTimeMin, initTimeMax);
    }

    // Update is called once per frame
    void Update()
    {
        if (moleIsRunning)
        {
            timeToStartMoving -= Time.deltaTime;
            if (timeToStartMoving <= 0.0f && isShown == false)
            {
                transform.position = originalPosition;
                isShown = true;

                timeToStartMoving = Random.Range(3.5f, 7.8f);
            }
            else if (timeToStartMoving <= 0.0f && isShown == true)
            {
                ResetMole(3.5f, 7.8f);
            }
        }
       
    }

    public void ResetMole()
    {

        ResetMole(initTimeMin, initTimeMax);
    }
    public void ResetMole(float minTime, float maxTime)
    {
        moleIsRunning = true;
        isShown = false;
        Vector3 newPos = originalPosition;
        newPos.y -= 1.0f;
        transform.position = newPos;
        timeToStartMoving = Random.Range(minTime, maxTime);
    }

    public void OnHitMole()
    {
        if (isShown == true)
        {
            ResetMole(3.5f, 7.8f);
        }
    }

    public void StopMole()
    {
        moleIsRunning = false;
        isShown = false;
        Vector3 newPos = originalPosition;
        newPos.y -= 1.0f;
        transform.position = newPos;
    }


}
