using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class player : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "chunlian")
        {
            if (GameManager.instance.playerInfo.haveChunLian == 0)
            {
                GameManager.instance.playerInfo.haveChunLian = 1;
                Destroy(collision.gameObject);
            }
        }
        if (collision.gameObject.tag == "jiandao")
        {
            GameManager.instance.playerInfo.jiandao = true;
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.tag == "hongzhi")
        {
            GameManager.instance.playerInfo.hongzhi = true;
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "men")
        {
            if (GameManager.instance.playerInfo.haveChunLian != 2)
            {
                GameManager.instance.panelBase1.SetActive(true);
            }
        }
        if (collision.gameObject.tag == "men2")
        {
             GameManager.instance.panelBase3.SetActive(true);
        }
        if (collision.gameObject.tag == "bandeng")
        {
            GameManager.instance.panelBase2.SetActive(true);
          
        }
        if (collision.gameObject.tag == "bianpao")
        {
            StartCoroutine(GameManager.instance.Boom(collision.gameObject));
        }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "men")
        {
            
             GameManager.instance.panelBase1.SetActive(false);
           
        }
        if (collision.gameObject.tag == "men2")
        {

            GameManager.instance.panelBase3.SetActive(false);

        }
        if (collision.gameObject.tag == "bandeng")
        {

            GameManager.instance.panelBase2.SetActive(false);

        }
    }
}
