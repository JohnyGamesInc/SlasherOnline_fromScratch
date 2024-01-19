using ExitGames.Client.Photon;
using Photon.Realtime;


namespace SlasherOnline
{
    
    public static class PhotonExtensions
    {
        
        public static bool SetExpectedUsers(this Room room, string[] newExpectedUsers, string[] oldExpectedUsers = null, WebFlags webFlags = null, bool broadcast = true)
        {
            Hashtable gameProperties = new Hashtable(1);
            gameProperties.Add(GamePropertyKey.ExpectedUsers, newExpectedUsers);
            ParameterDictionary parameterDictionary = new ParameterDictionary();
            parameterDictionary.Add(ParameterCode.Properties, gameProperties);
            
            if (broadcast)
            {
                parameterDictionary.Add(ParameterCode.Broadcast, true);
            }
            
            if (oldExpectedUsers != null)
            {
                Hashtable expectedProperties = new Hashtable(1);
                expectedProperties.Add(GamePropertyKey.ExpectedUsers, oldExpectedUsers);
                parameterDictionary.Add(ParameterCode.ExpectedValues, expectedProperties);
            }
            
            if (webFlags!=null && webFlags.HttpForward)
            {
                parameterDictionary[ParameterCode.EventForward] = webFlags.WebhookFlags;
            }
            
            return room.LoadBalancingClient.LoadBalancingPeer.SendOperation(OperationCode.SetProperties, parameterDictionary, SendOptions.SendReliable);
        }
        
        
    }
}