using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wizard : MonoBehaviour
{
    public enum Spell
    {
        None,
        Attractive,
        Repulsive,
        Stoped
    }
    public Spell ActiveSpell
    {
        get
        {
            return m_activeSpell;
        }
        set
        {
            m_activeSpell = value;
        }
    }
    private Spell m_activeSpell;

    [SerializeField] float m_radius;
    [SerializeField] float m_attractSpeed;
    [SerializeField] float m_repulseSpeed;
    [SerializeField] float m_stopForce;
    [SerializeField] float m_stopAngularForce;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.A))
            StartUsingSpell(Spell.Attractive);

        if (Input.GetKeyDown(KeyCode.Z))
            StartUsingSpell(Spell.Repulsive);

        if (Input.GetKeyDown(KeyCode.E))
            StartUsingSpell(Spell.Stoped);

        if(Input.GetKeyUp(KeyCode.E) || Input.GetKeyUp(KeyCode.Z) || Input.GetKeyUp(KeyCode.A))
            StartUsingSpell(Spell.None);

        CheckActiveSpell();
    }

    private Rigidbody[] GetBodiesInRange()
    {
        LayerMask layerMask = 1 << 9;
        Collider[] colliders = Physics.OverlapSphere(transform.position, m_radius,layerMask);
        Rigidbody[] bodeisInRange = new Rigidbody[colliders.Length];
        for (int i = 0; i < colliders.Length; i++)
        {
            bodeisInRange[i] = colliders[i].GetComponent<Rigidbody>();
        }
        return bodeisInRange;
    }

    private void StartUsingSpell(Spell spellUsed)
    {
        ActiveSpell = spellUsed;
        switch (spellUsed)
        {
            case Spell.None:
                break;
            case Spell.Attractive:
                AttractBodies();
                break;
            case Spell.Repulsive:
                RepulseBodies();
                break;
            case Spell.Stoped:
                break;
        }
    }

    private void CheckActiveSpell()
    {
        switch (m_activeSpell)
        {
            case Spell.None:
                break;
            case Spell.Attractive:
                break;
            case Spell.Repulsive:
                break;
            case Spell.Stoped:
                StopBodies();
                break;
        }
    }

    private void AttractBodies()
    {
        Rigidbody[] rbs = GetBodiesInRange();
        for (int i = 0; i < rbs.Length; i++)
        {
            Vector3 attractDirection = (transform.position - rbs[i].position).normalized;
            rbs[i].MovePosition(rbs[i].position + attractDirection * m_attractSpeed * Time.deltaTime);
        }
    }

    private void RepulseBodies()
    {
        Rigidbody[] rbs = GetBodiesInRange();
        for (int i = 0; i < rbs.Length; i++)
        {
            Vector3 attractDirection = - (transform.position - rbs[i].position).normalized;
            rbs[i].MovePosition(rbs[i].position + attractDirection * m_repulseSpeed * Time.deltaTime);
        }
    }
    
    private void StopBodies()
    {
        Rigidbody[] rbs = GetBodiesInRange();
        for (int i = 0; i < rbs.Length; i++)
        {
            if (rbs[i].velocity.magnitude < 0.1f)
                rbs[i].velocity = Vector3.zero;
            else
                rbs[i].velocity -= rbs[i].velocity.normalized * m_stopForce * Time.deltaTime;

            if (rbs[i].angularVelocity.magnitude < 0.1f)
                rbs[i].angularVelocity = Vector3.zero;
            else
                rbs[i].angularVelocity -= rbs[i].angularVelocity.normalized * m_stopAngularForce * Time.deltaTime;
            print(rbs[i].velocity.magnitude);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, m_radius);
    }
}
