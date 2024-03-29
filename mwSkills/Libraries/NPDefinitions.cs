using System;

namespace mwSkills.Libraries
{
    public struct SD3NP_PKG
    {
        public String User {get; set;}
        public Int32 SID {get; set;}
        public SD3NP_CMD Type {get; set;}
        public Object Content {get; set;}
        public Int32 Code {get; set;}
        public String Message {get; set;}
    }
    
    public enum SD3NP_CMD : ushort
    {
        None            = 0,
    }
}