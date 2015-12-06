using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

    private Board board;

    void Awake()
    {
        board = GameObject.Find("Board").GetComponent<Board>();
    }

    void Update()
    {
        // Camera zoom
        if (Input.GetAxis("Mouse ScrollWheel") < 0) // Mousewheeldown -> Zoom out
        {
            Camera.main.orthographicSize += 1;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0) // Mousewheelup -> Zoom in
        {
            if (Camera.main.orthographicSize - 1 > 0) // Prevents player from zooming orthographic size to a negative number
            {
                Camera.main.orthographicSize -= 1;
            }
        }

        // Camera drag using arrow + WASD keys
        float vertical = Input.GetAxis("Vertical");
        float horizontal = Input.GetAxis("Horizontal");
        
        if (vertical != 0 || horizontal != 0)
        {
            Vector3 distToMove = new Vector3(horizontal / Camera.main.orthographicSize, vertical / Camera.main.orthographicSize);

            transform.Translate(distToMove, Space.World);
        }
    }

	// Camera initialization
	public void SetCamera()
    {
		Camera.main.transform.position = new Vector3((board.state.Size / 2) + ((board.state.Size / 2) * board.Offset), (board.state.Size / 2) + ((board.state.Size / 2) * board.Offset), -10);
		Camera.main.orthographicSize = (board.state.Size / 2) + (((board.state.Size / 2) - 1) * board.Offset);
	}
}
