using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using System.Collections;

public class CameraMoveScript : MonoBehaviour
{
    private Camera camComponent;
    float verRotate;
    float horRotate;

    //shoot vars
    public float shootSpeed;
    public float inaccuracy;
    public float damage = 5;

    //thump vars
    public int normalFOV = 60;
    public int thumpFOV;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        camComponent = GetComponent<Camera>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseChange = Mouse.current.delta.ReadValue();

        float mouseX = mouseChange.x * 30 * Time.deltaTime;
        float mouseY = mouseChange.y * 30 * Time.deltaTime;

        verRotate -= mouseY;
        verRotate = Mathf.Clamp(verRotate, -45f, 45f);

        horRotate += mouseX;
        horRotate = Mathf.Clamp(horRotate, -45f, 45f);

        transform.localRotation = Quaternion.Euler(verRotate, horRotate, 0);
    }

    void Shoot()
    {
        Vector3 shootDir = transform.forward;
        shootDir.x += Random.Range(-inaccuracy, inaccuracy);
        shootDir.y += Random.Range(-inaccuracy, inaccuracy);

        if (Physics.Raycast(transform.position, shootDir, out RaycastHit hit, 1000))
        {
            MobScript mobScript = hit.collider.GetComponent<MobScript>();
            if (mobScript)
            {
                mobScript.TakeDamage(damage);
            }
        }
    }

    public IEnumerator ThumpFOV(float givenTime)
    {
        float timeElapsed = 0f;

        while (timeElapsed < givenTime/2)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed/ (givenTime/2);

            camComponent.fieldOfView = Mathf.Lerp(normalFOV, thumpFOV, t);
            yield return null;
        }

        while (timeElapsed < givenTime)
        {
            timeElapsed += Time.deltaTime;
            float t = timeElapsed/givenTime;

            camComponent.fieldOfView = Mathf.Lerp(thumpFOV, normalFOV, t);
            yield return null;
        }

        camComponent.fieldOfView = normalFOV;

        
    }
}
