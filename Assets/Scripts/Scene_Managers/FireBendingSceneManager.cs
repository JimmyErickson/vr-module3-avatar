using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.Azure.Kinect.BodyTracking;
using System;

public class FireBendingSceneManager : MonoBehaviour
{

    private List<string> poseKeys;
    public GameObject guideAvatar;
    //private GameObject poseChangerGO;
    //PoseChanger poseChanger;
    public GameObject poseChangerObject;
    public GameObject playerBody;
    //public GameObject poseBody;
    public float tolerance;
    public Material green;
    public Material red;
    private float timer = 0;
    private float menuTimer = 0;
    public float timerMax;
    public GameObject MenuUI;
    public float menuTimerMax;
    //private string poseStringKey;
    private List<string> filteredPoseKeys;
    public GameObject playerObject;

    // Start is called before the first frame update
    void Start()
    {
        //poseChanger = poseChangerGO.AddComponent<PoseChanger>();
        //Debug.Log("Hello There");
        //poseStringKey = "TaiChi";
        SceneManagerStuff.MenuUI = MenuUI;
        SceneManagerStuff.menuTimerMax = menuTimerMax;
        poseChanger.setPoseBody(poseChangerObject);
        poseChanger.LoadData();
        List<string> unfilteredPoseKeys = new List<string>(poseChanger.poses.Keys);
        filteredPoseKeys = unfilteredPoseKeys.FindAll(FindKey);
        poseChanger.currentPose = 0;
        poseChanger.numberOfKeys = filteredPoseKeys.Count;
        PlayerBody.playerObject = playerObject;

        //poseKeys = unfilteredPoseKeys.FindAll(item => unfilteredPoseKeys.Contains("TaiChi"));
    }
    // Update is called once per frame
    void Update()
    {
        try
        {
            setMenuPose();
            //Debug.Log(filteredPoseKeys[0]);
            poseChanger.DisplayPoseRelative(filteredPoseKeys[poseChanger.currentPose]);
            guideAvatar.GetComponent<Animator>().SetInteger("animationState", poseChanger.currentPose);
            checkFullPose();
            checkMenu();
        }
        catch(NullReferenceException)
        {
            Debug.Log("oh well");
        }
        

    }

    void setMenuPose()
    {
        //List<string> unfilteredMenuPoseKeys = new List<string>(poseChanger.poses.Keys);
        //List<string> menuKey = unfilteredMenuPoseKeys.FindAll(FindMenuKey);
        try
        {
            poseChanger.SetMenuBody("MenuPose");
        }
        catch(NullReferenceException)
        {
            Debug.Log("oh well");
        }
        //poseChanger.DisplayPoseRelative(menuKey[0], false);
    }

    /*private static bool FindMenuKey(string item)
    {

        if (item.Contains("Menu"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }*/

    private static bool FindKey(string item)
    {

        if (item.Contains("DD"))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void checkFullPose()
    {
        int[] focusPoints = { (int)JointId.HandRight, (int)JointId.HandLeft, (int)JointId.ElbowRight, (int)JointId.ElbowLeft, (int)JointId.KneeRight, (int)JointId.KneeLeft, (int)JointId.FootRight, (int)JointId.FootLeft };
        //float[] distances = new float[8];
        float count = 0;
        for (int i = 0; i < focusPoints.Length; i++)
        {
            if (System.Numerics.Vector3.Distance(PlayerBody.playerPos.JointPositions3D[focusPoints[i]], PlayerBody.guidePos.JointPositions3D[focusPoints[i]]) < tolerance)
            {
                count++;
            }
        }

        Transform[] parts = playerBody.GetComponentsInChildren<Transform>();

        if (count == 8)
        {
            timer += Time.deltaTime;
            for (int i = 1; i < parts.Length; i++)
            {

                parts[i].GetComponent<MeshRenderer>().material = green;

            }
        }
        else
        {

            timer = 0;
            for (int i = 1; i < parts.Length; i++)
            {
                parts[i].GetComponent<MeshRenderer>().material = red;
            }
        }
        if (timer >= 3.0)
        {
            timer = 0;
            Debug.Log("Yay you did it!");
            FindObjectOfType<AudioManager>().Play("GoodJob");

            if (poseChanger.currentPose == filteredPoseKeys.Count - 1)
            {
                poseChanger.currentPose = 0;
            }
            else
            {
                poseChanger.currentPose++;
            }

        }
    }

    void checkMenu()
    {
        int[] focusPoints = { (int)JointId.HandRight, (int)JointId.HandLeft, (int)JointId.ElbowRight, (int)JointId.ElbowLeft, (int)JointId.KneeRight, (int)JointId.KneeLeft, (int)JointId.FootRight, (int)JointId.FootLeft };
        //float[] distances = new float[8];
        float count = 0;
        for (int i = 0; i < focusPoints.Length; i++)
        {
            if (System.Numerics.Vector3.Distance(PlayerBody.playerPos.JointPositions3D[focusPoints[i]], PlayerBody.menuPos.JointPositions3D[focusPoints[i]]) < tolerance)
            {
                count++;
            }
        }

        Transform[] parts = playerBody.GetComponentsInChildren<Transform>();

        if (count == focusPoints.Length)
        {
            //Debug.Log("MenuCheck");
            menuTimer += Time.deltaTime;
            for (int i = 1; i < parts.Length; i++)
            {

                parts[i].GetComponent<MeshRenderer>().material = green;

            }
        }
        else
        {

            menuTimer = 0;
            for (int i = 1; i < parts.Length; i++)
            {
                parts[i].GetComponent<MeshRenderer>().material = red;
            }
        }
        if (menuTimer >= menuTimerMax)
        {
            menuTimer = 0;
            SceneManagerStuff.MenuUI.SetActive(true);

        }
    }
}