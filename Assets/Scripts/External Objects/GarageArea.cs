using System.Collections;
using System.Collections.Generic;
using NBLD.Ship;
using NBLD.Utils;
using UnityEngine;

namespace NBLD.ExteriorObjects
{
    public class GarageArea : MonoBehaviour
    {
        public CampaignLevel campaignLevel;
        [SerializeField]
        private bool garageAvailable = true;
        public int garageMaxUses = 1;
        private int garageUsesCount = 0;
        private bool shipInArea = false;
        private ShipMovement ship;
        [Header("Animation")]
        public float timeUntilEnter = 2f;
        public float shipPullSpeed = 2f;
        public List<Transform> doors = new List<Transform>();
        public List<SpriteRenderer> bigDoorsSpriteRenderers = new List<SpriteRenderer>();
        public List<SpriteRenderer> smallDoorsSpriteRenderers = new List<SpriteRenderer>();

        public List<SpriteRenderer> smallestDoorsSpriteRenderers = new List<SpriteRenderer>();
        public int bigDoorClosedSpriteOrder = 1, smallDoorClosedSpriteOrder = 2, smallestDoorClosedSpriteOrder = 3;
        public int bigDoorOpenSpriteOrder = 401, smallDoorOpenSpriteOrder = 402, smallestDoorOpenSpriteOrder = 403;
        public string doorsClosedSortingLayer = "Default";
        public string doorsOpenSortingLayer = "LitAbove";

        public Transform bigDoorsRoot, smallDoorsRoot, smallestDoorsRoot;
        public Vector2 doorsClosedLocalOffset, doorsOpenLocalOffset;
        private float doorsOpenT = 0;
        public float doorsOpenSpeed = 2;
        public float bigDoorsRotSpeed, smallDoorsRotSpeed, smallestDoorsRotSpeed;
        public float openingRotSpeedMultiplier = 4f;
        private Timer timerBeforeEnter;
        private bool enteringGarage = false;
        private bool startedClosingDoors = false;
        private bool doorsClosed = false;
        private bool shipInPosition = false;
        public delegate void StateHandler();
        public event StateHandler OnGarageAreaStateChanged;

        private void Awake()
        {
            timerBeforeEnter = new Timer(timeUntilEnter);
            enteringGarage = false;
        }
        private void OnEnable()
        {
            enteringGarage = false;
            SetAllDoorsOffset(doorsClosedLocalOffset);
            SetAllDoorsSpriteOrder(false);
            startedClosingDoors = false;
            doorsClosed = false;
            shipInPosition = false;
        }

        private void Update()
        {
            if (enteringGarage)
            {
                if (doorsClosed)
                {
                    EnterGarage();
                }
                else
                {
                    if (!shipInPosition)
                    {
                        Vector3 deltaMove = (transform.position - ship.transform.position).normalized * shipPullSpeed * Time.deltaTime;
                        ship.MoveShip(deltaMove);
                        if (Vector3.Distance(ship.transform.position, transform.position) < 0.1f)
                        {
                            shipInPosition = true;
                        }
                    }

                    if (!startedClosingDoors)
                    {
                        doorsOpenT += Time.deltaTime * doorsOpenSpeed;
                        Vector2 curOffset = Vector2.Lerp(doorsClosedLocalOffset, doorsOpenLocalOffset, doorsOpenT);
                        SetAllDoorsOffset(curOffset);
                        if (doorsOpenT >= 1)
                        {
                            startedClosingDoors = true;
                            SetAllDoorsSpriteOrder(true);
                        }
                    }
                    if (startedClosingDoors && shipInPosition)
                    {
                        doorsOpenT -= Time.deltaTime * doorsOpenSpeed;
                        Vector2 curOffset = Vector2.Lerp(doorsClosedLocalOffset, doorsOpenLocalOffset, doorsOpenT);
                        SetAllDoorsOffset(curOffset);
                        if (doorsOpenT <= 0)
                        {
                            doorsClosed = true;
                        }
                    }

                }
            }
            //Rotate doors
            Vector3 baseRot = Vector3.forward * Time.deltaTime;
            if (enteringGarage)
            {
                baseRot = baseRot * openingRotSpeedMultiplier;
            }
            bigDoorsRoot.Rotate(baseRot * bigDoorsRotSpeed, Space.Self);
            smallDoorsRoot.Rotate(baseRot * smallDoorsRotSpeed, Space.Self);
            smallestDoorsRoot.Rotate(baseRot * smallestDoorsRotSpeed, Space.Self);
        }

        private void SetAllDoorsOffset(Vector2 offset)
        {
            for (int i = 0; i < doors.Count; i++)
            {
                Vector2 localPos = doors[i].localPosition;
                doors[i].localPosition = new Vector2(Mathf.Sign(localPos.x) * offset.x, Mathf.Sign(localPos.y) * offset.y);
            }
        }
        private void SetAllDoorsSpriteOrder(bool areOpen)
        {
            for (int i = 0; i < bigDoorsSpriteRenderers.Count; i++)
            {
                bigDoorsSpriteRenderers[i].sortingOrder = (areOpen) ? bigDoorOpenSpriteOrder : bigDoorClosedSpriteOrder;
                bigDoorsSpriteRenderers[i].sortingLayerID = SortingLayer.NameToID((areOpen) ? doorsOpenSortingLayer : doorsClosedSortingLayer);
            }
            for (int i = 0; i < smallDoorsSpriteRenderers.Count; i++)
            {
                smallDoorsSpriteRenderers[i].sortingOrder = (areOpen) ? smallDoorOpenSpriteOrder : smallDoorClosedSpriteOrder;
                smallDoorsSpriteRenderers[i].sortingLayerID = SortingLayer.NameToID((areOpen) ? doorsOpenSortingLayer : doorsClosedSortingLayer);

            }
            for (int i = 0; i < smallestDoorsSpriteRenderers.Count; i++)
            {
                smallestDoorsSpriteRenderers[i].sortingOrder = (areOpen) ? smallestDoorOpenSpriteOrder : smallestDoorClosedSpriteOrder;
                smallestDoorsSpriteRenderers[i].sortingLayerID = SortingLayer.NameToID((areOpen) ? doorsOpenSortingLayer : doorsClosedSortingLayer);

            }
        }

        private void OnTriggerEnter2D(Collider2D col)
        {
            if (col.tag == Tags.SHIP_BODY)
            {
                if (!shipInArea)
                {
                    shipInArea = true;
                    ship = col.GetComponentInParent<ShipMovement>();
                    OnGarageAreaStateChanged?.Invoke();

                }
            }
        }
        private void OnTriggerExit2D(Collider2D col)
        {
            if (col.tag == Tags.SHIP_BODY)
            {
                if (shipInArea)
                {
                    shipInArea = false;
                    OnGarageAreaStateChanged?.Invoke();
                }
            }
        }

        public bool CanGarageBeEntered()
        {
            return garageAvailable && shipInArea && (garageUsesCount < garageMaxUses);
        }
        public void StartToEnterGarage()
        {
            if (CanGarageBeEntered())
            {
                timerBeforeEnter.Restart();
                enteringGarage = true;
                ship.LockMovement();
            }
        }
        private void EnterGarage()
        {
            if (CanGarageBeEntered())
            {
                bool prevGarageEnterState = CanGarageBeEntered();
                garageUsesCount++;
                if (prevGarageEnterState != CanGarageBeEntered())
                {
                    OnGarageAreaStateChanged?.Invoke();
                }
                campaignLevel.EnterGarage();
            }
        }
    }
}

