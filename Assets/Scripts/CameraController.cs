using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    #region Serialize Variables

    [SerializeField] float _speed;
    [SerializeField] Vector3 _offset;
    [SerializeField] float _force;
    [SerializeField] float downAngle;
    [SerializeField] float _maxDownDrag;
    [SerializeField] Text _powerIndicatorText;
    [SerializeField] Scrollbar _scrollbar;

    #endregion

    #region Other Variables

    bool _movementMode, _shootMode;
    Transform _target;
    GameObject ball, _gameManager;
    float _horizontalInput, _horizontalTouchInput; public float _verticalTouchInput;
    bool _isShooting = false;
    Touch _startPos;
    public int _pixleDistToDetect;
    bool _fingerDown = false;
    public float temp;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _gameManager = GameObject.Find("GameManager");
        FindWhiteBall();
        ResetCamera();
        _movementMode = true;
        _shootMode = false;
    }

    // Update is called once per frame
    void Update()
    {
        FindWhiteBall();
        Movement();
        ShootWhiteBall();TouchInput();
        SliderPositions();
        TouchInput();
    }

    void FindWhiteBall()
    {
        ball = GameObject.FindGameObjectWithTag("WhiteBall");
        if (ball != null)
        {
            _target = ball.transform;
        }
        else
        { Debug.LogError("ball is not found"); }
    }

    public void Movement()
    {
        if (_movementMode == true)
        {
            _horizontalInput = _horizontalTouchInput * _speed * Time.deltaTime;
            transform.RotateAround(_target.position, Vector3.up, _horizontalInput);
        }
    }

    public void ResetCamera()
    {
        transform.position = _target.position + _offset;
        transform.LookAt(_target.position);
        transform.localEulerAngles = new Vector3(downAngle, transform.localEulerAngles.y, 0);
    }

    public void ShootWhiteBall()
    {
        if (gameObject.GetComponent<Camera>().enabled && _shootMode == true)
        {
            if (_isShooting == true)
            {
                if (_verticalTouchInput >= 0)
                {
                    if (_verticalTouchInput >= _maxDownDrag)
                    { _verticalTouchInput = _maxDownDrag; }
                }

                float powerIndicator = Mathf.RoundToInt((_verticalTouchInput / _maxDownDrag) * 100);
                _powerIndicatorText.text = ("Power = " + powerIndicator + "%");
            }
        }
    }

    void TouchInput()
    {
        foreach (Touch touch in Input.touches)
        {
            if (_fingerDown == false && touch.phase == TouchPhase.Began)
            {
                _startPos = touch;
                _fingerDown = true;
                _isShooting = true;
                _verticalTouchInput = 0.0f;
            }

            if (_fingerDown == true)
            {
                if (touch.phase == TouchPhase.Stationary)
                {
                    _horizontalTouchInput = 0.0f;
                }

                if (touch.phase == TouchPhase.Moved)
                {
                    if (_shootMode == true)
                    { _verticalTouchInput = ((_startPos.position.y - touch.position.y) * temp); }

                    if (_movementMode == true)
                    {
                        float deltaY = ((_startPos.position.x - touch.position.x));
                        //Debug.Log(deltaY);
                        //_horizontalTouchInput = deltaY;

                        if (deltaY > _pixleDistToDetect)
                        { _horizontalTouchInput = +1.0f; }
                        else if (deltaY < _pixleDistToDetect)
                        { _horizontalTouchInput = -1.0f; }
                        //StartCoroutine(TouchInputRouite());
                    }
                }

                if (touch.phase == TouchPhase.Ended)
                { 
                    _horizontalTouchInput = 0.0f; 

                    if (_shootMode == true)
                    {
                        Vector3 hitDirection = transform.forward;
                        hitDirection = new Vector3(hitDirection.x, 0, hitDirection.z).normalized;
                        ball.GetComponent<Rigidbody>().AddForce(hitDirection * Mathf.Abs(_verticalTouchInput) * _force, ForceMode.Impulse);
                        _gameManager.GetComponent<GameManager>().SwitchingCameras();
                        _isShooting = false;
                        _shootMode = false;
                        _movementMode = true;
                    }

                    _fingerDown = false;
                }
            }
        }
    }

    void SliderPositions()
    {
        if (_movementMode == true)
        { _scrollbar.value = 0.0f; }

        if (_shootMode == true)
        { _scrollbar.value = 1.0f; }
    }

    public void MovementModeActive()
    {
        _movementMode = true;
        _shootMode = false;
    }

    public void ShootModeActive()
    {
        StartCoroutine(ShootButtonActiveRouite());
    }

    IEnumerator ShootButtonActiveRouite()
    {
        yield return new WaitForSeconds(0.5f);
        _shootMode = true;
        _movementMode = false;
    }
}
