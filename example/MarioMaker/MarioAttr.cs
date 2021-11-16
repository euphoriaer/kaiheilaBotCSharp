using System;

namespace MarioMaker
{
    [AttributeUsage(AttributeTargets.Method)]
    public class MarioAttr : DistributeAttr
    {
        public readonly string typeName;

        public MarioAttr(string msgName) : base(msgName)
        {
            this.typeName = msgName;
        }
    }
}