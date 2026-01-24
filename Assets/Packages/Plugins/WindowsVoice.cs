using UnityEngine;
using System.Text;
using System.Runtime.InteropServices;
 
public static class CoroutineExtensions
{
    public static Coroutine ExecuteLater(this MonoBehaviour behaviour, float delay, System.Action fn)
    {
        return behaviour.StartCoroutine(_realExecute(delay, fn));
    }
    
    private static System.Collections.IEnumerator _realExecute(float delay, System.Action fn)
    {
        yield return new WaitForSeconds(delay);
        fn?.Invoke();
    }
}

public class WindowsVoice : MonoBehaviour {
    [DllImport("WindowsVoice")]
    public static extern void initSpeech();
    [DllImport("WindowsVoice")]
    public static extern void destroySpeech();
    [DllImport("WindowsVoice")]
    public static extern void addToSpeechQueue(string s);
    [DllImport("WindowsVoice")]
    public static extern void clearSpeechQueue();
    [DllImport("WindowsVoice")]
    public static extern void statusMessage(StringBuilder str, int length);
    
    public static WindowsVoice theVoice = null;
    
    void OnEnable()
    {
        if (theVoice == null)
        {
            theVoice = this;
            initSpeech();
        }
    }
    
    public void test()
    {
        speak("Testing");
    }
    
    public static void speak(string msg)
    {
        Debug.Log($"Received message to speak: {msg}");
        if (theVoice == null)
        {
            Debug.LogError("WindowsVoice instance is null!");
            return;
        }
        addToSpeechQueue(msg);
        Debug.Log("Message added to speech queue.");
    }
    
    void OnDestroy()
    {
        if (theVoice == this)
        {
            Debug.Log("Destroying speech");
            destroySpeech();
            Debug.Log("Speech destroyed");
            theVoice = null;
        }
    }
    
    public static string GetStatusMessage()
    {
        StringBuilder sb = new StringBuilder(40);
        statusMessage(sb, 40);
        return sb.ToString();
    }
}

