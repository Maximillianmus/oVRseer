using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

public class RigidbodyControllerNPC : MonoBehaviour
{
	[Header("NPC")]
	[Tooltip("The character rigidbody")]
	public Rigidbody NPCRigidbody;
	[Tooltip("The orientation of the character")]
	public Transform Orientation;
	[Tooltip("Move speed of the character in m/s")]
	public float MoveSpeed = 2.0f;
	[Tooltip("Sprint speed of the character in m/s")]
	public float SprintSpeed = 5.335f;
	[Tooltip("How fast the character turns to face movement direction")]
	[Range(0.0f, 0.3f)]
	public float RotationSmoothTime = 0.12f;
	[Tooltip("Acceleration and deceleration")]
	public float SpeedChangeRate = 10.0f;

	[Space(10)]
	[Tooltip("The height the player can jump")]
	public float JumpHeight = 1.2f;
	[Space(10)]
	[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
	public float JumpTimeout = 0.50f;
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	public float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	public bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	public float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	public float GroundedRadius = 0.28f;
	[Tooltip("What layers the character uses as ground")]
	public LayerMask GroundLayers;

	// player
	private float _speed;
	private float _animationBlend;
	private float _targetRotation = 0.0f;
	private float _rotationVelocity;
	private Vector3 _normalVector = Vector3.up;

	// timeout deltatime
	private float _jumpTimeoutDelta;
	private float _fallTimeoutDelta;

	// animation IDs
	private int _animIDSpeed;
	private int _animIDGrounded;
	private int _animIDJump;
	private int _animIDFreeFall;
	private int _animIDMotionSpeed;

	public Animator _animator;
	private bool _hasAnimator;

	private const float _threshold = 0.01f;

	private void Start()
	{
		_hasAnimator = TryGetComponent(out _animator);

		AssignAnimationIDs();

		// reset our timeouts on start
		_jumpTimeoutDelta = JumpTimeout;
		_fallTimeoutDelta = FallTimeout;
	}

	public void MoveNPC(Vector3 input)
	{
		_hasAnimator = TryGetComponent(out _animator);

		//JumpAndGravity();
		GroundedCheck();
		Move(input);
	}

	private void AssignAnimationIDs()
	{
		_animIDSpeed = Animator.StringToHash("Speed");
		_animIDGrounded = Animator.StringToHash("Grounded");
		_animIDJump = Animator.StringToHash("Jump");
		_animIDFreeFall = Animator.StringToHash("FreeFall");
		_animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
	}



	//probably ok
	private void GroundedCheck()
	{
		// set sphere position, with offset
		Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);

		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetBool(_animIDGrounded, Grounded);
		}
	}


	//not ok
	private void Move(Vector3 input)
	{
		// set target speed based on move speed, sprint speed and if sprint is pressed
		float targetSpeed = MoveSpeed;

		float inputMagnitude = 1f;

		// normalise input direction
		Vector3 inputDirection = input.normalized;


		// note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is no input, set the target speed to 0
		//maybe remove
		if (input == Vector3.zero) targetSpeed = 0.0f;

		// a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon



		// a reference to the players current horizontal velocity
		float currentHorizontalSpeed = new Vector3(NPCRigidbody.velocity.x, 0.0f, NPCRigidbody.velocity.z).magnitude;

		float speedOffset = 0.1f;
		// accelerate or decelerate to target speed
		if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
		{
			// creates curved result rather than a linear one giving a more organic speed change
			// note T in Lerp is clamped, so we don't need to clamp our speed
			_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

			// round speed to 3 decimal places
			_speed = Mathf.Round(_speed * 1000f) / 1000f;
		}
		else
		{
			_speed = targetSpeed;
		}

		_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);




		// note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
		// if there is a move input rotate player when the player is moving
		if (input != Vector3.zero)
		{
			_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg;
			float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSmoothTime);

			// rotate to face input direction relative to camera position
			transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
		}

		Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;
		Orientation.forward = targetDirection;


		//Debug.DrawLine(transform.position, transform.position + forward * 10 * 2, Color.blue);

		// move the player
		//print(targetDirection.normalized);
		NPCRigidbody.velocity = (targetDirection.normalized * (_speed) + new Vector3(0.0f, NPCRigidbody.velocity.y, 0.0f));

		// update animator if using character
		if (_hasAnimator)
		{
			_animator.SetFloat(_animIDSpeed, _animationBlend);
			_animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
		}
	}

	/*
	private void CounterMovement(float x, float y, Vector2 mag, float moveSpeed)
	{
		if (!Grounded || _jumpTimeoutDelta == JumpTimeout) return;
		float threshold = 0.01f, counterMovement = 0.175f;

		//Counter movement
		if (Mathf.Abs(mag.x) > threshold && Mathf.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
		{
			PlayerRigidbody.AddForce(moveSpeed * Orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
		}
		if (Mathf.Abs(mag.y) > threshold && Mathf.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
		{
			PlayerRigidbody.AddForce(moveSpeed * Orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
		}
	}

	*/

	/*

	private void Jump()
	{
		if (Grounded && readyToJump)
		{
			readyToJump = false;

			//Add jump forces
			rb.AddForce(Vector2.up * jumpForce * 1.5f);
			rb.AddForce(normalVector * jumpForce * 0.5f);

			//If jumping while falling, reset y velocity.
			Vector3 vel = rb.velocity;
			if (rb.velocity.y < 0.5f)
				rb.velocity = new Vector3(vel.x, 0, vel.z);
			else if (rb.velocity.y > 0)
				rb.velocity = new Vector3(vel.x, vel.y / 2, vel.z);

			Invoke(nameof(ResetJump), jumpCooldown);
		}
	}

	*/

	/*
	//not ok
	private void JumpAndGravity()
	{
		if (Grounded)
		{
			// reset the fall timeout timer
			_fallTimeoutDelta = FallTimeout;

			// update animator if using character
			if (_hasAnimator)
			{
				_animator.SetBool(_animIDJump, false);
				_animator.SetBool(_animIDFreeFall, false);
			}

			// stop our velocity dropping infinitely when grounded
			//if (_verticalVelocity < 0.0f)
			//{
			//	_verticalVelocity = -2f;
			//}

			// Jump
			if (_input.jump && _jumpTimeoutDelta <= 0.0f)
			{
				// the square root of H * -2 * G = how much velocity needed to reach desired height
				//_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

				PlayerRigidbody.AddForce(Vector2.up * JumpHeight * 1.5f);
				PlayerRigidbody.AddForce(_normalVector * JumpHeight * 0.5f);


				// update animator if using character
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDJump, true);
				}
			}

			// jump timeout
			if (_jumpTimeoutDelta >= 0.0f)
			{
				_jumpTimeoutDelta -= Time.deltaTime;
			}
		}
		else
		{
			// reset the jump timeout timer
			_jumpTimeoutDelta = JumpTimeout;

			// fall timeout
			if (_fallTimeoutDelta >= 0.0f)
			{
				_fallTimeoutDelta -= Time.deltaTime;
			}
			else
			{
				// update animator if using character
				if (_hasAnimator)
				{
					_animator.SetBool(_animIDFreeFall, true);
				}
			}

			// if we are not grounded, do not jump
			_input.jump = false;
		}

		PlayerRigidbody.AddForce(Vector3.down * Time.deltaTime * 10);

		// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		//if (_verticalVelocity < _terminalVelocity)
		//{
		//	_verticalVelocity += Gravity * Time.deltaTime;
		//}
	}

	*/

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
	}
}