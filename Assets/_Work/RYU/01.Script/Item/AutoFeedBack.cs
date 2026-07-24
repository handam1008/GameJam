using _Work.RYU._01.Script.FeedBack;
using UnityEngine;


[RequireComponent(typeof(FeedBackPlayer))]
public class AutoFeedBack : MonoBehaviour
{
    [SerializeField] private float lifeTime = 2f;

    private void Start()
    {
        GetComponent<FeedBackPlayer>().PlayAllFeedBack();
        Destroy(gameObject, lifeTime);
    }
}
