using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class PlayerController : LivingEntity
{
    private Rigidbody rigidbody;
    private Vector3 movement;
    private Ray ray;
    private Camera mainCamera;
    [SerializeField] public float speed;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        rigidbody = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        movement = new Vector3 (Input.GetAxis("Horizontal"),0f,Input.GetAxis("Vertical"));
        LookAtCur();
    }

    private void LookAtCur()
    {
        ray = mainCamera.ScreenPointToRay(Input.mousePosition);//视角跟随鼠标
        // Plane plane = new Plane (Vector3.up,Vector3.zero);
        Plane plane = new Plane(new Vector3(Vector3.up.x,transform.position.y,Vector3.up.z),new Vector3(Vector3.zero.x,transform.position.y,Vector3.zero.z));
        float distToGround;
        if (plane.Raycast(ray,out distToGround))
        {
            // Vector3 point = ray.GetPoint(distToGround);
            // Vector3 rightPoint = new Vector3(point.x,transform.position.y,point.z);
            Vector3 rightPoint = ray.GetPoint(distToGround);
            transform.LookAt(rightPoint);
        }
    }

    private void FixedUpdate() 
    {
        rigidbody.velocity = new Vector3(movement.normalized.x * speed,rigidbody.velocity.y,movement.normalized.z * speed);
        // rigidbody.MovePosition(rigidbody.position + movement * speed * Time.fixedDeltaTime);
    }

    
}
