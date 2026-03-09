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
        public ShipList ShipList;

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
            SpaceShipPoint.gameObject.SetActive(false);
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
            Debug.Log("Loading ship: " + shipId);
            if (!ship.ContainsKey(shipId))
            {
                var shipData = ShipList.GetShipData(shipId);
                var newShip = Instantiate(Resources.Load<GameObject>("Ships/" + shipData.PrefabName));
                ship.Add(shipId, newShip);
                Debug.Log("Loaded ship: " + shipData.Name);
            }
            SpaceShip = ship[shipId].transform;
            SpaceShip.gameObject.SetActive(true);
            SpaceShip.transform.parent = SpaceShipPoint;
            SpaceShip.transform.localPosition = Vector3.zero;
            SpaceShip.transform.localRotation = Quaternion.identity;
        }

        public void StartGame()
        {
            LoadShip(CurrentShipId);    
            MapController.Instance.Stop();        
            EffectController.Instance.Reset();
            ExplosionVfx.SetActive(false);
            RunCoin = 0;            
            UIManager.Instance.PlayGame();
            StartCoroutine(StartGameCoroutine());
        }

        IEnumerator StartGameCoroutine()
        {
            UIManager.Instance.FlashAndLoad();
            yield return new WaitForSeconds(0.1f);
            Debug.Log("Map controller move" + MapController.IsMoving);
            ObstacleController.Instance.Reset();
            yield return new WaitForSeconds(0.5f);
            SpaceShipPoint.gameObject.SetActive(true);
            MapController.Instance.StartGame();
            InputController.Instance.SetInputEnabled(true);
            EffectController.Instance.Reset();
            Debug.Log("Map controller move" + MapController.IsMoving);
        }

        public void GameOver()
        {
            MapController.Instance.GameOver();
            ship[CurrentShipId].SetActive(false);
            SpaceShipPoint.gameObject.SetActive(false);
            EffectController.Instance.ClearEffects();
            if (ExplosionVfx != null)
            {
                ExplosionVfx.SetActive(true);
            }
            InputController.Instance.SetInputEnabled(false);
            UIManager.Instance.ShowPopUp(UIManager.Instance.GameOverUI);
            PlayerData.UpdateRecordDistance(MapController.RunDistance);
        }

        public void AddCoin(int coin)
        {
            PlayerData.AddCoins(coin);
            RunCoin += coin;
        }
    }

}


