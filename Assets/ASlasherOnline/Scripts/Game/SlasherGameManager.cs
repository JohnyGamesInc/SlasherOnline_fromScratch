using System.Collections.Generic;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Realtime;


//
namespace SlasherOnline
{
	/// <summary>
	/// Game manager.
	/// Connects and watch Photon Status, Instantiate Player
	/// Deals with quiting the room and the game
	/// Deals with level loading (outside the in room synchronization)
	/// </summary>
	public class SlasherGameManager : MonoBehaviourPunCallbacks
    {
	    
	    public static  SlasherGameManager Instance;

	    
		private GameObject instance;

        [Tooltip("The prefab to use for representing the player")]
        [SerializeField]
        private GameObject playerPrefab;

        public static int SpawnCounter = -1;

        public List<Transform> Spawners = new ();



        private void Start()
		{
			Instance = this;

			// in case we started this demo with the wrong scene being active, simply load the menu scene
			if (!PhotonNetwork.IsConnected)
			{
				SceneManager.LoadScene("LobbyPhotonScene");
				return;
			}

			if (playerPrefab == null) { // #Tip Never assume public properties of Components are filled up properly, always check and inform the developer of it.
				
				Debug.LogError("<Color=Red><b>Missing</b></Color> playerPrefab Reference. Please set it up in GameObject 'Game Manager'", this);
			} 
			else 
			{
				if (PhotonNetwork.InRoom && PlayerManager.LocalPlayerInstance == null)
				{
				    Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

				    SpawnCounter += 1;
				    var spawner = Instance.Spawners[SpawnCounter];
					// we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
					PhotonNetwork.Instantiate(this.playerPrefab.name, spawner.position, Quaternion.identity, 0);
					Debug.Log($"Should Spawn in [{spawner.position}]");
				}else{

					Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
				}
				
			}
		}

	
		private void Update()
		{
			// "back" button of phone equals "Escape". quit app if that's pressed
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				QuitApplication();
			}
		}
		
		
        public override void OnJoinedRoom()
        {
	        Debug.Log("GameScene: Joined the room");
            // Note: it is possible that this monobehaviour is not created (or active) when OnJoinedRoom happens
            // due to that the Start() method also checks if the local player character was network instantiated!
            if (PlayerManager.LocalPlayerInstance == null)
            {
                Debug.LogFormat("We are Instantiating LocalPlayer from {0}", SceneManagerHelper.ActiveSceneName);

                SpawnCounter += 1;
                var spawner = Instance.Spawners[SpawnCounter];
                // we're in a room. spawn a character for the local player. it gets synced by using PhotonNetwork.Instantiate
                PhotonNetwork.Instantiate(this.playerPrefab.name, spawner.position, Quaternion.identity, 0);
                Debug.Log($"Should Spawn in [{spawner.position}]");
            }
        }
        
        
        public override void OnPlayerEnteredRoom(Player other)
		{
			Debug.Log("OnPlayerEnteredRoom() " + other.NickName); // not seen if you're the player connecting
		}
        
		
		public override void OnPlayerLeftRoom(Player other)
		{
			Debug.Log("OnPlayerLeftRoom() " + other.NickName); // seen when other disconnects

			if (PhotonNetwork.IsMasterClient)
			{
				// Debug.LogFormat("OnPlayerEnteredRoom IsMasterClient {0}", PhotonNetwork.IsMasterClient); // called before OnPlayerLeftRoom
				// LoadArena(); 
			}
		}
		
		
		/// <summary>
		/// Called when the local player left the room. We need to load the launcher scene.
		/// </summary>
		public override void OnLeftRoom()
		{
			Debug.Log("Game Scene: Left Room");
			SceneManager.LoadScene("LobbyPhotonScene");
		}
		

		private void LeaveRoom() 
		{
			Debug.Log("Game Scene: Leave Room");
			PhotonNetwork.LeaveRoom();
		}

		private void QuitApplication()
		{
			Application.Quit();
		}

		
		private void LoadArena()
		{
			if (!PhotonNetwork.IsMasterClient)
			{
				Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
				return;
			}

			Debug.LogFormat("PhotonNetwork : Loading Level : {0}", PhotonNetwork.CurrentRoom.PlayerCount);

			PhotonNetwork.LoadLevel("PunBasicBigRoom");
		}
		
		
    }
}	