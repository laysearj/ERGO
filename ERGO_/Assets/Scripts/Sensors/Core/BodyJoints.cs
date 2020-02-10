using System.Collections.Generic;

namespace Voxar
{
    public class BodyJoints
    {
        public byte id;
        public Status status;
        public Dictionary<JointType, Joint> joints;

        public BodyJoints(int id, Status status, Dictionary<JointType, Joint> joints)
        {
            this.id = (byte) id;
            this.status = status;
            this.joints = new Dictionary<JointType, Joint>(joints);
        }
    }
}
