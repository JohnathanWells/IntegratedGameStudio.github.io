using UnityEngine;
using System.Collections;

public class cannonScript : MonoBehaviour {

    public Vector3 offsetShooting;
    public Vector3 flashOffset;
    public Transform[] possibleAmmo;
    public Transform mainBody;
    public bool shootMode = true;
    public bool changeSpeedOfBullets = false;
    public float[] newSpeed;
    ParticleSystem[] muzzleParticles;
    AudioClip[] ProjectilesSounds;
    int directionFacing;
    private int sizeOfArray = 1;
    int currentAmmo = 0;

    [Header("Other Scripts")]
    GameManager manager;
    SoundEffectManager SFX;
    ParticleManager PartM;
    Transform projectileFolder;

	void Start () {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<GameManager>();
        SFX = manager.SFX;
        PartM = manager.PM;
        projectileFolder = manager.ProjectilesFolder;
        mainBody = transform.parent;
        sizeOfArray = possibleAmmo.Length;
        offsetShooting.x *= directionFacing;
        directionFacing = getDirectionFacing();
        obtainPossibleMuzzleLights();
	}

    public void Shoot()
    {
        if (shootMode)
        {
            StartCoroutine(activateMuzzleLight(muzzleParticles[currentAmmo]));
            Vector3 rot = transform.rotation.eulerAngles;
            SFX.PlaySound(ProjectilesSounds[currentAmmo]);
            Transform shoot = Instantiate(possibleAmmo[currentAmmo], transform.position + offsetShooting, Quaternion.Euler(rot)) as Transform;
            shoot.parent = projectileFolder;
            
            if (changeSpeedOfBullets)
                shoot.GetComponent<ProjectileScript>().speed = newSpeed[currentAmmo];
        }
    }

    void obtainPossibleMuzzleLights()
    {
        muzzleParticles = new ParticleSystem[sizeOfArray];
        ProjectilesSounds = new AudioClip[sizeOfArray];
        ProjectileScript temp;

        for (int n = 0; n < sizeOfArray; n++)
        {
            temp = possibleAmmo[n].GetComponent<ProjectileScript>();
            ParticleSystem part = Instantiate(temp.getMuzzleParticles(), Vector3.zero, Quaternion.Euler(new Vector3(0, -90, 0))) as ParticleSystem;
            ProjectilesSounds[n] = temp.getShootingSound();
            part.transform.parent = transform;
            part.transform.localPosition = flashOffset;
            part.gameObject.SetActive(false);
            muzzleParticles[n] = part;
        }
    }

    IEnumerator activateMuzzleLight(ParticleSystem partSys)
    {
        //Debug.Log("CA = " + currentAmmo);
        partSys.gameObject.SetActive(true);
        partSys.time = 0;
        yield return new WaitForSeconds(partSys.duration);
        partSys.gameObject.SetActive(false);
    }

    int getDirectionFacing()
    {
        if (transform.position.x > mainBody.position.x)
            return 1;
        else
            return -1;
    }

    public void changeAmmo()
    {
        currentAmmo = Random.Range(0, sizeOfArray);
    }
}
