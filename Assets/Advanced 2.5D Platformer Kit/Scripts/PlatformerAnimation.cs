using UnityEngine;
using System.Collections;

public class PlatformerAnimation : MonoBehaviour
{
	public Transform animatedPlayerModel; //Animated model that will have all the animations in it
	bool mPlayerDead = false;
    bool mIdle = false;

	void Start () 
	{
		//Do some error checks first
		if (animatedPlayerModel == null)
		{
			Debug.LogError("The animated player model is not set.");
			this.enabled = false;
		}
		else if (!CheckAnims())
		{
			Debug.LogError("The animated player model does not seem to have the appropriate animations needed.");
			this.enabled = false;
		}
		else
		{
			//no errors
			animatedPlayerModel.animation["idle"].speed = 0;
		}
	}

	bool CheckAnims()
	{
		if (!animatedPlayerModel)
			return false;

		if (animatedPlayerModel.animation["walk"] == null ||
			animatedPlayerModel.animation["jump"] == null ||
			animatedPlayerModel.animation["slidein"] == null ||
			animatedPlayerModel.animation["slideout"] == null ||
			animatedPlayerModel.animation["death"] == null ||
			animatedPlayerModel.animation["onwall"] == null ||
			animatedPlayerModel.animation["idle"] == null) return false;

		return true;
	}

	void Update () 
	{
		//recalculate walking speed
		float walkingSpeed = Mathf.Abs(rigidbody.velocity.x)*0.075f;
		animatedPlayerModel.animation["walk"].speed = walkingSpeed;

		//switch to idle animation if needed
		if (walkingSpeed == 0 && animatedPlayerModel.animation["walk"].enabled)
		{
			animatedPlayerModel.animation.Play("idle");
            mIdle = true;
		}

        if (walkingSpeed > 0.01f && mIdle)
		{
            mIdle = false;
			animatedPlayerModel.animation.CrossFade("walk");
		}
	}

	void PlayAnim(string animName)
	{
		if (!mPlayerDead)
		{
			animatedPlayerModel.animation.Play(animName);
			animatedPlayerModel.transform.localPosition = Vector3.zero; //reset any position change made by on wall anim
		}
	}

	void GoLeft()
	{
		Vector3 localScale = animatedPlayerModel.transform.localScale;
		localScale.z = -Mathf.Abs(localScale.z);
		animatedPlayerModel.transform.localScale = localScale;
	}

	void GoRight()
	{
		Vector3 localScale = animatedPlayerModel.transform.localScale;
		localScale.z = Mathf.Abs(localScale.z);
		animatedPlayerModel.transform.localScale = localScale;
	}

	public void PlayerDied()
	{
        PlayAnim("death");
		mPlayerDead = true;
	}

	public void PlayerLives()
	{
		GoRight();
		mPlayerDead = false;
        PlayAnim("walk");
	}





	//MESSAGES CALLED BY PlatformerPhysics.cs:
	void StartedJump()
	{
        PlayAnim("jump");
	}

	void StartedWallJump()
	{
        PlayAnim("jump");
	}

	void StartedCrouching()
	{
        PlayAnim("slidein");
	}

	void StoppedCrouching()
	{
        PlayAnim("slideout");

		if (GetComponent<PlatformerPhysics>().IsOnWall())
			LandedOnWall();
		else
			animatedPlayerModel.animation.CrossFade("walk", 2.0f);
	}

	void LandedOnGround()
	{
		if (!GetComponent<PlatformerPhysics>().IsCrouching())
		{
            PlayAnim("walk");
		}
	}

	void LandedOnWall()
	{
        if (!GetComponent<PlatformerPhysics>().IsCrouching())
        {
            PlayAnim("onwall");

            if (!GetComponent<PlatformerPhysics>().IsWallOnRightSide())
            {
                animatedPlayerModel.transform.localPosition = new Vector3(0.45f, 0, 0);
                GoLeft();
            }
            else
            {
                animatedPlayerModel.transform.localPosition = new Vector3(-0.45f, 0, 0);
                GoRight();
            }
        }
	}

	void ReleasedWall()
	{
		print("released");
		if (!animatedPlayerModel.animation["jump"].enabled && !GetComponent<PlatformerPhysics>().IsCrouching())
            PlayAnim("walk");
	}

	void StartedSprinting()
	{
		//print("Start Sprint");
	}

	void StoppedSprinting()
	{
		//print("Stop Sprint");
	}
}

