using UnityEngine;

public class CameraMovement : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        if (Input.GetKey(KeyCode.A))
        {
            if (x-0.1 > -1.7)
            {
                x -= 0.1f;
            }   
        }else if (Input.GetKey(KeyCode.D))
        {
            if (x + 0.1 < 16)
            {
                x += 0.1f;
            }

        }else if (Input.GetKey(KeyCode.S))
        {
            if (y - 0.1 >-5.6)
            {
                y -= 0.1f;
            }

        }else if (Input.GetKey(KeyCode.W))
        {
            if (y + 0.1 <1.6)
            {
                y += 0.1f;
            }
        }

        transform.position = new Vector3(x, y, transform.position.z);

    }
}
