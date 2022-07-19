using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IDamageable<T>
{
       void TakeDamage(T damage);
}
