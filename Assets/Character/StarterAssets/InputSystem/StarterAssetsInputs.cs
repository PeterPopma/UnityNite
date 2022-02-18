using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool aim;
		public bool grenade;
		public bool toggleWeapon;
		public bool shoot;
		public bool stairs;
		public bool wall;
		public bool floor;
		public bool dance;

		[Header("Movement Settings")]
		public bool analogMovement;

#if !UNITY_IOS || !UNITY_ANDROID
		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;
#endif

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnAim(InputValue value)
		{
			AimInput(value.isPressed);
		}

		public void OnShoot(InputValue value)
		{
			ShootInput(value.isPressed);
		}

		public void OnGrenade(InputValue value)
		{
			GrenadeInput(value.isPressed);
		}
		public void OnToggleWeapon(InputValue value)
		{
			ToggleWeaponInput(value.isPressed);
		}

		public void OnStairs(InputValue value)
		{
			StairsInput(value.isPressed);
		}

		public void OnWall(InputValue value)
		{
			WallInput(value.isPressed);
		}

		public void OnFloor(InputValue value)
		{
			FloorInput(value.isPressed);
		}

		public void OnDance(InputValue value)
		{
			DanceInput(value.isPressed);
		}

#else
	// old input sys if we do decide to have it (most likely wont)...
#endif


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}
		public void AimInput(bool newAimState)
		{
			aim = newAimState;
		}

		public void ShootInput(bool newShootState)
		{
			shoot = newShootState;
		}

		public void GrenadeInput(bool newGrenadeState)
		{
			grenade = newGrenadeState;
		}

		public void ToggleWeaponInput(bool newToggleWeaponState)
		{
			toggleWeapon = newToggleWeaponState;
		}

		public void StairsInput(bool newStairs)
		{
			stairs = newStairs;
		}

		public void WallInput(bool newWall)
		{
			wall = newWall;
		}

		public void FloorInput(bool newFloor)
		{
			floor = newFloor;
		}

		public void DanceInput(bool newDance)
		{
			dance = newDance;
		}

#if !UNITY_IOS || !UNITY_ANDROID

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

#endif

	}
	
}