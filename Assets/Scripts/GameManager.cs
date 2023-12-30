using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region Serislized Varialble

    [SerializeField] GameObject _whiteBall;

    [SerializeField] GameObject _Balls;

    [SerializeField] Camera _mainCamera;

    [SerializeField] Camera _topViewCamera;

    [SerializeField] Text _playerActive;

    [SerializeField] GameObject _redBallsForPlayerOne;

    [SerializeField] GameObject _blueBallsForPlayerOne;

    [SerializeField] GameObject _redBallsForPlayerTwo;

    [SerializeField] GameObject _blueBallsForPlayerTwo;

    [SerializeField] GameObject _blackBallsForPlayerOne;

    [SerializeField] GameObject _blackBallsForPlayerTwo;

    [SerializeField] GameObject[] _redBallsPlayerOneImage, _redBallsPlayerTwoImage;

    [SerializeField] GameObject[] _blueBallsPlayerOneImage, _blueBallsPlayerTwoImage;

    [SerializeField] GameObject _playerInfo;

    [SerializeField] GameObject _pauseMenu;

    [SerializeField] GameObject _scrollbar;

    [SerializeField] GameObject _MovementModeButton;

    [SerializeField] GameObject _ShootModeButton;

    [SerializeField] GameObject _CameraRestButton;

    [SerializeField] GameObject _playerOneWon;

    [SerializeField] GameObject _playerTwoWon;

    [SerializeField] GameObject _playerOneWonByBlackBall;

    [SerializeField] GameObject _playerTwoWonByBlackBall;

    #endregion

    #region Other Variables

    public float _power;
    public Camera _currentCamera;
    GameObject _whiteB, _foul, _blackB, _cameraController, _cueStick;
    GameObject[] _redB, _blueB;
    public bool _isPlayerOneActive, _isPlayerTwoActive;
    public bool _isBallsRMoving, _isWaitingForBallsMovementStop, _ballDecider, _noBallPotted,
        _playerOneForRedB, _playerOneForBlueB, _playerTwoForRedB, _playerTwoForBlueB, _isGameOver = false;

    #endregion

    private void Awake()
    {
        Instantiate(_whiteBall, _whiteBall.transform.position, Quaternion.identity);
        Instantiate(_Balls, _Balls.transform.position, Quaternion.identity);
        _isWaitingForBallsMovementStop = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        _currentCamera = _mainCamera;
        _isPlayerOneActive = true;
        _noBallPotted = false;
        _ballDecider = true;
        _whiteB = GameObject.FindGameObjectWithTag("WhiteBall");
        _foul = GameObject.FindGameObjectWithTag("Floor");
        _blackB = GameObject.FindGameObjectWithTag("BlackBall");
        _redB = GameObject.FindGameObjectsWithTag("RedBall");
        _blueB = GameObject.FindGameObjectsWithTag("BlueBall");
        _cameraController = GameObject.Find("Main Camera");
        _cueStick = GameObject.FindGameObjectWithTag("CueStick");
    }

    // Update is called once per frame
    void Update()
    {
        FoulTurnChange();
        IsBallsAreMoving();
        SessionScenario();
        PottedBallsSummary();
        ActivePlayerUI();
        BlackBallAvailable();
        RedBallsUI();
        BlueBallsUI();
    }

    void FoulTurnChange()
    {
        if (_isPlayerOneActive == true)
        {
            if (_foul.GetComponent<BallsCount>().isFoul == true)
            {
                _noBallPotted = false;
                _isPlayerOneActive = false;
                _isPlayerTwoActive = true;
                Instantiate(_whiteBall, _whiteBall.transform.position, Quaternion.identity);
                SwitchingCameras();
                _foul.GetComponent<BallsCount>().isFoul = false;
            }
        }
        else if (_isPlayerTwoActive == true)
        {
            if (_foul.GetComponent<BallsCount>().isFoul == true)
            {
                _noBallPotted = false;
                _isPlayerTwoActive = false;
                _isPlayerOneActive = true;
                Instantiate(_whiteBall, _whiteBall.transform.position, Quaternion.identity);
                SwitchingCameras();
                _foul.GetComponent<BallsCount>().isFoul = false;
            }
        }
    }

    public void BlackBallWinner()
    {
        if (_isPlayerOneActive == true)
        { 
            _playerTwoWonByBlackBall.SetActive(true);
            _isGameOver = true;
        }

        else if (_isPlayerTwoActive == true)
        { 
            _playerOneWonByBlackBall.SetActive(true);
            _isGameOver = true;
        }
    }

    public void RealWinner()
    {
        if (_isPlayerOneActive == true)
        { 
            _playerOneWon.SetActive(true);
            _isGameOver = true;
        }

        else if (_isPlayerTwoActive == true)
        { 
            _playerTwoWon.SetActive(true);
            _isGameOver = true;
        }
    }

    void IsBallsAreMoving()
    {
        if (_whiteB != null && _blackB != null)
        {
            if (_whiteB.GetComponent<Balls>()._isballMoving == false &&
               _blackB.GetComponent<Balls>()._isballMoving == false &&
               IsRedBallsMoving() == false &&
               IsBlueBallsMoving() == false)

            { _isBallsRMoving = false; }
            else { _isBallsRMoving = true; }
        }
    }

    IEnumerator SwitchingCamerasRouite()
    {
        if (_currentCamera == _topViewCamera)
        {
            yield return new WaitForSeconds(0.5f);
            _mainCamera.enabled = true;
            _topViewCamera.enabled = false;
            _currentCamera = _mainCamera;
            _playerInfo.SetActive(true);
            _cameraController.GetComponent<CameraController>().ResetCamera();
            _foul.GetComponent<BallsCount>()._redBallPotted = 0;
            _foul.GetComponent<BallsCount>()._blueBallPotted = 0;
            _cueStick.SetActive(true);
            NoBallsPotted();
            _CameraRestButton.SetActive(false);
        }
        else if (_currentCamera == _mainCamera)
        {
            yield return new WaitForSeconds(0.5f);
            _mainCamera.enabled = false;
            _topViewCamera.enabled = true;
            _currentCamera = _topViewCamera;
            _playerInfo.SetActive(false);
            _isWaitingForBallsMovementStop = true;
            _cueStick.SetActive(false);
            _noBallPotted = true;
            StartCoroutine(GameStuckRoutine());
        }
    }

    public void SwitchingCameras()
    { StartCoroutine(SwitchingCamerasRouite()); }

    bool IsRedBallsMoving()
    {
        for (int i = 0; i <= _redB.Length - 1; i++)
        {
            if (_redB[i].GetComponent<Balls>()._isballMoving == true)
            { return true; }
        }
        return false;
    }

    bool IsBlueBallsMoving()
    {
        for (int i = 0; i <= _blueB.Length - 1; i++)
        {
            if (_blueB[i].GetComponent<Balls>()._isballMoving == true)
            { return true; }
        }
        return false;
    }

    void ActivePlayerUI()
    {
        if (_isPlayerOneActive == true)
        {
            _playerActive.text = ("Player One");
        }
        if (_isPlayerTwoActive == true)
        {
            _playerActive.text = ("Player Two");
        }
    }

    void SessionScenario()
    {
        if (_isWaitingForBallsMovementStop == true)
        {
            if (_isBallsRMoving == false)
            {
                SwitchingCameras();
                _isWaitingForBallsMovementStop = false;
            }
        }
    }

    void PottedBallsSummary()
    {
        if (_ballDecider == true)
        {
            if (_foul.GetComponent<BallsCount>()._redBallPotted > 0 && _foul.GetComponent<BallsCount>()._blueBallPotted > 0)
            { _ballDecider = true; }

            if (_foul.GetComponent<BallsCount>()._redBallPotted > 0 && _foul.GetComponent<BallsCount>()._blueBallPotted == 0)
            {
                if (_isPlayerOneActive == true)
                {
                    _playerOneForRedB = true;
                    _redBallsForPlayerOne.SetActive(true);
                    _playerTwoForBlueB = true;
                    _blueBallsForPlayerTwo.SetActive(true);
                }

                if (_isPlayerTwoActive == true)
                {
                    _playerTwoForRedB = true;
                    _redBallsForPlayerTwo.SetActive(true);
                    _playerOneForBlueB = true;
                    _blueBallsForPlayerOne.SetActive(true);
                }

                _ballDecider = false;
            }

            if (_foul.GetComponent<BallsCount>()._blueBallPotted > 0 && _foul.GetComponent<BallsCount>()._redBallPotted == 0)
            {
                if (_isPlayerOneActive == true)
                {
                    _playerOneForBlueB = true;
                    _blueBallsForPlayerOne.SetActive(true);
                    _playerTwoForRedB = true;
                    _redBallsForPlayerTwo.SetActive(true);
                }

                if (_isPlayerTwoActive == true)
                {
                    _playerOneForRedB = true;
                    _redBallsForPlayerOne.SetActive(true);
                    _playerTwoForBlueB = true;
                    _blueBallsForPlayerTwo.SetActive(true);
                }
                
                _ballDecider = false;
            }
        }

        if (_ballDecider == false)
        {
            if (_playerOneForRedB == true && _playerTwoForBlueB == true)
            {
                if (_foul.GetComponent<BallsCount>()._redBallPotted > 0 && _foul.GetComponent<BallsCount>()._blueBallPotted > 0)
                { return; }

                if (_foul.GetComponent<BallsCount>()._redBallPotted > 0 && _foul.GetComponent<BallsCount>()._blueBallPotted == 0)
                {
                    if (_isPlayerOneActive == true)
                    { return; }

                    if (_isPlayerTwoActive == true)
                    {
                        //SwappingPlayer(); 
                        _isPlayerTwoActive = false;
                        _isPlayerOneActive = true;
                    }
                }

                if (_foul.GetComponent<BallsCount>()._blueBallPotted > 0 && _foul.GetComponent<BallsCount>()._redBallPotted == 0)
                {
                    if (_isPlayerOneActive == true)
                    {
                        //SwappingPlayer(); 
                        _isPlayerOneActive = false;
                        _isPlayerTwoActive = true;
                    }

                    if (_isPlayerTwoActive == true)
                    { return; }
                }
            }

            if (_playerOneForBlueB == true && _playerTwoForRedB == true)
            {
                if (_foul.GetComponent<BallsCount>()._redBallPotted > 0 && _foul.GetComponent<BallsCount>()._blueBallPotted > 0)
                { return; }

                if (_foul.GetComponent<BallsCount>()._blueBallPotted > 0 && _foul.GetComponent<BallsCount>()._redBallPotted == 0)
                {
                    if (_isPlayerOneActive == true)
                    { return; }

                    if (_isPlayerTwoActive == true)
                    {
                        //SwappingPlayer();
                        _isPlayerTwoActive = false;
                        _isPlayerOneActive = true;
                    }
                }

                if (_foul.GetComponent<BallsCount>()._redBallPotted > 0 && _foul.GetComponent<BallsCount>()._blueBallPotted == 0)
                {
                    if (_isPlayerOneActive == true)
                    {
                        //SwappingPlayer(); 
                        _isPlayerOneActive = false;
                        _isPlayerTwoActive = true;
                    }

                    if (_isPlayerTwoActive == true)
                    { return; }
                }
            }
        }
    }

    void NoBallsPotted()
    {
        if (_foul.GetComponent<BallsCount>()._redBallPotted == 0 && _foul.GetComponent<BallsCount>()._blueBallPotted == 0)
        {
            if (_noBallPotted == true)
            {
                if (_isPlayerOneActive == true)
                {
                    _isPlayerOneActive = false;
                    _isPlayerTwoActive = true;
                    _noBallPotted = false;
                }
            }

            if (_noBallPotted == true)
            {
                if (_isPlayerTwoActive == true)
                {
                    _isPlayerTwoActive = false;
                    _isPlayerOneActive = true;
                    _noBallPotted = false;
                }
            }
        }
    }

    void BlackBallAvailable()
    {
        if (_foul.GetComponent<BallsCount>()._redballs == 0)
        {
            if (_playerOneForRedB == true)
            {

                _redBallsForPlayerOne.SetActive(false);
                _blackBallsForPlayerOne.SetActive(true);
                //_blackBallsForPlayerTwo.SetActive(false);
            }

            if (_playerTwoForRedB == true)
            {
                _redBallsForPlayerTwo.SetActive(false);
                _blackBallsForPlayerTwo.SetActive(true);
                //_blackBallsForPlayerOne.SetActive(false);
            }
        }

        if (_foul.GetComponent<BallsCount>()._blueballs == 0)
        {
            if (_playerOneForBlueB == true)
            {
                _blueBallsForPlayerOne.SetActive(false);
                _blackBallsForPlayerOne.SetActive(true);
                //_blackBallsForPlayerTwo.SetActive(false);
            }

            if (_playerTwoForBlueB == true)
            {
                _blueBallsForPlayerTwo.SetActive(false);
                _blackBallsForPlayerTwo.SetActive(true);
                //_blackBallsForPlayerOne.SetActive(false);
            }
        }

    }

    void RedBallsUI()
    {
        switch (_foul.GetComponent<BallsCount>()._redballs)
        {
            case 6:
                _redBallsPlayerOneImage[0].SetActive(false);
                _redBallsPlayerTwoImage[0].SetActive(false);
                break;

            case 5:
                _redBallsPlayerOneImage[1].SetActive(false);
                _redBallsPlayerTwoImage[1].SetActive(false);
                break;

            case 4:
                _redBallsPlayerOneImage[2].SetActive(false);
                _redBallsPlayerTwoImage[2].SetActive(false);
                break;

            case 3:
                _redBallsPlayerOneImage[3].SetActive(false);
                _redBallsPlayerTwoImage[3].SetActive(false);
                break;

            case 2:
                _redBallsPlayerOneImage[4].SetActive(false);
                _redBallsPlayerTwoImage[4].SetActive(false);
                break;

            case 1:
                _redBallsPlayerOneImage[5].SetActive(false);
                _redBallsPlayerTwoImage[5].SetActive(false);
                break;

            case 0:
                _redBallsPlayerOneImage[6].SetActive(false);
                _redBallsPlayerTwoImage[6].SetActive(false);
                break;
        }
    }

    void BlueBallsUI()
    {
        switch (_foul.GetComponent<BallsCount>()._blueballs)
        {
            case 6:
                _blueBallsPlayerOneImage[0].SetActive(false);
                _blueBallsPlayerTwoImage[0].SetActive(false);
                break;

            case 5:
                _blueBallsPlayerOneImage[1].SetActive(false);
                _blueBallsPlayerTwoImage[1].SetActive(false);
                break;

            case 4:
                _blueBallsPlayerOneImage[2].SetActive(false);
                _blueBallsPlayerTwoImage[2].SetActive(false);
                break;

            case 3:
                _blueBallsPlayerOneImage[3].SetActive(false);
                _blueBallsPlayerTwoImage[3].SetActive(false);
                break;

            case 2:
                _blueBallsPlayerOneImage[4].SetActive(false);
                _blueBallsPlayerTwoImage[4].SetActive(false);
                break;

            case 1:
                _blueBallsPlayerOneImage[5].SetActive(false);
                _blueBallsPlayerTwoImage[5].SetActive(false);
                break;

            case 0:
                _blueBallsPlayerOneImage[6].SetActive(false);
                _blueBallsPlayerTwoImage[6].SetActive(false);
                break;
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0.0f;
        _pauseMenu.SetActive(true);
        _scrollbar.SetActive(false);
        _MovementModeButton.SetActive(false);
        _ShootModeButton.SetActive(false);
        _currentCamera = _topViewCamera;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1.0f;
        _pauseMenu.SetActive(false);
        _scrollbar.SetActive(true);
        _MovementModeButton.SetActive(true);
        _ShootModeButton.SetActive(true);
        StartCoroutine(ResumeGameRoutine());
    }

    IEnumerator ResumeGameRoutine()
    {
        yield return new WaitForSeconds(0.25f);
        _currentCamera = _mainCamera;
    }

    public void GoToMainMenu()
    { SceneManager.LoadScene("MainMenu");    }

    public void QuitGame()
    {      Application.Quit();    }

    public void CameraRestButton()
    { SwitchingCameras(); }

    IEnumerator GameStuckRoutine()
    {
        if (_currentCamera == _topViewCamera)
        {
            yield return new WaitForSeconds(10.0f);
            _CameraRestButton.SetActive(true);
        }
    }

    public void IsGameOver()
    {
        if (_isGameOver == true)
        { SceneManager.LoadScene("MainMenu"); }
    }
}