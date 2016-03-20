using UnityEngine;
using System.Collections;

public class SocialDialogManager : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void OnRateClicked ()
    {
        // TODO: Add Code to link to store rating 
        UnityEngine.WSA.Launcher.LaunchUri("ms-windows-store:REVIEW?PFN=Microsoft.Channel9_8wekyb3d8bbwe", false); 
        DismissDialog(); 
    }

    public void OnShareClicked ()
    {
        DismissDialog(); 
    }

    public void OnLaterClicked ()
    {
        DismissDialog(); 
    }

    void DismissDialog ()
    {
        this.gameObject.SetActive(false); 
        Complete.GameManager.Instance.BroadCast("SocialDialogDismissed"); 
    }
}
