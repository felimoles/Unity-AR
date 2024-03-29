﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;
using UnityEngine.UI;
public class API : MonoBehaviour {
    string theName;
    public GameObject intputField;
    const string BundleFolder = "http://152.74.151.33/~fmorales/download/";
    //const string BundleFile = "AssetBundles";
    const string ItemList = "http://152.74.151.33/~fmorales/download/ItemList.php?folder=";
    string url = null;
    public void GetItemList(UnityAction<List<string>> callback) {
        StartCoroutine(GetItemListRoutine(callback));
    }

    IEnumerator GetItemListRoutine(UnityAction<List<string>> callback) {
        theName = intputField.GetComponent<Text>().text;
        url = ItemList + theName+"/";
        Debug.Log(url);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log("Network error");
        } else {
            string rawText = www.downloadHandler.text;
            //split string by comma
            string[] items = rawText.Split(',');
            //remove empty values and convert to list
            List<string> url = items.Where(x => !string.IsNullOrEmpty(x)).ToList();
            //return list to caller
            callback.Invoke(url);
        }
    }

    public void GetBundleObject(string assetName, UnityAction<GameObject> callback, Transform bundleParent) {
        // theName = inputField.GetComponent<Text>().text;
        StartCoroutine(GetDisplayBundleRoutine(assetName, callback, bundleParent));
    }

    IEnumerator GetDisplayBundleRoutine(string assetName, UnityAction<GameObject> callback, Transform bundleParent) {
        theName = intputField.GetComponent<Text>().text;
        Debug.Log(theName);
        string bundleURL = BundleFolder+ theName + "/" + assetName + "-";
        Debug.Log(bundleURL);
        //append platform to asset bundle name
#if UNITY_ANDROID
        bundleURL += "Android";
#else
        bundleURL += "IOS";
#endif

        Debug.Log("Requesting bundle at " + bundleURL);

        //request asset bundle
        UnityWebRequest www = UnityWebRequestAssetBundle.GetAssetBundle(bundleURL);
        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log("Network error");
        } else {
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(www);
            if (bundle != null) {
                string rootAssetPath = bundle.GetAllAssetNames()[0];
                GameObject arObject = Instantiate(bundle.LoadAsset(rootAssetPath) as GameObject,bundleParent);
                bundle.Unload(false);
                callback(arObject);
            } else {
                Debug.Log("Not a valid asset bundle");
            }
        }
    }
}
