using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Animations;

public class Seat : MonoBehaviour
{
    public Animator anim;
    public BoxCollider Collider;
    public Passenger passenger;

    public bool isEmpty() { return passenger == null; }
    public void Place(Passenger passenger) {
 
        passenger.transform.SetParent(transform);
        this.passenger = passenger;

        var p = passenger.GetComponent<RectTransform>();
        var my = GetComponent<RectTransform>();
        //p.sizeDelta = my.sizeDelta;
        //p.anchoredPosition = my.anchoredPosition;
        p.position = my.position;
        //p.localScale = my.localScale;
    }
    public Passenger Remove() {
        var tmp = passenger;
        passenger = null;
        return tmp;
    }

    public void leaveSeat()
    {
        if (passenger)
        {
            StartCoroutine(Leave());
        } 
    }


    private IEnumerator Leave()
    {
        var anim = passenger.GetComponent<Animator>();
        anim.Play("passenger_leave");
        var go = passenger.gameObject;
        passenger = null;
        yield return new WaitForSeconds(5);
        Destroy(go);
    }

    public void setActive(bool active)
    {
        if (passenger)
        {
            Collider.enabled = active;
            passenger.setActive(active);
        }
    }

}