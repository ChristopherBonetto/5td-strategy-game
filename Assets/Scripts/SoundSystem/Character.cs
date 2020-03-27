using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private bool m_canMove = false;

    Rigidbody2D rb;

    [SerializeField] private float m_playerSpeed;
    private bool m_isFacingRight = false;
    private float m_horizontalValue;

    [SerializeField] private float m_jumpForce = 5;
    private bool m_isGrounded = true;
    public bool IsGrounded
    {
        get
        {
            return m_isGrounded;
        }
        set
        {
            if (!m_isGrounded && value == true)
            {
                m_endJumpEvent.Play();
            }
            m_isGrounded = value;
        }
    }
    [SerializeField] private Transform m_feetPosition;
    [SerializeField] private float m_feetDetectRadius = 0.25f;
    [SerializeField] private LayerMask m_groundLayer;

    [SerializeField] private float m_inAirTime;
    private float m_jumpTimeCounter;
    private bool m_isJumping;

    private float m_timer;

    private Animator m_animator;

    #region Sound Events

    [UnityEngine.SerializeField, EventRef] private string m_footstepPath;
    private CustomEvent m_footstepsEvent;

    [UnityEngine.SerializeField, EventRef] private string m_startJumpPath;
    private CustomEvent m_startJumpEvent;

    [UnityEngine.SerializeField, EventRef] private string m_endJumpPath;
    private CustomEvent m_endJumpEvent;

    [UnityEngine.SerializeField, EventRef] private string m_waterMovPath;
    private CustomEvent m_waterMovEvent;

    [UnityEngine.SerializeField, EventRef] private string m_enterWaterPath;
    private CustomEvent m_enterWaterEvent;

    #endregion

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
        m_animator = gameObject.GetComponent<Animator>();
    }

    private void Update()
    {
        if (m_canMove)
        {
            PlayerMove();
            Jump();
        }
    }

    private void OnEnable()
    {
        HFEventManager.SubscribeTo<bool>(HFEventID.OnFinishedLoadEvents, TakeEvents);
    }
    private void OnDisable()
    {
        HFEventManager.UnsubscribeFrom<bool>(HFEventID.OnFinishedLoadEvents, TakeEvents);
    }

    public void TakeEvents(bool inValue)
    {
        if (inValue)
        {
            m_canMove = true;

            SoundManager tempIstance = SoundManager.Instance;

            m_footstepsEvent = tempIstance.GetEventFromDictionaryPath(m_footstepPath);
            m_startJumpEvent = tempIstance.GetEventFromDictionaryPath(m_startJumpPath);
            m_endJumpEvent = tempIstance.GetEventFromDictionaryPath(m_endJumpPath);
            m_waterMovEvent = tempIstance.GetEventFromDictionaryPath(m_waterMovPath);
            m_enterWaterEvent = tempIstance.GetEventFromDictionaryPath(m_enterWaterPath);
        }
    }

    //method to move the player
    void PlayerMove()
    {
        m_horizontalValue = Input.GetAxis("Horizontal");

        m_animator.SetFloat("speed", Mathf.Abs(m_horizontalValue));



        if (m_horizontalValue < 0.0f && m_isFacingRight == false)
        {
            FlipPlayer();
        }
        else if (m_horizontalValue > 0.0f && m_isFacingRight == true)
        {
            FlipPlayer();
        }

        rb.velocity = new Vector2(m_horizontalValue * m_playerSpeed, rb.velocity.y);
    }

    void Jump()
    {
        IsGrounded = Physics2D.OverlapCircle(new Vector2(m_feetPosition.transform.position.x, m_feetPosition.transform.position.y), m_feetDetectRadius, m_groundLayer);

        if (IsGrounded == true && Input.GetKeyDown(KeyCode.Space))
        {
            m_isJumping = true;
            m_jumpTimeCounter = m_inAirTime;
            rb.velocity = new Vector2(rb.velocity.x, m_jumpForce);

            m_startJumpEvent.Play();
        }
        if (m_isJumping == true && Input.GetKey(KeyCode.Space))
        {
            if (m_jumpTimeCounter > 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, m_jumpForce);
                m_jumpTimeCounter -= Time.deltaTime;
            }

            else if (m_jumpTimeCounter < 0)
            {
                m_isJumping = false;
            }
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            m_isJumping = false;
        }
    }

    //method to flip the player mesh 
    void FlipPlayer()
    {
        m_isFacingRight = !m_isFacingRight;
        Vector2 localScale = gameObject.transform.localScale;
        localScale.x *= -1;
        transform.localScale = localScale;
    }

    //Simple timer
    private bool Timer(float destinationTime)
    {
        m_timer += Time.deltaTime;
        if (m_timer >= destinationTime)
        {
            m_timer = 0;
            return true;
        }
        return false;
    }

    public void PlayFootstepsSound()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 0.8f, m_groundLayer);

        if(hit.transform != null)
        {
            if (hit.transform.tag != null)
            {
                DetectSoundFromTag(hit.transform.tag).Play();
            }
        }
        
    }

    public CustomEvent DetectSoundFromTag(string inTag)
    {
        switch (inTag)
        {
            case "Grass":
                if (CheckValidEvent(m_footstepsEvent))
                {
                    m_footstepsEvent.ChangeParamFromName("surface", 1);
                    return m_footstepsEvent;
                }
                break;

            case "Dirty":
                if (CheckValidEvent(m_footstepsEvent))
                {
                    m_footstepsEvent.ChangeParamFromName("surface",0);
                    return m_footstepsEvent;
                }
                break;

            case "Water":
                if (CheckValidEvent(m_waterMovEvent))
                {
                    return m_waterMovEvent;
                }
                break;

            default:
                return null;
                break;
        }
        return null;
    }

    public void SplashSound()
    {
        m_enterWaterEvent.Play();
    }

    public bool CheckValidEvent(CustomEvent inEvent)
    {
        return inEvent != null;
    }

    public void SoundEvent(CustomEvent inEvent)
    {
        if(inEvent != null)
        {
            inEvent.Play();
        }
    }

    public bool CheckEvent(CustomEvent inEvent)
    {
        return inEvent.CheckForErrors();
    }
    
}
