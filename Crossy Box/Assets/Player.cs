using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using TMPro;

public class Player : MonoBehaviour
{
    [SerializeField] TMP_Text stepText;
    [SerializeField] ParticleSystem dieParticles;
    [SerializeField, Range(0.01f, 1f)] float moveDuration = 0.2f;
    [SerializeField, Range(0.01f, 1f)] float jumpHeight = 0.5f;

    private int backBoundary;
    private int leftBoundary;
    private int rightBoundary;
    
    [SerializeField] private int maxTravel;

    public int MaxTravel { get => maxTravel;}

    [SerializeField] private int currentTravel;
    public int CurrentTravel { get => currentTravel; }
    public bool IsDie { get => this.enabled == false; }
    // Update is called once per frame

    public void SetUp(int minZPos, int extent)
    {
        backBoundary = minZPos - 1;
        leftBoundary = -(extent + 1);
        rightBoundary = extent + 1;

        Debug.Log(backBoundary);
        Debug.Log(leftBoundary);
        Debug.Log(rightBoundary);
        Debug.Log(minZPos);
        Debug.Log(extent);
    }

    private void Update()
    {
        var moveDir = Vector3.zero;
        if (Input.GetKey(KeyCode.UpArrow))
            moveDir += new Vector3(0, 0, 1);

        if (Input.GetKey(KeyCode.DownArrow))
            moveDir += new Vector3(0, 0, -1);

        if (Input.GetKey(KeyCode.RightArrow))
            moveDir += new Vector3(1, 0, 0);

        if (Input.GetKey(KeyCode.LeftArrow))
            moveDir += new Vector3(-1, 0, 0);

        if (moveDir != Vector3.zero && IsJumping() == false)
            Jump(moveDir);

    }

    public void Jump(Vector3 targetDirection)
    {
        //Debug.Log(backBoundary);
        //Debug.Log(leftBoundary);
        //Debug.Log(rightBoundary);
        Vector3 targetPosition =
            transform.position + targetDirection;

        transform.LookAt(targetPosition); 

        var moveSeq = DOTween.Sequence(transform);
        moveSeq.Append(transform.DOMoveY(jumpHeight, moveDuration / 2));
        moveSeq.Append(transform.DOMoveY(0, moveDuration / 2));


        if (targetPosition.z <= backBoundary||
            targetPosition.x <= leftBoundary ||
            targetPosition.x >= rightBoundary)
        {
            Debug.Log("A");
            return;
        }

        if (Tree.AllPositions.Contains(targetPosition))
        {
            return;

        }


        transform.DOMoveX(targetPosition.x, moveDuration);
        transform
            .DOMoveZ(targetPosition.z, moveDuration)
            .OnComplete(UpdateTravel);
    }
    private void UpdateTravel()
    {
        currentTravel = (int) this.transform.position.z;

        if (currentTravel > maxTravel)
            maxTravel = currentTravel;
        stepText.text = "STEP : " + maxTravel.ToString();
    }

    public bool IsJumping()
    {
        return DOTween.IsTweening(transform);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (this.enabled == false)
            return;

        // di execute sekali pada frame ketika nempel pertama kali
        var car = other.GetComponent<Car>();


        if (car != null)
        {
            AnimateCrash(car);
        }

        //if (other.tag == "Car")
        //{
            //AnimateDie();
        //}
    }

    private void AnimateCrash(Car car)
    {

        //Gepeng
        transform.DOScaleY(0.3f, 1);
        transform.DOScaleX(3, 0.2f);
        transform.DOScaleZ(2, 0.2f);
        this.enabled = false;
        dieParticles.Play();
    }

    private void OnTriggerStay(Collider other)
    {
        // di execute setiap frame selama masih nempel
    }

    private void OnTriggerExit(Collider other)
    {
        // di execute sekali pada frame ketika tidak nempel
    }
}
