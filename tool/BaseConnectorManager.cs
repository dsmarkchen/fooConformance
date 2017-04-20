using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Peak.Can.Basic;

namespace ToolConnector
{
    interface   BaseConnectorManager
    {
        void reconnect(int rate);
        void disconnect();
        TPCANStatus sendPCANData(ref TPCANMsg msg);
        bool isCanBusResponseContinue(int destID, int source, int responseID, double waitTime);
        bool IsConnected();
        byte[] getCanBusResponseData(int destID, int source, int command, double waitTime, ref int receiveDataLen);
        List<byte> receiveBoardInformation(int destId, int sourceId, int responseCommand);
        List<byte> receiveDirctionModuleInformation( int canbusID, int sourceId, int responseCommand );
        CanbusMessage getCanBusResponseData( int destID, int source, int command, double waitTime );
    }
}
