using UnityEngine;
using Thanos.GameEntity;
using Thanos.Resource;

class AbsorbEffect : MonoBehaviour
{
    public GameObject objStart;
    public GameObject objEnd;
    public IPlayer effectOwner;

    private static float totalTime = 3.0f;
    private float timeCounter = 0.0f;
    public static AbsorbEffect createAbsorbEffect(IPlayer owner, GameObject start, GameObject end)
    {        
        ResourceItem objUnit = ResourcesManager.Instance.loadImmediate("effect/other/soul_absorb", ResourceType.PREFAB);
        GameObject obj = objUnit.Asset as GameObject;
                      
        if (obj == null)
        {
            //Debug.LogError("Res Not Found:" + "effect/other/soul_absorb");
            return null;
        }
        GameObject rootNode = Instantiate(obj) as GameObject;
        AbsorbEffect effect = rootNode.GetComponent<AbsorbEffect>();
        if(effect != null)
        {
            effect.objStart = start;
            effect.objEnd = end;
            if(start != null)
            {
                effect.transform.position = start.transform.position;
            }
            effect.timeCounter = totalTime;
            effect.effectOwner = owner;
        }
        owner.AbsorbProgressEffect = rootNode;
        //
        return effect;
    }

    void Update()
    {
        if(objStart == null || objEnd == null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        float distance = Vector3.Distance(transform.position, objEnd.transform.position);
        float speed = distance / timeCounter;
        timeCounter -= Time.deltaTime;
        Vector3 dir = objEnd.transform.position - transform.position;
        dir.Normalize();
        transform.position += dir * speed * Time.deltaTime;
        if (timeCounter <= 0)
        {
            DestroyImmediate(gameObject);
        }
    }
}
