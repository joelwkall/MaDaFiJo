using Assets;
using System.Linq;
using Extensions;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    //these can be set in the unity editor
	public float speedMultiplier;
    public float maxSpeed;
    public float jumpStrength;

    private Rigidbody2D _rb2d;   
    private BoxCollider2D _boxCollider;

    private float _timeSinceLastFire;
    private float _timeSinceChangeWeapon;

    private Weapon _currentWeapon;

	// Use this for initialization
	void Start () {
		//Get and store a reference to the Rigidbody2D component so that we can access it.
        _rb2d = GetComponent<Rigidbody2D>();
        _rb2d.mass = 1;
        _boxCollider = GetComponent<BoxCollider2D>();
        _currentWeapon = WeaponStore.Weapons.First();
	}
	
	//FixedUpdate is called at a fixed interval and is independent of frame rate. Put physics code here.
    void FixedUpdate()
    {
        //Store the current horizontal input in the float moveHorizontal.
        var moveHorizontal = Input.GetAxis("Horizontal");

        //Use the two store floats to create a new Vector2 variable movement.
        var movement = new Vector2(moveHorizontal, 0);

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        if (Mathf.Abs(_rb2d.velocity.x + movement.x) < maxSpeed)
            _rb2d.AddForce (movement * speedMultiplier);

        //jump
        if (Input.GetButton("Jump"))
        {
            var bottomOfPlayer = transform.position + new Vector3(0, -((_boxCollider.size.y/2)+0.05f));

            RaycastHit2D hit = Physics2D.Raycast(bottomOfPlayer, Vector3.down, 0.05f);

            //TODO: make sure only ground counts (not projectiles)
            if (hit)
            {
                var jumpMovement = new Vector2(0, jumpStrength);
                _rb2d.AddForce(jumpMovement);
            }
        }
    }

    void OnGUI()
    {
        //show weapon label for 3 seconds
        if (_timeSinceChangeWeapon < 3)
        {
            var pos = Camera.main.WorldToScreenPoint(transform.position);

            //TODO: center text and make prettier. this is just an ugly hack
            GUI.Label(new Rect(pos.x, Camera.main.pixelHeight-pos.y - 50, 100, 20), _currentWeapon.Name);
        }
    }

    // Update is called once per frame
    void Update () {

        _timeSinceLastFire += Time.deltaTime;
        _timeSinceChangeWeapon += Time.deltaTime;

        //change weapon
        if (Input.GetMouseButtonUp(1))
        {
            var currentIndex = WeaponStore.Weapons.IndexOf(_currentWeapon);
            currentIndex++;
            if(currentIndex >= WeaponStore.Weapons.Count)
            {
                currentIndex = 0;
            }

            _currentWeapon = WeaponStore.Weapons[currentIndex];

            _timeSinceChangeWeapon = 0;
        }

        //fire bullets
        //TODO: change so it sets the time of next fire, so that you can switch from minigun to rockets and have it shoot instantly
        if (Input.GetMouseButton(0) && _timeSinceLastFire > _currentWeapon.FireDelay)
        {
            var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var diff = new Vector2(mouse.x, mouse.y) - _rb2d.position;

            var emitter = _currentWeapon.ProjectileEmitter;

            emitter.EmitProjectiles(_boxCollider, _rb2d, diff);

            _timeSinceLastFire = 0;
        }

        //"respawn"
        if (_rb2d.position.y < -50)
        {
            _rb2d.velocity = new Vector2(0f, 0f);
            _rb2d.angularVelocity = 0;
            _rb2d.rotation = 0;
            _rb2d.MovePosition(new Vector2(0f, 0f));
        }
    }
}
