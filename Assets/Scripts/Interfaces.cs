using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnemy
{
    public void Hurt(float damage, Vector3 dir);
    public void SetMaxHealth(float givenHealth);
    public void SetStopDistance(float distance);
    public void SetShootInterval(float interval);
    public void SetMoveSpeed(float speed);
}
public interface IPlayer
{
    bool IsShieldOn{get;}
    public void Hurt(float damage, Vector3 dir);

}