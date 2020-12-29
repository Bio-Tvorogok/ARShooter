using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponFire : MonoBehaviour
{

    Animator anim;
	public bool randomMuzzleflash = false;    
	public bool enableMuzzleflash = true;
	public ParticleSystem muzzleParticles;

	public Light muzzleflashLight;

	public float lightDuration = 0.02f;

	private int randomMuzzleflashValue;

    	private int minRandomValue = 1;

	[Range(2, 25)]
	public int maxRandomValue = 5;

	public bool enableSparks = true;
	public ParticleSystem sparkParticles;
    public int minSparkEmission = 1;
	public int maxSparkEmission = 7;
    

    [Header("Bullet Settings")]
	//Bullet
	[Tooltip("How much force is applied to the bullet when shooting.")]
	public float bulletForce = 400.0f;
	[Tooltip("How long after reloading that the bullet model becomes visible " +
		"again, only used for out of ammo reload animations.")]
	public float showBulletInMagDelay = 0.6f;
	[Tooltip("The bullet model inside the mag, not used for all weapons.")]
	public SkinnedMeshRenderer bulletInMagRenderer;

	[System.Serializable]
    public class prefabs
	{  
		[Header("Prefabs")]
		public Transform bulletPrefab;
		public Transform casingPrefab;
		public Transform grenadePrefab;
	}
    public prefabs Prefabs;

    [System.Serializable]
	public class spawnpoints
	{  
		[Header("Spawnpoints")]
		//Array holding casing spawn points 
		//(some weapons use more than one casing spawn)
		//Casing spawn point array
		public Transform casingSpawnPoint;
		//Bullet prefab spawn from this point
		public Transform bulletSpawnPoint;

		public Transform grenadeSpawnPoint;
	}
	public spawnpoints Spawnpoints;






    private void Awake () {
		
		//Set the animator component
		anim = GetComponent<Animator>();
		//Set current ammo to total ammo value
		muzzleflashLight.enabled = false;

	}

    public void Fire() {
        anim.Play ("Fire", 0, 0f);
        //If random muzzle is false
        if (!randomMuzzleflash && 
            enableMuzzleflash == true) 
        {
            muzzleParticles.Emit (1);
            //Light flash start
            StartCoroutine(MuzzleFlashLight());
        } 
        else if (randomMuzzleflash == true)
        {
            //Only emit if random value is 1
            if (randomMuzzleflashValue == 1) 
            {
                if (enableSparks == true) 
                {
                    //Emit random amount of spark particles
                    sparkParticles.Emit (Random.Range (minSparkEmission, maxSparkEmission));
                }
                if (enableMuzzleflash == true) 
                {
                    muzzleParticles.Emit (1);
                    //Light flash start
                    StartCoroutine (MuzzleFlashLight ());
                }
            }
        }

        				//Spawn bullet from bullet spawnpoint
        var bullet = (Transform)Instantiate (
            Prefabs.bulletPrefab,
            Spawnpoints.bulletSpawnPoint.transform.position,
            Spawnpoints.bulletSpawnPoint.transform.rotation);

        //Add velocity to the bullet
        bullet.GetComponent<Rigidbody>().velocity = 
            bullet.transform.forward * bulletForce;
        
        //Spawn casing prefab at spawnpoint
        Instantiate (Prefabs.casingPrefab, 
            Spawnpoints.casingSpawnPoint.transform.position, 
            Spawnpoints.casingSpawnPoint.transform.rotation);
    }

    private void Update() {
        if (randomMuzzleflash == true) 
		{
			randomMuzzleflashValue = Random.Range (minRandomValue, maxRandomValue);
		}
    }

    private IEnumerator MuzzleFlashLight () {
		
		muzzleflashLight.enabled = true;
		yield return new WaitForSeconds (lightDuration);
		muzzleflashLight.enabled = false;
	}
}
