using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateManager : MonoBehaviour
{
    struct UpdateData
    {
        public System.Action m_Del;
        public Object m_Handle;
    }

    private static UpdateManager s_Instance;

    public static void Add(Object Handle, System.Action Del)
    {
        if (s_Instance.m_Count == s_Instance.m_Data.Length)
            System.Array.Resize(ref s_Instance.m_Data, s_Instance.m_Count * 2);

        s_Instance.m_Data[s_Instance.m_Count++] = new UpdateData()
        {
            m_Del    = Del,
            m_Handle = Handle
        };
    }

    public static void Remove(Object Handle)
    {

    }

    private UpdateData[] m_Data = new UpdateData[1];
    private int          m_Count = 0;

    private void Update()
    {
        
    }
}
