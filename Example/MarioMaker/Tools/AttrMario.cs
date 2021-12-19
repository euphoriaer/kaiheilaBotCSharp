using System;

namespace MarioMaker
{
    [AttributeUsage(AttributeTargets.Method)]
    public class AttrMario : DistributeAttr
    {
        public AttrMario(string msgName) : base(msgName)
        {
        }
    }
}