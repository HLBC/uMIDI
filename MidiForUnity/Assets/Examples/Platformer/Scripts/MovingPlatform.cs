using UnityEngine;
using System.Collections;
public class MovingPlatform : MonoBehaviour
{
    public Vector3 finishPos; 
    public float speed = 0.5f;

    private Vector3 _startPos;
    private float _trackPercent = 0;
    private int _direction = 1;

    private BoxCollider2D _box;

    void Start()
    {
        _startPos = transform.position;
        finishPos = new Vector3(0, transform.position.y, 0);
        _box = GetComponent<BoxCollider2D>();
    }

    bool isGrandChild(GameObject gameObject)
    {
        foreach (Transform child in transform)
        {
            foreach (Transform grandchild in child.transform)
            {
                if (GameObject.ReferenceEquals(grandchild.gameObject, gameObject))
                {
                    return true;
                }
            }
        }
        return false;
    }

    void Update()
    {
        _trackPercent += _direction * speed * Time.deltaTime;
        float x = (finishPos.x - _startPos.x) * _trackPercent + _startPos.x;
        transform.position = new Vector3(x, _startPos.y, _startPos.z);

        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        Vector2 corner1 = new Vector2(max.x, max.y + .1f);
        Vector2 corner2 = new Vector2(min.x, max.y + .2f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        if (hit != null)
        {
            GameObject gameObject = hit.GetComponent<BoxCollider2D>().gameObject;
            if (!isGrandChild(gameObject))
            {
                GameObject emptyGameObject = new GameObject();
                emptyGameObject.tag = "Empty";
                gameObject.transform.parent = emptyGameObject.transform;
                emptyGameObject.transform.parent = transform;
            }
        } else
        {
            foreach (Transform child in transform)
            {
                child.transform.parent = null;
                foreach (Transform grandchild in child.transform)
                {
                    grandchild.transform.parent = null;
                }
                if (string.Equals(child.tag, "Empty"))
                {
                    Destroy(child.gameObject);
                }
            }
        }

        if ((_direction == 1 && _trackPercent > .9f) ||
        (_direction == -1 && _trackPercent < .1f))
        {
            _direction *= -1;
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, new Vector3(0, transform.position.y, transform.position.z));
    }
}