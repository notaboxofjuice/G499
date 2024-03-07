using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class RelayUI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_inputJoinCode;
    private string m_joinCode;
    [SerializeField] TextMeshProUGUI m_textJoinCode;
    public async void OnCreateRelay()
    {
        string joinCode = await RelayManager.Instance.CreateRelay();
        if (joinCode != null)
        {
            m_textJoinCode.text = joinCode;
        }
        else Debug.LogError("Failed to create relay");
    }
    public async void OnJoinRelay()
    {
        if (m_joinCode != null) await RelayManager.Instance.JoinRelay(m_joinCode);
        else Debug.LogError("Join code is empty");
    }
    public void OnValueChanged()
    {
        m_joinCode = m_inputJoinCode.text;
    }
}