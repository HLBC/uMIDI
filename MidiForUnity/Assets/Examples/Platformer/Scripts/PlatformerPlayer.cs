using UnityEngine;
using System.Collections;

public class PlatformerPlayer : MonoBehaviour
{
    public float speed = 250f;

    [SerializeField] private AudioClip eatingSound;
    [SerializeField] private AudioClip celebrationSound;

    private Rigidbody2D _body;
    private Animator _anim;
    private BoxCollider2D _box;
    private AudioSource _soundSource;
    public float jumpForce = 12f;

    void Start()
    {
        _body = GetComponent<Rigidbody2D>();
        _anim = GetComponent<Animator>();
        _box = GetComponent<BoxCollider2D>();
        _soundSource = GetComponent<AudioSource>();

    }

    void Update()
    {
        float deltaX = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        Vector2 movement = new Vector2(deltaX, _body.velocity.y);
        _body.velocity = movement;
        Vector3 max = _box.bounds.max;
        Vector3 min = _box.bounds.min;
        Vector2 corner1 = new Vector2(max.x, min.y - .1f);
        Vector2 corner2 = new Vector2(min.x, min.y - .2f);
        Collider2D hit = Physics2D.OverlapArea(corner1, corner2);
        bool grounded = false;
        if (hit != null)
        {
            grounded = true;
        }
        _body.gravityScale = grounded && deltaX == 0 ? 0 : 1;
        if (grounded && Input.GetKeyDown(KeyCode.Space))
        {
            _body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
        _anim.SetFloat("speed", Mathf.Abs(deltaX));
        if (!Mathf.Approximately(deltaX, 0))
        {
            transform.localScale = new Vector3(Mathf.Sign(deltaX), 1, 1);
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Food")
        {
            _soundSource.PlayOneShot(eatingSound);
            Destroy(other.collider.gameObject);
            return;
        } 
        if (other.collider.tag == "Trophy")
        {
            _soundSource.PlayOneShot(celebrationSound);
            Destroy(other.collider.gameObject);
            return;
        }
    }
   
}