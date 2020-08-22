using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FbShare : MonoBehaviour
{
  public void FacebookShare()
    {
        User u = DataBridge.instance.userProfile;
        FB.ShareLink(new System.Uri("https://github.com/RiderII"), u.username + "ha terminado una carrera",
            "RiderII es genial para ejercitar en cuarentena!",
            new System.Uri("https://avatars0.githubusercontent.com/u/65631755?s=200&v=4"));
    }
}
