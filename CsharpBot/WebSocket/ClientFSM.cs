using System;
using System.Collections.Generic;
using Org.BouncyCastle.Asn1.Cmp;

namespace CsharpBot
{
    public class ClientFSM
    {
        private IState currentState;
        private Dictionary<StateType, IState> states = new Dictionary<StateType, IState>();
        public enum StateType
        {
            Connection, Disconnection
        }

        public ClientFSM()
        {

        }

        public void TransitionState(StateType type)
        {
            if (currentState != null)
            {
                currentState.OnExit();
            }
            else
            {
                Console.WriteLine("对象是空的");
            }
            currentState = states[type];
            currentState.OnEnter();
        }
    }
}