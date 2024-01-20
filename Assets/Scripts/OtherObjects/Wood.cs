using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Wood : MonoBehaviour
{
    public float destroyDelay;
    //[SerializeField] private GameObject fire;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("FireSpell"))
        {
            //Instantiate(fire, transform.position, Quaternion.identity);
            StartCoroutine(DestroyWoodWithDelay());
        }
    }

    private IEnumerator DestroyWoodWithDelay()
    {
        yield return new WaitForSeconds(destroyDelay);
        //Destroy(fire);
        Destroy(gameObject);
        
    }

    
}
