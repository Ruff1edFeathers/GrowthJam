using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateWater : MonoBehaviour
{
    public Material m_Water;
    public float    m_Speed;

    public Vector2 m_FreqX;
    public Vector2 m_FreqY;

    private Vector2 m_Offset;

    private void Update()
    {
        Vector2 PerlinCoord_X = m_FreqX * Time.time;
        Vector2 PerlinCoord_Y = m_FreqY * Time.time;

        m_Offset.x += (0.5f - Mathf.PerlinNoise(PerlinCoord_X.x, PerlinCoord_X.y)) * m_Speed * Time.deltaTime;
        m_Offset.y += (0.5f - Mathf.PerlinNoise(PerlinCoord_Y.x, PerlinCoord_X.y)) * m_Speed * Time.deltaTime;

        m_Water.mainTextureOffset = m_Offset;
    }
}
