using System;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SupplyPlane : MonoBehaviour
{
    // 떨굴 지점을 고를 기준이 되는 바닥 사각형 (Map)
    [SerializeField] private Transform floor;

    // 몇 초마다 자동으로 올지. 0이면 자동으로 오지 않는다
    [SerializeField] private float interval = 15f;
    [SerializeField] private float minInterval = 5f;
    private float targetinterval;

    // 비행 속도 (초당 유닛)
    [SerializeField] private float speed = 20f;

    // 화면 밖 여유 거리. 이만큼 밖에서 출발하고 이만큼 지나서 멈춘다
    [SerializeField] private float margin = 5f;

    // 대각선 기울기 범위 (도). 15~75면 가로도 세로도 아닌 어중간한 대각선만 나온다
    [SerializeField] private float minAngle = 15f;
    [SerializeField] private float maxAngle = 75f;

    // 스프라이트가 원래 바라보는 각도. 그림이 위-오른쪽 대각선을 보고 있으면 45
    [SerializeField] private float spriteAngle = 45f;

    // 맵이 돌아 지점이 움직일 때 비행기가 진로를 트는 속도. 클수록 잘 따라간다
    [SerializeField] private float turnSpeed = 3f;

    // 떨어질 자리에 미리 뜨는 표시
    [SerializeField] private GameObject markerPrefab;

    // 비행기가 지나간 뒤 표시가 몇 초 더 남을지. 보급 낙하 시간과 맞추면 된다
    [SerializeField] private float markerHideDelay = 1.5f;

    // 표시가 두근거리는 정도. 0이면 안 움직인다
    [SerializeField] private float markerPulse = 0.15f;

    [SerializeField] private int count;
    private int currentCount;

    // 이 비행기가 떨어뜨릴 후보들. 이 중 하나가 랜덤으로 나온다
    // 적 비행기엔 적들을, 아이템 비행기엔 아이템들을 넣는다
    [SerializeField] private GameObject[] dropPrefabs;

    // 나중에 마커/보급이 구독할 신호. 떨굴 지점 위를 지나는 순간 위치를 넘긴다
    public event Action<Vector2> OnDropPoint;

    private float timer;
    private bool flying;
    private bool dropped;
    private Vector2[] dropLocal;      // 떨굴 지점. 바닥 기준 좌표라 맵이 돌면 같이 돈다
    private Vector2 flyDir;         // 진행 방향 (단위 벡터)
    private float offscreenDist;    // 화면 밖까지의 거리
    private float distAfterDrop;    // 떨군 뒤 날아간 거리
    private GameObject[] marker;

    private void Start()
    {
        // 평소엔 화면 밖에 세워둔다
        transform.position = new Vector3(-9999f, -9999f, transform.position.z);
        timer = 9999999;
    }

    private void Update()
    {
        if (!flying)
        {
            if (interval <= 0f)
                return;

            timer += Time.deltaTime;
            if (timer >= interval)
            {
                CallPlane();
                targetinterval = Random.Range(minInterval, interval);
            }

            return;
        }
        
            Vector2 target = CurrentDropPoint(0);

            // 아직 안 떨궜으면 지점을 향해 서서히 진로를 튼다. 맵이 돌아도 따라간다
            if (!dropped)
            {
                Vector2 aim = (target - (Vector2)transform.position).normalized;
                flyDir = Vector2.Lerp(flyDir, aim, turnSpeed * Time.deltaTime).normalized;
            }

            transform.position += (Vector3)(flyDir * (speed * Time.deltaTime));
            FaceFlyDirection();

            // 지점을 지나치는 순간 한 번만 신호를 보낸다
            if (!dropped && Vector2.Dot(target - (Vector2)transform.position, flyDir) <= 0f)
            {
                for (int i = 0; i < currentCount; i++)
                {
                    dropped = true;
                    OnDropPoint?.Invoke(target);

                    // 적이냐 아이템이냐 확률로 정하고, 그 리스트에서 하나 뽑아 떨어뜨린다
                    GameObject prefab = PickDrop();
                    if (prefab != null)
                    {
                        GameObject drop = Instantiate(prefab, CurrentDropPoint(i), Quaternion.identity);
                        if (floor != null)
                            drop.transform.SetParent(floor, true);
                    }

                    // 낙하물이 내려올 시간만큼 있다가 표시를 거둔다
                    if (marker[i] != null)
                    {
                        Destroy(marker[i], markerHideDelay);
                    }
                }
        }


        // 떨군 뒤에는 직진해서 화면 밖으로 나간다
        if (dropped)
        {
            distAfterDrop += speed * Time.deltaTime;
            if (distAfterDrop >= offscreenDist)
            {
                flying = false;
                transform.position = new Vector3(-9999f, -9999f, transform.position.z);
            }
        }
    }

    // 웨이브 매니저 등 밖에서 불러도 바로 출발한다
    public void CallPlane()
    {
        if (flying)
            return;

        timer = 0f;
        flying = true;
        dropped = false;
        distAfterDrop = 0f;

        // 바닥 사각형 안에서 랜덤 지점을 하나 고른다. 가장자리 10%는 피한다
        currentCount = Random.Range(1, count+1);
        dropLocal = new Vector2[currentCount];
        marker = new GameObject[currentCount];
        for (int i = 0; i < currentCount; i++)
        {
            dropLocal[i] = new Vector2(
                UnityEngine.Random.Range(-0.4f, 0.4f),
                UnityEngine.Random.Range(-0.4f, 0.4f));
            ShowMarker(i);
        }



        // 대각선 방향을 하나 뽑는다. 좌우, 상하 진행 방향도 반반이다
        float angle = UnityEngine.Random.Range(minAngle, maxAngle) * Mathf.Deg2Rad;
        flyDir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
        if (UnityEngine.Random.value < 0.5f) flyDir.x = -flyDir.x;
        if (UnityEngine.Random.value < 0.5f) flyDir.y = -flyDir.y;

        // 화면 대각선 절반 + 여유 = 어느 방향이든 확실히 화면 밖인 거리
        Camera cam = Camera.main;
        float halfW = cam.orthographicSize * cam.aspect;
        float halfH = cam.orthographicSize;
        offscreenDist = Mathf.Sqrt(halfW * halfW + halfH * halfH) + margin;

        // 지점 반대편 화면 밖에서 출발한다
        Vector2 start = CurrentDropPoint(0) - flyDir * offscreenDist;
        transform.position = new Vector3(start.x, start.y, transform.position.z);
        FaceFlyDirection();
    }

    // 후보 리스트에서 랜덤으로 하나 뽑는다
    private GameObject PickDrop()
    {
        if (dropPrefabs == null || dropPrefabs.Length == 0)
            return null;

        return dropPrefabs[UnityEngine.Random.Range(0, dropPrefabs.Length)];
    }

    // 바닥이 돌아간 걸 반영한 지금 이 순간의 떨굴 지점
    private Vector2 CurrentDropPoint(int index)
    {
        return floor != null ? (Vector2)floor.TransformPoint(dropLocal[index]) : dropLocal[index];
    }

    // 기체가 진행 방향을 바라보게 돌린다. 그림이 원래 기울어진 만큼은 빼준다
    private void FaceFlyDirection()
    {
        float deg = Mathf.Atan2(flyDir.y, flyDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, deg - spriteAngle);
    }

    // 떨어질 자리에 표시를 띄운다. 바닥의 자식으로 붙여서 맵이 돌면 같이 돈다
    private void ShowMarker(int index)
    {
        if (markerPrefab == null)
            return;

        if (marker[index] != null)
            Destroy(marker[index]);

        marker[index] = Instantiate(markerPrefab, CurrentDropPoint(index), Quaternion.identity);

        // 나중에 붙여야 바닥의 큰 스케일에 마커가 뻥튀기되지 않는다
        if (floor != null)
            marker[index].transform.SetParent(floor, true);

        if (markerPulse > 0f)
        {
            marker[index].transform.DOScale(marker[index].transform.localScale * (1f + markerPulse), 0.4f)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(marker[index]);
        }
    }
}
