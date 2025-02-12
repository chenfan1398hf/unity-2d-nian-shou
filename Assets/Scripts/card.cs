using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class card : MonoBehaviour
{
    // Start is called before the first frame update
    public int id = 0;
    public Vector2 oldVec3 = new Vector2();
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void InitCard(int _id, Vector2 _oldVec3)
    {
        id = _id;
        oldVec3 = _oldVec3;
        this.rectTransform().anchoredPosition = _oldVec3;
        //this.transform.position = oldVec3;
    }
    public void DragMethod()
    {
        //��������϶�����
        transform.position = Input.mousePosition;
    }
    //�����϶�
    public void EndDragMethod()
    {
        bool isBool = GameManager.instance.CheckCardVec(id, this.transform.position);
        if (isBool)
        {
            //�ɹ�
            Destroy(this.gameObject);
        }
        else
        {
            this.rectTransform().anchoredPosition = oldVec3;
        }
        
    }
}
