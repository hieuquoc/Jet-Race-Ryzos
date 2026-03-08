using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ZyroX
{
    public class GameManager : MonoBehaviour
    {
        public string CurrentShipId;
        public Transform SpaceShip;
        public Transform SpaceShipPoint;
        public static GameManager Instance;
        public GameObject ExplosionVfx;
        public static int LastRandomLineIndex = 0;
        public static int RunCoin = 0;

        private Dictionary<string, GameObject> ship = new Dictionary<string, GameObject>();

        private void Awake()
        {
            Instance = this;
            Instantiate(Resources.Load<GameObject>("ObstacleManager"), Vector3.zero, Quaternion.identity);
        }

        // Start is called before the first frame update
        void Start()
        {
            CurrentShipId = PlayerData.SelectedShip;
            LoadShip(CurrentShipId);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadShip(string shipId)
        {
            if (SpaceShip != null)
            {
                SpaceShip.gameObject.SetActive(false);
            }
            if (!ship.ContainsKey(shipId))
            {
                var newShip = Instantiate(Resources.Load<GameObject>("Ships/" + shipId));
                ship.Add(shipId, newShip);
            }
            SpaceShip = ship[shipId].transform;
            SpaceShip.gameObject.SetActive(true);
            SpaceShip.transform.parent = SpaceShipPoint;
            SpaceShip.transform.localPosition = Vector3.zero;
            SpaceShip.transform.localRotation = Quaternion.identity;
        }

        public void StartGame()
        {
            MapController.Instance.StartGame();
            ObstacleController.Instance.Reset();
            EffectController.Instance.Reset();
            ExplosionVfx.SetActive(false);
            RunCoin = 0;
        }

        public void GameOver()
        {
            MapController.Instance.GameOver();
            ship[CurrentShipId].SetActive(false);
            EffectController.Instance.Reset();
            if (ExplosionVfx != null)
            {
                ExplosionVfx.SetActive(true);
            }
        }

        public void AddCoin(int coin)
        {
            PlayerData.AddCoins(coin);
            RunCoin += coin;
        }
    }

}


