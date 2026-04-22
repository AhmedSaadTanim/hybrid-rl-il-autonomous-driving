using UnityEngine;

public class CarFreezeControl : MonoBehaviour
{
    private Rigidbody rb;
    private CarController car; // your driving script

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        car = GetComponent<CarController>(); // change if name differs
    }

    public void SetFrozen(bool frozen)
    {
        if (car) car.enabled = !frozen;

        if (rb)
        {
            rb.isKinematic = frozen;
            if (frozen)
            {
                //rb.linearVelocity = Vector3.zero;
                //rb.angularVelocity = Vector3.zero;
            }
        }
    }
}