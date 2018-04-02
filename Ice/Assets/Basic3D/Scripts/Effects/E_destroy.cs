using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class E_destroy : MonoBehaviour {

    public float delayTime = 0.2f;
    private void Start() {
        StartCoroutine(Delay(delayTime));
    }

    IEnumerator Delay(float delay) {
        yield return new WaitForSeconds(delay);
        Destroy(this.gameObject);
    }
}
