using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Balls : MonoBehaviour
{
    #region Serialized varialble

    [SerializeField] bool _isWhiteBall;
    [SerializeField] bool _isBlackBall;
    [SerializeField] bool _isRedBall;
    [SerializeField] bool _isBlueBall;

    #endregion

    #region Other Variable

    public bool _isballMoving = false;
    public float force;
    Rigidbody _body;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _body = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        DestoryBallifPotted();
        StartCoroutine(BallMovementCheck());
    }

    private void FixedUpdate()
    {
        if (_body != null)
        {
            if (_body.velocity.y > 0)
            {
                Vector3 newVelocity = _body.velocity;
                newVelocity.y = 0f;
                _body.velocity = newVelocity;
            }
        }
    }

    void DestoryBallifPotted()
    {
        if (transform.position.y < -1.0f)
        {
            if (_isWhiteBall == true)
            { Destroy(this.gameObject); }
            else
            {
                Destroy(GetComponent<Rigidbody>());
                Destroy(GetComponent<SphereCollider>());
                GetComponent<MeshRenderer>().enabled = false;
            }
        }
    }

    IEnumerator BallMovementCheck()
    {
        Vector3 currentPosi = transform.position;

        yield return new WaitForSeconds(1f);

        Vector3 newPosi = transform.position;

        if (currentPosi == newPosi)
        {
            _isballMoving = false;
        }
        
        if (currentPosi != newPosi)
        {
            _isballMoving = true;
        }
    }
}
