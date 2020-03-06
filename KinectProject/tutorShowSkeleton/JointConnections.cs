using Microsoft.Kinect;
using System;
using System.Linq;

namespace tutorShowSkeleton
{
  class JointConnection
  {
    private JointConnection()
    {
    }
    public JointConnection(JointType startJoint, int jointCount)
    {
      this.joints =
         Enumerable
         .Range((int)startJoint, jointCount)
         .Select(j => (JointType)j)
         .ToArray();
    }
    public JointConnection(params JointType[] joints)
    {
      this.joints = joints;
    }
    public void ForEachPair(Action<JointType, JointType> handler)
    {
      for (int i = 0; i < this.joints.Length - 1; i++)
      {
        handler(this.joints[i], this.joints[i + 1]);
      }
    }
    JointType[] joints;
  }
}
