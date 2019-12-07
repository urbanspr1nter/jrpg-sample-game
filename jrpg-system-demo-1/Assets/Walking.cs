using System.Collections;
using System.Collections.Generic;
using Jrpg.CharacterSystem.StatusEffects;
using UnityEngine;

public class Walking : MonoBehaviour
{
    public Sprite alexaIdleDown;
    public Sprite alexaIdleUp;
    public Sprite alexaIdleLeft;
    public Sprite alexaIdleRight;

    public Sprite rogerIdleDown;
    public Sprite rogerIdleUp;
    public Sprite rogerIdleLeft;
    public Sprite rogerIdleRight;

    private class StateNames
    {
        public static string AlexaIdle = "AlexaIdle";
        public static string AlexaUp = "AlexaUp";
        public static string AlexaDown = "AlexaDown";
        public static string AlexaLeft = "AlexaLeft";
        public static string AlexaRight = "AlexaRight";

        public static string RogerIdle = "RogerIdle";
        public static string RogerUp = "RogerUp";
        public static string RogerDown = "RogerDown";
        public static string RogerLeft = "RogerLeft";
        public static string RogerRight = "RogerRight";
    }

    private class DirectionFlagKeys
    {
        public static string FacingUp = "FacingUp";
        public static string FacingDown = "FacingDown";
        public static string FacingLeft = "FacingLeft";
        public static string FacingRight = "FacingRight";
    }

    private SpriteRenderer SpriteRenderer;
    private float MaxSpeed;
    private Rigidbody2D RigidBody;
    private Animator Animator;
    private string PreviousState = StateNames.AlexaDown;
    private string AlexaTag;
    private string RogerTag;
    private GameParty CurrentParty;
    private bool AlexaFallen;
    private bool RogerFallen;

    private void Awake()
    {
        MaxSpeed = 1.8f;
        RigidBody = GetComponent<Rigidbody2D>();
        Animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        AlexaTag = MainGame.Store().Get<string>("AlexaTag");
        RogerTag = MainGame.Store().Get<string>("RogerTag");
        PreviousState = StateNames.AlexaDown;
        CurrentParty = GameObject.FindGameObjectWithTag("World").GetComponent<GameParty>();
    }

    private void FaceLeft()
    {
        Animator.SetBool(DirectionFlagKeys.FacingLeft, true);
        Animator.SetBool(DirectionFlagKeys.FacingUp, false);
        Animator.SetBool(DirectionFlagKeys.FacingRight, false);
        Animator.SetBool(DirectionFlagKeys.FacingDown, false);
    }

    private void FaceRight()
    {
        Animator.SetBool(DirectionFlagKeys.FacingLeft, false);
        Animator.SetBool(DirectionFlagKeys.FacingUp, false);
        Animator.SetBool(DirectionFlagKeys.FacingRight, true);
        Animator.SetBool(DirectionFlagKeys.FacingDown, false);
    }

    private void FaceUp()
    {
        Animator.SetBool(DirectionFlagKeys.FacingLeft, false);
        Animator.SetBool(DirectionFlagKeys.FacingUp, true);
        Animator.SetBool(DirectionFlagKeys.FacingRight, false);
        Animator.SetBool(DirectionFlagKeys.FacingDown, false);
    }

    private void FaceDown()
    {
        Animator.SetBool(DirectionFlagKeys.FacingLeft, false);
        Animator.SetBool(DirectionFlagKeys.FacingUp, false);
        Animator.SetBool(DirectionFlagKeys.FacingRight, false);
        Animator.SetBool(DirectionFlagKeys.FacingDown, true);
    }

    private void Idle()
    {
        Animator.SetBool(DirectionFlagKeys.FacingLeft, false);
        Animator.SetBool(DirectionFlagKeys.FacingUp, false);
        Animator.SetBool(DirectionFlagKeys.FacingRight, false);
        Animator.SetBool(DirectionFlagKeys.FacingDown, false);
    }

    private void SetPreviousState()
    {
        var currentState = Animator.GetCurrentAnimatorStateInfo(0);
        var activeCharacterName = CurrentParty.GetActiveCharacter();

        if (activeCharacterName.Equals(AlexaTag))
        {
            if (currentState.IsName(StateNames.AlexaUp))
            {
                PreviousState = StateNames.AlexaUp;
            }
            else if (currentState.IsName(StateNames.AlexaLeft))
            {
                PreviousState = StateNames.AlexaLeft;
            }
            else if (currentState.IsName(StateNames.AlexaRight))
            {
                PreviousState = StateNames.AlexaRight;
            }
            else if (currentState.IsName(StateNames.AlexaDown))
            {
                PreviousState = StateNames.AlexaDown;
            }
        }
        else if (activeCharacterName.Equals(RogerTag))
        {
            if (currentState.IsName(StateNames.RogerUp))
            {
                PreviousState = StateNames.RogerUp;
            }
            else if (currentState.IsName(StateNames.RogerLeft))
            {
                PreviousState = StateNames.RogerLeft;
            }
            else if (currentState.IsName(StateNames.RogerRight))
            {
                PreviousState = StateNames.RogerRight;
            }
            else if (currentState.IsName(StateNames.RogerDown))
            {
                PreviousState = StateNames.RogerDown;
            }
        }

        MainGame.Store().StatusEffectManager.AfterEffects();
    }

    private void UpdateVelocity()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float newHorizontal = h * MaxSpeed;
        float newVertical = v * MaxSpeed;

        RigidBody.velocity = new Vector2(newHorizontal, newVertical);
    }

    // Fun fact, override the sprite set by animator here.
    private void LateUpdate()
    {
        var currentState = Animator.GetCurrentAnimatorStateInfo(0);

        if (currentState.IsName(StateNames.AlexaIdle))
        {
            if (PreviousState.Equals(StateNames.AlexaLeft))
            {
                SpriteRenderer.sprite = alexaIdleLeft;
            }
            else if (PreviousState.Equals(StateNames.AlexaRight))
            {
                SpriteRenderer.sprite = alexaIdleRight;
            }
            else if (PreviousState.Equals(StateNames.AlexaUp))
            {
                SpriteRenderer.sprite = alexaIdleUp;
            }
            else if (PreviousState.Equals(StateNames.AlexaDown))
            {
                SpriteRenderer.sprite = alexaIdleDown;
            } else
            {
                SpriteRenderer.sprite = alexaIdleDown;
            }
        }
        else if (currentState.IsName(StateNames.RogerIdle))
        {
            if (PreviousState.Equals(StateNames.RogerLeft))
            {
                SpriteRenderer.sprite = rogerIdleLeft;
            }
            else if (PreviousState.Equals(StateNames.RogerRight))
            {
                SpriteRenderer.sprite = rogerIdleRight;
            }
            else if (PreviousState.Equals(StateNames.RogerUp))
            {
                SpriteRenderer.sprite = rogerIdleUp;
            }
            else if (PreviousState.Equals(StateNames.RogerDown))
            {
                SpriteRenderer.sprite = rogerIdleDown;
            }
            else
            {
                SpriteRenderer.sprite = rogerIdleDown;
            }
        }
    }

    private void CheckFallenEvent()
    {
        if (!AlexaFallen && MainGame.Party().GetMember(AlexaTag).Statistics[Jrpg.CharacterSystem.StatisticType.HpCurrent].CurrentValue <= 0)
        {
            MainGame.Store().StatusEffectManager.RemoveEffect(MainGame.Party().GetMember(AlexaTag), StatusEffectType.Poison);
            MainGame.Log($"{AlexaTag} has fallen!");
            AlexaFallen = true;
        }
        if (!RogerFallen && MainGame.Party().GetMember(RogerTag).Statistics[Jrpg.CharacterSystem.StatisticType.HpCurrent].CurrentValue <= 0)
        {
            MainGame.Store().StatusEffectManager.RemoveEffect(MainGame.Party().GetMember(RogerTag), StatusEffectType.Poison);
            MainGame.Log($"{RogerTag} has fainted!");
            RogerFallen = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        MainGame.Store().StatusEffectManager.BeforeEffects();

        CheckFallenEvent();

        var keyLeftPressed = Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A);
        var keyRightPressed = Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D);
        var keyUpPressed = Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W);
        var keyDownPressed = Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S);

        if (!(keyLeftPressed || keyRightPressed || keyUpPressed || keyDownPressed))
        {
            SetPreviousState();

            // No key is being held down.
            Idle();

            // Make the sprite still.
            RigidBody.velocity = new Vector2(0, 0);
            return;
        }

        if (keyLeftPressed)
        {
            FaceLeft();
        }
        else if (keyRightPressed)
        {
            FaceRight();
        }
        else if (keyUpPressed)
        {
            FaceUp();
        }
        else if (keyDownPressed)
        {
            FaceDown();
        }

        var distance = GetDistance();
        if (distance >= 2.25)
        {
            MainGame.PreviousPosition = new Vector2(RigidBody.position.x, RigidBody.position.y);
            MainGame.Store().StatusEffectManager.PerformEffects();
            AdjustHp();
        }

        UpdateVelocity();
    }

    private float GetDistance()
    {
        return Mathf.Sqrt(
            Mathf.Pow(RigidBody.position.x - MainGame.PreviousPosition.x, 2) +
            Mathf.Pow(RigidBody.position.y - MainGame.PreviousPosition.y, 2)
        );
    }

    private void AdjustHp()
    {
        if (MainGame.Store().MainParty.GetMember(AlexaTag).Statistics[Jrpg.CharacterSystem.StatisticType.HpCurrent].CurrentValue < 0)
        {
            MainGame.Store().MainParty.GetMember(AlexaTag).Statistics[Jrpg.CharacterSystem.StatisticType.HpCurrent].CurrentValue = 0;
        }
        if (MainGame.Store().MainParty.GetMember(RogerTag).Statistics[Jrpg.CharacterSystem.StatisticType.HpCurrent].CurrentValue < 0)
        {
            MainGame.Store().MainParty.GetMember(RogerTag).Statistics[Jrpg.CharacterSystem.StatisticType.HpCurrent].CurrentValue = 0;
        }
    }
}
