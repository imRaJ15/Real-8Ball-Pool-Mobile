using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallsCount : MonoBehaviour
{
    #region Variables

    public int _redballs = 7, _blueballs = 7, _redBallPotted, _blueBallPotted;
    GameObject _gm;
    public bool isFoul, _isBlackBallAvailable;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _gm = GameObject.Find("GameManager");
        isFoul = false;
        _isBlackBallAvailable = false;
    }

    // Update is called once per frame
    void Update()
    {
        IsBlackBallAvailable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "RedBall")
        { 
            _redballs--;
            _redBallPotted++;
        }

        if (other.tag == "BlueBall")
        { 
            _blueballs--;
            _blueBallPotted++;
        }

        if (other.tag == "WhiteBall")
        { isFoul = true; }

        if (other.tag == "BlackBall")
        {
            if (_isBlackBallAvailable == false)
            { _gm.GetComponent<GameManager>().BlackBallWinner(); }

            if (_isBlackBallAvailable == true)
            { _gm.GetComponent<GameManager>().RealWinner(); }
        }
    }

    void IsBlackBallAvailable()
     {
        if (_redballs == 0)
        { _isBlackBallAvailable = true; }
        else _isBlackBallAvailable = false;

        if (_blueballs == 0)
        { _isBlackBallAvailable = true; }
        else _isBlackBallAvailable = false;
    }
}
