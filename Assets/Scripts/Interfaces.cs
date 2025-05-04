using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface IEnemy
{
    public void Hurt(float damage, Vector3 dir);
}
public interface IPlayer
{
    bool IsShieldOn{get;}
    public void Hurt(float damage, Vector3 dir);

}