using Sandbox;

public sealed class CameraController : Component
{
	[Property] private float moveSpeed = 200.0f; // Speed of the camera movement

	protected override void OnUpdate()
	{
		// Ensure the component is active
		if ( this.GameObject == null || !this.GameObject.IsValid() )
			return;

		// Get input values
		float forward = Input.Down("Forward") ? 1.0f : (Input.Down("Backward") ? -1.0f : 0.0f);
		float right = Input.Down("Right") ? 1.0f : (Input.Down("Left") ? -1.0f : 0.0f);

		// Calculate movement delta
		Vector3 moveDelta = new Vector3(right, forward, 0) * moveSpeed * Time.Delta;

		// Update the camera's position
		this.GameObject.WorldPosition += moveDelta;
	}
}
