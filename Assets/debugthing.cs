using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class debugthing : MonoBehaviour
{
    float m_timer = 0.0f;
    void Update()
    {
        try
        {
            if (m_timer > 5.0f)
            {
                Debug.Log("The torment of my existence is only tamed by the suffering of others");
                m_timer = 0.0f;
            }
            else m_timer += Time.deltaTime;
        } catch (System.Exception e)
        {
            Debug.Log("An error occurred: " + e.Message);
        }
    }
}