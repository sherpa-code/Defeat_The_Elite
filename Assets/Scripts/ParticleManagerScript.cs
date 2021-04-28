using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class ParticleManagerScript : MonoBehaviour {
    //public GameObject particlePoison;
    //public List<GameObject> particles = new List<GameObject>() {
    //    ball = GameObject ball,
    //    public GameObject ball2,
    //    public GameObject buff,
    //    public GameObject buff2,
    //    public GameObject buff3,
    //    public GameObject cube,
    //    public GameObject darkBall,
    //    public GameObject darkBall2,
    //    public GameObject explosion,
    //    public GameObject explosion2,
    //    public GameObject explosion3,
    //    public GameObject explosion4,
    //    public GameObject explosion5,
    //    public GameObject explosion6,
    //    public GameObject explosion7,
    //    public GameObject explosion8,
    //    public GameObject explosion9,
    //    public GameObject fantasyEffect,
    //    public GameObject fantasySun,
    //    public GameObject fantasySun2,
    //    public GameObject fireball,
    //    public GameObject fireball2,
    //    public GameObject fireball3,
    //    public GameObject flame,
    //    public GameObject flame2,
    //    public GameObject flameEmission,
    //    public GameObject greenPlane,
    //    public GameObject hole,
    //    public GameObject laserBombardment,
    //    public GameObject laserBombardment2,
    //    public GameObject laserBombardment3,
    //    public GameObject laserBombardment4,
    //    public GameObject lightningBall,
    //    public GameObject lightningBall2,
    //    public GameObject lightningCloud,
    //    public GameObject lightningState,
    //    public GameObject lightningState2,
    //    public GameObject lightningState3,
    //    public GameObject poison,
    //    public GameObject portal,
    //    public GameObject portal2,
    //    public GameObject portal3,
    //    public GameObject smoke,
    //    public GameObject snow,
    //    public GameObject sparking,
    //    public GameObject sparking2,
    //    public GameObject sparking3,
    //    public GameObject spirEmission,
    //};


    public GameObject ball;
    public GameObject ball2;
    public GameObject buff;
    public GameObject buff2;
    public GameObject buff3;
    public GameObject cube;
    public GameObject darkBall;
    public GameObject darkBall2;
    public GameObject explosion;
    public GameObject explosion2;
    public GameObject explosion3;
    public GameObject explosion4;
    public GameObject explosion5;
    public GameObject explosion6;
    public GameObject explosion7;
    public GameObject explosion8;
    public GameObject explosion9;
    public GameObject fantasyEffect;
    public GameObject fantasySun;
    public GameObject fantasySun2;
    public GameObject fireball;
    public GameObject fireball2;
    public GameObject fireball3;
    public GameObject flame;
    public GameObject flame2;
    public GameObject flameEmission;
    public GameObject greenPlane;
    public GameObject hole;
    public GameObject laserBombardment;
    public GameObject laserBombardment2;
    public GameObject laserBombardment3;
    public GameObject laserBombardment4;
    public GameObject lightningBall;
    public GameObject lightningBall2;
    public GameObject lightningCloud;
    public GameObject lightningState;
    public GameObject lightningState2;
    public GameObject lightningState3;
    public GameObject poison;
    public GameObject portal;
    public GameObject portal2;
    public GameObject portal3;
    public GameObject smoke;
    public GameObject snow;
    public GameObject sparking;
    public GameObject sparking2;
    public GameObject sparking3;


    public Transform allySpawnTransform;
    public Transform enemySpawnTransform;


    void Start() {
        //List<GameObject> particles();
        //particles.Add(ball);
    }

    public IEnumerator PlayAllParticles(Monster monster) {
        Transform targetLocation;
        if (monster.isAllyMonster) {
            targetLocation = allySpawnTransform;
        } else {
            targetLocation = enemySpawnTransform;
        }

        GameObject currentParticle;


        //currentParticle = Instantiate(snow, targetLocation.position, Quaternion.identity);
        //yield return new WaitForSeconds(6f);
        //Destroy(currentParticle);
        currentParticle = Instantiate(sparking, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(sparking2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(sparking3, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        //currentParticle = Instantiate(, targetLocation, Quaternion.identity);
        currentParticle = Instantiate(ball, targetLocation.position, Quaternion.identity);
        //currentParticle.startLifetime = 1f;
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(ball2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(buff, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(buff2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(buff3, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(cube, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(darkBall, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(darkBall2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion3, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion4, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion5, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion6, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion7, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion8, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(explosion9, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(fantasyEffect, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(fantasySun, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(fantasySun2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(fireball, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(fireball2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(fireball3, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(flame, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(flame2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(flameEmission, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(greenPlane, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(hole, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(laserBombardment, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(laserBombardment2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(laserBombardment3, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(laserBombardment4, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(lightningBall, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(lightningBall2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(lightningCloud, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(lightningState, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(lightningState2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(lightningState3, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(poison, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(portal, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(portal2, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);
        currentParticle = Instantiate(portal3, targetLocation.position, Quaternion.identity);
        yield return new WaitForSeconds(3f);
        Destroy(currentParticle);


    }


    //public void ParticleHit(Monster monster) {
    //    Transform targetLocation;
    //    if (monster.isAllyMonster) {
    //        targetLocation = allySpawnTransform;
    //    } else {
    //        targetLocation = enemySpawnTransform;
    //    }

    //    GameObject currentParticle;

    //    //currentParticle = Instantiate(, targetLocation, Quaternion.identity);
    //    currentParticle = Instantiate(ball, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(ball2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(buff, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(buff2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(buff3, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(cube, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(darkBall, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(darkBall2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion3, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion4, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion5, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion6, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion7, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion8, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(explosion9, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(fantasyEffect, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(fantasySun, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(fantasySun2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(fireball, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(fireball2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(fireball3, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(flame, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(flame2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(flameEmission, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(greenPlane, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(hole, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(laserBombardment, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(laserBombardment2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(laserBombardment3, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(laserBombardment4, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(lightningBall, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(lightningBall2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(lightningCloud, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(lightningState, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(lightningState2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(lightningState3, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(poison, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(portal, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(portal2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(portal3, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(smoke, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(snow, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(sparking, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(sparking2, targetLocation.position, Quaternion.identity);
    //    currentParticle = Instantiate(sparking3, targetLocation.position, Quaternion.identity);


    //}
}

