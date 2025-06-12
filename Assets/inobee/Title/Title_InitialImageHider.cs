using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialImageHider : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(HideAfterDelay());
    }

    IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        this.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update() { }
}
