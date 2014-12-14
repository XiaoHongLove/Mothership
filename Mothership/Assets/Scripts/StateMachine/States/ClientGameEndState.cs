﻿using UnityEngine;
using System.Collections;
using Mothership;

namespace MothershipStateMachine
{
    public class ClientGameEndState : ClientGameState
    {
        public ClientGameEndState(ClientManager manager)
            : base(manager)
        {

        }

        public override void OnGameMessage(GameMessage message)
        {
            GameResultReceived result = message as GameResultReceived;
            if(result != null)
            {
                if(clientManager.OnGameResultReceived != null)
                {
                    clientManager.OnGameResultReceived(result.Result);
                }
            }

            base.OnGameMessage(message);
        }

        public override void OnStateMessage(StateMessage message)
        {
            OnEnterState enter = message as OnEnterState;
            if(enter != null)
            {
                clientManager.Die();
                if(clientManager.OnMatchEnded != null)
                {
                    clientManager.OnMatchEnded();
                }
            }
        }
    }

}